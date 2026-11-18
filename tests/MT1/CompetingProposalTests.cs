using EduConnect.Mvc.Areas.KetNoi.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

public sealed class CompetingProposalTests
{
    private static NhuCauHoc Need(Guid owner) => new()
    {
        Id = Guid.NewGuid(), ChuSoHuuId = owner, MonHocId = 1, LoaiNhuCau = "TimGiaSu", TrangThai = "DangMo",
        HinhThucHoc = "TrucTuyen", MucTieu = "Ôn tập theo nhu cầu", TaoLuc = DateTimeOffset.UtcNow
    };

    private static DeXuatLopViewModel Proposal(Guid contact, Guid need) => new()
    {
        TenNguoiHoc = "Người học kiểm thử", LienHeId = contact, NhuCauId = need, TenLop = "Lớp kiểm thử đề xuất",
        MucTieu = "Ôn tập", LoiNhan = "Đề xuất lớp học", HocPhi = 0, ThoiLuongPhut = 60, BatDauHocLuc = DateTime.Today.AddDays(7),
        LichRanh = [new() { ThuTrongTuan = 1, GioBatDau = new(19, 0), GioKetThuc = new(20, 0) }]
    };

    private static async Task<Guid> Contact(AppDbContext db, Guid tutor, Guid learner)
    {
        var id = Guid.NewGuid();
        db.Add(new YeuCauLienHe { Id = id, GiaSuId = tutor, BenHocId = learner, NguoiGuiId = learner, NguoiPhanHoiId = tutor, TrangThai = "ChapNhan", LoiNhan = "Trao đổi việc học", GuiLuc = DateTimeOffset.UtcNow, PhanHoiLuc = DateTimeOffset.UtcNow });
        await db.SaveChangesAsync();
        return id;
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Choosing_one_tutor_finishes_only_other_pending_requests_of_the_same_need(bool waitingForTutor)
    {
        await using var db = ProposalRegressionTests.Open();
        var a = await ProposalRegressionTests.Seed(db); var b = await ProposalRegressionTests.Seed(db); var c = await ProposalRegressionTests.Seed(db);
        var need = Need(a.Learner); var otherNeed = Need(a.Learner); db.AddRange(need, otherNeed); await db.SaveChangesAsync();
        var contactB = await Contact(db, b.Tutor, a.Learner); var contactC = await Contact(db, c.Tutor, a.Learner);
        var selectedId = await ProposalRegressionTests.Service(db, a.Tutor).GuiDeXuatAsync(Proposal(a.Contact, need.Id));
        var remainingId = waitingForTutor
            ? await ProposalRegressionTests.Service(db, a.Learner).GuiYeuCauAsync(new() { LienHeId = contactB, NhuCauId = need.Id, LoiNhan = "Nhờ gia sư chuẩn bị đề xuất." })
            : await ProposalRegressionTests.Service(db, b.Tutor).GuiDeXuatAsync(Proposal(contactB, need.Id));
        var unrelatedId = await ProposalRegressionTests.Service(db, c.Tutor).GuiDeXuatAsync(Proposal(contactC, otherNeed.Id));
        var selected = await db.Set<YeuCauKetNoiLopHoc>().AsNoTracking().SingleAsync(x => x.Id == selectedId);
        var remaining = await db.Set<YeuCauKetNoiLopHoc>().AsNoTracking().SingleAsync(x => x.Id == remainingId);
        var version = Convert.ToBase64String(remaining.RowVersion);
        var noticeCount = await db.Set<ThongBao>().CountAsync(x => x.DoiTuongId == remainingId);
        var classroom = await ProposalRegressionTests.Service(db, a.Learner).ChapNhanDeXuatAsync(selectedId, Convert.ToBase64String(selected.RowVersion));
        var after = await db.Set<YeuCauKetNoiLopHoc>().AsNoTracking().SingleAsync(x => x.Id == remainingId);
        Assert.Equal("NhuCauDaKetNoi", after.TrangThai);
        Assert.Equal("Nhu cầu đã được kết nối", Rules.Label(after.TrangThai));
        Assert.False(remaining.RowVersion.SequenceEqual(after.RowVersion));
        Assert.Equal(remaining.HetHanLuc, after.HetHanLuc);
        Assert.Equal("DaChapNhan", (await db.Set<YeuCauKetNoiLopHoc>().AsNoTracking().SingleAsync(x => x.Id == selectedId)).TrangThai);
        Assert.Equal("ChoBenHoc", (await db.Set<YeuCauKetNoiLopHoc>().AsNoTracking().SingleAsync(x => x.Id == unrelatedId)).TrangThai);
        Assert.Equal("DangMo", (await db.Set<NhuCauHoc>().AsNoTracking().SingleAsync(x => x.Id == otherNeed.Id)).TrangThai);
        Assert.False(await db.Set<LopHoc>().AnyAsync(x => x.YeuCauKetNoiId == remainingId));
        var notices = await db.Set<ThongBao>().Where(x => x.DoiTuongId == remainingId && x.NoiDungTomTat == "Nhu cầu đã được kết nối. Đề nghị này đã kết thúc.").ToListAsync();
        Assert.Equal(2, notices.Count);
        Assert.Equal(new[] { a.Learner, b.Tutor }.Order(), notices.Select(x => x.NguoiNhanId).Order());
        Assert.All(notices, notice => Assert.True(db.Set<SuKienChoXuLy>().Any(x => x.Id == notice.MaSuKien && x.LoaiSuKien == "KetNoi")));
        Assert.Equal(noticeCount + 2, await db.Set<ThongBao>().CountAsync(x => x.DoiTuongId == remainingId));
        var rejected = await Assert.ThrowsAsync<LoiNghiepVu>(() => ProposalRegressionTests.Service(db, a.Learner).ChapNhanDeXuatAsync(remainingId, version));
        Assert.Equal(409, rejected.Status);
        Assert.Equal(409, (await Assert.ThrowsAsync<LoiNghiepVu>(() => ProposalRegressionTests.Service(db, b.Tutor).LayDeXuatAsync(contactB, remainingId))).Status);
        Assert.Equal(409, (await Assert.ThrowsAsync<LoiNghiepVu>(() => ProposalRegressionTests.Service(db, b.Tutor).PhanHoiAsync(remainingId, "TuChoi", version))).Status);
        Assert.Equal(classroom, await ProposalRegressionTests.Service(db, a.Learner).ChapNhanDeXuatAsync(selectedId, Convert.ToBase64String(selected.RowVersion)));
        Assert.Equal(noticeCount + 2, await db.Set<ThongBao>().CountAsync(x => x.DoiTuongId == remainingId));
        Assert.Contains((await ProposalRegressionTests.Service(db, b.Tutor).LayAsync()).Requests, x => x.Id == remainingId && x.TrangThai == "NhuCauDaKetNoi");
        var edit = await Assert.ThrowsAnyAsync<Exception>(() => db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.YeuCauKetNoiLopHoc SET LoiNhan=N'Sửa yêu cầu đã kết thúc' WHERE Id={remainingId}"));
        Assert.Contains("Yêu cầu đã đóng", Rules.ErrorMessage(edit));
    }

    [Fact]
    public async Task Two_simultaneous_acceptances_create_one_class_and_finish_the_other_request()
    {
        await using var db = ProposalRegressionTests.Open();
        var a = await ProposalRegressionTests.Seed(db); var b = await ProposalRegressionTests.Seed(db);
        var need = Need(a.Learner); db.Add(need); await db.SaveChangesAsync();
        var contactB = await Contact(db, b.Tutor, a.Learner);
        var first = await ProposalRegressionTests.Service(db, a.Tutor).GuiDeXuatAsync(Proposal(a.Contact, need.Id));
        var second = await ProposalRegressionTests.Service(db, b.Tutor).GuiDeXuatAsync(Proposal(contactB, need.Id));
        var requests = await db.Set<YeuCauKetNoiLopHoc>().AsNoTracking().Where(x => x.Id == first || x.Id == second).ToListAsync();
        var start = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        async Task<(Guid? Classroom, int? Error)> Accept(YeuCauKetNoiLopHoc request)
        {
            await start.Task;
            await using var context = ProposalRegressionTests.Open();
            try { return (await ProposalRegressionTests.Service(context, a.Learner).ChapNhanDeXuatAsync(request.Id, Convert.ToBase64String(request.RowVersion)), null); }
            catch (Exception error) { return (null, Rules.ErrorStatus(error)); }
        }
        var tasks = requests.Select(Accept).ToArray(); start.SetResult(); var results = await Task.WhenAll(tasks);
        Assert.Single(results, x => x.Classroom.HasValue);
        Assert.Equal(409, Assert.Single(results, x => x.Error.HasValue).Error);
        var states = await db.Set<YeuCauKetNoiLopHoc>().AsNoTracking().Where(x => x.Id == first || x.Id == second).Select(x => x.TrangThai).ToListAsync();
        Assert.Single(states, x => x == "DaChapNhan"); Assert.Single(states, x => x == "NhuCauDaKetNoi");
        Assert.Equal(1, await db.Set<LopHoc>().CountAsync(x => x.YeuCauKetNoiId == first || x.YeuCauKetNoiId == second));
    }
}
