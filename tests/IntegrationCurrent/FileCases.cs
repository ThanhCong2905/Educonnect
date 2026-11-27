using System.IO.Compression;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Sockets;
using System.Buffers.Binary;
using Xunit;

internal static partial class Cases
{
    static IFormFile Upload(string name, byte[] bytes) => new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Files", name)
    { Headers = new HeaderDictionary(), ContentType = "application/octet-stream" };

    static byte[] DocxBytes(params string[] entries)
    {
        using var stream = new MemoryStream();
        using (var zip = new ZipArchive(stream, ZipArchiveMode.Create, true))
            foreach (var name in entries)
            {
                var entry = zip.CreateEntry(name);
                using var writer = new StreamWriter(entry.Open());
                writer.Write("fixture");
            }
        return stream.ToArray();
    }

    static async Task<(TcpListener Listener, Task Server)> Scanner(string reply, bool hold = false)
    {
        var listener = new TcpListener(IPAddress.Loopback, 0); listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        var server = Task.Run(async () =>
        {
            using var client = await listener.AcceptTcpClientAsync();
            await using var stream = client.GetStream();
            var one = new byte[1];
            do { if (await stream.ReadAsync(one) == 0) return; } while (one[0] != 0);
            var header = new byte[4];
            while (true)
            {
                await stream.ReadExactlyAsync(header);
                var count = BinaryPrimitives.ReadInt32BigEndian(header);
                if (count == 0) break;
                var remaining = count; var buffer = new byte[Math.Min(count, 65536)];
                while (remaining > 0) { var read = await stream.ReadAsync(buffer.AsMemory(0, Math.Min(buffer.Length, remaining))); if (read == 0) throw new EndOfStreamException(); remaining -= read; }
            }
            if (hold) { await Task.Delay(TimeSpan.FromSeconds(14)); return; }
            await stream.WriteAsync(System.Text.Encoding.UTF8.GetBytes(reply + "\0"));
        });
        return (listener, server);
    }

    internal static async Task FileCases(Fixture f, string key)
    {
        switch (key)
        {
            case "file_store":
            {
                var root = Path.Combine(Path.GetTempPath(), "EduConnect-IT-" + f.Tag); Directory.CreateDirectory(root);
                var (listener, server) = await Scanner("stream: OK");
                try
                {
                    await using var scope = f.Scope(f.H);
                    var env = new TestEnvironment { EnvironmentName = "Integration", ContentRootPath = root, WebRootPath = Path.Combine(root, "wwwroot") };
                    var cfg = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["FileScanning:AllowUnscannedUploads"] = "false", ["FileScanning:ClamdPort"] = ((IPEndPoint)listener.LocalEndpoint).Port.ToString() }).Build();
                    var files = new TepService(scope.Db, scope.Current, scope.Rights, env, configuration: cfg);
                    var prepared = await files.KiemTraAsync([Upload("clean.pdf", System.Text.Encoding.ASCII.GetBytes("%PDF-1.7\nvalid\n%%EOF"))]);
                    Assert.Single(prepared);
                    var saved = await files.LuuAsync(prepared);
                    try { Assert.Equal("Sach", Assert.Single(scope.Db.Set<EduConnect.Mvc.Models.TepDinhKem>().Local).TrangThaiQuet); }
                    finally { TepService.DonDep(saved); }
                    await server;
                }
                finally { listener.Stop(); if (Directory.Exists(root)) Directory.Delete(root, true); }
                break;
            }
            case "file_scanner":
            {
                var root = Path.Combine(Path.GetTempPath(), "EduConnect-IT-" + f.Tag); Directory.CreateDirectory(root);
                try
                {
                    await using var scope = f.Scope(f.H);
                    var env = new TestEnvironment { EnvironmentName = "Integration", ContentRootPath = root, WebRootPath = Path.Combine(root, "wwwroot") };
                    var bytes = System.Text.Encoding.ASCII.GetBytes("%PDF-1.7\nvalid\n%%EOF");
                    foreach (var (reply, hold) in new[] { ("stream: Eicar-Test-Signature FOUND", false), ("stream: ERROR", false), ("", true) })
                    {
                        var (listener, server) = await Scanner(reply, hold);
                        try
                        {
                            var cfg = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["FileScanning:AllowUnscannedUploads"] = "false", ["FileScanning:ClamdPort"] = ((IPEndPoint)listener.LocalEndpoint).Port.ToString() }).Build();
                            var files = new TepService(scope.Db, scope.Current, scope.Rights, env, configuration: cfg);
                            var rejected = await Assert.ThrowsAsync<LoiNghiepVu>(() => files.KiemTraAsync([Upload("scan.pdf", bytes)]));
                            Assert.NotEmpty(rejected.Message);
                            Assert.DoesNotContain("Sach", rejected.Message, StringComparison.OrdinalIgnoreCase);
                            if (hold) Assert.True(await Task.WhenAny(server, Task.Delay(15_000)) == server);
                            else await server;
                        }
                        finally { listener.Stop(); }
                    }
                }
                finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
                break;
            }
            case "file_content":
            {
                var root = Path.Combine(Path.GetTempPath(), "EduConnect-IT-" + f.Tag);
                Directory.CreateDirectory(root);
                try
                {
                    await using var scope = f.Scope(f.H);
                    var env = new TestEnvironment { EnvironmentName = "Development", ContentRootPath = root, WebRootPath = Path.Combine(root, "wwwroot") };
                    var cfg = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["FileScanning:AllowUnscannedUploads"] = "true" }).Build();
                    var files = new TepService(scope.Db, scope.Current, scope.Rights, env, configuration: cfg);
                    var bad = new[]
                    {
                        Upload("wrong.pdf", new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }),
                        Upload("broken.docx", DocxBytes("[Content_Types].xml")),
                        Upload("macro.docx", DocxBytes("[Content_Types].xml", "word/document.xml", "word/vbaProject.bin"))
                    };
                    foreach (var upload in bad)
                        await Assert.ThrowsAsync<LoiNghiepVu>(() => files.KiemTraAsync([upload]));

                    var valid = Upload("../a.pdf", System.Text.Encoding.ASCII.GetBytes("%PDF-1.4\ncontent\n%%EOF"));
                    var prepared = await files.KiemTraAsync([valid]);
                    Assert.Single(prepared); Assert.Equal("a.pdf", prepared[0].Name); Assert.DoesNotContain("..", prepared[0].Name); Assert.False(Path.IsPathRooted(prepared[0].Name));
                    var written = await files.LuuAsync(prepared, giao: null);
                    try
                    {
                        var rootFull = Path.GetFullPath(Path.Combine(root, "uploads")) + Path.DirectorySeparatorChar;
                        var blob = Path.GetFullPath(Assert.Single(written));
                        Assert.StartsWith(rootFull, blob, StringComparison.OrdinalIgnoreCase);
                        Assert.Equal(32, Path.GetFileName(blob).Length); Assert.True(File.Exists(blob));
                    }
                    finally { TepService.DonDep(written); }
                }
                finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
                break;
            }
            case "file_development":
            {
                var root = Path.Combine(Path.GetTempPath(), "EduConnect-IT-" + f.Tag);
                Directory.CreateDirectory(root);
                try
                {
                    await using var scope = f.Scope(f.H);
                    var valid = Upload("development.pdf", System.Text.Encoding.ASCII.GetBytes("%PDF-1.4\ncontent\n%%EOF"));
                    var dev = new TestEnvironment { EnvironmentName = "Development", ContentRootPath = root, WebRootPath = Path.Combine(root, "wwwroot") };
                    var enabled = new TepService(scope.Db, scope.Current, scope.Rights, dev,
                        configuration: new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["FileScanning:AllowUnscannedUploads"] = "true" }).Build());
                    var prepared = await enabled.KiemTraAsync([valid]); Assert.Single(prepared);
                    var stored = await enabled.LuuAsync(prepared, giao: null);
                    try
                    {
                        var metadata = Assert.Single(scope.Db.Set<EduConnect.Mvc.Models.TepDinhKem>().Local);
                        Assert.Equal("ChoQuet", metadata.TrangThaiQuet); Assert.True(File.Exists(Assert.Single(stored)));
                    }
                    finally { TepService.DonDep(stored); }

                    var productionLike = new TestEnvironment { EnvironmentName = "Integration", ContentRootPath = root, WebRootPath = Path.Combine(root, "wwwroot") };
                    var closed = new TepService(scope.Db, scope.Current, scope.Rights, productionLike,
                        configuration: new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["FileScanning:AllowUnscannedUploads"] = "true" }).Build());
                    var rejected = await Assert.ThrowsAsync<LoiNghiepVu>(() => closed.KiemTraAsync([valid]));
                    Assert.Contains("chưa sẵn sàng", rejected.Message, StringComparison.OrdinalIgnoreCase);
                    Assert.Single(scope.Db.Set<EduConnect.Mvc.Models.TepDinhKem>().Local);
                }
                finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
                break;
            }
            case "file_boundary":
            {
                var root = Path.Combine(Path.GetTempPath(), "EduConnect-IT-" + f.Tag);
                Directory.CreateDirectory(root);
                try
                {
                    await using var scope = f.Scope(f.H);
                    var env = new TestEnvironment { EnvironmentName = "Development", ContentRootPath = root, WebRootPath = Path.Combine(root, "wwwroot") };
                    var cfg = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["FileScanning:AllowUnscannedUploads"] = "true" }).Build();
                    var files = new TepService(scope.Db, scope.Current, scope.Rights, env, configuration: cfg);
                    var pdf = new byte[] { 37, 80, 68, 70, 45 };
                    Assert.Equal(5, (await files.KiemTraAsync(Enumerable.Range(1, 5).Select(i => Upload($"ok{i}.pdf", pdf)).ToList())).Count);
                    var six = await Assert.ThrowsAsync<LoiNghiepVu>(() => files.KiemTraAsync(Enumerable.Range(1, 6).Select(i => Upload($"six{i}.pdf", pdf)).ToList()));
                    Assert.Equal("Files", six.Truong);
                    var exact = new byte[10_000_000]; pdf.CopyTo(exact, 0);
                    Assert.Single(await files.KiemTraAsync([Upload("exact.pdf", exact)]));
                    var over = new byte[10_000_001]; pdf.CopyTo(over, 0);
                    var tooLarge = await Assert.ThrowsAsync<LoiNghiepVu>(() => files.KiemTraAsync([Upload("over.pdf", over)]));
                    Assert.Equal("Files", tooLarge.Truong);
                }
                finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
                break;
            }
            case "file_transaction_failure":
            {
                await f.SeedContact();
                Guid room;
                await using (var db = f.Db()) room = await db.Set<EduConnect.Mvc.Models.CuocHoiThoai>().Where(x => x.LienHeId == f.Contact).Select(x => x.Id).SingleAsync();
                var root = Path.Combine(Path.GetTempPath(), "EduConnect-IT-" + f.Tag); Directory.CreateDirectory(root);
                await f.Sql("ALTER TABLE dbo.TepDinhKem ADD CONSTRAINT CK_IT_RejectUpload CHECK (TenGoc <> N'force-fail.pdf')");
                try
                {
                    await using var scope = f.Scope(f.H);
                    var env = new TestEnvironment { EnvironmentName = "Development", ContentRootPath = root, WebRootPath = Path.Combine(root, "wwwroot") };
                    var cfg = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["FileScanning:AllowUnscannedUploads"] = "true" }).Build();
                    var files = new TepService(scope.Db, scope.Current, scope.Rights, env, configuration: cfg);
                    var chat = new EduConnect.Mvc.Areas.HocTap.Services.TinNhanService(scope.Db, scope.Current, scope.Rights, files);
                    var op = Guid.NewGuid();
                    await Assert.ThrowsAnyAsync<Exception>(() => chat.GuiAsync(new EduConnect.Mvc.Areas.HocTap.ViewModels.GuiTinNhanViewModel
                    {
                        MaHoiThoai = room, MaThaoTac = op, NoiDung = "Gửi kèm file gặp lỗi lưu metadata", Files = [Upload("force-fail.pdf", System.Text.Encoding.ASCII.GetBytes("%PDF-1.7\nvalid\n%%EOF"))]
                    }));
                    await using var verify = f.Db();
                    Assert.False(await verify.Set<EduConnect.Mvc.Models.TinNhan>().AnyAsync(x => x.MaThaoTac == op));
                    Assert.False(await verify.Set<EduConnect.Mvc.Models.TepDinhKem>().AnyAsync(x => x.TenGoc == "force-fail.pdf"));
                    Assert.Empty(Directory.GetFiles(Path.Combine(root, "uploads")));
                }
                finally
                {
                    await f.Sql("ALTER TABLE dbo.TepDinhKem DROP CONSTRAINT CK_IT_RejectUpload");
                    if (Directory.Exists(root)) Directory.Delete(root, true);
                }
                break;
            }
            default: throw new InvalidOperationException(key);
        }
    }
}

