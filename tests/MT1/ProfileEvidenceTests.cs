using System.Security.Claims;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

public sealed class ProfileEvidenceTests
{
    [Theory]
    [InlineData(false, "Chưa có", false, "TepTrinhDo")]
    [InlineData(false, "1–3 năm", false, "TepTrinhDo")]
    [InlineData(true, "1–3 năm", false, "TepKinhNghiem")]
    public async Task Save_reports_missing_evidence_before_writing_files(
        bool hasDegree, string experience, bool hasExperience, string expectedField)
    {
        await using var db = ProposalRegressionTests.Open();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var seed = await ProposalRegressionTests.Seed(db);
        var stored = await db.Set<HoSoGiaSu>().SingleAsync(profile => profile.GiaSuId == seed.Tutor);
        stored.DangHienThi = false;
        stored.TrangThaiDuyet = "BanNhap";
        await db.SaveChangesAsync();
        var version = stored.RowVersion.ToArray();
        var http = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim(ClaimTypes.NameIdentifier, seed.Tutor.ToString())], "Test"))
        };
        var current = new NguoiDungHienTaiService(db, new HttpContextAccessor { HttpContext = http });
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(
            new Dictionary<string, string?> { ["FileScanning:AllowUnscannedUploads"] = "true" }).Build();
        var files = new TepService(db, current, new QuyenLopHocService(db, current),
            new LocalEnv { EnvironmentName = "Development" }, configuration: configuration);
        var service = new HoSoGiaSuService(db, current, new MonHocCatalog(db), files,
            new ThongBaoService(db, current));
        var input = new HoSoInput
        {
            HoTen = "Gia sư kiểm thử",
            TrinhDo = "Đại học",
            KinhNghiem = experience,
            MoTa = new string('a', 100),
            KhuVuc = "Hà Nội",
            DayTrucTuyen = true,
            RowVersion = Convert.ToBase64String(version),
            TepTrinhDo = hasDegree ? Image() : null,
            TepKinhNghiem = hasExperience ? Image() : null,
            MonDay = [new() { MonHocId = 1, LopDay = [12], HocPhiDeXuat = 0, DonViHocPhi = "VND/Gio" }]
        };

        var error = await Assert.ThrowsAsync<LoiNghiepVu>(() => service.CapNhatAsync(input));

        Assert.Equal(expectedField, error.Truong);
        var saved = await db.Set<HoSoGiaSu>().AsNoTracking().SingleAsync(profile => profile.Id == stored.Id);
        Assert.Equal(version, saved.RowVersion);
        Assert.False(await db.Set<TepDinhKem>().AnyAsync(file => file.HoSoGiaSuId == stored.Id));
    }

    private static IFormFile Image()
    {
        var bytes = Convert.FromBase64String(
            "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+/nD8AAAAASUVORK5CYII=");
        return new FormFile(new MemoryStream(bytes), 0, bytes.Length, "file", "minh-chung.png")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/png"
        };
    }
}
