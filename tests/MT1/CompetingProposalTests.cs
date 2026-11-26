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
        MucTieu = "Ôn tập", HocPhi = 0, ThoiLuongPhut = 60, BatDauHocLuc = DateTime.Today.AddDays(7),
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

    [Theory]
    [InlineData("DaRut")]
    [InlineData("TuChoi")]
    public async Task Current_version_can_be_answered_once_before_expiry(string action)
    {
        await using var db = ProposalRegressionTests.Open();
        var people = await ProposalRegressionTests.Seed(db);
        var need = Need(people.Learner);
        db.Add(need);
        await db.SaveChangesAsync();
        var id = await ProposalRegressionTests.Service(db, people.Tutor)
            .GuiDeXuatAsync(Proposal(people.Contact, need.Id));
        var request = await db.Set<YeuCauKetNoiLopHoc>().AsNoTracking().SingleAsync(x => x.Id == id);
        var version = Convert.ToBase64String(request.RowVersion);
        var actor = action == "DaRut" ? people.Tutor : people.Learner;
        var recipient = action == "DaRut" ? people.Learner : people.Tutor;
        var notices = db.Set<ThongBao>().Where(x => x.DoiTuongId == id && x.NguoiNhanId == recipient);
        var before = await notices.CountAsync();
        await using (var wrongActor = ProposalRegressionTests.Open())
        {
            var forbidden = await Assert.ThrowsAsync<LoiNghiepVu>(() =>
                ProposalRegressionTests.Service(wrongActor, recipient).PhanHoiAsync(id, action, version));
            Assert.Equal(403, forbidden.Status);
        }
        await using (var stale = ProposalRegressionTests.Open())
        {
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() =>
                ProposalRegressionTests.Service(stale, actor).PhanHoiAsync(id, action, Convert.ToBase64String(new byte[8])));
        }
        await ProposalRegressionTests.Service(db, actor).PhanHoiAsync(id, action, version);
        await using var retry = ProposalRegressionTests.Open();
        var conflict = await Assert.ThrowsAsync<LoiNghiepVu>(() =>
            ProposalRegressionTests.Service(retry, actor).PhanHoiAsync(id, action, version));
        Assert.Equal(409, conflict.Status);
        Assert.Equal(action, (await db.Set<YeuCauKetNoiLopHoc>().AsNoTracking().SingleAsync(x => x.Id == id)).TrangThai);
        Assert.Equal(before + 1, await notices.CountAsync());
    }

    [Theory]
    [InlineData("DaRut")]
    [InlineData("TuChoi")]
    public async Task Response_waiting_for_the_request_lock_cannot_be_saved_after_expiry(string action)
    {
        await using var db = ProposalRegressionTests.Open();
        var people = await ProposalRegressionTests.Seed(db);
        var need = Need(people.Learner);
        db.Add(need);
        await db.SaveChangesAsync();
        var id = await ProposalRegressionTests.Service(db, people.Tutor)
            .GuiDeXuatAsync(Proposal(people.Contact, need.Id));
        var noticesBefore = await db.Set<ThongBao>().CountAsync(x => x.DoiTuongId == id);
        await using var responder = ProposalRegressionTests.Open();
        await responder.Database.OpenConnectionAsync();
        await db.Database.ExecuteSqlInterpolatedAsync($"""
            DECLARE @expires datetimeoffset(0) = DATEADD(second, 4, SYSDATETIMEOFFSET());
            UPDATE dbo.YeuCauKetNoiLopHoc
            SET GuiLuc = DATEADD(hour, -72, @expires), DeXuatLuc = DATEADD(hour, -72, @expires), HetHanLuc = @expires
            WHERE Id = {id}
            """);
        var request = await db.Set<YeuCauKetNoiLopHoc>().AsNoTracking().SingleAsync(x => x.Id == id);
        await using var blocker = await db.Database.BeginTransactionAsync();
        await db.Database.ExecuteSqlInterpolatedAsync($"SELECT Id FROM dbo.YeuCauKetNoiLopHoc WITH (UPDLOCK,HOLDLOCK) WHERE Id={id}");
        var actor = action == "DaRut" ? people.Tutor : people.Learner;
        var response = ProposalRegressionTests.Service(responder, actor)
            .PhanHoiAsync(id, action, Convert.ToBase64String(request.RowVersion));
        var remaining = request.HetHanLuc - await GiaoDichNghiepVu.GioMayChuAsync(db);
        Assert.True(remaining > TimeSpan.Zero);
        await Task.Delay(remaining + TimeSpan.FromMilliseconds(150));
        Assert.False(response.IsCompleted);
        await blocker.CommitAsync();
        var error = await Assert.ThrowsAsync<LoiNghiepVu>(() => response);
        Assert.Equal(409, error.Status);
        Assert.Equal("ChoBenHoc", (await db.Set<YeuCauKetNoiLopHoc>().AsNoTracking().SingleAsync(x => x.Id == id)).TrangThai);
        Assert.Equal(noticesBefore, await db.Set<ThongBao>().CountAsync(x => x.DoiTuongId == id));
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
