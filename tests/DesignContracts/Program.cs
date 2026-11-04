using System.Text.Json;
using EduConnect.Mvc.Areas.NhuCau.Controllers;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Areas.TimKiem.Controllers;
using EduConnect.Mvc.Areas.TimKiem.Services;
using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var cs = Environment.GetEnvironmentVariable("EDU_TEST_CONNECTION") ?? throw new Exception("EDU_TEST_CONNECTION required");
var database = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(cs).InitialCatalog;
if (!database.StartsWith("EduConnect_Verification_")) throw new Exception("Verification DB only");
using var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(cs).Options);
var http = new DefaultHttpContext { Session = new TestSession() };
var current = new NguoiDungMauService(db, new HttpContextAccessor { HttpContext = http });
var needs = new NhuCauService(db, current, new MonHocCatalog(db));
var rights = new QuyenLopHocService(db, current);
var profile = new HoSoGiaSuService(db, current, new MonHocCatalog(db), new TepService(db, current, rights, new TestEnv()), new ThongBaoService(db, current));
var search = new TimKiemService(db, current);
var learner = Guid.NewGuid(); var other = Guid.NewGuid(); var tutor = Guid.NewGuid(); var hid = Guid.NewGuid();
db.AddRange(new NguoiThamGia { Id = learner, HoTen = "Contract learner " + learner, LoaiNguoi = "HocSinh" },
    new NguoiThamGia { Id = other, HoTen = "Contract outsider", LoaiNguoi = "HocSinh" },
    new NguoiThamGia { Id = tutor, HoTen = "Contract tutor " + tutor, LoaiNguoi = "GiaSu" });
await db.SaveChangesAsync(); db.Add(new GiaSu { TaiKhoanId = tutor }); await db.SaveChangesAsync();
db.Add(new HoSoGiaSu { Id = hid, GiaSuId = tutor, TrinhDo = "Đại học", ChuyenMon = "Toán", KinhNghiem = "3 năm", MoTa = "Hồ sơ kiểm thử", KhuVuc = "Hà Nội", DangHienThi = true, TrangThaiDuyet = "DaDuyet", DayTrucTuyen = true });
await db.SaveChangesAsync();
db.Add(new HoSoGiaSuMonDay { HoSoGiaSuId = hid, MonHocId = 1, PhamViDay = "Lớp 10–12", HocPhiDeXuat = 150000, DonViHocPhi = "VND/Gio" });
await db.SaveChangesAsync(); await current.ChonAsync(learner);
var checks = new List<string>();
void Check(bool ok, string message) { if (!ok) throw new Exception(message); }
async Task Pass(string name, Func<Task> test) { db.ChangeTracker.Clear(); await test(); checks.Add(name); Console.WriteLine("PASS " + name); }
TaoNhuCauBenHocViewModel Input() => new() { MonHocIds = [1], MucTieu = "Contract need " + learner, SoBuoiTuan = 1 };
void Signature(Type owner, string name, Type result, params Type[] parameters)
    => Check(owner.GetMethod(name, parameters)?.ReturnType == result, owner.Name + "." + name + " does not match design");
await Pass("Audited controller/service signatures match design", () =>
{
    Signature(typeof(NhuCauController), "TaoNhuCauHoc", typeof(Task<IActionResult>), typeof(TaoNhuCauBenHocViewModel));
    Signature(typeof(NhuCauController), "CapNhat", typeof(Task<IActionResult>), typeof(Guid), typeof(TaoNhuCauBenHocViewModel));
    Signature(typeof(NhuCauController), "Sua", typeof(Task<IActionResult>), typeof(Guid?));
    Signature(typeof(NhuCauService), "LayDeSuaAsync", typeof(Task<KetQua<NhuCauInput>>), typeof(Guid));
    Signature(typeof(NhuCauService), "TaoNhuCauHocAsync", typeof(Task<KetQua<Guid>>), typeof(TaoNhuCauBenHocViewModel), typeof(Guid), typeof(CancellationToken));
    Signature(typeof(NhuCauService), "CapNhatAsync", typeof(Task<KetQua<Guid>>), typeof(Guid), typeof(TaoNhuCauBenHocViewModel), typeof(byte[]), typeof(Guid), typeof(CancellationToken));
    Signature(typeof(TimKiemController), "Tim", typeof(Task<IActionResult>), typeof(BoLocTimKiemViewModel));
    Signature(typeof(TimKiemController), "SoSanh", typeof(Task<IActionResult>), typeof(IReadOnlyList<Guid>));
    Signature(typeof(TimKiemService), "TimKiemAsync", typeof(Task<KetQua<Page<UngVienViewModel>>>), typeof(BoLocTimKiemViewModel), typeof(Guid?), typeof(CancellationToken));
    Signature(typeof(TimKiemService), "SoSanhAsync", typeof(Task<KetQua<SoSanhViewModel>>), typeof(IReadOnlyList<Guid>), typeof(Guid?), typeof(CancellationToken));
    Signature(typeof(HoSoGiaSuController), "ChiTiet", typeof(Task<IActionResult>), typeof(Guid));
    Signature(typeof(HoSoGiaSuService), "LayHoSoAsync", typeof(Task<KetQua<HoSoGiaSuCongKhaiViewModel>>), typeof(Guid), typeof(Guid?), typeof(CancellationToken));
    return Task.CompletedTask;
});
Guid need = Guid.Empty;
await Pass("Create multiple subjects atomically with schedules", async () =>
{
    var m = Input(); m.MonHocIds = [1, 2]; need = (await needs.TaoNhuCauHocAsync(m, learner, default)).DuLieu;
    Check(await db.Set<NhuCauHoc>().CountAsync(n => n.ChuSoHuuId == learner) == 2, "Missing subject");
    Check(await db.Set<KhungGioRanh>().AnyAsync(s => s.NhuCauId == need), "Missing schedule");
});
await Pass("CT03 loads owned demand with version, subject and schedule", async () =>
{
    var result = await needs.LayDeSuaAsync(need);
    Check(result.ThanhCong, "Owned demand not loaded");
    var model = result.DuLieu;
    Check(model.Id == need && Rules.Version(model.RowVersion).Length == 8 && model.MonHocIds.Count == 1 && model.LichRanh.Count == 1, "Edit data incomplete");
});
await Pass("CT03 returns failed result for empty and nonexistent IDs", async () =>
{
    foreach (var id in new[] { Guid.Empty, Guid.NewGuid() })
    {
        var result = await needs.LayDeSuaAsync(id);
        Check(!result.ThanhCong && result.MaTrangThai == 404, "Missing demand did not return 404 result");
    }
});
await Pass("CT03 hides other owner's demand and rejects tutor role", async () =>
{
    await current.ChonAsync(other);
    var result = await needs.LayDeSuaAsync(need);
    Check(!result.ThanhCong && result.MaTrangThai == 404, "Other owner's demand exposed");
    await current.ChonAsync(tutor);
    result = await needs.LayDeSuaAsync(need);
    Check(!result.ThanhCong && result.MaTrangThai == 403, "Tutor edited learner demand");
    await current.ChonAsync(learner);
});
await Pass("CT03 controller separates new form from missing edit ID", async () =>
{
    var controller = new NhuCauController(needs, current)
    {
        ControllerContext = new ControllerContext { HttpContext = http }
    };
    Check(await controller.Sua(null) is NotFoundResult, "Missing ID opened create form");
    Check(await controller.Sua(Guid.Empty) is NotFoundResult, "Empty ID not rejected");
    Check(await controller.Sua(Guid.NewGuid()) is NotFoundResult, "Missing record not rejected");
    var created = await controller.Tao() as ViewResult;
    Check(created?.ViewName == "Tao" && created.Model is NhuCauInput { Id: null, RowVersion: null }, "Create form is not empty");
    var edited = await controller.Sua(need) as ViewResult;
    Check(edited?.Model is NhuCauInput model && model.Id == need, "Edit form not loaded");
    Check(await db.Set<NhuCauHoc>().CountAsync(n => n.ChuSoHuuId == learner) == 2, "GET created a demand");
});
await Pass("Reject forged actor without creating data", async () =>
{
    var r = await needs.TaoNhuCauHocAsync(Input(), other, default);
    Check(!r.ThanhCong && r.MaTrangThai == 403, "Forged actor allowed");
    Check(await db.Set<NhuCauHoc>().CountAsync(n => n.ChuSoHuuId == learner) == 2, "Unexpected write");
});
await Pass("Invalid second subject rolls back entire create", async () =>
{
    var m = Input(); m.MonHocIds = [1, int.MaxValue];
    Check(!(await needs.TaoNhuCauHocAsync(m, learner, default)).ThanhCong, "Invalid subject allowed"); db.ChangeTracker.Clear();
    Check(await db.Set<NhuCauHoc>().CountAsync(n => n.ChuSoHuuId == learner) == 2, "Partial write");
});
byte[] stale = [];
await Pass("Update with byte rowVersion preserves subject and schedule", async () =>
{
    var m = TaoNhuCauBenHocViewModel.FromInput((await needs.LayDeSuaAsync(need)).DuLieu); stale = m.RowVersion!;
    m.MucTieu = "Updated contract need " + learner;
    Check((await needs.CapNhatAsync(need, m, stale, learner, default)).DuLieu == need, "Wrong ID");
    db.ChangeTracker.Clear(); Check(((await needs.LayDeSuaAsync(need)).DuLieu).MucTieu == m.MucTieu, "Update lost");
});
await Pass("Stale update returns conflict without overwriting", async () =>
{
    var m = TaoNhuCauBenHocViewModel.FromInput((await needs.LayDeSuaAsync(need)).DuLieu); m.MucTieu = "Must not persist";
    var r = await needs.CapNhatAsync(need, m, stale, learner, default);
    Check(!r.ThanhCong && r.MaTrangThai == 409, "Missing conflict"); db.ChangeTracker.Clear();
    Check(((await needs.LayDeSuaAsync(need)).DuLieu).MucTieu != m.MucTieu, "Stale overwrite");
});
await Pass("Other owner cannot update a demand", async () =>
{
    var m = TaoNhuCauBenHocViewModel.FromInput((await needs.LayDeSuaAsync(need)).DuLieu); await current.ChonAsync(other);
    var r = await needs.CapNhatAsync(need, m, m.RowVersion!, other, default);
    Check(!r.ThanhCong && r.MaTrangThai == 403, "Ownership bypass"); await current.ChonAsync(learner);
});
await Pass("Create rejects existing ID and missing subject", async () =>
{
    var m = Input(); m.MaNhuCau = need;
    Check(!(await needs.TaoNhuCauHocAsync(m, learner, default)).ThanhCong, "Create became update");
    m = Input(); m.MonHocIds = [];
    Check(!(await needs.TaoNhuCauHocAsync(m, learner, default)).ThanhCong, "Missing subject accepted");
});
await Pass("Cancelled request propagates cancellation without writes", async () =>
{
    using var cts = new CancellationTokenSource(); cts.Cancel();
    try { await needs.TaoNhuCauHocAsync(Input(), learner, cts.Token); throw new Exception("Cancellation swallowed"); }
    catch (OperationCanceledException) { }
    Check(await db.Set<NhuCauHoc>().CountAsync(n => n.ChuSoHuuId == learner) == 2, "Cancelled write");
});
await Pass("Profile service returns public data and hides private fields", async () =>
{
    var m = (await profile.LayHoSoAsync(hid, learner, default)).DuLieu;
    Check(m.Id == hid && m.TenHienThi.Contains(tutor.ToString()) && m.MonTheoHoSo.Count == 1, "Bad public mapping");
    var json = JsonSerializer.Serialize(m);
    Check(!json.Contains("RowVersion") && !json.Contains("TepDinhKem") && !json.Contains("LyDoTuChoi"), "Private field leak");
});
await Pass("Typed search retains both role branches and legacy results", async () =>
{
    var f = new BoLocTimKiemViewModel { TuKhoa = tutor.ToString() };
    var page = (await search.TimKiemAsync(f, learner, default)).DuLieu;
    Check(page.Total == 1 && page.Items.Single().Id == hid, "Learner search mismatch");
    Check((await search.TimAsync(f)).Items.Single().Id == hid, "Legacy search mismatch"); await current.ChonAsync(tutor);
    page = (await search.TimKiemAsync(new() { TuKhoa = learner.ToString() }, tutor, default)).DuLieu;
    Check(page.Total == 2 && page.Items.All(n => n.Slots.Count == 1 && n is NhuCauCongKhaiViewModel), "Tutor search mismatch");
    await current.ChonAsync(learner);
});
await Pass("Comparison retains selection order and removes duplicates", async () =>
{
    var r = (await search.SoSanhAsync([need, hid, need], learner, default)).DuLieu;
    Check(r.HoSo.Count == 1 && r.NhuCau.Count == 1 && r.Items.Select(n => n.Id).SequenceEqual(new[] { need, hid }), "Comparison lost objects/order");
    Check(!(await search.SoSanhAsync([need, hid, Guid.NewGuid(), Guid.NewGuid()], learner, default)).ThanhCong, "Over limit allowed");
});
await Pass("Hidden profile rejected by profile/search/comparison APIs", async () =>
{
    var h = await db.Set<HoSoGiaSu>().SingleAsync(h => h.Id == hid); h.DangHienThi = false; await db.SaveChangesAsync();
    Check((await profile.LayHoSoAsync(hid, learner, default)).MaTrangThai == 404, "Hidden profile leaked");
    Check((await search.SoSanhAsync([hid], learner, default)).MaTrangThai == 404, "Hidden comparison leaked");
    Check((await search.TimKiemAsync(new() { TuKhoa = tutor.ToString() }, learner, default)).DuLieu.Total == 0, "Hidden search leaked");
    h.DangHienThi = true; await db.SaveChangesAsync();
});
await Pass("Read APIs reject forged viewer and invalid criteria", async () =>
{
    Check((await profile.LayHoSoAsync(hid, other, default)).MaTrangThai == 403, "Forged profile viewer");
    Check((await search.TimKiemAsync(new(), other, default)).MaTrangThai == 403, "Forged search viewer");
    Check(!(await search.TimKiemAsync(new() { Trang = 0 }, learner, default)).ThanhCong, "Invalid page allowed");
});
await Pass("Tutor search applies multi-subject filter", async () =>
{
    await current.ChonAsync(tutor);
    var page=(await search.TimKiemAsync(new(){MonHocIds=[2]},tutor,default)).DuLieu;
    Check(page.Total>0 && page.Items.All(x=>x.MonHocId==2),"Multi-subject criteria ignored");
    await current.ChonAsync(learner);
});
await Pass("Suggestions use reasons without percentage scores", async () =>
{
    var suggestions=await search.GoiYAsync();
    Check(suggestions.Items.All(x=>x.LyDo.Count>0),"Suggestion has no matching reason");
    Check(typeof(UngVienViewModel).GetProperty("DiemPhuHop")==null,"Unexpected percentage score");
});
await Pass("Learner without open needs has no unsupported suggestions", async () =>
{
    await current.ChonAsync(other);
    var suggestions=await search.GoiYAsync();
    Check(!suggestions.HasNeed && suggestions.Items.Count==0,"Unmatched fallback suggestions");
    await current.ChonAsync(learner);
});
await Pass("Tutor without registered subjects receives no unrelated suggestions", async () =>
{
    var emptyTutor=Guid.NewGuid();
    db.Add(new NguoiThamGia{Id=emptyTutor,HoTen="Empty tutor",LoaiNguoi="GiaSu"});
    await db.SaveChangesAsync();
    await current.ChonAsync(emptyTutor);
    var suggestions=await search.GoiYAsync();
    Check(!suggestions.HasNeed&&suggestions.Items.Count==0,"Unrelated tutor suggestions");
    await current.ChonAsync(learner);
});
await File.WriteAllTextAsync(Path.Combine(AppContext.BaseDirectory, "results.json"), JsonSerializer.Serialize(new { passed = checks.Count, checks, database, learner, other, tutor, hid, need }, new JsonSerializerOptions { WriteIndented = true }));
Console.WriteLine(checks.Count + " checks passed");

sealed class TestSession : ISession
{
    readonly Dictionary<string, byte[]> data = [];
    public bool IsAvailable => true;
    public string Id => "contracts-test";
    public IEnumerable<string> Keys => data.Keys;
    public void Clear() => data.Clear();
    public Task CommitAsync(CancellationToken token = default) => Task.CompletedTask;
    public Task LoadAsync(CancellationToken token = default) => Task.CompletedTask;
    public void Remove(string key) => data.Remove(key);
    public void Set(string key, byte[] value) => data[key] = value;
    public bool TryGetValue(string key, out byte[] value) => data.TryGetValue(key, out value!);
}
sealed class TestEnv : IWebHostEnvironment
{
    public string ApplicationName { get; set; } = "DesignContracts";
    public string EnvironmentName { get; set; } = "Development";
    public string WebRootPath { get; set; } = "";
    public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    public string ContentRootPath { get; set; } = Path.Combine(Path.GetTempPath(), "EduConnectContractTests");
    public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
}
