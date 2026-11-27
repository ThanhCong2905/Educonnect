using System.Net;
using EduConnect.Mvc.Areas.QuanTri.Services;
using EduConnect.Mvc.Areas.QuanTri.ViewModels;
using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Xunit;

internal static partial class Cases
{
    static async Task GiveAdmin(Fixture f, Guid user, params string[] permissions)
    {
        await using var db = f.Db();
        foreach (var permission in permissions)
            db.UserClaims.Add(new IdentityUserClaim<Guid> { UserId = user, ClaimType = QuyenQuanTriService.ClaimType, ClaimValue = permission });
        await db.SaveChangesAsync();
    }

    static (QuyenQuanTriService Rights, QuanTriService Service, PhienCapNhat Realtime) AdminServices(Scope scope)
    {
        var realtime = new PhienCapNhat();
        var rights = new QuyenQuanTriService(scope.Db, scope.Current, new PlainAccessor());
        var service = new QuanTriService(scope.Db, scope.Current, rights, new PhienDangNhapService(scope.Db, realtime), scope.Notices, realtime);
        return (rights, service, realtime);
    }

    static async Task<Guid> SeedReportRow(Fixture f, Guid target)
    {
        await using var db = f.Db();
        var report = new BaoCaoViPham { Id = Guid.NewGuid(), NguoiBaoCaoId = f.H, NguoiBiBaoCaoId = f.G,
            LoaiDoiTuong = "NhuCauHoc", DoiTuongId = target, LyDo = f.Tag + " reason", MoTa = f.Tag + " detail",
            TrangThai = "ChoXuLy", GuiLuc = DateTimeOffset.UtcNow, MaThaoTac = Guid.NewGuid(), BanChup = "{\"TieuDe\":\"fixture\"}" };
        db.Add(report); await db.SaveChangesAsync(); return report.Id;
    }

    static BaoCaoViPhamService ReportService(Scope scope)
    {
        var http = new PlainAccessor { HttpContext = new DefaultHttpContext() };
        var admin = new QuyenQuanTriService(scope.Db, scope.Current, http);
        var files = new TepService(scope.Db, scope.Current, scope.Rights, new TestEnvironment(), configuration: new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["FileScanning:AllowUnscannedUploads"] = "true" }).Build());
        return new BaoCaoViPhamService(scope.Db, scope.Current, scope.Rights, scope.Chat, files, admin, scope.Notices);
    }

    internal static async Task AdminCases(Fixture f, string key)
    {
        switch (key)
        {
            case "admin_store":
            {
                await using var a0 = f.Scope(f.A);
                var (_, noClaims, _) = AdminServices(a0);
                var denied = await Assert.ThrowsAsync<LoiNghiepVu>(() => noClaims.LayTaiKhoanAsync()); Assert.Equal(403, denied.Status);
                await GiveAdmin(f, f.A, "QuanLyTaiKhoan");
                await using var a = f.Scope(f.A);
                var (_, admin, _) = AdminServices(a);
                var page = await admin.LayTaiKhoanAsync(q: f.Tag);
                Assert.Contains(page.DanhSach.Items, x => x.Id == f.H); Assert.Contains(page.DanhSach.Items, x => x.Id == f.G); Assert.Contains(page.DanhSach.Items, x => x.Id == f.P);
                Assert.Equal("HocSinh", page.VaiTro[f.H]); Assert.Equal("GiaSu", page.VaiTro[f.G]); Assert.Equal("PhuHuynh", page.VaiTro[f.P]);
                Assert.Empty((await admin.TaiKhoanAsync(f.H)).Quyen);
                break;
            }
            case "report_store":
            {
                await f.SeedNeed(); await GiveAdmin(f, f.A, "XuLyBaoCao");
                await using var scope = f.Scope(f.G);
                var http = new PlainAccessor { HttpContext = new DefaultHttpContext() };
                var adminRights = new QuyenQuanTriService(scope.Db, scope.Current, http);
                var files = new TepService(scope.Db, scope.Current, scope.Rights, new TestEnvironment(), configuration: new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["FileScanning:AllowUnscannedUploads"] = "true" }).Build());
                var service = new BaoCaoViPhamService(scope.Db, scope.Current, scope.Rights, scope.Chat, files, adminRights, scope.Notices);
                var input = new TaoBaoCaoViewModel { MaThaoTac = Guid.NewGuid(), LoaiDoiTuong = "NhuCauHoc", DoiTuongId = f.Need, LyDo = "Nội dung cần xem xét", MoTa = "Báo cáo integration với mô tả đủ độ dài." };
                var result = await service.GuiAsync(input, f.G, default);
                Assert.True(result.ThanhCong, result.ThongBao); var reportId = result.DuLieu;
                await using var verify = f.Db();
                var report = await verify.Set<BaoCaoViPham>().SingleAsync(x => x.Id == reportId);
                Assert.Equal(f.G, report.NguoiBaoCaoId); Assert.Equal(f.H, report.NguoiBiBaoCaoId); Assert.Equal("NhuCauHoc", report.LoaiDoiTuong); Assert.Equal(f.Need, report.DoiTuongId); Assert.Equal("ChoXuLy", report.TrangThai); Assert.Contains(f.Tag, report.BanChup, StringComparison.Ordinal);
                Assert.Empty(await verify.Set<TepDinhKem>().Where(x => x.BaoCaoViPhamId == reportId).ToListAsync());
                Assert.Contains(await verify.Set<ThongBao>().Where(x => x.DoiTuongId == reportId).ToListAsync(), x => x.NguoiNhanId == f.A && x.LoaiThongBao == "BaoCao");
                var notice = await verify.Set<ThongBao>().SingleAsync(x => x.DoiTuongId == reportId && x.NguoiNhanId == f.A);
                Assert.Contains(await verify.Set<SuKienChoXuLy>().ToListAsync(), x => x.Id == notice.MaSuKien && x.LoaiSuKien == "BaoCao" && x.TrangThai == "ChoXuLy");
                break;
            }
            case "report_visibility":
            {
                await f.SeedNeed();
                await using var scope = f.Scope(f.G);
                var http = new PlainAccessor { HttpContext = new DefaultHttpContext() };
                var adminRights = new QuyenQuanTriService(scope.Db, scope.Current, http);
                var files = new TepService(scope.Db, scope.Current, scope.Rights, new TestEnvironment(), configuration: new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["FileScanning:AllowUnscannedUploads"] = "true" }).Build());
                var service = new BaoCaoViPhamService(scope.Db, scope.Current, scope.Rights, scope.Chat, files, adminRights, scope.Notices);
                var op = Guid.NewGuid();
                var model = new TaoBaoCaoViewModel { MaThaoTac = op, LoaiDoiTuong = "NhuCauHoc", DoiTuongId = f.Need, LyDo = "Nội dung cần xem xét", MoTa = "Mô tả đủ dài cho báo cáo này." };
                var first = await service.GuiAsync(model, f.G, default); Assert.True(first.ThanhCong, first.ThongBao);
                var retry = await service.GuiAsync(model, f.G, default); Assert.True(retry.ThanhCong, retry.ThongBao); Assert.Equal(first.DuLieu, retry.DuLieu);
                var duplicate = await service.GuiAsync(new TaoBaoCaoViewModel { MaThaoTac = Guid.NewGuid(), LoaiDoiTuong = model.LoaiDoiTuong, DoiTuongId = model.DoiTuongId, LyDo = model.LyDo, MoTa = model.MoTa }, f.G, default);
                Assert.False(duplicate.ThanhCong); Assert.Equal("409", duplicate.MaLoi);
                var invalid = await service.GuiAsync(new TaoBaoCaoViewModel { MaThaoTac = Guid.NewGuid(), LoaiDoiTuong = model.LoaiDoiTuong, DoiTuongId = Guid.NewGuid(), LyDo = model.LyDo, MoTa = model.MoTa }, f.G, default);
                Assert.False(invalid.ThanhCong); Assert.Equal("404", invalid.MaLoi);
                await using var verify = f.Db(); Assert.Equal(1, await verify.Set<BaoCaoViPham>().CountAsync(x => x.NguoiBaoCaoId == f.G && x.DoiTuongId == f.Need));
                Assert.Empty(await verify.Set<TepDinhKem>().Where(x => x.BaoCaoViPhamId != null && x.NguoiTaiLenId == f.G).ToListAsync());
                break;
            }
            case "report_http":
            {
                await f.SeedClass(); var reportNeed = await f.SeedNeed(owner:f.P); Guid room; await using(var db=f.Db()) room=await db.Set<CuocHoiThoai>().Where(x=>x.LopHocId==f.Class).Select(x=>x.Id).SingleAsync();
                var messageOperation=Guid.NewGuid();await using(var tutor=f.Scope(f.G))await tutor.Chat.GuiAsync(new GuiTinNhanViewModel{MaHoiThoai=room,MaThaoTac=messageOperation,NoiDung="Bằng chứng báo cáo "+f.Tag});
                Guid message;await using(var db=f.Db())message=await db.Set<TinNhan>().Where(x=>x.MaThaoTac==messageOperation).Select(x=>x.Id).SingleAsync();
                var review=Guid.NewGuid();await using(var db=f.Db()){db.Add(new DanhGiaDoiTac{Id=review,LopHocId=f.Class,NguoiGuiId=f.G,NguoiNhanId=f.H,SoSao=2,NhanXet="Đánh giá cần xem "+f.Tag,GuiLuc=DateTimeOffset.UtcNow});await db.SaveChangesAsync();}
                var targets=new[]{("TaiKhoan",f.G),("HoSoGiaSu",f.Profile),("NhuCauHoc",reportNeed),("LopHoc",f.Class),("TinNhan",message),("DanhGiaDoiTac",review)};var reportIds=new List<Guid>();
                await using(var reporter=f.Scope(f.H)){var service=ReportService(reporter);foreach(var (type,target) in targets){var input=new TaoBaoCaoViewModel{MaThaoTac=Guid.NewGuid(),LoaiDoiTuong=type,DoiTuongId=target,LyDo="Cần xem xét "+type,MoTa="Mô tả báo cáo đủ dài cho loại đối tượng "+type};var result=await service.GuiAsync(input,f.H,default);Assert.True(result.ThanhCong,$"{type}: {result.ThongDiep}");reportIds.Add(result.DuLieu);}}
                await using(var verify=f.Db()){Assert.Equal(6,await verify.Set<BaoCaoViPham>().CountAsync(x=>reportIds.Contains(x.Id)));foreach(var (type,target) in targets){var row=await verify.Set<BaoCaoViPham>().SingleAsync(x=>x.LoaiDoiTuong==type&&x.DoiTuongId==target);Assert.Equal(f.H,row.NguoiBaoCaoId);Assert.Contains(f.Tag,row.BanChup,StringComparison.Ordinal);}}
                break;
            }
            case "report_evidence":
            {
                await f.SeedNeed();await GiveAdmin(f,f.A,"XuLyBaoCao");var reportId=await SeedReportRow(f,f.Need);var fileId=Guid.NewGuid();var stored="it-"+Guid.NewGuid().ToString("N")+".pdf";var root=Path.Combine(Fixture.Root,"uploads");Directory.CreateDirectory(root);var path=Path.Combine(root,stored);var bytes=System.Text.Encoding.ASCII.GetBytes("IT evidence "+f.Tag);await File.WriteAllBytesAsync(path,bytes);
                try{await using(var db=f.Db()){db.Add(new TepDinhKem{Id=fileId,BaoCaoViPhamId=reportId,NguoiTaiLenId=f.H,TenGoc="evidence.pdf",TenLuuTru=stored,LoaiNoiDung="application/pdf",KichThuoc=bytes.Length,Checksum=Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(bytes)),TrangThaiQuet="Sach",TaoLuc=DateTimeOffset.UtcNow});await db.SaveChangesAsync();}
                    using(var reporter=await f.Client(f.H)){using var r=await reporter.GetAsync("/QuanTri/BaoCaoViPham/Tep?id="+fileId);Assert.Equal(HttpStatusCode.OK,r.StatusCode);Assert.Equal(bytes,await r.Content.ReadAsByteArrayAsync());}
                    using(var admin=await f.Client(f.A)){using var r=await admin.GetAsync("/QuanTri/BaoCaoViPham/Tep?id="+fileId);Assert.Equal(HttpStatusCode.OK,r.StatusCode);Assert.Equal(bytes,await r.Content.ReadAsByteArrayAsync());}
                    using(var outsider=await f.Client(f.X)){using var r=await outsider.GetAsync("/QuanTri/BaoCaoViPham/Tep?id="+fileId);Assert.Equal(HttpStatusCode.Forbidden,r.StatusCode);using var direct=await outsider.GetAsync("/HocTap/TinNhan/Tai?id="+fileId);Assert.Equal(HttpStatusCode.NotFound,direct.StatusCode);}
                    await using var verify=f.Db();var metadata=await verify.Set<TepDinhKem>().SingleAsync(x=>x.Id==fileId);Assert.Equal(reportId,metadata.BaoCaoViPhamId);Assert.Equal("Sach",metadata.TrangThaiQuet);Assert.Equal(bytes.Length,metadata.KichThuoc);Assert.Equal(Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(bytes)),metadata.Checksum);
                }finally{if(File.Exists(path))File.Delete(path);}break;
            }
            case "report_claim":
            {
                var id = await SeedReportRow(f, await f.SeedNeed());
                await using (var db = f.Db()) { db.UserClaims.Add(new IdentityUserClaim<Guid> { UserId = f.A, ClaimType = QuyenQuanTriService.ClaimType, ClaimValue = "XuLyBaoCao" }); var other = await f.User("QuanTri"); db.UserClaims.Add(new IdentityUserClaim<Guid> { UserId = other, ClaimType = QuyenQuanTriService.ClaimType, ClaimValue = "XuLyBaoCao" }); await db.SaveChangesAsync(); f.Proof["otherAdmin"] = other; }
                var otherAdmin = (Guid)f.Proof["otherAdmin"]!;
                var version = await f.Version<BaoCaoViPham>(id);
                await using (var scope = f.Scope(f.A)) { var (_, admin, _) = AdminServices(scope); await admin.NhanXuLyAsync(id, Convert.ToBase64String(version)); }
                await using (var scope = f.Scope(otherAdmin)) { var (_, admin, _) = AdminServices(scope); var ex = await Assert.ThrowsAsync<LoiNghiepVu>(() => admin.NhanXuLyAsync(id, Convert.ToBase64String(version))); Assert.Equal(409, ex.Status); }
                await using (var verify = f.Db()) { var row = await verify.Set<BaoCaoViPham>().SingleAsync(x => x.Id == id); Assert.Equal("DangXuLy", row.TrangThai); Assert.Equal(f.A, row.NguoiXuLyId); Assert.Single(await verify.Set<LichSuXuLyBaoCao>().Where(x => x.BaoCaoId == id && x.HanhDong == "NhanXuLy").ToListAsync()); }
                break;
            }
            case "report_resolve":
            {
                await GiveAdmin(f, f.A, "XuLyBaoCao");
                foreach (var reject in new[] { false, true })
                {
                    var target = await f.SeedNeed(); var id = await SeedReportRow(f, target);
                    await using (var claim = f.Scope(f.A)) { var (_, admin, _) = AdminServices(claim); await admin.NhanXuLyAsync(id, Convert.ToBase64String(await f.Version<BaoCaoViPham>(id))); }
                    var version = await f.Version<BaoCaoViPham>(id);
                    await using (var scope = f.Scope(f.A)) { var (_, admin, _) = AdminServices(scope); await admin.XuLyBaoCaoAsync(new XuLyBaoCaoViewModel { Id = id, RowVersion = Convert.ToBase64String(version), KetQua = reject ? "BacBo" : "XuLy", KetLuan = f.Tag + " kết luận", BienPhap = "AnNoiDung" }); }
                    await using var verify = f.Db(); var row = await verify.Set<BaoCaoViPham>().SingleAsync(x => x.Id == id);
                    Assert.Equal("DaXuLy", row.TrangThai); Assert.Equal(reject ? "BacBo" : "XuLy", row.KetQua); Assert.Equal(f.Tag + " kết luận", row.KetLuan);
                    Assert.Equal(reject ? 0 : 1, await verify.Set<BienPhapQuanTri>().CountAsync(x => x.BaoCaoId == id && x.Loai == "AnNoiDung" && x.GoLuc == null));
                    Assert.True(await verify.Set<LichSuXuLyBaoCao>().AnyAsync(x => x.BaoCaoId == id && x.HanhDong == (reject ? "BacBo" : "XuLy")));
                }
                break;
            }
            case "report_reopen":
            {
                var target = await f.SeedNeed(); var id = await SeedReportRow(f, target);
                await using (var db = f.Db()) { var row = await db.Set<BaoCaoViPham>().SingleAsync(x => x.Id == id); row.TrangThai = "DaXuLy"; row.KetQua = "XuLy"; row.KetLuan = "Ẩn nội dung đến khi sửa"; db.Add(new BienPhapQuanTri { LoaiDoiTuong = "NhuCauHoc", DoiTuongId = target, Loai = "AnNoiDung", LyDo = "Giữ ẩn", BaoCaoId = id, NguoiApDungId = f.A, ApDungLuc = DateTimeOffset.UtcNow }); await db.SaveChangesAsync(); }
                var version = await f.Version<BaoCaoViPham>(id);
                await using (var reporter = f.Scope(f.H)) Assert.True(await new BaoCaoViPhamService(reporter.Db, reporter.Current, reporter.Rights, reporter.Chat, new TepService(reporter.Db, reporter.Current, reporter.Rights, new TestEnvironment()), AdminServices(reporter).Rights, reporter.Notices).BoSungAsync(id, "Đã khắc phục, đề nghị xem lại", Convert.ToBase64String(version), true));
                await using (var verify = f.Db()) { var row = await verify.Set<BaoCaoViPham>().SingleAsync(x => x.Id == id); Assert.Equal("ChoXuLy", row.TrangThai); Assert.Equal("XuLy", row.KetQua); Assert.True(await verify.Set<BienPhapQuanTri>().AnyAsync(x => x.BaoCaoId == id && x.GoLuc == null)); Assert.True(await verify.Set<LichSuXuLyBaoCao>().AnyAsync(x => x.BaoCaoId == id && x.HanhDong == "DeNghiXemLai")); }
                var deniedVersion = Convert.ToBase64String(await f.Version<BaoCaoViPham>(id));
                await using (var denied = f.Scope(f.X)) { var http = new PlainAccessor { HttpContext = new DefaultHttpContext() }; var files = new TepService(denied.Db, denied.Current, denied.Rights, new TestEnvironment()); var service = new BaoCaoViPhamService(denied.Db, denied.Current, denied.Rights, denied.Chat, files, new QuyenQuanTriService(denied.Db, denied.Current, http), denied.Notices); var error = await Assert.ThrowsAsync<LoiNghiepVu>(() => service.BoSungAsync(id, "Tampering", deniedVersion, true)); Assert.Equal(403, error.Status); }
                break;
            }
            case "need_moderation":
            {
                var id = await f.SeedNeed(); await GiveAdmin(f, f.A, "XuLyBaoCao");
                await using (var adminScope = f.Scope(f.A))
                {
                    var (_, admin, _) = AdminServices(adminScope);
                    await admin.XuLyNhuCauAsync(new XuLyNhuCauViewModel { Id = id, RowVersion = Convert.ToBase64String(await f.Version<NhuCauHoc>(id)), QuyetDinh = "AnNoiDung", LyDo = "Nội dung cần chỉnh sửa theo quy định." });
                }
                await using (var learner = f.Scope(f.H))
                {
                    var pending = await learner.Needs.DeNghiXemLaiAsync(id, Convert.ToBase64String(await f.Version<NhuCauHoc>(id)));
                    Assert.True(pending);
                }
                await using (var verify = f.Db())
                {
                    Assert.True(await verify.Set<BienPhapQuanTri>().AnyAsync(x => x.DoiTuongId == id && x.Loai == "AnNoiDung" && x.GoLuc == null));
                    Assert.True(await verify.Set<ThongBao>().AnyAsync(x => x.NguoiNhanId == f.H && x.DoiTuongId == id && x.LoaiThongBao == "NhuCau"));
                    Assert.Equal("DangMo", (await verify.Set<NhuCauHoc>().SingleAsync(x => x.Id == id)).TrangThai);
                }
                break;
            }
            case "admin_permissions":
            {
                await GiveAdmin(f, f.A, "QuanLyQuyen", "QuanLyTaiKhoan", "XuLyBaoCao");
                var target = await f.User("QuanTri"); await GiveAdmin(f, target, "QuanLyQuyen", "QuanLyTaiKhoan", "XemNhatKy");
                using var priorSession = await f.Client(target);
                string oldStamp; await using (var db = f.Db()) oldStamp = (await db.Users.SingleAsync(x => x.Id == target)).ConcurrencyStamp;
                await using (var scope = f.Scope(f.A))
                {
                    var (_, admin, _) = AdminServices(scope);
                    await admin.PhanQuyenAsync(new PhanQuyenQuanTriViewModel { TaiKhoanId = target, ConcurrencyStamp = oldStamp, Quyen = ["XemNhatKy"], LyDo = "Thu hẹp quyền theo nhiệm vụ." });
                }
                await using (var verify = f.Db())
                {
                    Assert.Equal("QuanTri", await (from ur in verify.UserRoles join r in verify.Roles on ur.RoleId equals r.Id where ur.UserId == target select r.Name).SingleAsync());
                    Assert.Equal(new[] { "XemNhatKy" }, await verify.UserClaims.Where(x => x.UserId == target && x.ClaimType == QuyenQuanTriService.ClaimType).Select(x => x.ClaimValue!).ToArrayAsync());
                    Assert.True(await verify.Set<PhienDangNhap>().AnyAsync(x => x.TaiKhoanId == target && x.ThuHoiLuc != null && x.LyDoThuHoi == "Quyền quản trị đã thay đổi"));
                }
                break;
            }
            case "admin_subject":
            {
                await GiveAdmin(f, f.A, "QuanLyDanhMuc");
                await f.SeedNeed();
                byte[] original, originalM2; string priorName;
                await using (var db = f.Db()) { var m1 = await db.Set<MonHoc>().SingleAsync(x => x.Id == f.M1); var m2 = await db.Set<MonHoc>().SingleAsync(x => x.Id == f.M2); original = m1.RowVersion; originalM2 = m2.RowVersion; priorName = m1.TenMon; }
                await using (var scope = f.Scope(f.A))
                {
                    var (_, admin, _) = AdminServices(scope);
                    await admin.LuuMonAsync(new MonHocQuanTriViewModel { Id = f.M1, TenMon = f.Tag + " Renamed", DangSuDung = true, RowVersion = Convert.ToBase64String(original) });
                    var currentVersion = (await scope.Db.Set<MonHoc>().AsNoTracking().SingleAsync(x => x.Id == f.M1)).RowVersion;
                    await admin.LuuMonAsync(new MonHocQuanTriViewModel { Id = f.M1, TenMon = f.Tag + " Renamed", DangSuDung = false, RowVersion = Convert.ToBase64String(currentVersion) });
                    var duplicate = await Assert.ThrowsAsync<LoiNghiepVu>(() => admin.LuuMonAsync(new MonHocQuanTriViewModel { Id = f.M2, TenMon = f.Tag + " Renamed", DangSuDung = true, RowVersion = Convert.ToBase64String(originalM2) })); Assert.Equal(409, duplicate.Status);
                    var stale = await Record.ExceptionAsync(() => admin.LuuMonAsync(new MonHocQuanTriViewModel { Id = f.M1, TenMon = priorName, DangSuDung = true, RowVersion = Convert.ToBase64String(original) }));
                    Assert.True(stale is DbUpdateConcurrencyException || stale is LoiNghiepVu { Status: 409 }, stale?.ToString() ?? "Stale row-version was incorrectly accepted.");
                }
                await using (var verify = f.Db())
                {
                    var row = await verify.Set<MonHoc>().SingleAsync(x => x.Id == f.M1); Assert.Equal(f.Tag + " Renamed", row.TenMon); Assert.False(row.DangSuDung);
                    Assert.True(await verify.Set<NhuCauHoc>().AnyAsync(x => x.MonHocId == f.M1)); Assert.True(await verify.Set<HoSoGiaSuMonDay>().AnyAsync(x => x.MonHocId == f.M1));
                }
                break;
            }
            case "account_lock":
            {
                await f.SeedClass(); await GiveAdmin(f, f.A, "QuanLyTaiKhoan");
                using var oldClient = await f.Client(f.G);
                string stamp; await using (var db = f.Db()) stamp = (await db.Users.SingleAsync(x => x.Id == f.G)).ConcurrencyStamp;
                await using (var scope = f.Scope(f.A)) { var (_, admin, _) = AdminServices(scope); await admin.KhoaTaiKhoanAsync(f.G, "Tài khoản cần xác minh theo báo cáo.", stamp); }
                await using (var verify = f.Db())
                {
                    Assert.Equal("BiKhoa", (await verify.Users.SingleAsync(x => x.Id == f.G)).TrangThai);
                    Assert.Equal("DaRut", (await verify.Set<YeuCauKetNoiLopHoc>().SingleAsync(x => x.Id == f.Request)).TrangThai);
                    Assert.Equal("TamDung", (await verify.Set<LopHoc>().SingleAsync(x => x.Id == f.Class)).TrangThai);
                    Assert.True(await verify.Set<PhienDangNhap>().AnyAsync(x => x.TaiKhoanId == f.G && x.ThuHoiLuc != null && x.LyDoThuHoi == "Tài khoản bị quản trị khóa"));
                    Assert.True(await verify.Set<BienPhapQuanTri>().AnyAsync(x => x.LoaiDoiTuong == "TaiKhoan" && x.DoiTuongId == f.G && x.Loai == "KhoaTaiKhoan" && x.GoLuc == null));
                }
                oldClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Requested-With", "XMLHttpRequest");
                using var denied = await oldClient.GetAsync("/TaiKhoan/HoSo"); Assert.True((int)denied.StatusCode is 401 or 403, $"Old cookie after lock returned {(int)denied.StatusCode}.");
                break;
            }
            case "account_protect":
            {
                await GiveAdmin(f, f.A, "QuanLyTaiKhoan", "QuanLyQuyen");
                await using (var scope = f.Scope(f.A))
                {
                    var (_, admin, _) = AdminServices(scope);
                    var stamp = (await scope.Db.Users.SingleAsync(x => x.Id == f.A)).ConcurrencyStamp;
                    var self = await Assert.ThrowsAsync<LoiNghiepVu>(() => admin.KhoaTaiKhoanAsync(f.A, "Không được tự khóa.", stamp)); Assert.Equal(400, self.Status);
                    var target = await f.User("QuanTri");
                    var stale = await Assert.ThrowsAsync<LoiNghiepVu>(() => admin.KhoaTaiKhoanAsync(target, "Thử row-version cũ.", "stale-stamp")); Assert.Equal(409, stale.Status);
                    Assert.Equal("HoatDong", (await scope.Db.Users.SingleAsync(x => x.Id == target)).TrangThai);
                }
                break;
            }
            case "measure_release":
            {
                await f.SeedClass(); await GiveAdmin(f, f.A, "QuanLyTaiKhoan");
                var first = Guid.NewGuid(); var second = Guid.NewGuid();
                await f.Sql("UPDATE dbo.LopHoc SET TrangThai='TamDung' WHERE Id=@id", new() { ["@id"] = f.Class });
                await using (var db = f.Db())
                {
                    db.AddRange(
                        new BienPhapQuanTri { Id = first, LoaiDoiTuong = "TaiKhoan", DoiTuongId = f.G, Loai = "KhoaTaiKhoan", LyDo = "IT parent G", NguoiApDungId = f.A, ApDungLuc = DateTimeOffset.UtcNow },
                        new BienPhapQuanTri { Id = second, LoaiDoiTuong = "TaiKhoan", DoiTuongId = f.H, Loai = "KhoaTaiKhoan", LyDo = "IT parent H", NguoiApDungId = f.A, ApDungLuc = DateTimeOffset.UtcNow });
                    await db.SaveChangesAsync();
                    db.AddRange(
                        new BienPhapQuanTri { NguonBienPhapId = first, LoaiDoiTuong = "LopHoc", DoiTuongId = f.Class, Loai = "TamDungLop", LyDo = "G bị khóa", NguoiApDungId = f.A, ApDungLuc = DateTimeOffset.UtcNow },
                        new BienPhapQuanTri { NguonBienPhapId = second, LoaiDoiTuong = "LopHoc", DoiTuongId = f.Class, Loai = "TamDungLop", LyDo = "H bị khóa", NguoiApDungId = f.A, ApDungLuc = DateTimeOffset.UtcNow });
                    await db.SaveChangesAsync();
                }
                await using (var scope = f.Scope(f.A))
                {
                    var (_, admin, _) = AdminServices(scope);
                    await admin.GoBienPhapAsync(first, "Gỡ nguyên nhân G", Convert.ToBase64String(await f.Version<BienPhapQuanTri>(first)));
                }
                await using (var check = f.Db()) Assert.Equal("TamDung", (await check.Set<LopHoc>().SingleAsync(x => x.Id == f.Class)).TrangThai);
                await using (var scope = f.Scope(f.A))
                {
                    var (_, admin, _) = AdminServices(scope);
                    await admin.GoBienPhapAsync(second, "Gỡ nguyên nhân cuối", Convert.ToBase64String(await f.Version<BienPhapQuanTri>(second)));
                }
                await using (var check = f.Db()) Assert.Equal("DangHoatDong", (await check.Set<LopHoc>().SingleAsync(x => x.Id == f.Class)).TrangThai);
                break;
            }
            default: throw new InvalidOperationException(key);
        }
    }
}
