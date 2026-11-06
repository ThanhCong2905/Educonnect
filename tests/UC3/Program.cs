using System.Reflection;
using System.Text;
using EduConnect.Mvc.Areas.HocTap.Services;
using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Areas.KetNoi.Services;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;

var connection = Environment.GetEnvironmentVariable("EDU_TEST_CONNECTION") ?? throw new Exception("EDU_TEST_CONNECTION required");
if (!new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connection).InitialCatalog.StartsWith("EduConnect_Verification_")) throw new Exception("Verification database only");
var root = Path.Combine(Path.GetTempPath(), "EduConnect-UC3-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(Path.Combine(root, "tools", "clamav"));
var stub = Path.GetFullPath(".uc3-build/scanner-stub");
foreach (var file in Directory.GetFiles(stub)) File.Copy(file, Path.Combine(root, "tools", "clamav", Path.GetFileName(file)));
AppDbContext Db() => new(new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(connection).Options);
var tutor = Guid.Parse("10000000-0000-0000-0000-000000000001");
var learner = Guid.Parse("10000000-0000-0000-0000-000000000002");
var reader = Guid.NewGuid(); var editor = Guid.NewGuid(); var outsider = Guid.NewGuid(); var departed = Guid.NewGuid();
(LichHocService Schedule, BaiTapService Tasks, ThongBaoService Notices, TepService Files, QuyenLopHocService Rights, LopHocService Classes) Services(AppDbContext db, Guid actor)
{
    var http = new DefaultHttpContext { Session = new TestSession() };
    http.Session.SetString("participant", actor.ToString());
    var user = new NguoiDungMauService(db, new HttpContextAccessor { HttpContext = http });
    var rights = new QuyenLopHocService(db, user); var notices = new ThongBaoService(db, user);
    var files = new TepService(db, user, rights, new TestEnvironment(root));
    return (new(db, user, rights, notices), new(db, user, rights, notices, files), notices, files, rights, new(db, user, rights, notices, new KetNoiService(db, user, notices)));
}
var checks = new List<string>();
void Check(bool ok, string name) { if (!ok) throw new Exception(name); checks.Add(name); Console.WriteLine("PASS " + name); }
async Task Denied(Func<Task> action, int status, string name)
{
    try { await action(); throw new Exception("Expected rejection: " + name); }
    catch (Exception ex) when (Rules.ErrorStatus(ex) == status) { Check(true, name); }
}
using var setup = Db();
var original = await setup.Set<LopHoc>().FirstAsync();
var request = await setup.Set<YeuCauKetNoiLopHoc>().SingleAsync(x => x.Id == original.YeuCauKetNoiId);
var copiedRequest = (YeuCauKetNoiLopHoc)setup.Entry(request).CurrentValues.ToObject();
copiedRequest.Id = Guid.NewGuid(); copiedRequest.RowVersion = []; setup.Add(copiedRequest); await setup.SaveChangesAsync();
var classroom = (LopHoc)setup.Entry(original).CurrentValues.ToObject();
classroom.Id = Guid.NewGuid(); classroom.YeuCauKetNoiId = copiedRequest.Id; classroom.RowVersion = []; classroom.TrangThai = "DangHoatDong";
setup.Add(classroom);
foreach (var person in new[] { reader, editor, outsider, departed }) setup.Add(new NguoiThamGia { Id = person, HoTen = "UC3 " + person, LoaiNguoi = "PhuHuynh" });
await setup.SaveChangesAsync();
foreach (var person in new[] { reader, editor, departed }) setup.Add(new ThanhVienLopHoc { Id = Guid.NewGuid(), LopHocId = classroom.Id, TaiKhoanId = person, NguoiDeNghiId = learner, TrangThaiDuyet = "DaDuyet", MucQuyen = person == reader ? "Xem" : "ChinhSua", DeNghiLuc = DateTimeOffset.UtcNow, DuyetLuc = DateTimeOffset.UtcNow, RoiLopLuc = person == departed ? DateTimeOffset.UtcNow : null });
await setup.SaveChangesAsync();
using (var db = Db()) Check((await Services(db, learner).Rights.KiemTraQuyenGhiAsync(classroom.Id)).Id == classroom.Id, "Bên học nộp được khi không có bản ghi thành viên phụ");
using (var db = Db()) await Denied(() => Services(db, reader).Rights.KiemTraQuyenGhiAsync(classroom.Id), 403, "Thành viên Xem không được ghi");

async Task<Guid> Assignment(bool late = true)
{
    using var db = Db(); var id = Guid.NewGuid();
    db.Add(new BaiTapGiao { Id = id, LopHocId = classroom.Id, NguoiGiaoId = tutor, TieuDe = "UC3 bài tập", NoiDung = "Nội dung", HanNopLuc = late ? DateTimeOffset.UtcNow.AddDays(-1) : DateTimeOffset.UtcNow.AddDays(2) });
    await db.SaveChangesAsync(); return id;
}
async Task<Guid> Schedule(DateTimeOffset start)
{
    using var db = Db(); var id = Guid.NewGuid();
    db.Add(new LichHoc { Id = id, LopHocId = classroom.Id, TieuDe = "UC3 buổi học", BatDauLuc = start, KetThucLuc = start.AddHours(1), TrangThai = "DaLenLich" });
    await db.SaveChangesAsync(); return id;
}
LichInput Slot(DateTimeOffset start) => new() { TieuDe = "UC3 lịch mới", BatDauLuc = start.ToOffset(TimeSpan.FromHours(7)).DateTime, KetThucLuc = start.AddHours(1).ToOffset(TimeSpan.FromHours(7)).DateTime };
var png = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+nmVQAAAAASUVORK5CYII=");
IFormFile FileOf(string name, byte[]? bytes = null, long? size = null) { var data = bytes ?? png; return new FormFile(new MemoryStream(data), 0, size ?? data.Length, "Files", name); }
var assignment = await Assignment();
var submission = new NopBaiViewModel { BaiTapGiaoId = assignment, NoiDung = "Bài làm giữ nguyên", Files = [FileOf("bài.png")] };
using (var db = Db()) await Denied(() => Services(db, tutor).Tasks.NopAsync(submission), 403, "Gia sư không được nộp bài lớp mình");
using (var db = Db()) await Denied(() => Services(db, reader).Tasks.NopAsync(submission), 403, "Thành viên Xem không được nộp");
using (var db = Db()) await Denied(() => Services(db, outsider).Tasks.NopAsync(submission), 403, "Người ngoài không được nộp");
using (var db = Db()) await Services(db, learner).Tasks.NopAsync(submission);
Guid firstSubmission;
using (var db = Db())
{
    var n = await db.Set<BaiTapNop>().SingleAsync(x => x.MaThaoTac == submission.MaThaoTac); firstSubmission = n.Id;
    Check(n.NopLuc > (await db.Set<BaiTapGiao>().SingleAsync(x => x.Id == assignment)).HanNopLuc, "Chấp nhận nộp muộn, giờ máy chủ");
    Check(await db.Set<TepDinhKem>().CountAsync(x => x.BaiTapNopId == n.Id) == 1, "Lưu tệp đã kiểm tra cùng bài nộp");
}
using (var db = Db()) await Services(db, learner).Tasks.NopAsync(submission);
using (var db = Db()) Check(await db.Set<BaiTapNop>().CountAsync(x => x.BaiTapGiaoId == assignment) == 1 && await db.Set<ThongBao>().CountAsync(x => x.DoiTuongId == assignment) == 1 && await db.Set<TepDinhKem>().CountAsync(x => x.BaiTapNopId == firstSubmission) == 1, "Gửi lặp không thêm bài, tệp hoặc thông báo");
using (var db = Db()) await Denied(() => Services(db, learner).Tasks.NopAsync(new() { BaiTapGiaoId = assignment, MaThaoTac = submission.MaThaoTac, NoiDung = submission.NoiDung, Files = [FileOf("khác.png")] }), 409, "Cùng mã nhưng tệp khác bị từ chối");
using (var db = Db()) await Denied(() => Services(db, learner).Tasks.NopAsync(new() { BaiTapGiaoId = assignment, MaThaoTac = submission.MaThaoTac, NoiDung = "Khác", Files = [FileOf("bài.png")] }), 409, "Cùng mã nhưng nội dung khác bị từ chối");
using (var db = Db()) await Services(db, editor).Tasks.NopAsync(new() { BaiTapGiaoId = assignment, NoiDung = "Lần mới do thành viên Chỉnh sửa nộp" });
using (var db = Db()) Check(await db.Set<BaiTapNop>().CountAsync(x => x.BaiTapGiaoId == assignment) == 2, "Thành viên Chỉnh sửa nộp lại, giữ lần trước");
using (var db = Db()) await Denied(() => Services(db, tutor).Tasks.ChamAsync(new() { BaiTapNopId = firstSubmission, NhanXet = "Chấm lần cũ" }), 409, "Không chấm lần nộp cũ");
Guid newest; using (var db = Db()) newest = await db.Set<BaiTapNop>().Where(x => x.BaiTapGiaoId == assignment).OrderByDescending(x => x.LanNop).Select(x => x.Id).FirstAsync();
using (var db = Db()) await Denied(() => Services(db, learner).Tasks.ChamAsync(new() { BaiTapNopId = newest, NhanXet = "Tự chấm" }), 403, "Bên học không được tự chấm");
using (var db = Db()) await Denied(() => Services(db, tutor).Tasks.ChamAsync(new() { BaiTapNopId = newest, Diem = 10.001m, NhanXet = "Sai" }), 400, "Từ chối điểm quá hai chữ số thập phân");
using (var db = Db()) await Denied(() => Services(db, tutor).Tasks.ChamAsync(new() { BaiTapNopId = newest, Diem = -1, NhanXet = "Sai" }), 400, "Từ chối điểm âm");
using (var db = Db()) await Services(db, tutor).Tasks.ChamAsync(new() { BaiTapNopId = newest, NhanXet = "Nhận xét không điểm" });
KetQuaChamBai grade;
using (var db = Db())
{
    grade = await db.Set<KetQuaChamBai>().AsNoTracking().SingleAsync(x => x.BaiTapNopId == newest);
    Check(grade.Diem == null, "Cho nhận xét không có điểm");
    var recipients = await db.Set<ThongBao>().Where(x => x.DoiTuongId == assignment && x.NoiDungTomTat == "Bài tập đã có nhận xét và kết quả mới.").Select(x => x.NguoiNhanId).ToListAsync();
    Check(recipients.Order().SequenceEqual(new[] { learner, reader, editor }.Order()), "Thông báo đúng người còn quyền, không trùng và không gửi người đã rời");
}
using (var db = Db()) await Denied(() => Services(db, learner).Tasks.NopAsync(new() { BaiTapGiaoId = assignment, NoiDung = "Sau chấm" }), 409, "Không nhận lần mới sau khi đã chấm");
using (var db = Db()) await Services(db, learner).Tasks.NopAsync(submission);
using (var db = Db()) Check(await db.Set<BaiTapNop>().CountAsync(x => x.BaiTapGiaoId == assignment) == 2, "Gửi lại kết quả cũ sau chấm không tạo lần mới");
using (var db = Db()) await Services(db, tutor).Tasks.ChamAsync(new() { BaiTapNopId = newest, Diem = 10, NhanXet = "Sửa kết quả", RowVersion = Convert.ToBase64String(grade.RowVersion) });
using (var db = Db()) await Denied(() => Services(db, tutor).Tasks.ChamAsync(new() { BaiTapNopId = newest, Diem = 8, NhanXet = "Bản cũ", RowVersion = Convert.ToBase64String(grade.RowVersion) }), 409, "Sửa kết quả bằng phiên bản cũ bị từ chối");
using (var db = Db()) Check(await db.Set<KetQuaChamBai>().CountAsync(x => x.BaiTapNopId == newest) == 1 && (await db.Set<KetQuaChamBai>().SingleAsync(x => x.BaiTapNopId == newest)).Diem == 10, "Chỉ một kết quả hiện hành, không ghi đè bản mới");

var start = DateTimeOffset.UtcNow.AddDays(10 + await setup.Set<LopHoc>().CountAsync() * 3).Date.AddHours(5); var future = new DateTimeOffset(start, TimeSpan.Zero);
var oldId = await Schedule(future); LichHoc old;
using (var db = Db()) old = await db.Set<LichHoc>().AsNoTracking().SingleAsync(x => x.Id == oldId);
var change = new SuaLichInput { Id = old.Id, RowVersion = Convert.ToBase64String(old.RowVersion), Lich = Slot(future.AddHours(3)), TrangThai = "DaDieuChinh" };
using (var db = Db()) await Services(db, tutor).Schedule.SuaAsync(change);
using (var db = Db())
{
    var history = await db.Set<LichHoc>().SingleAsync(x => x.Id == old.Id); var replacement = await db.Set<LichHoc>().SingleAsync(x => x.LichGocId == old.Id);
    Check(history.BatDauLuc == old.BatDauLuc && history.TrangThai == "DaDieuChinh", "Đổi lịch giữ nguyên giờ cũ và đánh dấu lịch đã thay thế");
    Check(replacement.TrangThai == "DaLenLich" && replacement.BatDauLuc == future.AddHours(3), "Tạo lịch thay thế có liên kết gốc");
}
using (var db = Db()) await Denied(() => Services(db, tutor).Schedule.SuaAsync(change), 409, "Lịch đã thay thế không được sửa lần nữa");
using (var db = Db()) await Services(db, tutor).Schedule.ThemHangLoatAsync(new() { LopHocId = classroom.Id, Buoi = [Slot(future)] });
Check(true, "Khung giờ lịch cũ đã thay thế có thể dùng lại");
using (var db = Db()) await Denied(() => Services(db, tutor).Schedule.ThemHangLoatAsync(new() { LopHocId = classroom.Id, Buoi = [Slot(future)] }), 409, "Từ chối lịch chồng lấn");
var pastId = await Schedule(DateTimeOffset.UtcNow.AddDays(-3));
LichHoc past; using (var db = Db()) past = await db.Set<LichHoc>().AsNoTracking().SingleAsync(x => x.Id == pastId);
using (var db = Db()) await Services(db, tutor).Schedule.TaoBuoiBuAsync(new() { Id = pastId, RowVersion = Convert.ToBase64String(past.RowVersion), Lich = Slot(future.AddHours(6)) });
using (var db = Db()) Check((await db.Set<LichHoc>().SingleAsync(x => x.Id == pastId)).TrangThai == "DaLenLich" && (await db.Set<LichHoc>().SingleAsync(x => x.LichGocId == pastId)).LaBuoiBu, "Buổi bù giữ nguyên buổi gốc");
using (var db = Db()) await Denied(() => Services(db, learner).Schedule.TaoBuoiBuAsync(new() { Id = pastId, RowVersion = Convert.ToBase64String(past.RowVersion), Lich = Slot(future.AddHours(7)) }), 403, "Bên học không trực tiếp tạo buổi bù");
using (var db = Db()) { var id = (await db.Set<LichHoc>().SingleAsync(x => x.LichGocId == oldId)).Id; await Denied(() => Services(db, tutor).Schedule.GhiNhanAsync(new() { LichHocId = id, NoiDung = "Sớm", NhanXet = "Sớm" }), 409, "Không ghi nhận trước giờ kết thúc"); }
using (var db = Db()) await Services(db, tutor).Schedule.GhiNhanAsync(new() { LichHocId = pastId, NoiDung = "Nội dung đã học", NhanXet = "Vắng", KetQuaBuoiHoc = "KhongDienRa", TrangThaiBenHoc = "VangKhongBaoTruoc" });
BuoiHoc record; using (var db = Db()) record = await db.Set<BuoiHoc>().AsNoTracking().SingleAsync(x => x.LichHocId == pastId);
using (var db = Db()) await Services(db, tutor).Schedule.GhiNhanAsync(new() { LichHocId = pastId, NoiDung = "Bổ sung", NhanXet = "Cập nhật", RowVersion = Convert.ToBase64String(record.RowVersion) });
using (var db = Db()) await Denied(() => Services(db, tutor).Schedule.GhiNhanAsync(new() { LichHocId = pastId, NoiDung = "Cũ", NhanXet = "Cũ", RowVersion = Convert.ToBase64String(record.RowVersion) }), 409, "Ghi nhận cũ không ghi đè phiên bản mới");
using (var db = Db()) Check(await db.Set<LichSuGhiNhanBuoiHoc>().AnyAsync(x => x.BuoiHocId == record.Id && x.PhienBan == record.PhienBan && x.NhanXet == "Vắng") && (await db.Set<BuoiHoc>().SingleAsync(x => x.Id == record.Id)).PhienBan == record.PhienBan + 1, "Giữ phiên bản cũ và ghi nhận hiện hành khi sửa");
using (var db = Db()) Check(await db.Set<ThongBao>().AnyAsync(x => x.DoiTuongId == pastId && x.NguoiNhanId == tutor && x.NoiDungTomTat == "Bên học vắng không báo trước."), "Thông báo vắng không báo trước cho bên còn lại");

var raceSchedule = await Schedule(future.AddDays(2)); LichHoc raceOld;
using (var db = Db()) raceOld = await db.Set<LichHoc>().AsNoTracking().SingleAsync(x => x.Id == raceSchedule);
async Task<bool> ChangeAt(int hours) { using var db = Db(); try { await Services(db, tutor).Schedule.SuaAsync(new() { Id = raceSchedule, RowVersion = Convert.ToBase64String(raceOld.RowVersion), Lich = Slot(future.AddDays(2).AddHours(hours)) }); return true; } catch (Exception ex) when (Rules.ErrorStatus(ex) == 409) { return false; } }
var scheduleRace = await Task.WhenAll(ChangeAt(2), ChangeAt(4)); Check(scheduleRace.Count(x => x) == 1, "Hai lần đổi cùng phiên bản chỉ một lần thành công");
var raceAssignment = await Assignment(); var token = Guid.NewGuid();
async Task SubmitRace() { using var db = Db(); await Services(db, learner).Tasks.NopAsync(new() { BaiTapGiaoId = raceAssignment, MaThaoTac = token, NoiDung = "Gửi đồng thời" }); }
await Task.WhenAll(SubmitRace(), SubmitRace());
using (var db = Db()) Check(await db.Set<BaiTapNop>().CountAsync(x => x.BaiTapGiaoId == raceAssignment) == 1 && await db.Set<ThongBao>().CountAsync(x => x.DoiTuongId == raceAssignment) == 1, "Hai yêu cầu cùng mã chỉ tạo một bài và một thông báo");
Guid raceSubmission; using (var db = Db()) raceSubmission = await db.Set<BaiTapNop>().Where(x => x.BaiTapGiaoId == raceAssignment).Select(x => x.Id).SingleAsync();
async Task<bool> Race(Func<BaiTapService, Task> run, Guid actor) { using var db = Db(); try { await run(Services(db, actor).Tasks); return true; } catch (Exception ex) when (Rules.ErrorStatus(ex) == 409) { return false; } }
var outcome = await Task.WhenAll(Race(s => s.ChamAsync(new() { BaiTapNopId = raceSubmission, NhanXet = "Chấm cạnh tranh" }), tutor), Race(s => s.NopAsync(new() { BaiTapGiaoId = raceAssignment, NoiDung = "Nộp cạnh tranh" }), learner));
Check(outcome.Count(x => x) == 1, "Nộp cạnh tranh chấm chỉ một thao tác hợp lệ");
using (var db = Db()) { var last = await db.Set<BaiTapNop>().Where(x => x.BaiTapGiaoId == raceAssignment).MaxAsync(x => x.LanNop); Check(!await (from k in db.Set<KetQuaChamBai>() join n in db.Set<BaiTapNop>() on k.BaiTapNopId equals n.Id where n.BaiTapGiaoId == raceAssignment && n.LanNop != last select k).AnyAsync(), "Không có kết quả chấm gắn lần nộp cũ sau cạnh tranh"); }

using (var db = Db())
{
    var fs = Services(db, tutor).Files;
    await Denied(() => fs.KiemTraAsync([FileOf("x.png", size: 10000001)]), 400, "Giới hạn 10 MB mỗi tệp");
    await Denied(() => fs.KiemTraAsync(Enumerable.Range(0, 6).Select(i => FileOf(i + ".png")).ToList()), 400, "Giới hạn 5 tệp mỗi lần");
    await Denied(() => fs.KiemTraAsync([FileOf("x.png", "sai định dạng"u8.ToArray())]), 400, "Không chấp nhận tệp giả định dạng");
    await Denied(() => fs.KiemTraAsync([FileOf("x.png", png.Concat(Encoding.UTF8.GetBytes("EICAR")).ToArray())]), 400, "Từ chối khi máy quét trả phát hiện mã độc");
    await Denied(() => fs.KiemTraAsync([FileOf("x.png", png.Concat(Encoding.UTF8.GetBytes("SCAN_ERROR")).ToArray())]), 400, "Từ chối khi máy quét trả lỗi");
    var boundary = new byte[10000000]; png.CopyTo(boundary, 0);
    Check((await fs.KiemTraAsync([FileOf("boundary.png", boundary)])).Count == 1, "Chấp nhận đúng biên 10.000.000 byte");
    var missingScanner = new TepService(db, new NguoiDungMauService(db, new HttpContextAccessor()), new QuyenLopHocService(db, new NguoiDungMauService(db, new HttpContextAccessor())), new TestEnvironment(root + "-missing"));
    await Denied(() => missingScanner.KiemTraAsync([FileOf("x.png")]), 400, "Không dùng tệp khi chưa cài máy quét");
    Check(!Directory.EnumerateFiles(Path.Combine(root, "uploads", ".quarantine")).Any(), "Dọn vùng cách ly sau kiểm tra thành công/lỗi");
}
var rollbackTitle = "rollback-" + Guid.NewGuid();
var filesBefore = Directory.GetFiles(Path.Combine(root, "uploads")).Length;
using (var db = Db()) await db.Database.ExecuteSqlRawAsync("CREATE OR ALTER TRIGGER dbo.tr_UC3_TestFailure ON dbo.ThongBao AFTER INSERT AS BEGIN IF EXISTS(SELECT 1 FROM inserted WHERE NoiDungTomTat=N'Có bài tập mới.') THROW 51300,N'Test rollback after file write',1; END;");
try
{
    using var db = Db();
    await Denied(() => Services(db, tutor).Tasks.GiaoAsync(new() { LopHocId = classroom.Id, TieuDe = rollbackTitle, NoiDung = "Kiểm tra rollback", HanNopLuc = DateTime.Today.AddDays(3), Files = [FileOf("rollback.png")] }), 400, "Lỗi lưu thông báo sau ghi tệp làm thao tác thất bại");
}
finally { using var db = Db(); await db.Database.ExecuteSqlRawAsync("DROP TRIGGER dbo.tr_UC3_TestFailure;"); }
using (var db = Db()) Check(!await db.Set<BaiTapGiao>().AnyAsync(x => x.TieuDe == rollbackTitle) && Directory.GetFiles(Path.Combine(root, "uploads")).Length == filesBefore, "Rollback cả bài tập và dọn tệp đã ghi");
var due = await Schedule(DateTimeOffset.UtcNow.AddHours(-27)); var notDue = await Schedule(DateTimeOffset.UtcNow.AddHours(-2));
var services = new ServiceCollection(); services.AddDbContext<AppDbContext>(o => o.UseSqlServer(connection)); services.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor()); services.AddScoped<NguoiDungMauService>(); services.AddScoped<ThongBaoService>();
using var provider = services.BuildServiceProvider();
var worker = new BaoTriWorker(provider.GetRequiredService<IServiceScopeFactory>(), NullLogger<BaoTriWorker>.Instance);
var remind = typeof(BaoTriWorker).GetMethod("XuLyNhacHocTapAsync", BindingFlags.Instance | BindingFlags.NonPublic)!;
await (Task)remind.Invoke(worker, [CancellationToken.None])!; await (Task)remind.Invoke(worker, [CancellationToken.None])!;
using (var db = Db())
{
    Check(await db.Set<ThongBao>().CountAsync(x => x.DoiTuongId == due && x.LoaiThongBao == "NhacViec") == 1, "Nhắc quá 24 giờ không gửi trùng");
    Check(!await db.Set<ThongBao>().AnyAsync(x => x.DoiTuongId == notDue && x.LoaiThongBao == "NhacViec"), "Không nhắc trước 24 giờ");
}
var browserAssignment = await Assignment(); var browserSchedule = await Schedule(future.AddDays(5));
using (var db = Db()) await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.LopHoc SET TrangThai='DaKetThuc' WHERE Id={classroom.Id}");
try { using var db = Db(); await Denied(() => Services(db, learner).Tasks.NopAsync(new() { BaiTapGiaoId = browserAssignment, NoiDung = "Lớp đã kết thúc" }), 409, "Lớp kết thúc khóa thao tác học tập mới"); }
finally { using var db = Db(); await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.LopHoc SET TrangThai='DangHoatDong' WHERE Id={classroom.Id}"); }
await File.WriteAllTextAsync(".uc3-build/fixture.json", System.Text.Json.JsonSerializer.Serialize(new { database = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connection).InitialCatalog, tutor, learner, reader, editor, classroom = classroom.Id, assignment = browserAssignment, schedule = browserSchedule, past = pastId, future = future.ToOffset(TimeSpan.FromHours(7)).DateTime.ToString("yyyy-MM-ddTHH:mm") }));
await File.WriteAllTextAsync(".uc3-build/results.json", System.Text.Json.JsonSerializer.Serialize(new { checks.Count, checks }, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
Console.WriteLine($"{checks.Count} checks passed; scanner fixture checks protocol/control flow, not malware detection effectiveness.");

sealed class TestSession : ISession
{
    readonly Dictionary<string, byte[]> values = [];
    public bool IsAvailable => true; public string Id => "uc3-test"; public IEnumerable<string> Keys => values.Keys;
    public void Clear() => values.Clear(); public Task CommitAsync(CancellationToken t = default) => Task.CompletedTask; public Task LoadAsync(CancellationToken t = default) => Task.CompletedTask;
    public void Remove(string key) => values.Remove(key); public void Set(string key, byte[] value) => values[key] = value; public bool TryGetValue(string key, out byte[] value) => values.TryGetValue(key, out value!);
}
sealed class TestEnvironment(string root) : IWebHostEnvironment
{
    public string ApplicationName { get; set; } = "UC3"; public string EnvironmentName { get; set; } = "Development";
    public string ContentRootPath { get; set; } = root; public string WebRootPath { get; set; } = Path.Combine(root, "wwwroot");
    public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider(); public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
}
