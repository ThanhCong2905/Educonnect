using System.Security.Claims;
using EduConnect.Mvc.Areas.KetNoi.Services;
using EduConnect.Mvc.Areas.KetNoi.ViewModels;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Areas.NhuCau.Controllers;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Xunit;

public sealed class ProposalRegressionTests
{
    [Theory]
    [InlineData(false, false, "Tạo nhu cầu thành công")]
    [InlineData(true, false, "Đã lưu nháp")]
    [InlineData(false, true, "Cập nhật nhu cầu thành công")]
    public async Task Need_controller_sets_the_message_for_the_completed_operation(bool draft, bool update, string message)
    {
        await using var db = Open();
        var seed = await Seed(db);
        var http = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, seed.Learner.ToString())], "Test"))
        };
        var current = new NguoiDungHienTaiService(db, new HttpContextAccessor { HttpContext = http });
        var needs = new NhuCauService(db, current);
        var input = new TaoNhuCauBenHocViewModel { MonHocIds = [1], MucTieu = "Ôn tập", BanNhap = draft };
        if (update)
        {
            var id = (await needs.TaoNhuCauHocAsync(input, seed.Learner, default)).DuLieu;
            var stored = await db.Set<NhuCauHoc>().AsNoTracking().SingleAsync(item => item.Id == id);
            input.MaNhuCau = id;
            input.RowVersion = stored.RowVersion;
        }
        var controller = new NhuCauController(needs, current)
        {
            ControllerContext = new() { HttpContext = http },
            TempData = new TempDataDictionary(http, new MemoryTempData())
        };
        var result = update ? await controller.CapNhat(input.MaNhuCau!.Value, input) : await controller.TaoNhuCauHoc(input);
        Assert.Equal("Index", Assert.IsType<RedirectToActionResult>(result).ActionName);
        Assert.Equal(message, controller.TempData["Success"]);
    }

    private sealed class MemoryTempData : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
        public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
    }

    internal static AppDbContext Open()
    {
        var connection = Environment.GetEnvironmentVariable("EDU_TEST_CONNECTION")
            ?? throw new InvalidOperationException("Đặt EDU_TEST_CONNECTION đến database kiểm thử.");
        Assert.StartsWith("EduConnect_Verification_", new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connection).InitialCatalog);
        return new(new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(connection).Options);
    }

    internal static KetNoiService Service(AppDbContext db, Guid actor)
    {
        var context = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, actor.ToString())], "Test")) };
        var current = new NguoiDungHienTaiService(db, new HttpContextAccessor { HttpContext = context });
        return new(db, current, new ThongBaoService(db, current));
    }

    internal static async Task<(Guid Tutor, Guid Learner, Guid Contact)> Seed(AppDbContext db)
    {
        var tutor = Guid.NewGuid(); var learner = Guid.NewGuid(); var contact = Guid.NewGuid();
        foreach (var pair in new[] { (tutor, "GiaSu"), (learner, "HocSinh") })
        {
            var email = pair.Item1.ToString("N") + "@example.test";
            db.Users.Add(new TaiKhoan { Id = pair.Item1, HoTen = "Kiểm thử " + pair.Item2, UserName = email, NormalizedUserName = email.ToUpperInvariant(), Email = email, NormalizedEmail = email.ToUpperInvariant(), SecurityStamp = Guid.NewGuid().ToString() });
            var role = await db.Roles.SingleAsync(role => role.Name == pair.Item2);
            db.UserRoles.Add(new() { UserId = pair.Item1, RoleId = role.Id });
        }
        await db.SaveChangesAsync();
        db.Add(new GiaSu { TaiKhoanId = tutor }); db.Add(new HocSinh { TaiKhoanId = learner });
        var profile = new HoSoGiaSu { Id = Guid.NewGuid(), GiaSuId = tutor, TrinhDo = "Đại học", KinhNghiem = "Chưa có", MoTa = new string('a', 100), KhuVuc = "Hà Nội", DayTrucTuyen = true, DangHienThi = true, TrangThaiDuyet = "DaDuyet" };
        db.Add(profile); await db.SaveChangesAsync();
        db.Add(new HoSoGiaSuMonDay { HoSoGiaSuId = profile.Id, MonHocId = 1, PhamViDay = "Lớp 12", HocPhiDeXuat = 0, DonViHocPhi = "VND/Gio" });
        db.Add(new YeuCauLienHe { Id = contact, GiaSuId = tutor, BenHocId = learner, NguoiGuiId = learner, NguoiPhanHoiId = tutor, LoiNhan = "Trao đổi việc học", TrangThai = "ChapNhan", GuiLuc = DateTimeOffset.UtcNow, PhanHoiLuc = DateTimeOffset.UtcNow });
        await db.SaveChangesAsync();
        return (tutor, learner, contact);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Tutor_proposes_directly_or_after_learning_request_and_learner_accepts_once(bool tutorSends)
    {
        await using var db = Open();
        var seed = await Seed(db);
        var sender = seed.Tutor;
        var recipient = seed.Learner;
        var input = new DeXuatLopViewModel { TenNguoiHoc = "Người học kiểm thử", LienHeId = seed.Contact, TenLop = "Lớp kiểm thử", MucTieu = "Ôn tập", HocPhi = 0, ThoiLuongPhut = 60, BatDauHocLuc = DateTime.Today.AddDays(7), LichRanh = [new SlotInput { ThuTrongTuan = 1, GioBatDau = new(19, 0), GioKetThuc = new(20, 0) }] };
        await Assert.ThrowsAsync<LoiNghiepVu>(() => Service(db, seed.Learner).GuiDeXuatAsync(input));
        if (!tutorSends)
        {
            var need = new NhuCauHoc { Id = Guid.NewGuid(), ChuSoHuuId = seed.Learner, MonHocId = 1, LoaiNhuCau = "TimGiaSu", TrangThai = "DangMo", HinhThucHoc = "TrucTuyen", MucTieu = "Ôn tập", TaoLuc = DateTimeOffset.UtcNow };
            db.Add(need);
            await db.SaveChangesAsync();
            input.NhuCauId = need.Id;
            input.Id = await Service(db, seed.Learner).GuiYeuCauAsync(new() { LienHeId = seed.Contact, NhuCauId = need.Id, LoiNhan = "Nhờ gia sư chuẩn bị đề xuất cho nhu cầu này." });
            var pending = await db.Set<YeuCauKetNoiLopHoc>().AsNoTracking().SingleAsync(x => x.Id == input.Id);
            Assert.Equal("ChoGiaSu", pending.TrangThai);
            input.RowVersion = Convert.ToBase64String(pending.RowVersion);
            await Assert.ThrowsAsync<LoiNghiepVu>(() => Service(db, seed.Tutor).ChapNhanDeXuatAsync(pending.Id, input.RowVersion));
        }
        var request = await Service(db, sender).GuiDeXuatAsync(input);
        var saved = await db.Set<YeuCauKetNoiLopHoc>().AsNoTracking().SingleAsync(row => row.Id == request);
        Assert.Equal(sender, saved.NguoiDeXuatId);
        Assert.Equal("ChoBenHoc", saved.TrangThai);
        Assert.Equal(input.NhuCauId, saved.NhuCauId);
        Assert.Equal(input.TenNguoiHoc, saved.TenNguoiHoc);
        Assert.InRange((saved.HetHanLuc - saved.DeXuatLuc!.Value).TotalHours, 71.99, 72.01);
        Assert.Equal(tutorSends ? null : "Nhờ gia sư chuẩn bị đề xuất cho nhu cầu này.", saved.LoiNhan);
        await Assert.ThrowsAsync<LoiNghiepVu>(() => Service(db, sender).ChapNhanDeXuatAsync(request, Convert.ToBase64String(saved.RowVersion)));
        var classroom = await Service(db, recipient).ChapNhanDeXuatAsync(request, Convert.ToBase64String(saved.RowVersion));
        Assert.Equal(classroom, await Service(db, recipient).ChapNhanDeXuatAsync(request, Convert.ToBase64String(saved.RowVersion)));
        Assert.Equal(1, await db.Set<LopHoc>().CountAsync(row => row.YeuCauKetNoiId == request));
    }

    [Fact]
    public async Task Missing_contact_returns_404()
    {
        await using var db = Open();
        var seed = await Seed(db);
        var error = await Assert.ThrowsAsync<LoiNghiepVu>(() => Service(db, seed.Tutor).LayDeXuatAsync(Guid.NewGuid(), null));
        Assert.Equal(404, error.Status);
    }

    [Fact]
    public async Task Locked_recipient_cannot_receive_proposals()
    {
        await using var db = Open();
        var seed = await Seed(db);
        var learner = await db.Users.SingleAsync(user => user.Id == seed.Learner);
        learner.TrangThai = "BiKhoa";
        await db.SaveChangesAsync();
        var error = await Assert.ThrowsAsync<LoiNghiepVu>(() => Service(db, seed.Tutor).LayDeXuatAsync(seed.Contact, null));
        Assert.Equal(403, error.Status);
    }

    [Fact]
    public async Task Search_orders_ratings_and_distinct_learners_and_excludes_hidden_reviews()
    {
        await using var db = Open();
        var first = await Seed(db); var second = await Seed(db); var extra = await Seed(db);
        var tag = "Sort" + Guid.NewGuid().ToString("N");
        foreach (var id in new[] { first.Tutor, second.Tutor })
            (await db.Users.SingleAsync(user => user.Id == id)).HoTen = tag + id;
        await db.SaveChangesAsync();

        async Task<Guid> Classroom((Guid Tutor, Guid Learner, Guid Contact) pair, byte day)
        {
            var input = new DeXuatLopViewModel { TenNguoiHoc = "Người học kiểm thử", LienHeId = pair.Contact, TenLop = "Kiểm thử sắp xếp", MucTieu = "Ôn tập", HocPhi = 0, ThoiLuongPhut = 60, BatDauHocLuc = DateTime.Today.AddDays(7), LichRanh = [new SlotInput { ThuTrongTuan = day, GioBatDau = new(19, 0), GioKetThuc = new(20, 0) }] };
            var request = await Service(db, pair.Tutor).GuiDeXuatAsync(input);
            var row = await db.Set<YeuCauKetNoiLopHoc>().AsNoTracking().SingleAsync(item => item.Id == request);
            return await Service(db, pair.Learner).ChapNhanDeXuatAsync(request, Convert.ToBase64String(row.RowVersion));
        }

        var firstClass = await Classroom(first, 1); var secondClass = await Classroom(second, 1);
        var extraContact = Guid.NewGuid();
        db.Add(new YeuCauLienHe { Id = extraContact, GiaSuId = first.Tutor, BenHocId = extra.Learner, NguoiGuiId = extra.Learner, NguoiPhanHoiId = first.Tutor, LoiNhan = "Kiểm thử thêm bên học", TrangThai = "ChapNhan", GuiLuc = DateTimeOffset.UtcNow, PhanHoiLuc = DateTimeOffset.UtcNow });
        await db.SaveChangesAsync();
        await Classroom((first.Tutor, extra.Learner, extraContact), 2);
        var firstReview = new DanhGiaDoiTac { LopHocId = firstClass, NguoiGuiId = first.Learner, NguoiNhanId = first.Tutor, SoSao = 4, NhanXet = "Nhận xét kiểm thử", GuiLuc = DateTimeOffset.UtcNow };
        var secondReview = new DanhGiaDoiTac { LopHocId = secondClass, NguoiGuiId = second.Learner, NguoiNhanId = second.Tutor, SoSao = 5, NhanXet = "Nhận xét kiểm thử", GuiLuc = DateTimeOffset.UtcNow };
        db.AddRange(firstReview, secondReview); await db.SaveChangesAsync();
        var http = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, first.Learner.ToString())], "Test")) };
        var current = new NguoiDungHienTaiService(db, new HttpContextAccessor { HttpContext = http });
        var search = new EduConnect.Mvc.Areas.TimKiem.Services.TimKiemService(db, current);
        var rating = await search.TimAsync(new() { TuKhoa = tag, SapXep = "DanhGia" });
        Assert.Equal(second.Tutor, rating.Items.First().ChuSoHuuId);
        Assert.Equal(5, rating.Items.First().DiemDanhGia);
        var learners = await search.TimAsync(new() { TuKhoa = tag, SapXep = "HocVien" });
        Assert.Equal(first.Tutor, learners.Items.First().ChuSoHuuId);
        Assert.Equal(2, learners.Items.First().SoBenHoc);
        db.Add(new BienPhapQuanTri { LoaiDoiTuong = "DanhGiaDoiTac", DoiTuongId = secondReview.Id, Loai = "AnNoiDung", LyDo = "Dữ liệu kiểm thử", NguoiApDungId = first.Learner, ApDungLuc = DateTimeOffset.UtcNow });
        await db.SaveChangesAsync();
        var hidden = await search.TimAsync(new() { TuKhoa = tag, SapXep = "DanhGia" });
        Assert.Equal(first.Tutor, hidden.Items.First().ChuSoHuuId);
        Assert.Null(hidden.Items.Single(item => item.ChuSoHuuId == second.Tutor).DiemDanhGia);
    }
}
