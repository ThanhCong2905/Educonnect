using System.IO.Compression;
using System.Security.Cryptography;
using System.Diagnostics;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Infrastructure;

public sealed class TepService(AppDbContext db, NguoiDungMauService current, QuyenLopHocService rights, IWebHostEnvironment env, ILogger<TepService>? logger = null)
{
    private string Root => Path.Combine(env.ContentRootPath, "uploads");
    public void DonDepDaXoa(IEnumerable<string> keys)
    {
        foreach (var key in keys)
            try
            {
                File.Delete(Path.Combine(Root, key));
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { logger?.LogWarning(ex, "Unable to remove retired file {Key}", key); }
    }
    public async Task<List<(string Name, string Mime, byte[] Bytes)>> KiemTraAsync(List<IFormFile> files)
    {
        Rules.Check(files.Count <= 5, "Tối đa 5 tệp.");
        Rules.Check(files.Sum(f => f.Length) <= 50000000, "Tổng dung lượng tối đa 50 MB.");
        var result = new List<(string, string, byte[])>();
        foreach (var f in files)
        {
            Rules.Check(f.Length is > 0 and <= 10000000, "Mỗi tệp tối đa 10.000.000 byte.");
            var ext = Path.GetExtension(f.FileName).ToLowerInvariant();
            Rules.Check(new[] { ".pdf", ".jpg", ".jpeg", ".png", ".docx" }.Contains(ext), "Chỉ nhận PDF, JPG, PNG, DOCX.");
            using var stream = new MemoryStream();
            await f.CopyToAsync(stream);
            var bytes = stream.ToArray();
            var signature = ext switch
            {
                ".pdf" => bytes.AsSpan().StartsWith("%PDF-"u8),
                ".png" => bytes.AsSpan().StartsWith(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }),
                ".jpg" or ".jpeg" => bytes.AsSpan().StartsWith(new byte[] { 255, 216, 255 }),
                ".docx" => Docx(bytes),
                _ => false
            };
            Rules.Check(signature, "Nội dung tệp không khớp định dạng.");
            var name = Path.GetFileName(f.FileName);
            Rules.Check(name.Length <= 256, "Tên tệp quá dài.");
            await QuetMaDocAsync(bytes);
            result.Add((name, ext switch
            {
                ".pdf" => "application/pdf",
                ".png" => "image/png",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "image/jpeg"
            }, bytes));
        }
        return result;
    }

    private async Task QuetMaDocAsync(byte[] bytes)
    {
        var scanner = Path.Combine(env.ContentRootPath, "tools", "clamav", OperatingSystem.IsWindows() ? "clamscan.exe" : "clamscan");
        Rules.Check(File.Exists(scanner), "Tính năng tải tệp chưa sẵn sàng. Vui lòng liên hệ người quản lý hệ thống.");
        var quarantine = Path.Combine(Root, ".quarantine");
        Directory.CreateDirectory(quarantine);
        var path = Path.Combine(quarantine, Guid.NewGuid().ToString("N"));
        try
        {
            await File.WriteAllBytesAsync(path, bytes);
            var start = new ProcessStartInfo(scanner)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            start.ArgumentList.Add("--no-summary");
            start.ArgumentList.Add("--database=" + Path.Combine(env.ContentRootPath, "tools", "clamav", "database"));
            start.ArgumentList.Add(path);
            using var process = Process.Start(start) ?? throw new IOException("Không thể khởi động ClamAV.");
            var output = process.StandardOutput.ReadToEndAsync();
            var error = process.StandardError.ReadToEndAsync();
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            try { await process.WaitForExitAsync(timeout.Token); }
            catch (OperationCanceledException)
            {
                if (!process.HasExited) process.Kill(entireProcessTree: true);
                await process.WaitForExitAsync();
                throw;
            }
            await Task.WhenAll(output, error);
            Rules.Check(process.ExitCode == 0, process.ExitCode == 1 ? "Tệp bị từ chối vì không an toàn." : "Chưa thể kiểm tra an toàn của tệp. Hãy thử lại sau.");
        }
        catch (Exception ex) when (ex is IOException or System.ComponentModel.Win32Exception or OperationCanceledException)
        {
            logger?.LogWarning("ClamAV chưa sẵn sàng để kiểm tra tệp.");
            throw new LoiNghiepVu("Chưa thể kiểm tra an toàn của tệp. Hãy thử lại sau.");
        }
        finally { if (File.Exists(path)) File.Delete(path); }
    }
    private static bool Docx(byte[] bytes)
    {
        try
        {
            using var zip = new ZipArchive(new MemoryStream(bytes));
            return zip.GetEntry("[Content_Types].xml") != null && zip.GetEntry("word/document.xml") != null && zip.Entries.Count < 2000 && zip.Entries.Sum(e => e.Length) < 50000000 && !zip.Entries.Any(e => e.FullName.Contains("vbaProject", StringComparison.OrdinalIgnoreCase) || e.FullName.Contains(".."));
        }
        catch (InvalidDataException) { return false; }
    }
    public async Task<List<string>> LuuAsync(List<(string Name, string Mime, byte[] Bytes)> files, Guid? giao = null, Guid? nop = null, Guid? hoSo = null, Guid? tinNhan = null)
    {
        var u = await current.HienTaiAsync();
        Directory.CreateDirectory(Root);
        var written = new List<string>();
        try
        {
            foreach (var f in files)
            {
                var key = Guid.NewGuid().ToString("N");
                var path = Path.Combine(Root, key);
                await File.WriteAllBytesAsync(path, f.Bytes);
                written.Add(path);
                db.Add(new TepDinhKem { Id = Guid.NewGuid(), NguoiTaiLenId = u.Id, TenGoc = f.Name, TenLuuTru = key, LoaiNoiDung = f.Mime, KichThuoc = f.Bytes.LongLength, Checksum = Convert.ToHexString(SHA256.HashData(f.Bytes)), TrangThaiQuet = "Sach", TaoLuc = DateTimeOffset.UtcNow, BaiTapGiaoId = giao, BaiTapNopId = nop, HoSoGiaSuId = hoSo, TinNhanId = tinNhan });
            }
            return written;
        }
        catch { DonDep(written); throw; }
    }
    public static void DonDep(IEnumerable<string> paths)
    {
        foreach (var p in paths)
            File.Delete(p);
    }
    public async Task<List<TepDinhKem>> DanhSachAsync(Guid giao)
    {
        var a = await db.Set<BaiTapGiao>().SingleAsync(x => x.Id == giao);
        await rights.KiemTraQuyenXemAsync(a.LopHocId);
        var ids = await db.Set<BaiTapNop>().Where(x => x.BaiTapGiaoId == giao).Select(x => x.Id).ToListAsync();
        return await db.Set<TepDinhKem>().AsNoTracking().Where(x => x.BaiTapGiaoId == giao || (x.BaiTapNopId != null && ids.Contains(x.BaiTapNopId.Value))).ToListAsync();
    }
    public async Task<(TepDinhKem File, string Path)> TaiAsync(Guid id)
    {
        var f = await db.Set<TepDinhKem>().AsNoTracking().SingleOrDefaultAsync(x => x.Id == id && x.TrangThaiQuet == "Sach") ?? throw new LoiNghiepVu("Tệp chưa khả dụng.", 404);
        Guid? assignment = f.BaiTapGiaoId;
        if (assignment == null && f.BaiTapNopId != null)
            assignment = await db.Set<BaiTapNop>().Where(x => x.Id == f.BaiTapNopId).Select(x => (Guid?)x.BaiTapGiaoId).SingleOrDefaultAsync();
        Rules.Check(assignment != null, "Tệp không thuộc bài tập.", 403);
        var a = await db.Set<BaiTapGiao>().SingleAsync(x => x.Id == assignment);
        await rights.KiemTraQuyenXemAsync(a.LopHocId);
        var path = Path.Combine(Root, f.TenLuuTru);
        Rules.Check(File.Exists(path), "Tệp không còn trên máy chủ.", 404);
        return (f, path);
    }
    public string DuongDan(TepDinhKem file)
    {
        var path=Path.Combine(Root,file.TenLuuTru);
        Rules.Check(File.Exists(path),"Tệp không còn trên máy chủ.",404);
        return path;
    }
}
