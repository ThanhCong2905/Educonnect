using System.Net;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using EduConnect.Mvc.Areas.QuanTri.Services;
using EduConnect.Mvc.Areas.QuanTri.ViewModels;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

internal static class IntegrationLogCapture
{
    private static readonly ConcurrentQueue<string> Entries = new();
    internal static ILoggerProvider Provider { get; } = new ProviderImpl();
    internal static void Clear() { while (Entries.TryDequeue(out _)) { } }
    internal static string[] Snapshot() => Entries.ToArray();
    private sealed class ProviderImpl : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName) => new LoggerImpl(categoryName);
        public void Dispose() { }
    }
    private sealed class LoggerImpl(string category) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Warning;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        { if (IsEnabled(logLevel)) Entries.Enqueue($"{category}: {formatter(state, exception)} {exception}"); }
    }
}

internal static partial class Cases
{
    internal static async Task AuditCases(Fixture f, string key)
    {
        switch (key)
        {
            case "audit_store":
            {
                using var client = await f.Client(f.H);
                using var page = await client.GetAsync("/TaiKhoan/HoSo"); Assert.Equal(HttpStatusCode.OK, page.StatusCode);
                await using var verify = f.Db();
                var row = await verify.Set<NhatKyAnToan>().Where(x => x.Nhom == "Authentication" && x.SuKien == "TaiKhoan.DangNhap" && x.TaiKhoanId == f.H).OrderByDescending(x => x.ThoiDiem).FirstAsync();
                Assert.Equal("ThanhCong", row.KetQua); Assert.False(string.IsNullOrWhiteSpace(row.MaYeuCau)); Assert.Equal("POST", row.PhuongThuc);
                var protectedFields = string.Join("|", row.Nhom, row.SuKien, row.KetQua, row.LyDo, row.DuongDan, row.MaYeuCau, row.DinhDanhBam);
                Assert.DoesNotContain(Fixture.Password, protectedFields, StringComparison.Ordinal);
                break;
            }
            case "audit_http":
            {
                using (var learner = await f.Client(f.H))
                {
                    using var sent = await f.Post(learner, "/KetNoi/KetNoi/GuiLienHe", new() { ["DoiTuongId"] = f.G.ToString(), ["LoiNhan"] = "Gửi lời mời kiểm toán" });
                    Assert.Equal(HttpStatusCode.OK, sent.StatusCode);
                    using var denied = await learner.GetAsync("/QuanTri/QuanTri/Index"); Assert.Equal(HttpStatusCode.Forbidden, denied.StatusCode);
                    using var csrf = await learner.PostAsync("/KetNoi/KetNoi/GuiLienHe", new FormUrlEncodedContent(new Dictionary<string, string> { ["DoiTuongId"] = f.G.ToString(), ["LoiNhan"] = "Thiếu CSRF" }));
                    Assert.Equal(HttpStatusCode.BadRequest, csrf.StatusCode);
                }

                var tempRoot = await SaveProfileFixture(f, false);
                string? appBlob = null;
                try
                {
                    Guid fileId; string keyOnDisk;
                    await using (var db = f.Db())
                    {
                        var profile = await db.Set<HoSoGiaSu>().SingleAsync(x => x.Id == f.Profile);
                        fileId = profile.MinhChungTrinhDoId!.Value;
                        var file = await db.Set<TepDinhKem>().SingleAsync(x => x.Id == fileId);
                        keyOnDisk = file.TenLuuTru;
                        file.TrangThaiQuet = "Sach"; await db.SaveChangesAsync();
                    }
                    var source = Path.Combine(tempRoot, "uploads", keyOnDisk);
                    appBlob = Path.Combine(Fixture.Root, "uploads", Path.GetFileName(keyOnDisk));
                    Directory.CreateDirectory(Path.GetDirectoryName(appBlob)!); File.Copy(source, appBlob, true);
                    await using (var db = f.Db()) { db.UserClaims.Add(new IdentityUserClaim<Guid> { UserId = f.A, ClaimType = "EduConnect.AdminPermission", ClaimValue = "DuyetHoSo" }); await db.SaveChangesAsync(); }
                    using (var admin = await f.Client(f.A)) using (var download = await admin.GetAsync("/NhuCau/HoSoGiaSu/BangCap?id=" + fileId)) Assert.Equal(HttpStatusCode.OK, download.StatusCode);
                }
                finally { if (appBlob != null && File.Exists(appBlob)) File.Delete(appBlob); if (Directory.Exists(tempRoot)) Directory.Delete(tempRoot, true); }

                await using var verify = f.Db();
                var audit = await verify.Set<NhatKyAnToan>().AsNoTracking().ToListAsync();
                Assert.Contains(audit, x => x.Nhom == "Accounting" && x.SuKien == "KetNoi.GuiLienHe" && x.KetQua == "ThanhCong" && x.TaiKhoanId == f.H);
                Assert.Contains(audit, x => x.Nhom == "Authorization" && x.KetQua == "TuChoi");
                Assert.Contains(audit, x => x.Nhom == "Accounting" && x.SuKien == "HoSoGiaSu.BangCap" && x.KetQua == "ThanhCong" && x.DoiTuongId != null);
                Assert.DoesNotContain(audit, x => string.Join("|", x.Nhom, x.SuKien, x.KetQua, x.LyDo, x.DuongDan, x.DinhDanhBam).Contains(Fixture.Password, StringComparison.Ordinal));
                break;
            }
            case "audit_failure":
            {
                IntegrationLogCapture.Clear();
                await f.Sql("CREATE OR ALTER TRIGGER dbo.TR_IT_RejectAudit ON dbo.NhatKyAnToan AFTER INSERT AS BEGIN THROW 51099,N'IT audit storage failure',1; END");
                try
                {
                    using var learner = await f.Client(f.H);
                    using var response = await f.Post(learner, "/KetNoi/KetNoi/GuiLienHe", new() { ["DoiTuongId"] = f.G.ToString(), ["LoiNhan"] = "Nghiệp vụ phải giữ sau khi audit lỗi" });
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    await using var verify = f.Db();
                    Assert.Single(await verify.Set<YeuCauLienHe>().Where(x => x.GiaSuId == f.G && x.BenHocId == f.H && x.LoiNhan == "Nghiệp vụ phải giữ sau khi audit lỗi").ToListAsync());
                    Assert.False(await verify.Set<NhatKyAnToan>().AnyAsync(x => x.SuKien == "KetNoi.GuiLienHe" && x.TaiKhoanId == f.H));
                    Assert.Contains(IntegrationLogCapture.Snapshot(), x => x.Contains("Không ghi được nhật ký an toàn", StringComparison.OrdinalIgnoreCase) && x.Contains("51099", StringComparison.Ordinal));
                }
                finally { await f.Sql("DROP TRIGGER IF EXISTS dbo.TR_IT_RejectAudit"); }
                break;
            }
            case "audit_query":
            {
                var day = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7).Date);
                var first = new DateTimeOffset(day.ToDateTime(new TimeOnly(8, 0)), TimeSpan.FromHours(7));
                await using (var db = f.Db())
                {
                    db.AddRange(Enumerable.Range(0, 23).Select(i => new NhatKyAnToan
                    {
                        Id = Guid.NewGuid(), ThoiDiem = first.AddMinutes(i), Nhom = i % 2 == 0 ? "Authentication" : "Accounting",
                        SuKien = f.Tag + " event " + i, KetQua = i % 3 == 0 ? "ThatBai" : "ThanhCong",
                        TaiKhoanId = f.H, TaiKhoanDichId = i == 7 ? f.G : null, DiaChiIp = i < 22 ? "192.0.2.44" : "192.0.2.45",
                        DuongDan = "/integration", PhuongThuc = "GET", MaYeuCau = f.Tag + "-" + i
                    }));
                    await db.SaveChangesAsync();
                }
                await using (var deniedDb = f.Db(true))
                {
                    var deniedHttp = AuditHttp(f, f.H); var deniedCurrent = new NguoiDungHienTaiService(deniedDb, deniedHttp);
                    var deniedQuery = new NhatKyAnToanQuery(deniedDb, new QuyenQuanTriService(deniedDb, deniedCurrent, deniedHttp));
                    await Assert.ThrowsAsync<LoiNghiepVu>(() => deniedQuery.LayAsync(new NhatKyAnToanFilter(), default));
                }
                await GiveAdmin(f, f.A, "XemNhatKy");
                await using var queryDb = f.Db(true);
                var queryHttp = AuditHttp(f, f.A); var current = new NguoiDungHienTaiService(queryDb, queryHttp);
                var query = new NhatKyAnToanQuery(queryDb, new QuyenQuanTriService(queryDb, current, queryHttp));
                var p1 = await query.LayAsync(new NhatKyAnToanFilter { TuNgay = day, DenNgay = day, Nhom = "Authentication", Ip = "192.0.2.44", Q = f.Tag }, default);
                Assert.Equal(11, p1.Total); Assert.Equal(11, p1.Items.Count); Assert.Equal(11, p1.Items.Select(x => x.BanGhi.Id).Distinct().Count());
                Assert.All(p1.Items, x => { Assert.Equal("Authentication", x.BanGhi.Nhom); Assert.Equal("192.0.2.44", x.BanGhi.DiaChiIp); Assert.Contains(f.Tag, x.BanGhi.SuKien); });
                var page1 = await query.LayAsync(new NhatKyAnToanFilter { TuNgay = day, DenNgay = day, Ip = "192.0.2.44", Q = f.Tag }, default);
                var page2 = await query.LayAsync(new NhatKyAnToanFilter { Trang = 2, TuNgay = day, DenNgay = day, Ip = "192.0.2.44", Q = f.Tag }, default);
                Assert.Equal(22, page1.Total); Assert.Equal(20, page1.Items.Count); Assert.Equal(2, page2.Items.Count);
                Assert.Empty(page1.Items.Select(x => x.BanGhi.Id).Intersect(page2.Items.Select(x => x.BanGhi.Id)));
                var rejected = await Assert.ThrowsAsync<LoiNghiepVu>(() => query.LayAsync(new NhatKyAnToanFilter { TuNgay = day.AddDays(1), DenNgay = day }, default));
                Assert.Equal(400, rejected.Status);
                break;
            }
            default: throw new InvalidOperationException(key);
        }
    }

    private static PlainAccessor AuditHttp(Fixture f, Guid actor)
    {
        return new PlainAccessor { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, actor.ToString()), new Claim(ClaimTypes.Role, f.Roles[actor])], "IntegrationDriver")) } };
    }
}
