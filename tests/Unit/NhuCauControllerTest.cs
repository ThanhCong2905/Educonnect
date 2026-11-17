using EduConnect.Mvc.Areas.NhuCau.Controllers;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Xunit;

namespace EduConnect.UnitTests;

public sealed class NhuCauControllerTest
{
    [Fact]
    [Trait("TestCaseId", "UT-001")]
    public async Task UT_001_Tao_MoBieuMau_KhongTaoNhuCau()
    {
        using var cancellation = new CancellationTokenSource();
        var ct = cancellation.Token;
        var service = new Mock<INhuCauService>(MockBehavior.Strict);
        var current = new Mock<INguoiDungHienTaiService>(MockBehavior.Strict);
        var u1 = new NguoiThamGia
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            HoTen = "U1",
            LoaiNguoi = "HocSinh"
        };
        var subjects = new List<MonHoc> { new() { Id = 1, TenMon = "Toan" } };
        current.Setup(x => x.HienTaiAsync(It.IsAny<CancellationToken>())).ReturnsAsync(u1);
        service.Setup(x => x.MonDuocChonAsync()).ReturnsAsync(subjects);
        var controller = new NhuCauController(service.Object, current.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { RequestAborted = ct }
            }
        };

        IActionResult result = await controller.Tao();

        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal("Tao", view.ViewName);
        var model = Assert.IsType<NhuCauInput>(view.Model);
        Assert.Null(model.Id);
        Assert.Empty(model.MonHocIds);
        Assert.True(Assert.IsType<bool>(view.ViewData["AllowOtherSubjects"]));
        Assert.Equal(subjects, Assert.IsType<List<MonHoc>>(view.ViewData["Subjects"]));
        service.Verify(x => x.MonDuocChonAsync(), Times.Once);
        service.Verify(x => x.TaoNhuCauHocAsync(
            It.IsAny<TaoNhuCauBenHocViewModel>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        service.Verify(x => x.LuuAsync(It.IsAny<NhuCauInput>()), Times.Never);
        service.VerifyNoOtherCalls();
        // Tao checks the role, then its form helper reads the same current user.
        current.Verify(x => x.HienTaiAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        current.VerifyNoOtherCalls();
    }

    [Fact, Trait("TestCaseId", "UT-002")]
    public async Task UT_002_ModelStateKhongHopLe_GiuDuLieu_KhongTao()
    {
        using var f = new Fixture();
        var vm = f.Input();
        f.FormSubjects();
        f.Controller.ModelState.AddModelError(nameof(vm.MucTieu), "Loi mau");

        var result = await f.Controller.TaoNhuCauHoc(vm);

        AssertForm(f, result, vm, "Tao");
        Assert.Equal("Loi mau", Assert.Single(f.Controller.ModelState[nameof(vm.MucTieu)]!.Errors).ErrorMessage);
        f.Service.Verify(x => x.TaoNhuCauHocAsync(It.IsAny<TaoNhuCauBenHocViewModel>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        f.VerifyForm();
    }

    [Fact, Trait("TestCaseId", "UT-008")]
    public async Task UT_008_TaoCongKhai_ChuyenDanhSach_ThongBaoTaoThanhCong()
    {
        using var f = new Fixture();
        var vm = f.Input();
        f.Service.Setup(x => x.TaoNhuCauHocAsync(vm, f.U1.Id, f.Token)).Returns(Success(f.N1));

        var result = await f.Controller.TaoNhuCauHoc(vm);

        AssertRedirect(result);
        f.Service.Verify(x => x.TaoNhuCauHocAsync(vm, f.U1.Id, f.Token), Times.Once);
        f.Service.VerifyNoOtherCalls();
        f.VerifyCurrent();
        Assert.Equal("Tạo nhu cầu thành công", f.Controller.TempData["Success"]);
    }

    [Fact, Trait("TestCaseId", "UT-009")]
    public async Task UT_009_TaoNhap_ChuyenDanhSach_ThongBaoLuuNhap()
    {
        using var f = new Fixture();
        var vm = f.Input();
        vm.BanNhap = true;
        f.Service.Setup(x => x.TaoNhuCauHocAsync(vm, f.U1.Id, f.Token)).Returns(Success(f.N1));

        var result = await f.Controller.TaoNhuCauHoc(vm);

        AssertRedirect(result);
        f.Service.Verify(x => x.TaoNhuCauHocAsync(vm, f.U1.Id, f.Token), Times.Once);
        f.Service.VerifyNoOtherCalls();
        f.VerifyCurrent();
        Assert.Equal("Đã lưu nháp", f.Controller.TempData["Success"]);
    }

    [Fact, Trait("TestCaseId", "UT-010")]
    public async Task UT_010_ServiceTuChoi_GiuModel_HienLoi_KhongBaoThanhCong()
    {
        using var f = new Fixture();
        var vm = f.Input();
        const string message = "Môn học không hợp lệ.";
        f.FormSubjects();
        f.Service.Setup(x => x.TaoNhuCauHocAsync(vm, f.U1.Id, f.Token))
            .Returns(KetQua<Guid>.ThucHienAsync(() => throw new LoiNghiepVu(message, 400) { Truong = "MonHocIds" }));

        var result = await f.Controller.TaoNhuCauHoc(vm);

        AssertForm(f, result, vm, "Tao");
        Assert.False(f.Controller.ModelState.IsValid);
        Assert.Contains(f.Controller.ModelState.Values.SelectMany(x => x.Errors), x => x.ErrorMessage == message);
        f.Service.Verify(x => x.TaoNhuCauHocAsync(vm, f.U1.Id, f.Token), Times.Once);
        f.VerifyForm(currentReads: 2);
    }

    [Fact, Trait("TestCaseId", "UT-013")]
    public async Task UT_013_ModelStateSuaKhongHopLe_GiuIdPhienBan_KhongCapNhat()
    {
        using var f = new Fixture();
        var vm = f.Input();
        vm.MaNhuCau = f.N1;
        vm.RowVersion = [1, 2, 3, 4, 5, 6, 7, 8];
        f.FormSubjects();
        f.Controller.ModelState.AddModelError(nameof(vm.MucTieu), "Loi mau");

        var result = await f.Controller.CapNhat(f.N1, vm);

        AssertForm(f, result, vm, "Sua");
        Assert.Equal("Loi mau", Assert.Single(f.Controller.ModelState[nameof(vm.MucTieu)]!.Errors).ErrorMessage);
        f.Service.Verify(x => x.CapNhatAsync(It.IsAny<Guid>(), It.IsAny<TaoNhuCauBenHocViewModel>(), It.IsAny<byte[]>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        f.VerifyForm();
    }

    [Fact, Trait("TestCaseId", "UT-019")]
    public async Task UT_019_CapNhat_DungIdDuLieuPhienBan_ThongBaoCapNhat()
    {
        using var f = new Fixture();
        var vm = f.Input();
        vm.MaNhuCau = f.N1;
        byte[] rv1 = [1, 2, 3, 4, 5, 6, 7, 8];
        vm.RowVersion = rv1;
        f.Service.Setup(x => x.CapNhatAsync(f.N1, vm, rv1, f.U1.Id, f.Token)).Returns(Success(f.N1));

        var result = await f.Controller.CapNhat(f.N1, vm);

        AssertRedirect(result);
        f.Service.Verify(x => x.CapNhatAsync(f.N1, vm, rv1, f.U1.Id, f.Token), Times.Once);
        f.Service.VerifyNoOtherCalls();
        f.VerifyCurrent();
        Assert.Equal("Cập nhật nhu cầu thành công", f.Controller.TempData["Success"]);
    }

    private static Task<KetQua<Guid>> Success(Guid id) => KetQua<Guid>.ThucHienAsync(() => Task.FromResult(id));

    private static void AssertRedirect(IActionResult result)
    {
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    private static void AssertForm(Fixture f, IActionResult result, TaoNhuCauBenHocViewModel vm, string viewName)
    {
        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal(viewName, view.ViewName);
        var model = Assert.IsType<NhuCauInput>(view.Model);
        Assert.Equal(vm.MaNhuCau, model.Id);
        Assert.Equal(vm.RowVersion == null ? null : Convert.ToBase64String(vm.RowVersion), model.RowVersion);
        Assert.Equal(vm.MaThaoTac, model.MaThaoTac);
        Assert.Equal(vm.MonHocIds, model.MonHocIds);
        Assert.Equal(vm.MucTieu, model.MucTieu);
        Assert.Equal(vm.KhoiLop, model.KhoiLop);
        Assert.Equal(vm.HocLuc, model.HocLuc);
        Assert.Equal(vm.SoBuoiTuan, model.SoBuoiTuan);
        Assert.Equal(vm.HinhThucHoc, model.HinhThucHoc);
        Assert.Equal(vm.KhuVuc, model.KhuVuc);
        Assert.Equal(vm.DiaChiChiTiet, model.DiaChiChiTiet);
        Assert.Equal(vm.HocPhiDuKien, model.HocPhiDuKien);
        Assert.Equal(vm.DonViHocPhi, model.DonViHocPhi);
        Assert.Equal(vm.TenNguoiHoc, model.TenNguoiHoc);
        Assert.Equal(vm.TuoiNguoiHoc, model.TuoiNguoiHoc);
        Assert.Equal(vm.BanNhap, model.BanNhap);
        Assert.Equal(vm.LichRanh.Select(s => (s.ThuTrongTuan, s.GioBatDau, s.GioKetThuc)),
            model.LichRanh.Select(s => (s.ThuTrongTuan, s.GioBatDau, s.GioKetThuc)));
        Assert.False(f.Controller.TempData.ContainsKey("Success"));
        Assert.False(f.Controller.ModelState.IsValid);
        Assert.Single(Assert.IsType<List<MonHoc>>(view.ViewData["Subjects"]));
    }

    private sealed class Fixture : IDisposable
    {
        private readonly CancellationTokenSource cancellation = new();
        public CancellationToken Token => cancellation.Token;
        public Mock<INhuCauService> Service { get; } = new(MockBehavior.Strict);
        public Mock<INguoiDungHienTaiService> Current { get; } = new(MockBehavior.Strict);
        public NguoiThamGia U1 { get; } = new() { Id = Guid.NewGuid(), HoTen = "U1", LoaiNguoi = "HocSinh" };
        public Guid N1 { get; } = Guid.NewGuid();
        public NhuCauController Controller { get; }

        public Fixture()
        {
            Current.Setup(x => x.HienTaiAsync(It.IsAny<CancellationToken>())).ReturnsAsync(U1);
            var context = new DefaultHttpContext { RequestAborted = Token };
            Controller = new NhuCauController(Service.Object, Current.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = context },
                TempData = new TempDataDictionary(context, Mock.Of<ITempDataProvider>())
            };
        }

        public TaoNhuCauBenHocViewModel Input() => new()
        {
            MonHocId = 1, MonHocIds = [1], MucTieu = "On tap dai so", KhoiLop = "10", HocLuc = "Kha",
            SoBuoiTuan = 2, HinhThucHoc = "TrucTuyen", HocPhiDuKien = 150000, DonViHocPhi = "VND/Buoi",
            LichRanh = [new() { ThuTrongTuan = 1, GioBatDau = new(18, 0), GioKetThuc = new(19, 0) }]
        };

        public void FormSubjects()
        {
            Service.Setup(x => x.MonDuocChonAsync()).ReturnsAsync(() => [new MonHoc { Id = 1, TenMon = "Toan", DangSuDung = true }]);
            Service.Setup(x => x.MonTenAsync()).ReturnsAsync(() => [new MonHoc { Id = 1, TenMon = "Toan", DangSuDung = true }]);
        }

        public void VerifyCurrent(int count = 1)
        {
            Current.Verify(x => x.HienTaiAsync(It.IsAny<CancellationToken>()), Times.Exactly(count));
            Current.VerifyNoOtherCalls();
        }

        public void VerifyForm(int currentReads = 1)
        {
            Service.Verify(x => x.MonDuocChonAsync(), Times.Once);
            Service.Verify(x => x.MonTenAsync(), Times.Once);
            Service.VerifyNoOtherCalls();
            VerifyCurrent(currentReads);
        }

        public void Dispose() => cancellation.Dispose();
    }
}
