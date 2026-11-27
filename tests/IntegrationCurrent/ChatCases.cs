using System.Text.Json;
using System.Net;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

internal static partial class Cases
{
    internal static async Task ChatCases(Fixture f, string key)
    {
        switch (key)
        {
            case "file_download":
            {
                await f.SeedClass();
                Guid room;
                await using (var db = f.Db()) room = await db.Set<CuocHoiThoai>().Where(x => x.LopHocId == f.Class && x.LoaiHoiThoai == "TheoLop").Select(x => x.Id).SingleAsync();
                var bytes = System.Text.Encoding.ASCII.GetBytes("%PDF-1.7\nverified download\n%%EOF");
                var messageId = Guid.NewGuid(); var fileId = Guid.NewGuid(); var stored = Guid.NewGuid().ToString("N");
                var blob = Path.Combine(Fixture.Root, "uploads", stored); Directory.CreateDirectory(Path.GetDirectoryName(blob)!); await File.WriteAllBytesAsync(blob, bytes);
                try
                {
                    await using (var seed = f.Db())
                    {
                        seed.Add(new TinNhan { Id = messageId, CuocHoiThoaiId = room, NguoiGuiId = f.H, NoiDung = "File tải kiểm thử", GuiLuc = DateTimeOffset.UtcNow, MaThaoTac = Guid.NewGuid() });
                        await seed.SaveChangesAsync();
                        seed.Add(new TepDinhKem { Id = fileId, TinNhanId = messageId, NguoiTaiLenId = f.H, TenGoc = "download.pdf", TenLuuTru = stored, LoaiNoiDung = "application/pdf", KichThuoc = bytes.Length, Checksum = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(bytes)), TrangThaiQuet = "Sach", TaoLuc = DateTimeOffset.UtcNow });
                        await seed.SaveChangesAsync();
                    }
                    using (var owner = await f.Client(f.H))
                    using (var response = await owner.GetAsync($"/HocTap/TinNhan/Tai?id={fileId}"))
                    {
                        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                        var received = await response.Content.ReadAsByteArrayAsync();
                        Assert.Equal(Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(bytes)), Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(received)));
                        Assert.DoesNotContain(Fixture.Root, await response.Content.ReadAsStringAsync(), StringComparison.OrdinalIgnoreCase);
                    }
                    using (var outsider = await f.Client(f.X))
                    using (var denied = await outsider.GetAsync($"/HocTap/TinNhan/Tai?id={fileId}")) Assert.Equal(HttpStatusCode.Forbidden, denied.StatusCode);

                    var pendingId = Guid.NewGuid();
                    await using (var seed = f.Db()) { seed.Add(new TepDinhKem { Id = pendingId, TinNhanId = messageId, NguoiTaiLenId = f.H, TenGoc = "pending.pdf", TenLuuTru = Guid.NewGuid().ToString("N"), LoaiNoiDung = "application/pdf", KichThuoc = bytes.Length, Checksum = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(bytes)), TrangThaiQuet = "ChoQuet", TaoLuc = DateTimeOffset.UtcNow }); await seed.SaveChangesAsync(); }
                    using (var owner = await f.Client(f.H)) using (var rejected = await owner.GetAsync($"/HocTap/TinNhan/Tai?id={pendingId}")) Assert.Equal(HttpStatusCode.NotFound, rejected.StatusCode);
                    var absentId = Guid.NewGuid();
                    await using (var seed = f.Db()) { seed.Add(new TepDinhKem { Id = absentId, TinNhanId = messageId, NguoiTaiLenId = f.H, TenGoc = "missing.pdf", TenLuuTru = Guid.NewGuid().ToString("N"), LoaiNoiDung = "application/pdf", KichThuoc = bytes.Length, Checksum = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(bytes)), TrangThaiQuet = "Sach", TaoLuc = DateTimeOffset.UtcNow }); await seed.SaveChangesAsync(); }
                    using (var owner = await f.Client(f.H)) using (var missing = await owner.GetAsync($"/HocTap/TinNhan/Tai?id={absentId}")) Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
                }
                finally { if (File.Exists(blob)) File.Delete(blob); }
                break;
            }
            case "chat_http":
            {
                await f.SeedContact();
                Guid room;
                await using (var db = f.Db())
                    room = await db.Set<CuocHoiThoai>().Where(x => x.LienHeId == f.Contact).Select(x => x.Id).SingleAsync();
                var operation = Guid.NewGuid();
                using (var learner = await f.Client(f.H))
                {
                    var formUrl = $"/HocTap/TinNhan/Index?id={room}";
                    using var response = await f.Post(learner, "/HocTap/TinNhan/Gui", new()
                    {
                        ["MaHoiThoai"] = room.ToString(),
                        ["MaThaoTac"] = operation.ToString(),
                        ["NoiDung"] = "<script>alert(1)</script>",
                        ["NguoiGuiId"] = f.G.ToString()
                    }, formUrl);
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    using var result = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                    Assert.True(result.RootElement.GetProperty("ok").GetBoolean());
                    Assert.Contains("/HocTap/TinNhan/Index", result.RootElement.GetProperty("redirectUrl").GetString(), StringComparison.Ordinal);
                    using var history = await learner.GetAsync(formUrl);
                    Assert.Equal(HttpStatusCode.OK, history.StatusCode);
                    var html = await history.Content.ReadAsStringAsync();
                    Assert.Contains("&lt;script&gt;alert(1)&lt;/script&gt;", html, StringComparison.OrdinalIgnoreCase);
                    Assert.DoesNotContain("<script>alert(1)</script>", html, StringComparison.Ordinal);
                }
                await using var verify = f.Db();
                var saved = await verify.Set<TinNhan>().SingleAsync(x => x.MaThaoTac == operation);
                Assert.Equal(f.H, saved.NguoiGuiId);
                Assert.Equal("<script>alert(1)</script>", saved.NoiDung);
                break;
            }
            case "chat_history":
            {
                await f.SeedClass();
                var link = await Link(f);
                var memberId = await Member(f, link, consent: true, approved: true);
                await f.Sql("UPDATE dbo.ThanhVienLopHoc SET DeNghiLuc=DATEADD(minute,-31,SYSDATETIMEOFFSET()),DongYThamGiaLuc=DATEADD(minute,-30,SYSDATETIMEOFFSET()),DuyetLuc=DATEADD(minute,-30,SYSDATETIMEOFFSET()) WHERE Id=@m", new() { ["@m"] = memberId });
                Guid room;
                await using (var seed = f.Db())
                {
                    room = await seed.Set<CuocHoiThoai>().Where(x => x.LopHocId == f.Class && x.LoaiHoiThoai == "TheoLop").Select(x => x.Id).SingleAsync();
                    var now = DateTimeOffset.UtcNow;
                    var join = now.AddMinutes(-30);
                    var rows = new List<TinNhan>();
                    for (var i = 0; i < 80; i++)
                    {
                        var visible = i >= 20;
                        var ordinal = visible ? i - 20 : i;
                        var sent = visible
                            ? ordinal < 20 ? now.AddMinutes(-20) : now.AddMinutes(-20).AddSeconds(ordinal - 19)
                            : join.AddMinutes(-10).AddSeconds(i);
                        rows.Add(new TinNhan { Id = Guid.NewGuid(), CuocHoiThoaiId = room, NguoiGuiId = i % 2 == 0 ? f.H : f.G, NoiDung = $"MSG-{i:D2}", GuiLuc = sent, MaThaoTac = Guid.NewGuid() });
                    }
                    seed.AddRange(rows); await seed.SaveChangesAsync();
                }
                await using var parent = f.Scope(f.P);
                var service = parent.Chat;
                var first = await service.DuLieuAsync(room, null, null, null);
                using var json1 = JsonDocument.Parse(JsonSerializer.Serialize(first));
                var page1 = json1.RootElement.GetProperty("items").EnumerateArray().ToList();
                Assert.Equal(50, page1.Count); Assert.True(json1.RootElement.GetProperty("hasMore").GetBoolean());
                Assert.All(page1, row => Assert.True(int.Parse(row.GetProperty("text").GetString()![4..]) >= 20));
                var cursor = Guid.Parse(page1[0].GetProperty("id").GetString()!);
                var second = await service.DuLieuAsync(room, cursor, null, null);
                using var json2 = JsonDocument.Parse(JsonSerializer.Serialize(second));
                var page2 = json2.RootElement.GetProperty("items").EnumerateArray().ToList();
                Assert.Equal(10, page2.Count);
                var allIds = page1.Concat(page2).Select(row => row.GetProperty("id").GetString()).ToList();
                Assert.Equal(60, allIds.Distinct().Count());
                Assert.DoesNotContain(page1.Concat(page2), row => int.Parse(row.GetProperty("text").GetString()![4..]) < 20);
                break;
            }
            default: throw new InvalidOperationException(key);
        }
    }
}


