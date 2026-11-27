using System.Net;
using Microsoft.EntityFrameworkCore;
using Xunit;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

internal static partial class Cases
{
    internal static async Task AuthCases(Fixture f, string key)
    {
        switch (key)
        {
            case "auth_forgot_http":
            {
                string known;
                await using (var db = f.Db()) known = (await db.Users.Where(x => x.Id == f.H).Select(x => x.Email).SingleAsync())!;
                using var client = await f.Client();
                using var existing = await f.Post(client, "/TaiKhoan/QuenMatKhau", new() { ["Email"] = known }, "/TaiKhoan/QuenMatKhau", ajax: false);
                var existingBody = await existing.Content.ReadAsStringAsync();
                using var unknown = await f.Post(client, "/TaiKhoan/QuenMatKhau", new() { ["Email"] = f.Tag + "-missing@example.invalid" }, "/TaiKhoan/QuenMatKhau", ajax: false);
                var unknownBody = await unknown.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, existing.StatusCode); Assert.Equal(HttpStatusCode.OK, unknown.StatusCode);
                Assert.Equal(existingBody, unknownBody);
                Assert.Contains("Nếu email đã được xác minh", existingBody, StringComparison.Ordinal);
                Assert.DoesNotContain(known, existingBody, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain(known, unknownBody, StringComparison.OrdinalIgnoreCase);
                break;
            }
            case "auth_store":
            {
                var host = f.Host ??= new Factory();
                foreach (var role in new[] { "HocSinh", "PhuHuynh", "GiaSu" })
                {
                    var email = $"{f.Tag}-{role}@example.invalid";
                    using var scope = host.Services.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<TaiKhoanService>();
                    var result = await service.DangKyAsync(new DangKyInput { DongYChinhSach = true, HoTen = f.Tag + " " + role, Email = email, VaiTro = role, MatKhau = "ValidTest!Password!2026", XacNhanMatKhau = "ValidTest!Password!2026" });
                    Assert.True(result.Succeeded, string.Join(";", result.Errors.Select(x => x.Description)));
                    var manager = scope.ServiceProvider.GetRequiredService<UserManager<TaiKhoan>>();
                    var user = await manager.FindByEmailAsync(email); Assert.NotNull(user); Assert.Equal(f.Tag + " " + role, user!.HoTen); Assert.Equal(TaiKhoanService.PhienBanChinhSach, user.PhienBanChinhSach);
                    Assert.Equal(new[] { role }, await manager.GetRolesAsync(user));
                    var db = scope.ServiceProvider.GetRequiredService<EduConnect.Mvc.Data.AppDbContext>();
                    Assert.Equal(role == "HocSinh", await db.Set<HocSinh>().AnyAsync(x => x.TaiKhoanId == user.Id));
                    Assert.Equal(role == "PhuHuynh", await db.Set<PhuHuynh>().AnyAsync(x => x.TaiKhoanId == user.Id));
                    Assert.Equal(role == "GiaSu", await db.Set<GiaSu>().AnyAsync(x => x.TaiKhoanId == user.Id));
                }
                await using (var db = f.Db())
                {
                    var newUsers = await db.Users.Where(x => x.Email != null && x.Email.StartsWith(f.Tag + "-")).Select(x => x.Id).ToListAsync();
                    Assert.False(await db.UserClaims.AnyAsync(x => newUsers.Contains(x.UserId) && x.ClaimType == "EduConnect.AdminPermission"));
                    Assert.False(await db.UserRoles.Join(db.Roles, x => x.RoleId, r => r.Id, (x, r) => new { x.UserId, r.Name }).AnyAsync(x => newUsers.Contains(x.UserId) && x.Name == "QuanTri"));
                }
                break;
            }
            case "auth_verify":
            {
                string email; await using (var db = f.Db()) email = (await db.Users.Where(x => x.Id == f.H).Select(x => x.Email).SingleAsync())!;
                using var anonymous = await f.Client();
                using var ok = await f.Post(anonymous, "/TaiKhoan/DangNhap", new() { ["Email"] = email, ["MatKhau"] = Fixture.Password }, "/TaiKhoan/DangNhap", ajax: false);
                Assert.Equal(HttpStatusCode.Redirect, ok.StatusCode);
                await using (var db = f.Db()) Assert.True(await db.Set<PhienDangNhap>().AnyAsync(x => x.TaiKhoanId == f.H && x.ThuHoiLuc == null));
                using var wrong = await f.Client();
                using var bad = await f.Post(wrong, "/TaiKhoan/DangNhap", new() { ["Email"] = email, ["MatKhau"] = "Wrong!Password!2026" }, "/TaiKhoan/DangNhap", ajax: false);
                Assert.Equal(HttpStatusCode.OK, bad.StatusCode);
                await using (var db = f.Db())
                {
                    Assert.Equal(1, await db.Set<PhienDangNhap>().CountAsync(x => x.TaiKhoanId == f.H && x.ThuHoiLuc == null));
                    Assert.True(await db.Set<TrangThaiThuDangNhap>().AnyAsync(x => x.LoaiKhoa == "TaiKhoan" && x.GiaTriKhoa == email.ToUpperInvariant()));
                }
                break;
            }
            case "auth_reset_mail":
            {
                await f.Sql("UPDATE dbo.AspNetUsers SET EmailConfirmed=1 WHERE Id=@id", new() { ["@id"] = f.H });
                string email; await using (var db = f.Db()) email = (await db.Users.Where(x => x.Id == f.H).Select(x => x.Email).SingleAsync())!;
                var inbox = Path.Combine(Fixture.Root, ".local-mail"); Directory.CreateDirectory(inbox);
                var before = Directory.GetFiles(inbox, "*.json").ToHashSet(StringComparer.OrdinalIgnoreCase);
                string? created = null;
                try
                {
                    using var client = await f.Client();
                    using var response = await f.Post(client, "/TaiKhoan/QuenMatKhau", new() { ["Email"] = email }, "/TaiKhoan/QuenMatKhau", ajax: false);
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    created = Directory.GetFiles(inbox, "*.json").FirstOrDefault(path => !before.Contains(path) && File.ReadAllText(path).Contains(email, StringComparison.OrdinalIgnoreCase));
                    Assert.NotNull(created);
                    var mail = System.Text.Json.JsonSerializer.Deserialize<ThuTaiKhoan>(await File.ReadAllTextAsync(created!)); Assert.NotNull(mail);
                    Assert.Equal(email, mail!.Email); Assert.Contains("Đặt lại mật khẩu", mail.TieuDe, StringComparison.Ordinal);
                    var link = new Uri(mail.LienKet); Assert.Equal("https", link.Scheme); Assert.Equal("localhost", link.Host); Assert.Contains("/TaiKhoan/DatLaiMatKhau", link.AbsolutePath);
                    Assert.NotEmpty(link.Query); Assert.DoesNotContain(Fixture.Password, mail.LienKet, StringComparison.Ordinal);
                }
                finally { if (created != null && File.Exists(created)) File.Delete(created); }
                break;
            }
            case "auth_profile":
            {
                using var client = await f.Client(f.H);
                string email; string stamp; await using (var db = f.Db()) { var user = await db.Users.SingleAsync(x => x.Id == f.H); email = user.Email!; stamp = user.ConcurrencyStamp!; }
                using var first = await f.Post(client, "/TaiKhoan/HoSo", new() { ["HoTen"] = f.Tag + " Updated", ["SoDienThoai"] = "+84 912345678", ["Email"] = "attacker@example.invalid", ["ConcurrencyStamp"] = stamp }, "/TaiKhoan/HoSo?chinhSua=true", ajax: false);
                Assert.Equal(HttpStatusCode.Redirect, first.StatusCode);
                await using (var verify = f.Db()) { var user = await verify.Users.SingleAsync(x => x.Id == f.H); Assert.Equal(f.Tag + " Updated", user.HoTen); Assert.Equal("+84 912345678", user.PhoneNumber); Assert.Equal(email, user.Email); }
                using var stale = await f.Post(client, "/TaiKhoan/HoSo", new() { ["HoTen"] = f.Tag + " Stale", ["SoDienThoai"] = "+84 900000000", ["Email"] = "other@example.invalid", ["ConcurrencyStamp"] = stamp }, "/TaiKhoan/HoSo?chinhSua=true");
                Assert.Equal(HttpStatusCode.UnprocessableEntity, stale.StatusCode);
                await using (var verify = f.Db()) { var user = await verify.Users.SingleAsync(x => x.Id == f.H); Assert.Equal(f.Tag + " Updated", user.HoTen); Assert.Equal(email, user.Email); }
                break;
            }
            case "auth_rate_limit":
            {
                using var host = new Factory().WithWebHostBuilder(builder => builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string?> { ["Authentication:AccountRequestLimit"] = "20" })));
                using var client = host.CreateClient(new() { BaseAddress = new Uri("https://localhost"), AllowAutoRedirect = false, HandleCookies = true });
                var token = await Fixture.Token(client, "/TaiKhoan/DangKy");
                var statuses = new List<int>();
                for (var i = 0; i < 21; i++)
                {
                    client.DefaultRequestHeaders.Remove("X-Requested-With"); client.DefaultRequestHeaders.TryAddWithoutValidation("X-Requested-With", "XMLHttpRequest");
                    using var response = await client.PostAsync("/TaiKhoan/DangKy", new FormUrlEncodedContent(new Dictionary<string, string> { ["Email"] = "", ["HoTen"] = "", ["VaiTro"] = "HocSinh", ["MatKhau"] = "", ["XacNhanMatKhau"] = "", ["DongYChinhSach"] = "true", ["__RequestVerificationToken"] = token }));
                    statuses.Add((int)response.StatusCode);
                }
                Assert.All(statuses.Take(20), code => Assert.NotEqual(429, code)); Assert.Equal(429, statuses[20]);
                break;
            }
            case "auth_reset_http":
            {
                await f.Sql("UPDATE dbo.AspNetUsers SET EmailConfirmed=1 WHERE Id IN(@h,@x)", new() { ["@h"] = f.H, ["@x"] = f.X });
                using var host = new Factory();
                using var client = host.CreateClient(new() { BaseAddress = new Uri("https://localhost"), AllowAutoRedirect = false, HandleCookies = true });
                string email; string token; string wrongAccountToken;
                using (var scope = host.Services.CreateScope())
                {
                    var manager = scope.ServiceProvider.GetRequiredService<UserManager<TaiKhoan>>();
                    var user = await manager.FindByIdAsync(f.H.ToString()); var other = await manager.FindByIdAsync(f.X.ToString()); Assert.NotNull(user); Assert.NotNull(other);
                    email = user!.Email!; token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(await manager.GeneratePasswordResetTokenAsync(user)));
                    wrongAccountToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(await manager.GeneratePasswordResetTokenAsync(other!)));
                }
                using (var changed = await f.Post(client, "/TaiKhoan/DatLaiMatKhau", new() { ["Email"] = email, ["Ma"] = (token[0] == 'A' ? "B" : "A") + token[1..], ["MatKhau"] = "Reset!Password!2026", ["XacNhanMatKhau"] = "Reset!Password!2026" }, "/TaiKhoan/DatLaiMatKhau?email=" + Uri.EscapeDataString(email) + "&ma=" + Uri.EscapeDataString(token))) Assert.Equal(HttpStatusCode.UnprocessableEntity, changed.StatusCode);
                using (var other = await f.Post(client, "/TaiKhoan/DatLaiMatKhau", new() { ["Email"] = email, ["Ma"] = wrongAccountToken, ["MatKhau"] = "Reset!Password!2026", ["XacNhanMatKhau"] = "Reset!Password!2026" }, "/TaiKhoan/DatLaiMatKhau?email=" + Uri.EscapeDataString(email) + "&ma=" + Uri.EscapeDataString(wrongAccountToken))) Assert.Equal(HttpStatusCode.UnprocessableEntity, other.StatusCode);
                using (var accepted = await f.Post(client, "/TaiKhoan/DatLaiMatKhau", new() { ["Email"] = email, ["Ma"] = token, ["MatKhau"] = "Reset!Password!2026", ["XacNhanMatKhau"] = "Reset!Password!2026" }, "/TaiKhoan/DatLaiMatKhau?email=" + Uri.EscapeDataString(email) + "&ma=" + Uri.EscapeDataString(token), ajax: false)) Assert.Equal(HttpStatusCode.Redirect, accepted.StatusCode);
                using (var reused = await f.Post(client, "/TaiKhoan/DatLaiMatKhau", new() { ["Email"] = email, ["Ma"] = token, ["MatKhau"] = "Reset!Password!2027", ["XacNhanMatKhau"] = "Reset!Password!2027" }, "/TaiKhoan/DatLaiMatKhau?email=" + Uri.EscapeDataString(email) + "&ma=" + Uri.EscapeDataString(token))) Assert.Equal(HttpStatusCode.UnprocessableEntity, reused.StatusCode);
                using (var scope = host.Services.CreateScope()) { var manager = scope.ServiceProvider.GetRequiredService<UserManager<TaiKhoan>>(); var user = await manager.FindByIdAsync(f.H.ToString()); Assert.NotNull(user); Assert.True(await manager.CheckPasswordAsync(user!, "Reset!Password!2026")); Assert.False(await manager.CheckPasswordAsync(user!, "Reset!Password!2027")); }
                break;
            }
            case "auth_avatar":
            {
                await f.SeedNeed(); f.AllowUnscannedUploads = true;
                using var client = await f.Client(f.H);
                var url = "/TaiKhoan/HoSo?chinhSua=true";
                async Task<HttpResponseMessage> SaveAvatar(string? name, byte[]? bytes, bool remove)
                {
                    string stamp, email, nameValue, phone;
                    await using (var db = f.Db()) { var user = await db.Users.SingleAsync(x => x.Id == f.H); stamp = user.ConcurrencyStamp!; email = user.Email!; nameValue = user.HoTen; phone = user.PhoneNumber ?? ""; }
                    using var form = new MultipartFormDataContent();
                    form.Add(new StringContent(nameValue), "HoTen"); form.Add(new StringContent(phone), "SoDienThoai"); form.Add(new StringContent(email), "Email"); form.Add(new StringContent(stamp), "ConcurrencyStamp"); form.Add(new StringContent(remove ? "true" : "false"), "XoaAnhDaiDien"); form.Add(new StringContent(await Fixture.Token(client, url)), "__RequestVerificationToken");
                    if (name != null && bytes != null) form.Add(new ByteArrayContent(bytes), "AnhDaiDienFile", name);
                    client.DefaultRequestHeaders.Remove("X-Requested-With");
                    return await client.PostAsync("/TaiKhoan/HoSo", form);
                }
                var png = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 0, 73, 69, 78, 68 };
                string? avatarPath = null;
                try
                {
                    using (var uploaded = await SaveAvatar("avatar.png", png, false)) Assert.Equal(HttpStatusCode.Redirect, uploaded.StatusCode);
                    string avatarKey; await using (var db = f.Db()) avatarKey = (await db.Users.SingleAsync(x => x.Id == f.H)).AnhDaiDien!;
                    avatarPath = Path.Combine(Fixture.Root, "uploads", avatarKey.Replace('/', Path.DirectorySeparatorChar)); Assert.True(File.Exists(avatarPath));
                    using (var image = await client.GetAsync("/TaiKhoan/Anh/" + f.H)) { Assert.Equal(HttpStatusCode.OK, image.StatusCode); Assert.Equal(png, await image.Content.ReadAsByteArrayAsync()); }
                    using (var rejected = await SaveAvatar("fake.jpg", png, false)) Assert.Equal(HttpStatusCode.OK, rejected.StatusCode);
                    await using (var db = f.Db()) Assert.Equal(avatarKey, (await db.Users.SingleAsync(x => x.Id == f.H)).AnhDaiDien);
                    using (var removed = await SaveAvatar(null, null, true)) Assert.Equal(HttpStatusCode.Redirect, removed.StatusCode);
                    await using (var db = f.Db()) Assert.Null((await db.Users.SingleAsync(x => x.Id == f.H)).AnhDaiDien);
                    Assert.False(File.Exists(avatarPath));
                }
                finally { if (avatarPath != null && File.Exists(avatarPath)) File.Delete(avatarPath); }
                break;
            }
            default: throw new InvalidOperationException(key);
        }
    }
}
