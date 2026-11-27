using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

internal static partial class Cases
{
    static HoSoInput ProfileInput(Fixture f, byte[]? rowVersion = null, bool withProof = true)
    {
        var m = new HoSoInput
        {
            HoTen = f.Tag + " Gia su", TrinhDo = HoSoLuaChon.TrinhDo[2], KinhNghiem = HoSoLuaChon.KinhNghiem[0], MoTa = new string('G', 120), KhuVuc = HoSoLuaChon.KhuVuc[11],
            DayTrucTuyen = true, DayTrucTiep = false, RowVersion = Convert.ToBase64String(rowVersion ?? Array.Empty<byte>()),
            TepTrinhDo = withProof ? Pdf("degree") : null,
            MonDay =
            [
                new() { MonHocId = f.M1, LopDay = [8, 9], HocPhiDeXuat = 100000, DonViHocPhi = "VND/Gio" },
                new() { MonHocId = f.M2, LopDay = [8, 9], HocPhiDeXuat = 0, DonViHocPhi = "VND/Buoi" }
            ]
        };
        return m;
    }

    static IFormFile Pdf(string name)
    {
        var bytes = System.Text.Encoding.ASCII.GetBytes($"%PDF-1.4\nIntegration fixture {name}\n%%EOF");
        return new FormFile(new MemoryStream(bytes), 0, bytes.Length, name, name + ".pdf") { Headers = new HeaderDictionary(), ContentType = "application/pdf" };
    }

    static TepService ProfileFiles(Scope scope, string root)
    {
        var env = new TestEnvironment { EnvironmentName = "Development", ContentRootPath = root, WebRootPath = Path.Combine(root, "wwwroot") };
        var cfg = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["FileScanning:AllowUnscannedUploads"] = "true" }).Build();
        return new TepService(scope.Db, scope.Current, scope.Rights, env, configuration: cfg);
    }

    static async Task<(int Status,string Body,string? Location)> PostProfile(Fixture f, bool withProof, string rowVersion, bool ajax=false)
    {
        f.AllowUnscannedUploads=true;using var client=await f.Client(f.G);var token=await Fixture.Token(client,"/NhuCau/HoSoGiaSu/Sua");using var form=new MultipartFormDataContent();
        void Add(string name,string value)=>form.Add(new StringContent(value),name);
        Add("__RequestVerificationToken",token);Add("RowVersion",rowVersion);Add("HoTen",f.Tag+" Gia su");Add("TrinhDo",HoSoLuaChon.TrinhDo[2]);Add("KinhNghiem",HoSoLuaChon.KinhNghiem[0]);Add("MoTa",new string('G',120));Add("KhuVuc",HoSoLuaChon.KhuVuc[11]);Add("DayTrucTuyen","true");Add("DayTrucTiep","false");
        Add("MonDay[0].MonHocId",f.M1.ToString());Add("MonDay[0].LopDay[0]","8");Add("MonDay[0].LopDay[1]","9");Add("MonDay[0].HocPhiDeXuat","100000");Add("MonDay[0].DonViHocPhi","VND/Gio");Add("MonDay[0].PhamViDay","Lop 8-9");
        Add("MonDay[1].MonHocId",f.M2.ToString());Add("MonDay[1].LopDay[0]","8");Add("MonDay[1].LopDay[1]","9");Add("MonDay[1].HocPhiDeXuat","0");Add("MonDay[1].DonViHocPhi","VND/Buoi");Add("MonDay[1].PhamViDay","Lop 8-9");
        if(withProof){var bytes=System.Text.Encoding.ASCII.GetBytes("%PDF-1.4\nHTTP profile fixture\n%%EOF");var part=new ByteArrayContent(bytes);part.Headers.ContentType=new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");form.Add(part,"TepTrinhDo","http-degree.pdf");}
        if(ajax)client.DefaultRequestHeaders.TryAddWithoutValidation("X-Requested-With","XMLHttpRequest");using var response=await client.PostAsync("/NhuCau/HoSoGiaSu/Luu",form);await f.Record(response);return ((int)response.StatusCode,await response.Content.ReadAsStringAsync(),response.Headers.Location?.OriginalString);
    }

    static async Task<string> SaveProfileFixture(Fixture f, bool submit)
    {
        var root = Path.Combine(Path.GetTempPath(), "EduConnect-IT-" + f.Tag);
        Directory.CreateDirectory(root);
        await using var scope = f.Scope(f.G);
        await scope.Db.Set<HoSoGiaSu>().Where(x => x.Id == f.Profile).ExecuteUpdateAsync(s => s.SetProperty(x => x.DangHienThi, false).SetProperty(x => x.TrangThaiDuyet, "BanNhap"));
        var files = ProfileFiles(scope, root);
        var service = new HoSoGiaSuService(scope.Db, scope.Current, new MonHocCatalog(scope.Db), files, scope.Notices);
        var vm = new HoSoInput
        {
            HoTen = f.Tag + " Gia sư", TrinhDo = "Đại học", KinhNghiem = "Chưa có", MoTa = new string('G', 120), KhuVuc = "Hà Nội",
            DayTrucTuyen = true, DayTrucTiep = false, RowVersion = Convert.ToBase64String(await f.Version<HoSoGiaSu>(f.Profile)),
            TepTrinhDo = Pdf("degree"),
            MonDay =
            [
                new() { MonHocId = f.M1, LopDay = [8, 9], HocPhiDeXuat = 100000, DonViHocPhi = "VND/Gio" },
                new() { MonHocId = f.M2, LopDay = [8, 9], HocPhiDeXuat = 0, DonViHocPhi = "VND/Buoi" }
            ]
        };
        await service.CapNhatAsync(vm);
        if (submit)
            await service.DoiTrangThaiAsync("GuiDuyet", Convert.ToBase64String(await f.Version<HoSoGiaSu>(f.Profile)));
        await using var observer = f.Db();
        var profile = await observer.Set<HoSoGiaSu>().SingleAsync(x => x.Id == f.Profile);
        Assert.Equal(submit ? "ChoDuyet" : "BanNhap", profile.TrangThaiDuyet);
        var rows = await observer.Set<HoSoGiaSuMonDay>().Where(x => x.HoSoGiaSuId == f.Profile).ToListAsync();
        Assert.Equal(2, rows.Count);
        var evidence = await observer.Set<TepDinhKem>().Where(x => x.HoSoGiaSuId == f.Profile).ToListAsync();
        Assert.Single(evidence); Assert.All(evidence, x => Assert.Equal("ChoQuet", x.TrangThaiQuet));
        Assert.All(evidence, x => Assert.Equal(64, x.Checksum.Length));
        Assert.All(evidence, x => Assert.True(File.Exists(Path.Combine(root, "uploads", x.TenLuuTru))));
        return root;
    }

    internal static async Task ProfileCases(Fixture f, string key)
    {
        switch (key)
        {
            case "profile_http":
            case "profile_rv":
            {
                var root = Path.Combine(Path.GetTempPath(), "EduConnect-IT-" + f.Tag);
                Directory.CreateDirectory(root);
                try
                {
                    if (key != "profile_missing")
                        await f.Sql("UPDATE dbo.HoSoGiaSu SET DangHienThi=0 WHERE Id=@id", new() { ["@id"] = f.Profile });
                    await using var scope = f.Scope(f.G);
                    var service = new HoSoGiaSuService(scope.Db, scope.Current, new MonHocCatalog(scope.Db), ProfileFiles(scope, root), scope.Notices);
                    var original = await f.Version<HoSoGiaSu>(f.Profile);
                    if (key == "profile_http")
                    {
                        var http=await PostProfile(f,true,Convert.ToBase64String(original));Assert.Contains(http.Status,new[]{200,302});
                        await using var db = f.Db();
                        var h = await db.Set<HoSoGiaSu>().SingleAsync(x => x.Id == f.Profile);
                        Assert.Equal("BanNhap", h.TrangThaiDuyet); Assert.False(h.DangHienThi);
                        Assert.NotNull(h.MinhChungTrinhDoId);
                        var subjects = await db.Set<HoSoGiaSuMonDay>().Where(x => x.HoSoGiaSuId == f.Profile).ToListAsync();
                        Assert.Equal(2, subjects.Count);
                    }
                    else
                    {
                        await service.CapNhatAsync(ProfileInput(f, original));
                        var before = await f.Sql("SELECT COUNT(*) AS N FROM dbo.TepDinhKem WHERE HoSoGiaSuId=@id", new() { ["@id"] = f.Profile });
                        var http=await PostProfile(f,true,Convert.ToBase64String(original));Assert.Equal(409,http.Status);
                        await Assert.ThrowsAnyAsync<Exception>(() => service.CapNhatAsync(ProfileInput(f, original)));
                        var after = await f.Sql("SELECT COUNT(*) AS N FROM dbo.TepDinhKem WHERE HoSoGiaSuId=@id", new() { ["@id"] = f.Profile });
                        Assert.Equal(before[0]["N"], after[0]["N"]);
                    }
                }
                finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
                break;
            }
            case "profile_missing":
            {
                var root = Path.Combine(Path.GetTempPath(), "EduConnect-IT-" + f.Tag);
                Directory.CreateDirectory(root);
                try
                {
                    await using var scope = f.Scope(f.G);
                    var service = new HoSoGiaSuService(scope.Db, scope.Current, new MonHocCatalog(scope.Db), ProfileFiles(scope, root), scope.Notices);
                    var input = ProfileInput(f, await f.Version<HoSoGiaSu>(f.Profile), withProof: false);
                    await Assert.ThrowsAnyAsync<Exception>(() => service.CapNhatAsync(input));
                    var http=await PostProfile(f,false,Convert.ToBase64String(await f.Version<HoSoGiaSu>(f.Profile)));Assert.Equal(200,http.Status);Assert.Contains("minh chứng",System.Net.WebUtility.HtmlDecode(http.Body),StringComparison.OrdinalIgnoreCase);
                    await using var db = f.Db();
                    var h = await db.Set<HoSoGiaSu>().AsNoTracking().SingleAsync(x => x.Id == f.Profile);
                    Assert.Equal("DaDuyet", h.TrangThaiDuyet); Assert.True(h.DangHienThi);
                    Assert.Empty(await db.Set<TepDinhKem>().Where(x => x.HoSoGiaSuId == f.Profile).ToListAsync());
                    Assert.Empty(await db.Set<HoSoGiaSuMonDay>().Where(x => x.HoSoGiaSuId == f.Profile).Select(x => x.MinhChungId).Where(x => x != null).ToListAsync());
                }
                finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
                break;
            }
            case "profile_hidden":
            {
                for (var dl = 0; dl < 2; dl++)
                {
                    await using var d = new Fixture(f.Id + "-DL" + dl);
                    await d.Init();
                    var root = Path.Combine(Path.GetTempPath(), "EduConnect-IT-" + d.Tag);
                    Directory.CreateDirectory(root);
                    try
                    {
                        await using var scope = d.Scope(d.G);
                        var files = ProfileFiles(scope, root);
                        var service = new HoSoGiaSuService(scope.Db, scope.Current, new MonHocCatalog(scope.Db), files, scope.Notices);
                        if (dl == 0)
                        {
                            var rv = await d.Version<HoSoGiaSu>(d.Profile);
                            var error = await Assert.ThrowsAsync<LoiNghiepVu>(() => service.CapNhatAsync(ProfileInput(d, rv)));
                            Assert.Equal(409, error.Status);
                            await using var db = d.Db();
                            var unchanged = await db.Set<HoSoGiaSu>().SingleAsync(x => x.Id == d.Profile);
                            Assert.Equal("DaDuyet", unchanged.TrangThaiDuyet); Assert.True(unchanged.DangHienThi);
                        }
                        else
                        {
                            await d.Sql("UPDATE dbo.HoSoGiaSu SET DangHienThi=0 WHERE Id=@id", new() { ["@id"] = d.Profile });
                            await service.CapNhatAsync(ProfileInput(d, await d.Version<HoSoGiaSu>(d.Profile)));
                            var newRv = Convert.ToBase64String(await d.Version<HoSoGiaSu>(d.Profile));
                            var error = await Assert.ThrowsAsync<LoiNghiepVu>(() => service.DoiTrangThaiAsync("Hien", newRv));
                            Assert.Equal(400, error.Status);
                            await using var db = d.Db();
                            var saved = await db.Set<HoSoGiaSu>().SingleAsync(x => x.Id == d.Profile);
                            Assert.Equal("BanNhap", saved.TrangThaiDuyet); Assert.False(saved.DangHienThi);
                            Assert.Equal(1, await db.Set<TepDinhKem>().CountAsync(x => x.HoSoGiaSuId == d.Profile));
                        }
                    }
                    finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
                }
                break;
            }
            case "profile_store":
            {
                var root = await SaveProfileFixture(f, false);
                try
                {
                    await using var db = f.Db();
                    Assert.Equal(2, await db.Set<HoSoGiaSuMonDay>().CountAsync(x => x.HoSoGiaSuId == f.Profile));
                    var files = await db.Set<TepDinhKem>().Where(x => x.HoSoGiaSuId == f.Profile).ToListAsync();
                    Assert.Single(files);
                    Assert.Single(files.Select(x => x.Checksum).Distinct());
                    Assert.DoesNotContain(files, x => x.HoSoGiaSuId == null);
                }
                finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
                break;
            }
            case "profile_review":
            {
                var root = await SaveProfileFixture(f, true);
                try
                {
                    await using (var claimsDb = f.Db())
                    {
                        claimsDb.UserClaims.Add(new IdentityUserClaim<Guid> { UserId = f.A, ClaimType = "EduConnect.AdminPermission", ClaimValue = "DuyetHoSo" });
                        await claimsDb.SaveChangesAsync();
                    }
                    await using (var admin = f.Scope(f.A))
                    {
                        var reviews = new HoSoGiaSuService(admin.Db, admin.Current, new MonHocCatalog(admin.Db), ProfileFiles(admin, root), admin.Notices);
                        await reviews.DuyetAsync(f.Profile, Convert.ToBase64String(await f.Version<HoSoGiaSu>(f.Profile)), true, null);
                    }
                    await using (var tutor = f.Scope(f.G))
                    {
                        var service = new HoSoGiaSuService(tutor.Db, tutor.Current, new MonHocCatalog(tutor.Db), ProfileFiles(tutor, root), tutor.Notices);
                        await service.DoiTrangThaiAsync("Hien", Convert.ToBase64String(await f.Version<HoSoGiaSu>(f.Profile)));
                    }
                    await using var db = f.Db();
                    var h = await db.Set<HoSoGiaSu>().SingleAsync(x => x.Id == f.Profile);
                    Assert.Equal("DaDuyet", h.TrangThaiDuyet); Assert.True(h.DangHienThi); Assert.Equal(f.A, h.NguoiDuyetId); Assert.NotNull(h.DuyetLuc);
                    Assert.True(await db.Set<NhatKyQuanTri>().AnyAsync(x => x.DoiTuongId == f.Profile.ToString() && x.HanhDong == "DuyetHoSo"));
                    Assert.True(await db.Set<ThongBao>().AnyAsync(x => x.NguoiNhanId == f.G && x.LoaiThongBao == "HoSo" && x.DoiTuongId == f.Profile));
                    using var guest = await f.Client();
                    using var results = await guest.GetAsync("/TimKiem/TimKiem/GiaSu?TuKhoa=" + Uri.EscapeDataString(f.Tag));
                    Assert.Equal(System.Net.HttpStatusCode.OK, results.StatusCode);
                    var body = await results.Content.ReadAsStringAsync(); Assert.Contains(f.Profile.ToString(), body, StringComparison.OrdinalIgnoreCase);
                    Assert.DoesNotContain(f.H.ToString(), body, StringComparison.OrdinalIgnoreCase);
                }
                finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
                break;
            }
            case "profile_view_auth":
            {
                var root = await SaveProfileFixture(f, false);
                string? appEvidencePath=null;
                try
                {
                    await using(var scan=f.Db())await scan.Set<TepDinhKem>().Where(x=>x.HoSoGiaSuId==f.Profile).ExecuteUpdateAsync(s=>s.SetProperty(x=>x.TrangThaiQuet,"Sach"));
                    Guid evidenceId; string evidencePath; string expectedHash;
                    await using (var db=f.Db())
                    {
                        var profile=await db.Set<HoSoGiaSu>().SingleAsync(x=>x.Id==f.Profile);
                        evidenceId=profile.MinhChungTrinhDoId!.Value;
                        var file=await db.Set<TepDinhKem>().SingleAsync(x=>x.Id==evidenceId);
                        evidencePath=Path.Combine(root,"uploads",file.TenLuuTru);expectedHash=file.Checksum;
                        Assert.Equal("Sach",file.TrangThaiQuet);Assert.NotNull(file.HoSoGiaSuId);
                    }
                    appEvidencePath=Path.Combine(Fixture.Root,"uploads",Path.GetFileName(evidencePath));Directory.CreateDirectory(Path.GetDirectoryName(appEvidencePath)!);File.Copy(evidencePath,appEvidencePath,true);
                    using(var owner=await f.Client(f.G))
                    using(var preview=await owner.GetAsync("/NhuCau/HoSoGiaSu/XemTruoc"))
                    {Assert.Equal(System.Net.HttpStatusCode.OK,preview.StatusCode);Assert.Contains(f.Tag,await preview.Content.ReadAsStringAsync(),StringComparison.OrdinalIgnoreCase);}
                    var outcomes=new List<int>();
                    using(var learner=await f.Client(f.H))
                    using(var denied=await learner.GetAsync("/NhuCau/HoSoGiaSu/BangCap?id="+evidenceId))
                    {outcomes.Add((int)denied.StatusCode);}
                    await using(var adminDb=f.Db())
                    {adminDb.UserClaims.Add(new IdentityUserClaim<Guid>{UserId=f.A,ClaimType="EduConnect.AdminPermission",ClaimValue="DuyetHoSo"});await adminDb.SaveChangesAsync();}
                    using(var admin=await f.Client(f.A))
                    using(var download=await admin.GetAsync("/NhuCau/HoSoGiaSu/BangCap?id="+evidenceId))
                    {
                        outcomes.Add((int)download.StatusCode);
                        if(download.IsSuccessStatusCode){var bytes=await download.Content.ReadAsByteArrayAsync();Assert.Equal(expectedHash.ToLowerInvariant(),Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(bytes)).ToLowerInvariant());}
                    }
                    var a0=await f.User("QuanTri");
                    using(var unprivileged=await f.Client(a0))
                    using(var denied=await unprivileged.GetAsync("/NhuCau/HoSoGiaSu/BangCap?id="+evidenceId))
                    {outcomes.Add((int)denied.StatusCode);}
                    Assert.Equal(new[]{403,200,403},outcomes);
                    Assert.True(File.Exists(evidencePath));
                    await using var verify=f.Db();
                    Assert.Equal(1,await verify.Set<TepDinhKem>().CountAsync(x=>x.HoSoGiaSuId==f.Profile));
                    Assert.False(await verify.Set<HoSoGiaSu>().Where(x=>x.Id==f.Profile).Select(x=>x.DangHienThi).SingleAsync());
                }
                finally { if(appEvidencePath!=null&&File.Exists(appEvidencePath))File.Delete(appEvidencePath);if(Directory.Exists(root))Directory.Delete(root,true); }
                break;
            }
            case "admin_access":
            {
                var root = await SaveProfileFixture(f, true);
                try
                {
                    using (var ordinaryAdmin = await f.Client(f.A)) { ordinaryAdmin.DefaultRequestHeaders.TryAddWithoutValidation("X-Requested-With", "XMLHttpRequest"); using var denied = await ordinaryAdmin.GetAsync("/QuanTri/HoSo/Index"); Assert.Equal(System.Net.HttpStatusCode.Forbidden, denied.StatusCode); }
                    using (var tutor = await f.Client(f.G)) { tutor.DefaultRequestHeaders.TryAddWithoutValidation("X-Requested-With", "XMLHttpRequest"); using var denied = await tutor.GetAsync("/QuanTri/HoSo/Index"); Assert.Equal(System.Net.HttpStatusCode.Forbidden, denied.StatusCode); }
                    await using (var db = f.Db()) { db.UserClaims.Add(new IdentityUserClaim<Guid> { UserId = f.A, ClaimType = "EduConnect.AdminPermission", ClaimValue = "DuyetHoSo" }); await db.SaveChangesAsync(); }
                    using (var admin = await f.Client(f.A)) using (var allowed = await admin.GetAsync("/QuanTri/HoSo/Index")) { Assert.Equal(System.Net.HttpStatusCode.OK, allowed.StatusCode); Assert.Contains(f.Tag, await allowed.Content.ReadAsStringAsync()); }
                }
                finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
                break;
            }
            case "admin_profile_reject":
            {
                var root = await SaveProfileFixture(f, true);
                try
                {
                    await using (var db = f.Db()) { db.UserClaims.Add(new IdentityUserClaim<Guid> { UserId = f.A, ClaimType = "EduConnect.AdminPermission", ClaimValue = "DuyetHoSo" }); await db.SaveChangesAsync(); }
                    var original = await f.Version<HoSoGiaSu>(f.Profile);
                    using var admin = await f.Client(f.A);
                    using var missing = await f.Post(admin, "/QuanTri/HoSo/Duyet", new() { ["id"] = f.Profile.ToString(), ["rowVersion"] = Convert.ToBase64String(original), ["approve"] = "false" }, "/QuanTri/HoSo/ChiTiet?id=" + f.Profile);
                    Assert.Equal(System.Net.HttpStatusCode.UnprocessableEntity, missing.StatusCode);
                    using (var body = System.Text.Json.JsonDocument.Parse(await missing.Content.ReadAsStringAsync())) Assert.False(body.RootElement.GetProperty("ok").GetBoolean());
                    await using (var check = f.Db()) Assert.Equal("ChoDuyet", (await check.Set<HoSoGiaSu>().SingleAsync(x => x.Id == f.Profile)).TrangThaiDuyet);
                    using var accepted = await f.Post(admin, "/QuanTri/HoSo/Duyet", new() { ["id"] = f.Profile.ToString(), ["rowVersion"] = Convert.ToBase64String(original), ["approve"] = "false", ["reason"] = "Bổ sung minh chứng học vấn" }, "/QuanTri/HoSo/ChiTiet?id=" + f.Profile, ajax: false);
                    Assert.Equal(System.Net.HttpStatusCode.Redirect, accepted.StatusCode);
                    await using (var check = f.Db()) { var profile = await check.Set<HoSoGiaSu>().SingleAsync(x => x.Id == f.Profile); Assert.Equal("CanChinhSua", profile.TrangThaiDuyet); Assert.False(profile.DangHienThi); Assert.Equal(f.A, profile.NguoiDuyetId); }
                    using var stale = await f.Post(admin, "/QuanTri/HoSo/Duyet", new() { ["id"] = f.Profile.ToString(), ["rowVersion"] = Convert.ToBase64String(original), ["approve"] = "false", ["reason"] = "Thử phiên bản cũ" }, "/QuanTri/HoSo/ChiTiet?id=" + f.Profile);
                    Assert.Equal(System.Net.HttpStatusCode.UnprocessableEntity, stale.StatusCode);
                    await using (var check = f.Db()) Assert.Equal("CanChinhSua", (await check.Set<HoSoGiaSu>().SingleAsync(x => x.Id == f.Profile)).TrangThaiDuyet);
                }
                finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
                break;
            }
            default: throw new InvalidOperationException(key);
        }
    }
}
