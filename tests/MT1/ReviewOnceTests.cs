using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using EduConnect.Mvc.Areas.DanhGia.Services;
using EduConnect.Mvc.Areas.DanhGia.ViewModels;
using EduConnect.Mvc.Areas.KetNoi.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Xunit;

public sealed class ReviewOnceTests
{
    private static DanhGiaDoiTacService Service(AppDbContext db, Guid actor)
    {
        var http = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(ClaimTypes.NameIdentifier, actor.ToString())], "Test")) };
        var current = new NguoiDungHienTaiService(db, new HttpContextAccessor { HttpContext = http });
        return new(db, current, new QuyenLopHocService(db, current), new ThongBaoService(db, current));
    }

    private static DanhGiaDoiTacViewModel Input(Guid classroom, int stars = 4) => new()
    {
        MaLop = classroom, SoSao = stars, NhanXet = "Hai bên phối hợp tốt trong quá trình học.", XacNhanRiengTu = true
    };

    private static async Task<(Guid Classroom, Guid Tutor, Guid Learner)> Seed(AppDbContext db, bool ended = true, bool taught = true)
    {
        var people = await ProposalRegressionTests.Seed(db);
        var request = await ProposalRegressionTests.Service(db, people.Tutor).GuiDeXuatAsync(new DeXuatLopViewModel
        {
            LienHeId = people.Contact, TenNguoiHoc = "Học sinh kiểm thử", TenLop = "Kiểm thử đánh giá", MucTieu = "Ôn tập", HocPhi = 0,
            ThoiLuongPhut = 60, BatDauHocLuc = DateTime.Today.AddDays(7),
            LichRanh = [new() { ThuTrongTuan = 1, GioBatDau = new(19, 0), GioKetThuc = new(20, 0) }]
        });
        var saved = await db.Set<YeuCauKetNoiLopHoc>().AsNoTracking().SingleAsync(x => x.Id == request);
        var classroom = await ProposalRegressionTests.Service(db, people.Learner)
            .ChapNhanDeXuatAsync(request, Convert.ToBase64String(saved.RowVersion));
        if (taught)
        {
            await db.Database.ExecuteSqlInterpolatedAsync($"""
                DECLARE @slot uniqueidentifier=NEWID();
                INSERT dbo.LichHoc(Id,LopHocId,TieuDe,BatDauLuc,KetThucLuc,TrangThai)
                VALUES(@slot,{classroom},N'Buổi đã học',DATEADD(hour,-3,SYSDATETIMEOFFSET()),DATEADD(hour,-2,SYSDATETIMEOFFSET()),'DaLenLich');
                DECLARE @v binary(8)=(SELECT RowVersion FROM dbo.BuoiHoc WHERE LichHocId=@slot);
                EXEC dbo.usp_GhiNhanBuoiHoc @LichHocId=@slot,@GiaSuId={people.Tutor},@KetQuaBuoiHoc='DaDienRa',
                    @NhanXet=N'Đã học',@TrangThaiGiaSu='CoMat',@TrangThaiBenHoc='CoMat',@NoiDung=N'Ôn tập',@RowVersionCu=@v;
                """);
        }
        if (ended)
            await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.LopHoc SET TrangThai='DaKetThuc',KetThucLuc=SYSDATETIMEOFFSET(),CachKetThuc='DongY' WHERE Id={classroom}");
        db.ChangeTracker.Clear();
        return (classroom, people.Tutor, people.Learner);
    }

    [Fact]
    public async Task Each_side_rates_once_and_second_submission_keeps_original()
    {
        await using var db = ProposalRegressionTests.Open();
        var seed = await Seed(db);
        Assert.Equal(seed.Tutor, await Service(db, seed.Learner).GuiAsync(Input(seed.Classroom, 5)));
        Assert.Equal(seed.Learner, await Service(db, seed.Tutor).GuiAsync(Input(seed.Classroom, 3)));
        var before = await db.Set<DanhGiaDoiTac>().AsNoTracking().SingleAsync(x => x.LopHocId == seed.Classroom && x.NguoiGuiId == seed.Learner);
        var noticeCount = await db.Set<ThongBao>().CountAsync(x => x.DoiTuongId == before.Id);
        var error = await Assert.ThrowsAsync<LoiNghiepVu>(() => Service(db, seed.Learner).GuiAsync(Input(seed.Classroom, 1)));
        Assert.Equal(409, error.Status);
        db.ChangeTracker.Clear();
        Assert.Equal(2, await db.Set<DanhGiaDoiTac>().CountAsync(x => x.LopHocId == seed.Classroom));
        Assert.Equal(5, (await db.Set<DanhGiaDoiTac>().SingleAsync(x => x.Id == before.Id)).SoSao);
        Assert.Equal(noticeCount, await db.Set<ThongBao>().CountAsync(x => x.DoiTuongId == before.Id));
        Assert.Equal(409, (await Assert.ThrowsAsync<LoiNghiepVu>(() => Service(db, seed.Learner).LayDeDanhGiaAsync(seed.Classroom))).Status);
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public async Task Requires_ended_class_and_recorded_lesson(bool ended, bool taught)
    {
        await using var db = ProposalRegressionTests.Open();
        var seed = await Seed(db, ended, taught);
        Assert.Equal(409, (await Assert.ThrowsAsync<LoiNghiepVu>(() => Service(db, seed.Tutor).GuiAsync(Input(seed.Classroom)))).Status);
        Assert.False(await db.Set<DanhGiaDoiTac>().AnyAsync(x => x.LopHocId == seed.Classroom));
    }

    [Fact]
    public async Task Outsider_cannot_rate()
    {
        await using var db = ProposalRegressionTests.Open();
        var seed = await Seed(db);
        var other = await ProposalRegressionTests.Seed(db);
        Assert.Equal(403, (await Assert.ThrowsAsync<LoiNghiepVu>(() => Service(db, other.Learner).GuiAsync(Input(seed.Classroom)))).Status);
    }

    [Theory]
    [InlineData(0, 20, true)]
    [InlineData(6, 20, true)]
    [InlineData(5, 9, true)]
    [InlineData(5, 2001, true)]
    [InlineData(5, 20, false)]
    public async Task Rejects_invalid_input(int stars, int length, bool privacy)
    {
        await using var db = ProposalRegressionTests.Open();
        var input = Input(Guid.NewGuid(), stars);
        input.NhanXet = new string('a', length);
        input.XacNhanRiengTu = privacy;
        await Assert.ThrowsAsync<LoiNghiepVu>(() => Service(db, Guid.NewGuid()).GuiAsync(input));
    }

    [Fact]
    public async Task Concurrent_submissions_create_one_review_and_one_notification()
    {
        await using var setup = ProposalRegressionTests.Open();
        var seed = await Seed(setup);
        async Task<bool> Send()
        {
            await using var db = ProposalRegressionTests.Open();
            try { await Service(db, seed.Learner).GuiAsync(Input(seed.Classroom)); return true; }
            catch (LoiNghiepVu error) when (error.Status == 409) { return false; }
        }
        var results = await Task.WhenAll(Send(), Send());
        Assert.Single(results, x => x);
        var review = await setup.Set<DanhGiaDoiTac>().SingleAsync(x => x.LopHocId == seed.Classroom);
        Assert.Equal(1, await setup.Set<ThongBao>().CountAsync(x => x.DoiTuongId == review.Id));
    }

    [Fact]
    public async Task Agreed_ending_invites_both_principals_to_rate()
    {
        await using var db = ProposalRegressionTests.Open();
        var seed = await Seed(db, ended: false);
        var classroom = await db.Set<LopHoc>().AsNoTracking().SingleAsync(x => x.Id == seed.Classroom);
        var request = (await ProposalRegressionTests.Service(db, seed.Tutor).TaoYeuCauHuyAsync(seed.Classroom, "Đã học xong", classroom.RowVersion, seed.Tutor, default)).DuLieu;
        var stored = await db.Set<YeuCauHuyKetNoi>().AsNoTracking().SingleAsync(x => x.Id == request);
        (await ProposalRegressionTests.Service(db, seed.Learner).PhanHoiHuyAsync(request, true, stored.RowVersion, seed.Learner, default)).BaoLoiNeuThatBai();
        var recipients = await db.Set<ThongBao>().Where(x => x.DoiTuongId == seed.Classroom && x.NoiDungTomTat.Contains("gửi đánh giá"))
            .Select(x => x.NguoiNhanId).ToListAsync();
        Assert.Equal(2, recipients.Count);
        Assert.Contains(seed.Tutor, recipients);
        Assert.Contains(seed.Learner, recipients);
    }

    [Fact]
    public async Task Expired_ending_invites_each_side_once()
    {
        await using var db = ProposalRegressionTests.Open();
        var seed = await Seed(db, ended: false);
        var classroom = await db.Set<LopHoc>().AsNoTracking().SingleAsync(x => x.Id == seed.Classroom);
        var request = (await ProposalRegressionTests.Service(db, seed.Tutor).TaoYeuCauHuyAsync(seed.Classroom, "Đã hoàn thành", classroom.RowVersion, seed.Tutor, default)).DuLieu;
        await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.YeuCauHuyKetNoi SET GuiLuc=DATEADD(hour,-73,SYSDATETIMEOFFSET()),HetHanLuc=DATEADD(hour,-1,SYSDATETIMEOFFSET()) WHERE Id={request}");
        var connection = db.Database.GetConnectionString();
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connection));
        services.AddScoped<Microsoft.AspNetCore.Http.IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<NguoiDungHienTaiService>();
        services.AddScoped<ThongBaoService>();
        using var provider = services.BuildServiceProvider();
        using var worker = new BaoTriWorker(provider.GetRequiredService<Microsoft.Extensions.DependencyInjection.IServiceScopeFactory>(), Microsoft.Extensions.Logging.Abstractions.NullLogger<BaoTriWorker>.Instance);
        var method = typeof(BaoTriWorker).GetMethod("XuLyTuHuyKetNoiAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        for (var i = 0; i < 2; i++)
            await (Task)method.Invoke(worker, [DateTimeOffset.UtcNow, CancellationToken.None])!;
        db.ChangeTracker.Clear();
        Assert.Equal("DaKetThuc", (await db.Set<LopHoc>().SingleAsync(x => x.Id == seed.Classroom)).TrangThai);
        Assert.Equal(2, await db.Set<ThongBao>().CountAsync(x => x.DoiTuongId == seed.Classroom && x.NoiDungTomTat.Contains("gửi đánh giá")));
    }

}
