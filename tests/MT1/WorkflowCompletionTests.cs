using System.Reflection;
using System.Security.Claims;
using EduConnect.Mvc.Areas.HocTap.Services;
using EduConnect.Mvc.Areas.KetNoi.ViewModels;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

public sealed class WorkflowCompletionTests
{
    private static NguoiDungHienTaiService Current(AppDbContext db, Guid id)
        => new(db, new HttpContextAccessor { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, id.ToString())], "Test")) } });
    private static NhuCauService Needs(AppDbContext db, Guid id) => new(db, Current(db, id));
    private static LopHocService Classes(AppDbContext db, Guid id)
    {
        var user = Current(db, id); var rights = new QuyenLopHocService(db, user); var notices = new ThongBaoService(db, user);
        return new(db, user, rights, notices, ProposalRegressionTests.Service(db, id));
    }
    private static TaoNhuCauBenHocViewModel NeedInput(int hour = 19, string goal = "Ôn tập kiến thức") => new()
    {
        MonHocId = 1, MucTieu = goal, SoBuoiTuan = 1,
        LichRanh = [new() { ThuTrongTuan = 1, GioBatDau = new(hour, 0), GioKetThuc = new(hour + 1, 0) }]
    };
    private static DeXuatLopViewModel Proposal(Guid contact, Guid? need = null) => new()
    {
        LienHeId = contact, NhuCauId = need, TenNguoiHoc = "Người học kiểm thử", TenLop = "Lớp kiểm thử nghiệp vụ",
        MonHocId = 1, MucTieu = "Ôn tập", HocPhi = 0, ThoiLuongPhut = 60, BatDauHocLuc = DateTime.Today.AddDays(7),
        LichRanh = [new() { ThuTrongTuan = 1, GioBatDau = new(19, 0), GioKetThuc = new(20, 0) }]
    };
    private static async Task<(Guid Tutor, Guid Learner, Guid Contact, Guid Need, Guid Class)> Connected(AppDbContext db)
    {
        var seed = await ProposalRegressionTests.Seed(db);
        var need = await Needs(db, seed.Learner).TaoNhuCauHocAsync(NeedInput(), seed.Learner, default);
        Assert.True(need.ThanhCong, need.ThongBao);
        var request = await ProposalRegressionTests.Service(db, seed.Tutor).GuiDeXuatAsync(Proposal(seed.Contact, need.DuLieu));
        var row = await db.Set<YeuCauKetNoiLopHoc>().AsNoTracking().SingleAsync(x => x.Id == request);
        var classroom = await ProposalRegressionTests.Service(db, seed.Learner).ChapNhanDeXuatAsync(request, Convert.ToBase64String(row.RowVersion));
        return (seed.Tutor, seed.Learner, seed.Contact, need.DuLieu, classroom);
    }

    [Theory]
    [InlineData(19, false)]
    [InlineData(20, true)]
    [InlineData(14, true)]
    public async Task Posting_checks_future_class_times_and_allows_adjacent_slots(int hour, bool allowed)
    {
        await using var db = ProposalRegressionTests.Open(); var seed = await Connected(db);
        var result = await Needs(db, seed.Learner).TaoNhuCauHocAsync(NeedInput(hour, "Mục tiêu khác"), seed.Learner, default);
        Assert.Equal(allowed, result.ThanhCong);
        if (!allowed) Assert.Contains("trùng", result.ThongBao, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Editing_connected_need_keeps_it_closed_and_preserves_the_class_snapshot()
    {
        await using var db = ProposalRegressionTests.Open(); var seed = await Connected(db); var service = Needs(db, seed.Learner);
        var count = await db.Set<LuotDangNhuCau>().CountAsync(x => x.TaiKhoanId == seed.Learner);
        var snapshot = await db.Set<LopHoc>().AsNoTracking().SingleAsync(x => x.Id == seed.Class);
        var edit = (await service.LayDeSuaAsync(seed.Need)).DuLieu; edit.MucTieu = "Mục tiêu đã chỉnh sửa";
        await service.LuuAsync(edit); db.ChangeTracker.Clear();
        var stored = await db.Set<NhuCauHoc>().SingleAsync(x => x.Id == seed.Need);
        Assert.Equal("DaDong", stored.TrangThai); Assert.Equal("Mục tiêu đã chỉnh sửa", stored.MucTieu);
        Assert.Equal(count, await db.Set<LuotDangNhuCau>().CountAsync(x => x.TaiKhoanId == seed.Learner));
        Assert.Equal(snapshot.MucTieu, (await db.Set<LopHoc>().AsNoTracking().SingleAsync(x => x.Id == seed.Class)).MucTieu);
        Assert.Equal(409, (await Assert.ThrowsAsync<LoiNghiepVu>(() => service.TrangThaiAsync(stored.Id, "DangMo", Convert.ToBase64String(stored.RowVersion)))).Status);
        await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.LopHoc SET TrangThai='DaKetThuc',KetThucLuc=SYSDATETIMEOFFSET(),CachKetThuc='DongY' WHERE Id={seed.Class}");
        await service.TrangThaiAsync(stored.Id, "DangMo", Convert.ToBase64String(stored.RowVersion));
        Assert.Equal("DangMo", (await db.Set<NhuCauHoc>().AsNoTracking().SingleAsync(x => x.Id == seed.Need)).TrangThai);
    }

    [Fact]
    public async Task Cancelled_class_schedule_does_not_block_a_different_need()
    {
        await using var db = ProposalRegressionTests.Open(); var seed = await Connected(db);
        await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.LichHoc SET TrangThai='DaHuy' WHERE LopHocId={seed.Class}");
        var result = await Needs(db, seed.Learner).TaoNhuCauHocAsync(NeedInput(19, "Mục tiêu mới sau khi nghỉ"), seed.Learner, default);
        Assert.True(result.ThanhCong, result.ThongBao);
    }

    [Fact]
    public async Task Expiry_worker_notifies_both_accounts_once()
    {
        await using var db = ProposalRegressionTests.Open(); var seed = await ProposalRegressionTests.Seed(db);
        var request = await ProposalRegressionTests.Service(db, seed.Tutor).GuiDeXuatAsync(Proposal(seed.Contact));
        await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.YeuCauKetNoiLopHoc SET GuiLuc='2019-12-31T00:00:00+00:00',DeXuatLuc='2020-01-01T00:00:00+00:00',HetHanLuc='2020-01-04T00:00:00+00:00' WHERE Id={request}");
        var services = new ServiceCollection();
        services.AddDbContext<AppDbContext>(o => o.UseSqlServer(db.Database.GetConnectionString()));
        services.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor());
        services.AddScoped<NguoiDungHienTaiService>(); services.AddScoped<ThongBaoService>();
        await using var provider = services.BuildServiceProvider();
        using var worker = new BaoTriWorker(provider.GetRequiredService<IServiceScopeFactory>(), NullLogger<BaoTriWorker>.Instance);
        var method = typeof(BaoTriWorker).GetMethod("XuLyHetHanYeuCauAsync", BindingFlags.Instance | BindingFlags.NonPublic)!;
        for (var i = 0; i < 2; i++) await (Task)method.Invoke(worker, [DateTimeOffset.UtcNow, CancellationToken.None])!;
        Assert.Equal("HetHan", (await db.Set<YeuCauKetNoiLopHoc>().AsNoTracking().SingleAsync(x => x.Id == request)).TrangThai);
        var notices = await db.Set<ThongBao>().Where(x => x.DoiTuongId == request && x.NoiDungTomTat.Contains("hết hạn sau 72 giờ")).ToListAsync();
        Assert.Equal(2, notices.Count); Assert.Contains(notices, x => x.NguoiNhanId == seed.Tutor); Assert.Contains(notices, x => x.NguoiNhanId == seed.Learner);
        Assert.All(notices, x => Assert.True(db.Set<SuKienChoXuLy>().Any(e => e.Id == x.MaSuKien)));
    }

    [Fact]
    public async Task Permission_changes_keep_old_and_new_values_and_enforce_history_access()
    {
        await using var db = ProposalRegressionTests.Open(); var seed = await Connected(db);
        var parent = Guid.NewGuid(); var email = parent.ToString("N") + "@example.test";
        db.Users.Add(new TaiKhoan { Id = parent, HoTen = "Phụ huynh kiểm thử", UserName = email, NormalizedUserName = email.ToUpperInvariant(), Email = email, NormalizedEmail = email.ToUpperInvariant(), SecurityStamp = Guid.NewGuid().ToString() });
        db.UserRoles.Add(new() { UserId = parent, RoleId = (await db.Roles.SingleAsync(x => x.Name == "PhuHuynh")).Id }); await db.SaveChangesAsync();
        db.Add(new PhuHuynh { TaiKhoanId = parent }); await db.SaveChangesAsync();
        LienKetTaiKhoanService Links(Guid actor) { var current = Current(db, actor); return new(db, current, new ThongBaoService(db, current)); }
        await Links(seed.Learner).GuiDeNghiAsync(email);
        var link = await db.Set<LienKetPhuHuynhHocSinh>().AsNoTracking().SingleAsync(x => x.PhuHuynhId == parent && x.HocSinhId == seed.Learner);
        await Links(parent).PhanHoiAsync(link.Id, true, Convert.ToBase64String(link.RowVersion));
        await Classes(db, seed.Learner).DeNghiAsync(seed.Class, email);
        var member = await db.Set<ThanhVienLopHoc>().AsNoTracking().SingleAsync(x => x.LopHocId == seed.Class && x.TaiKhoanId == parent);
        await Classes(db, parent).PhanHoiLoiMoiAsync(member.Id, true, Convert.ToBase64String(member.RowVersion));
        member = await db.Set<ThanhVienLopHoc>().AsNoTracking().SingleAsync(x => x.Id == member.Id);
        await Classes(db, seed.Tutor).DuyetAsync(member.Id, "DaDuyet", "Xem", Convert.ToBase64String(member.RowVersion));
        member = await db.Set<ThanhVienLopHoc>().AsNoTracking().SingleAsync(x => x.Id == member.Id);
        await Classes(db, seed.Tutor).DuyetAsync(member.Id, "DaDuyet", "ChinhSua", Convert.ToBase64String(member.RowVersion), true);
        var history = await Classes(db, seed.Tutor).LichSuQuyenAsync(member.Id);
        Assert.Equal(2, history.Count); Assert.Contains(history, x => x.LichSu.MucQuyenCu == "Xem" && x.LichSu.MucQuyenMoi == "ChinhSua" && !x.LichSu.ChoPhepNopBaiCu && x.LichSu.ChoPhepNopBaiMoi);
        Assert.Equal(2, (await Classes(db, seed.Learner).LichSuQuyenAsync(member.Id)).Count);
        Assert.Equal(2, (await Classes(db, parent).LichSuQuyenAsync(member.Id)).Count);
        var other = await ProposalRegressionTests.Seed(db);
        Assert.Equal(403, (await Assert.ThrowsAsync<LoiNghiepVu>(() => Classes(db, other.Learner).LichSuQuyenAsync(member.Id))).Status);
    }
}
