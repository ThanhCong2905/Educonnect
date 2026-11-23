using System.Security.Claims;
using EduConnect.Mvc.Areas.KetNoi.ViewModels;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.TimKiem.Services;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Xunit;

public sealed class ConnectedNeedTests
{
    private static NguoiDungHienTaiService Current(AppDbContext db, Guid? actor = null)
    {
        var http = new DefaultHttpContext();
        if (actor.HasValue) http.User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, actor.Value.ToString())], "Test"));
        return new(db, new HttpContextAccessor { HttpContext = http });
    }

    private static NhuCauHoc Need(Guid owner) => new()
    {
        Id = Guid.NewGuid(), ChuSoHuuId = owner, MonHocId = 1, LoaiNhuCau = "TimGiaSu", TrangThai = "DangMo",
        HinhThucHoc = "TrucTuyen", MucTieu = "Need" + Guid.NewGuid().ToString("N"), TaoLuc = DateTimeOffset.UtcNow,
        NhacXacNhanLuc = DateTimeOffset.UtcNow
    };

    private static DeXuatLopViewModel Proposal(Guid contact, Guid? need = null) => new()
    {
        TenNguoiHoc = "Người học kiểm thử", LienHeId = contact, NhuCauId = need, TenLop = "Lớp kiểm thử đóng nhu cầu",
        MucTieu = "Ôn tập", HocPhi = 0, ThoiLuongPhut = 60, BatDauHocLuc = DateTime.Today.AddDays(7),
        LichRanh = [new() { ThuTrongTuan = 1, GioBatDau = new(19, 0), GioKetThuc = new(20, 0) }]
    };

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Creating_class_closes_only_its_need_and_removes_public_access(bool learnerInitiates)
    {
        await using var db = ProposalRegressionTests.Open();
        var seed = await ProposalRegressionTests.Seed(db);
        var need = Need(seed.Learner); var other = Need(seed.Learner);
        db.AddRange(need, other); await db.SaveChangesAsync();
        var previousVersion = need.RowVersion.ToArray();
        var proposal = Proposal(seed.Contact, need.Id);
        if (learnerInitiates)
        {
            proposal.Id = await ProposalRegressionTests.Service(db, seed.Learner).GuiYeuCauAsync(new() { LienHeId = seed.Contact, NhuCauId = need.Id, LoiNhan = "Nhờ gia sư lập đề xuất." });
            proposal.RowVersion = Convert.ToBase64String((await db.Set<YeuCauKetNoiLopHoc>().AsNoTracking().SingleAsync(x => x.Id == proposal.Id)).RowVersion);
        }
        var request = await ProposalRegressionTests.Service(db, seed.Tutor).GuiDeXuatAsync(proposal);
        await db.Entry(need).ReloadAsync();
        Assert.Equal("DangMo", need.TrangThai);
        var guest = new TimKiemService(db, Current(db));
        Assert.Equal(1, (await guest.TimKiemAsync(new() { LoaiDoiTuong = "NhuCau", TuKhoa = need.MucTieu }, null, default)).DuLieu.Total);
        var row = await db.Set<YeuCauKetNoiLopHoc>().AsNoTracking().SingleAsync(x => x.Id == request);
        var version = Convert.ToBase64String(row.RowVersion);
        var classroom = await ProposalRegressionTests.Service(db, seed.Learner).ChapNhanDeXuatAsync(request, version);
        await db.Entry(need).ReloadAsync(); await db.Entry(other).ReloadAsync();
        Assert.Equal("DaDong", need.TrangThai); Assert.Equal("DaKetNoi", need.LyDoDong);
        Assert.Null(need.NhacXacNhanLuc); Assert.False(previousVersion.SequenceEqual(need.RowVersion));
        Assert.Equal("DangMo", other.TrangThai);
        foreach (var actor in new Guid?[] { null, seed.Tutor })
        {
            var search = new TimKiemService(db, Current(db, actor));
            var result = await search.TimKiemAsync(new() { LoaiDoiTuong = "NhuCau", TuKhoa = need.MucTieu }, actor, default);
            Assert.True(result.ThanhCong); Assert.Equal(0, result.DuLieu.Total);
            Assert.Equal(404, (await Assert.ThrowsAsync<LoiNghiepVu>(() => search.ChiTietAsync(need.Id))).Status);
        }
        Assert.Contains(await new NhuCauService(db, Current(db, seed.Learner)).LayAsync(), x => x.Id == need.Id && x.TrangThai == "DaDong");
        Assert.Equal(classroom, await ProposalRegressionTests.Service(db, seed.Learner).ChapNhanDeXuatAsync(request, version));
        await db.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.usp_ChapNhanDeXuatTaoLop @YeuCauId={request},@BenHocId={seed.Learner},@RowVersionCu={row.RowVersion},@NguoiThucHienId={seed.Learner}");
        Assert.Equal(1, await db.Set<LopHoc>().CountAsync(x => x.YeuCauKetNoiId == request));
    }

    [Fact]
    public async Task Failure_after_class_insert_rolls_back_the_class_and_keeps_need_open()
    {
        await using var db = ProposalRegressionTests.Open();
        var first = await ProposalRegressionTests.Seed(db); var second = await ProposalRegressionTests.Seed(db);
        var firstRequest = await ProposalRegressionTests.Service(db, first.Tutor).GuiDeXuatAsync(Proposal(first.Contact));
        var firstRow = await db.Set<YeuCauKetNoiLopHoc>().AsNoTracking().SingleAsync(x => x.Id == firstRequest);
        await ProposalRegressionTests.Service(db, first.Learner).ChapNhanDeXuatAsync(firstRequest, Convert.ToBase64String(firstRow.RowVersion));
        var contact = Guid.NewGuid();
        db.Add(new YeuCauLienHe { Id = contact, GiaSuId = second.Tutor, BenHocId = first.Learner, NguoiGuiId = first.Learner, NguoiPhanHoiId = second.Tutor, TrangThai = "ChapNhan", LoiNhan = "Kiểm thử trùng lịch", GuiLuc = DateTimeOffset.UtcNow, PhanHoiLuc = DateTimeOffset.UtcNow });
        var need = Need(first.Learner); db.Add(need); await db.SaveChangesAsync();
        var request = await ProposalRegressionTests.Service(db, second.Tutor).GuiDeXuatAsync(Proposal(contact, need.Id));
        var row = await db.Set<YeuCauKetNoiLopHoc>().AsNoTracking().SingleAsync(x => x.Id == request);
        var conflict = await Assert.ThrowsAnyAsync<Exception>(() => ProposalRegressionTests.Service(db, first.Learner).ChapNhanDeXuatAsync(request, Convert.ToBase64String(row.RowVersion)));
        Assert.Contains("trùng", Rules.ErrorMessage(conflict), StringComparison.OrdinalIgnoreCase);
        db.ChangeTracker.Clear();
        Assert.Equal("DangMo", (await db.Set<NhuCauHoc>().SingleAsync(x => x.Id == need.Id)).TrangThai);
        Assert.Equal("ChoBenHoc", (await db.Set<YeuCauKetNoiLopHoc>().SingleAsync(x => x.Id == request)).TrangThai);
        Assert.False(await db.Set<LopHoc>().AnyAsync(x => x.YeuCauKetNoiId == request));
    }
}
