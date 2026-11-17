using EduConnect.Mvc.Areas.HocTap.Controllers;
using EduConnect.Mvc.Areas.HocTap.Services;
using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Xunit;

namespace EduConnect.HocTap.UnitTests;

public sealed class HocTapControllerTest
{
    [Fact,Trait("Suite","LichHoc"),Trait("TestCaseId","UT-065")]
    public async Task UT_065_IdRong_BadRequest_KhongDocService()
    {
        using var f=new Fixture();f.RecordData(Guid.Empty);
        var result=await f.LichController.Buoi(Guid.Empty);
        Assert.IsType<BadRequestResult>(result);f.Lich.Verify(x=>x.LayAsync(It.IsAny<Guid>()),Times.Never);
    }
    [Fact,Trait("Suite","LichHoc"),Trait("TestCaseId","UT-066")]
    public async Task UT_066_KhongTimThay_TraNotFound()
    {
        using var f=new Fixture();f.Lich.Setup(x=>x.LayAsync(f.Schedule.Id)).ThrowsAsync(new LoiNghiepVu("Không có lịch học.",404));
        Assert.IsType<NotFoundResult>(await f.LichController.Buoi(f.Schedule.Id));
    }
    [Fact,Trait("Suite","LichHoc"),Trait("TestCaseId","UT-067")]
    public async Task UT_067_KhongCoQuyen_TraForbid()
    {
        using var f=new Fixture();f.Lich.Setup(x=>x.LayAsync(f.Schedule.Id)).ThrowsAsync(new LoiNghiepVu("Không có quyền.",403));
        Assert.IsType<ForbidResult>(await f.LichController.Buoi(f.Schedule.Id));
    }
    [Fact,Trait("Suite","LichHoc"),Trait("TestCaseId","UT-068")]
    public async Task UT_068_NapBieuMau_DungIdVaRowVersion()
    {
        using var f=new Fixture();f.RecordData(f.Schedule.Id);
        var view=Assert.IsType<ViewResult>(await f.LichController.Buoi(f.Schedule.Id));
        Assert.True(view.ViewName==null||view.ViewName=="Buoi");var model=Assert.IsType<GhiNhanBuoiHocViewModel>(view.Model);
        Assert.Equal(f.Schedule.Id,model.LichHocId);Assert.Equal(HocTapFixture.Version,model.RowVersion);
        Assert.Equal(f.Record.NoiDungGiangDay,model.NoiDung);Assert.Equal(f.Record.NhanXet,model.NhanXet);
        Assert.Equal(f.Record.TrangThaiThamDuGiaSu,model.TrangThaiGiaSu);Assert.Equal(f.Record.TrangThaiThamDuBenHoc,model.TrangThaiBenHoc);
        f.Lich.Verify(x=>x.LayAsync(f.Schedule.Id),Times.Once);f.Lich.Verify(x=>x.GhiNhanChiTietAsync(f.Schedule.Id),Times.Once);
        Assert.True(f.Lich.Invocations.Any(call=>call.Arguments.Any(arg=>arg is CancellationToken ct&&ct==f.Token)),
            "UT-068: model đã nạp đúng nhưng controller không truyền RequestAborted xuống service như kỳ vọng thiết kế; các lời gọi hiện chỉ nhận Guid id.");
        Assert.True(f.Lich.Invocations.Any(call=>call.Arguments.Any(arg=>arg is Guid actor&&actor==f.ActorId)),
            "UT-068: controller không truyền mã người thực hiện xuống service như kỳ vọng thiết kế.");
    }
    [Fact,Trait("Suite","LichHoc"),Trait("TestCaseId","UT-069"),Trait("TestKind","DesignContract")]
    public void UT_069_ThietKe_Buoi_PhaiCoTrongCode()
    {
        HocTapDesignContract.RequireMethod("UT-069", typeof(LichHocController), "Buoi", typeof(GhiNhanBuoiHocViewModel), "GhiNhan(GhiNhanBuoiHocViewModel)");
    }
    [Fact,Trait("Suite","LichHoc"),Trait("TestCaseId","UT-073")]
    public async Task UT_073_ModelStateThemLoi_GiuModel_KhongThem()
    {
        using var f=new Fixture();var model=new LichHangLoatInput{LopHocId=f.Lop.Id,Buoi=[new()]};f.Classes.Setup(x=>x.ChiTietAsync(f.Lop.Id)).ReturnsAsync(f.Lop);
        f.LichController.ModelState.AddModelError("Buoi[0].TieuDe","Loi mau");
        var view=Assert.IsType<ViewResult>(await f.LichController.Them(model));Assert.Same(model,view.Model);Assert.False(f.LichController.ModelState.IsValid);
        f.Lich.Verify(x=>x.ThemHangLoatAsync(It.IsAny<LichHangLoatInput>()),Times.Never);
    }
    [Fact,Trait("Suite","LichHoc"),Trait("TestCaseId","UT-075"),Trait("TestKind","DesignContract")]
    public void UT_075_ThietKe_DoiLich_PhaiCoTrongCode()
    {
        HocTapDesignContract.RequireMethod("UT-075", typeof(LichHocController), "DoiLich", typeof(SuaLichInput), "Sua(SuaLichInput)");
    }
    [Fact,Trait("Suite","BaiTap"),Trait("TestCaseId","UT-080")]
    public async Task UT_080_ModelStateGiaoLoi_GiuModel_KhongGiao()
    {
        using var f=new Fixture();f.AssignmentData();f.Rights.Setup(x=>x.KiemTraQuyenGhiAsync(f.Lop.Id,true)).ReturnsAsync(f.Lop);
        var model=new GiaoBaiTapViewModel{LopHocId=f.Lop.Id,TieuDe="Du lieu cu",NoiDung="Noi dung cu"};
        f.TasksController.ModelState.AddModelError("TieuDe","Loi mau");var view=Assert.IsType<ViewResult>(await f.TasksController.Giao(model));
        Assert.Same(model,view.Model);Assert.False(f.TasksController.ModelState.IsValid);f.Tasks.Verify(x=>x.GiaoAsync(It.IsAny<GiaoBaiTapViewModel>()),Times.Never);
    }
    [Fact,Trait("Suite","BaiTap"),Trait("TestCaseId","UT-081"),Trait("TestKind","DesignContract")]
    public void UT_081_ThietKe_Sua_PhaiCoTrongCode()
    {
        HocTapDesignContract.RequireMethod("UT-081", typeof(BaiTapController), "Sua", null, "Không có action sửa nội dung và hạn bài tập");
    }
    [Fact,Trait("Suite","BaiTap"),Trait("TestCaseId","UT-082"),Trait("TestKind","DesignContract")]
    public void UT_082_ThietKe_NopBai_PhaiCoTrongCode()
    {
        HocTapDesignContract.RequireMethod("UT-082", typeof(BaiTapController), "NopBai", typeof(NopBaiViewModel), "Nop(NopBaiViewModel)");
    }
    [Fact,Trait("Suite","BaiTap"),Trait("TestCaseId","UT-083"),Trait("TestKind","DesignContract")]
    public void UT_083_ThietKe_ChamBai_PhaiCoTrongCode()
    {
        HocTapDesignContract.RequireMethod("UT-083", typeof(BaiTapController), "ChamBai", null, "Cham(ChamBaiViewModel)");
    }
    private sealed class Fixture:IDisposable
    {
        private readonly CancellationTokenSource cancellation=new();
        public CancellationToken Token=>cancellation.Token;
        public Guid ActorId {get;}=Guid.NewGuid();
        public LopHoc Lop {get;}=new(){Id=Guid.NewGuid()};public LichHoc Schedule {get;}
        public BuoiHoc Record {get;}public BaiTapGiao Assignment {get;}
        public Mock<ILichHocService> Lich {get;}=new(MockBehavior.Strict);public Mock<ILopHocService> Classes {get;}=new(MockBehavior.Strict);
        public Mock<IBaiTapService> Tasks {get;}=new(MockBehavior.Strict);public Mock<ITepService> Files {get;}=new(MockBehavior.Strict);public Mock<IQuyenLopHocService> Rights {get;}=new(MockBehavior.Strict);
        public LichHocController LichController {get;}public BaiTapController TasksController {get;}
        public Fixture()
        {
            Schedule=new(){Id=Guid.NewGuid(),LopHocId=Lop.Id,TrangThai="DaLenLich"};
            Record=new(){Id=Guid.NewGuid(),LichHocId=Schedule.Id,RowVersion=[..HocTapFixture.RV],NoiDungGiangDay="Noi dung cu",NhanXet="Nhan xet cu",TrangThaiThamDuGiaSu="CoMat",TrangThaiThamDuBenHoc="CoMat",KetQuaBuoiHoc="DaDienRa"};
            Assignment=new(){Id=Guid.NewGuid(),LopHocId=Lop.Id,TrangThai="DaGiao"};
            var lc=new DefaultHttpContext{RequestAborted=cancellation.Token};var bc=new DefaultHttpContext{RequestAborted=cancellation.Token};
            LichController=new(Lich.Object,Classes.Object,Tasks.Object){ControllerContext=new(){HttpContext=lc},TempData=new TempDataDictionary(lc,Mock.Of<ITempDataProvider>())};
            lc.User=new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[]{new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier,ActorId.ToString())},"UnitTest"));
            TasksController=new(Tasks.Object,Classes.Object,Files.Object,Rights.Object){ControllerContext=new(){HttpContext=bc},TempData=new TempDataDictionary(bc,Mock.Of<ITempDataProvider>())};
        }
        public void RecordData(Guid id)
        {
            Lich.Setup(x=>x.LayAsync(id)).ReturnsAsync(Schedule);Lich.Setup(x=>x.GhiNhanChiTietAsync(id)).ReturnsAsync((Record,new List<LichSuGhiNhanBuoiHoc>()));
            Lich.Setup(x=>x.LichSuDieuChinhAsync(id)).ReturnsAsync([]);Classes.Setup(x=>x.ChiTietAsync(Lop.Id)).ReturnsAsync(Lop);
            Tasks.Setup(x=>x.LayAsync(Lop.Id)).ReturnsAsync([]);Classes.Setup(x=>x.LichAsync(Lop.Id)).ReturnsAsync([Schedule]);Classes.Setup(x=>x.BuoiAsync(Lop.Id)).ReturnsAsync([Record]);
        }
        public void AssignmentData()
        {
            Tasks.Setup(x=>x.ChiTietAsync(Assignment.Id)).ReturnsAsync(Assignment);Classes.Setup(x=>x.ChiTietAsync(Lop.Id)).ReturnsAsync(Lop);
            Tasks.Setup(x=>x.BaiNopAsync(Assignment.Id)).ReturnsAsync([]);Tasks.Setup(x=>x.KetQuaAsync(Assignment.Id)).ReturnsAsync([]);Files.Setup(x=>x.DanhSachAsync(Assignment.Id)).ReturnsAsync([]);
            Classes.Setup(x=>x.ThanhVienAsync(Lop.Id)).ReturnsAsync([]);Classes.Setup(x=>x.BuoiAsync(Lop.Id)).ReturnsAsync([Record]);Classes.Setup(x=>x.LichAsync(Lop.Id)).ReturnsAsync([Schedule]);
        }
        public void Dispose()=>cancellation.Dispose();
    }
}
