using System.Security.Claims;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Areas.QuanTri.Services;
using EduConnect.Mvc.Areas.QuanTri.ViewModels;
using EduConnect.Mvc.Areas.TimKiem.Services;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Xunit;
using static ProposalRegressionTests;

public sealed class NeedModerationTests
{
    private static NguoiDungHienTaiService Current(AppDbContext db, Guid id)
        => new(db, new HttpContextAccessor { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, id.ToString())], "Test")) } });

    private static NhuCauService Needs(AppDbContext db, Guid id) => new(db, Current(db, id));

    private static QuanTriService AdminService(AppDbContext db, Guid id)
    {
        var current = Current(db, id);
        var realtime = new PhienCapNhat();
        return new(db, current, new QuyenQuanTriService(db, current, new HttpContextAccessor()),
            new PhienDangNhapService(db, realtime), new ThongBaoService(db, current), realtime);
    }

    private static async Task<Guid> Admin(AppDbContext db)
    {
        var id = Guid.NewGuid();
        var email = id.ToString("N") + "@example.test";
        db.Users.Add(new TaiKhoan { Id = id, HoTen = "Quản trị kiểm thử", UserName = email, NormalizedUserName = email.ToUpperInvariant(), Email = email, NormalizedEmail = email.ToUpperInvariant(), SecurityStamp = Guid.NewGuid().ToString() });
        db.UserRoles.Add(new() { UserId = id, RoleId = await db.Roles.Where(r => r.Name == "QuanTri").Select(r => r.Id).SingleAsync() });
        db.UserClaims.Add(new() { UserId = id, ClaimType = QuyenQuanTriService.ClaimType, ClaimValue = "XuLyBaoCao" });
        await db.SaveChangesAsync();
        return id;
    }

    private static TaoNhuCauBenHocViewModel Input(string goal = "Ôn tập", decimal fee = 20000)
        => new() { MonHocIds = [1, 2], MucTieu = goal, KhoiLop = "Lớp 12", HocLuc = "Khá", SoBuoiTuan = 1, HocPhiDuKien = fee, DonViHocPhi = "VND/Gio" };

    [Theory]
    [InlineData(0)]
    [InlineData(20000)]
    [InlineData(1000000)]
    public async Task Valid_prices_are_published_without_manual_approval(int fee)
    {
        await using var db = Open(); var seed = await Seed(db);
        var result = await Needs(db, seed.Learner).TaoNhuCauHocAsync(Input(fee: fee), seed.Learner, default);
        Assert.True(result.ThanhCong, result.ThongBao);
        var stored = await db.Set<NhuCauHoc>().SingleAsync(x => x.Id == result.DuLieu);
        Assert.Equal("DangMo", stored.TrangThai);
        Assert.Equal(fee, stored.HocPhiDuKien);
        Assert.False(await db.Set<BienPhapQuanTri>().AnyAsync(x => x.DoiTuongId == stored.Id));
    }

    [Fact]
    public async Task Duplicate_whitespace_case_and_subject_order_do_not_create_another_post()
    {
        await using var db = Open(); var seed = await Seed(db); var needs = Needs(db, seed.Learner);
        Assert.True((await needs.TaoNhuCauHocAsync(Input(), seed.Learner, default)).ThanhCong);
        var duplicate = Input("  ôn\n\t tập  "); duplicate.MonHocIds = [2, 1];
        var result = await needs.TaoNhuCauHocAsync(duplicate, seed.Learner, default);
        Assert.False(result.ThanhCong); Assert.Equal(409, result.MaTrangThai);
        Assert.Equal(1, await db.Set<NhuCauHoc>().CountAsync(x => x.ChuSoHuuId == seed.Learner));
    }

    [Fact]
    public async Task Concurrent_duplicate_submissions_publish_once()
    {
        await using var seedDb = Open(); var seed = await Seed(seedDb);
        async Task<KetQua<Guid>> Create()
        {
            await using var db = Open();
            return await Needs(db, seed.Learner).TaoNhuCauHocAsync(Input(), seed.Learner, default);
        }
        var results = await Task.WhenAll(Create(), Create());
        Assert.Single(results, x => x.ThanhCong);
        Assert.Single(results, x => !x.ThanhCong && x.MaTrangThai == 409);
    }

    [Fact]
    public async Task Daily_limit_keeps_edits_available_after_three_posts()
    {
        await using var db = Open(); var seed = await Seed(db); var needs = Needs(db, seed.Learner);
        Guid last = default;
        for (var i = 0; i < 3; i++)
        {
            last = (await needs.TaoNhuCauHocAsync(Input("Mục tiêu " + i), seed.Learner, default)).DuLieu;
            var need = await db.Set<NhuCauHoc>().AsNoTracking().SingleAsync(x => x.Id == last);
            await needs.TrangThaiAsync(last, "DaDong", Convert.ToBase64String(need.RowVersion));
        }
        var blocked = await needs.TaoNhuCauHocAsync(Input("Bài thứ tư"), seed.Learner, default);
        Assert.False(blocked.ThanhCong); Assert.Equal(429, blocked.MaTrangThai);
        var original = (await needs.LayDeSuaAsync(last)).DuLieu;
        original.BanNhap = true; original.MucTieu = "Chỉnh sửa bản đã có";
        var edit = TaoNhuCauBenHocViewModel.FromInput(original);
        Assert.True((await needs.CapNhatAsync(last, edit, edit.RowVersion!, seed.Learner, default)).ThanhCong);
    }

    [Fact]
    public async Task Draft_limit_is_five_and_drafts_allow_empty_goals()
    {
        await using var db = Open(); var seed = await Seed(db); var needs = Needs(db, seed.Learner);
        for (var i = 0; i < 5; i++)
        {
            var input = Input(); input.BanNhap = true; input.MucTieu = null;
            Assert.True((await needs.TaoNhuCauHocAsync(input, seed.Learner, default)).ThanhCong);
        }
        var sixth = Input(); sixth.BanNhap = true;
        Assert.Equal(409, (await needs.TaoNhuCauHocAsync(sixth, seed.Learner, default)).MaTrangThai);
    }

    [Theory]
    [InlineData("MucTieu")]
    [InlineData("KhoiLop")]
    [InlineData("LichRanh")]
    public async Task Reopening_applies_validation_to_legacy_data(string invalidField)
    {
        await using var db = Open(); var seed = await Seed(db); var needs = Needs(db, seed.Learner);
        var id = (await needs.TaoNhuCauHocAsync(Input(), seed.Learner, default)).DuLieu;
        var need = await db.Set<NhuCauHoc>().SingleAsync(x => x.Id == id);
        need.TrangThai = "DaDong";
        if (invalidField == "MucTieu") need.MucTieu = null;
        if (invalidField == "KhoiLop") need.KhoiLop = "Lớp 99";
        if (invalidField == "LichRanh") await db.Set<KhungGioRanh>().Where(x => x.NhuCauId == id).ExecuteDeleteAsync();
        await db.SaveChangesAsync();
        var count = await db.Set<LuotDangNhuCau>().CountAsync(x => x.TaiKhoanId == seed.Learner);
        var error = await Assert.ThrowsAsync<LoiNghiepVu>(() => needs.TrangThaiAsync(id, "DangMo", Convert.ToBase64String(need.RowVersion)));
        Assert.Equal(invalidField, error.Truong);
        Assert.Equal(count, await db.Set<LuotDangNhuCau>().CountAsync(x => x.TaiKhoanId == seed.Learner));
    }

    [Fact]
    public async Task Moderation_hides_search_keeps_history_and_requires_admin_release()
    {
        await using var db = Open(); var seed = await Seed(db); var admin = await Admin(db);
        var id = (await Needs(db, seed.Learner).TaoNhuCauHocAsync(Input(), seed.Learner, default)).DuLieu;
        var review = await AdminService(db, admin).XemNhuCauHauKiemAsync(id);
        await AdminService(db, admin).XuLyNhuCauAsync(new() { Id = id, RowVersion = Convert.ToBase64String(review.NhuCau.RowVersion), QuyetDinh = "YeuCauChinhSua", LyDo = "Cần bỏ nội dung quảng cáo." });
        review = await AdminService(db, admin).XemNhuCauHauKiemAsync(id);
        await AdminService(db, admin).XuLyNhuCauAsync(new() { Id = id, RowVersion = Convert.ToBase64String(review.NhuCau.RowVersion), QuyetDinh = "AnNoiDung", LyDo = "Nội dung quảng cáo cần chỉnh sửa." });
        Assert.Equal("DangMo", (await db.Set<NhuCauHoc>().SingleAsync(x => x.Id == id)).TrangThai);
        Assert.True((await Needs(db, seed.Learner).KiemDuyetAsync())[id].DangAn);
        Assert.Empty((await Service(db, seed.Tutor).NhuCauDeXuatAsync(seed.Contact)));
        var search = new TimKiemService(db, Current(db, seed.Tutor));
        await Assert.ThrowsAsync<LoiNghiepVu>(() => search.ChiTietAsync(id));
        var edit = TaoNhuCauBenHocViewModel.FromInput((await Needs(db, seed.Learner).LayDeSuaAsync(id)).DuLieu);
        edit.MucTieu = "Đã chỉnh sửa mục tiêu học";
        Assert.True((await Needs(db, seed.Learner).CapNhatAsync(id, edit, edit.RowVersion!, seed.Learner, default)).ThanhCong);
        Assert.True((await Needs(db, seed.Learner).KiemDuyetAsync())[id].DangAn);
        var version = Convert.ToBase64String((await db.Set<NhuCauHoc>().SingleAsync(x => x.Id == id)).RowVersion);
        Assert.True(await Needs(db, seed.Learner).DeNghiXemLaiAsync(id, version));
        Assert.False(await Needs(db, seed.Learner).DeNghiXemLaiAsync(id, version));
        var measure = await db.Set<BienPhapQuanTri>().SingleAsync(x => x.DoiTuongId == id && x.GoLuc == null);
        await AdminService(db, admin).GoBienPhapAsync(measure.Id, "Nội dung đã được chỉnh sửa phù hợp.", Convert.ToBase64String(measure.RowVersion));
        Assert.Equal(id, (await new TimKiemService(db, Current(db, seed.Tutor)).ChiTietAsync(id)).Id);
        Assert.False((await Needs(db, seed.Learner).KiemDuyetAsync()).ContainsKey(id));
        Assert.True(await db.Set<NhatKyQuanTri>().AnyAsync(x => x.DoiTuongId == id.ToString() && x.HanhDong == "AnNhuCau"));
    }

    [Fact]
    public async Task Hidden_need_cannot_be_accepted_from_an_old_proposal()
    {
        await using var db = Open(); var seed = await Seed(db); var admin = await Admin(db);
        var id = (await Needs(db, seed.Learner).TaoNhuCauHocAsync(Input(), seed.Learner, default)).DuLieu;
        var request = await Service(db, seed.Tutor).GuiDeXuatAsync(new()
        {
            LienHeId = seed.Contact, NhuCauId = id, TenLop = "Lớp kiểm thử hậu kiểm", MucTieu = "Ôn tập", HocPhi = 20000,
            ThoiLuongPhut = 60, BatDauHocLuc = DateTime.Today.AddDays(7), LichRanh = [new() { ThuTrongTuan = 1, GioBatDau = new(19, 0), GioKetThuc = new(20, 0) }]
        });
        var proposal = await db.Set<YeuCauKetNoiLopHoc>().AsNoTracking().SingleAsync(x => x.Id == request);
        var need = await db.Set<NhuCauHoc>().SingleAsync(x => x.Id == id);
        await AdminService(db, admin).XuLyNhuCauAsync(new() { Id = id, RowVersion = Convert.ToBase64String(need.RowVersion), QuyetDinh = "AnNoiDung", LyDo = "Cần xác minh nội dung nhu cầu." });
        var result = await Service(db, seed.Learner).ChapNhanDeXuatAsync(request, proposal.RowVersion, seed.Learner, default);
        Assert.False(result.ThanhCong); Assert.Equal(409, result.MaTrangThai);
        Assert.False(await db.Set<LopHoc>().AnyAsync(x => x.YeuCauKetNoiId == request));
    }

    [Fact]
    public async Task Correction_request_keeps_need_public_and_notifies_its_owner()
    {
        await using var db = Open();
        var seed = await Seed(db);
        var admin = await Admin(db);
        var id = (await Needs(db, seed.Learner).TaoNhuCauHocAsync(Input(), seed.Learner, default)).DuLieu;
        var need = await db.Set<NhuCauHoc>().SingleAsync(x => x.Id == id);
        await AdminService(db, admin).XuLyNhuCauAsync(new()
        {
            Id = id, RowVersion = Convert.ToBase64String(need.RowVersion),
            QuyetDinh = "YeuCauChinhSua", LyDo = "Bổ sung mục tiêu học cụ thể."
        });
        var comment = (await Needs(db, seed.Learner).KiemDuyetAsync())[id];
        Assert.False(comment.DangAn);
        Assert.Equal("Bổ sung mục tiêu học cụ thể.", comment.LyDo);
        Assert.Equal(id, (await new TimKiemService(db, Current(db, seed.Tutor)).ChiTietAsync(id)).Id);
        Assert.True(await db.Set<ThongBao>().AnyAsync(x => x.NguoiNhanId == seed.Learner && x.DoiTuongId == id));
    }

    [Fact]
    public async Task Stale_moderation_version_does_not_write_measure_or_notice()
    {
        await using var db = Open();
        var seed = await Seed(db);
        var admin = await Admin(db);
        var id = (await Needs(db, seed.Learner).TaoNhuCauHocAsync(Input(), seed.Learner, default)).DuLieu;
        var original = (await Needs(db, seed.Learner).LayDeSuaAsync(id)).DuLieu;
        var version = original.RowVersion!;
        var edit = TaoNhuCauBenHocViewModel.FromInput(original);
        edit.MucTieu = "Mục tiêu vừa thay đổi";
        Assert.True((await Needs(db, seed.Learner).CapNhatAsync(id, edit, edit.RowVersion!, seed.Learner, default)).ThanhCong);
        var notices = await db.Set<ThongBao>().CountAsync(x => x.DoiTuongId == id);
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => AdminService(db, admin).XuLyNhuCauAsync(new()
        {
            Id = id, RowVersion = version, QuyetDinh = "AnNoiDung", LyDo = "Nội dung cần kiểm tra."
        }));
        Assert.False(await db.Set<BienPhapQuanTri>().AnyAsync(x => x.DoiTuongId == id));
        Assert.False(await db.Set<NhatKyQuanTri>().AnyAsync(x => x.DoiTuongId == id.ToString()));
        Assert.Equal(notices, await db.Set<ThongBao>().CountAsync(x => x.DoiTuongId == id));
    }

    [Fact]
    public async Task Review_request_requires_owner_and_an_active_hide()
    {
        await using var db = Open();
        var seed = await Seed(db);
        var id = (await Needs(db, seed.Learner).TaoNhuCauHocAsync(Input(), seed.Learner, default)).DuLieu;
        var need = await db.Set<NhuCauHoc>().SingleAsync(x => x.Id == id);
        var version = Convert.ToBase64String(need.RowVersion);
        Assert.Equal(409, (await Assert.ThrowsAsync<LoiNghiepVu>(() => Needs(db, seed.Learner).DeNghiXemLaiAsync(id, version))).Status);
        Assert.Equal(404, (await Assert.ThrowsAsync<LoiNghiepVu>(() => Needs(db, seed.Tutor).DeNghiXemLaiAsync(id, version))).Status);
    }

    [Fact]
    public async Task Ordinary_user_cannot_moderate_or_read_drafts_as_admin()
    {
        await using var db = Open(); var seed = await Seed(db); var admin = await Admin(db);
        var input = Input(); input.BanNhap = true;
        var id = (await Needs(db, seed.Learner).TaoNhuCauHocAsync(input, seed.Learner, default)).DuLieu;
        Assert.Equal(403, (await Assert.ThrowsAsync<LoiNghiepVu>(() => AdminService(db, seed.Learner).XemNhuCauHauKiemAsync(id))).Status);
        Assert.Equal(404, (await Assert.ThrowsAsync<LoiNghiepVu>(() => AdminService(db, admin).XemNhuCauHauKiemAsync(id))).Status);
        Assert.Empty((await AdminService(db, admin).LayNhuCauHauKiemAsync(q: "not-existing-" + Guid.NewGuid())).Items);
    }
}
