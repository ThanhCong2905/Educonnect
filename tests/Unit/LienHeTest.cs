using EduConnect.Mvc.Areas.KetNoi.Controllers;
using EduConnect.Mvc.Areas.KetNoi.Services;
using EduConnect.Mvc.Areas.KetNoi.ViewModels;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Xunit;

namespace EduConnect.SearchContact.UnitTests;

[Trait("Suite","TimKiemLienHe")]
public sealed class LienHeTest
{
    [Fact,Trait("TestCaseId","UT-050")]
    public async Task UT_050_ModelStateLoi_KhongGoiService_Tra422TheoBangGoc()
    {
        var service=new Mock<IKetNoiService>(MockBehavior.Strict);var needs=new Mock<INhuCauService>(MockBehavior.Strict);
        var context=new DefaultHttpContext();var controller=new KetNoiController(service.Object,needs.Object){ControllerContext=new(){HttpContext=context},TempData=new TempDataDictionary(context,Mock.Of<ITempDataProvider>())};
        controller.ModelState.AddModelError("LoiNhan","Lời nhắn không hợp lệ.");
        IActionResult? result=null;
        var error=await Record.ExceptionAsync(async()=>result=await controller.GuiLienHe(new(){DoiTuongId=Guid.NewGuid(),LoiNhan=""}));
        service.Verify(x=>x.GuiLienHeAsync(It.IsAny<GuiLienHeInput>()),Times.Never);service.VerifyNoOtherCalls();needs.VerifyNoOtherCalls();
        // This deliberately keeps the original direct-action expectation. The
        // report distinguishes this from testing HTTP/filter status codes.
        Assert.Null(error);Assert.Equal(422,Assert.IsAssignableFrom<ObjectResult>(result).StatusCode);
    }
    [Fact,Trait("TestCaseId","UT-051")]
    public async Task UT_051_LoiNhanMotKyTu_DuocLuu()
    {using var f=new Fixture();await f.Service.GuiLienHeAsync(f.Input("A"));f.AssertSaved("A");}
    [Fact,Trait("TestCaseId","UT-052")]
    public async Task UT_052_LoiNhan256KyTu_DuocLuu()
    {using var f=new Fixture();var text=new string('A',256);await f.Service.GuiLienHeAsync(f.Input(text));f.AssertSaved(text);}
    [Fact,Trait("TestCaseId","UT-053")]
    public async Task UT_053_LoiNhanKhoangTrang_KhongGhi()
    {
        using var f=new Fixture();var ex=await Assert.ThrowsAsync<LoiNghiepVu>(()=>f.Service.GuiLienHeAsync(f.Input("   ")));
        Assert.Equal(400,ex.Status);f.AssertNoWrite();f.Store.VerifyNoOtherCalls();f.Current.VerifyNoOtherCalls();
    }
    [Fact,Trait("TestCaseId","UT-054")]
    public async Task UT_054_LoiNhan257KyTu_KhongGhi()
    {
        using var f=new Fixture();var ex=await Assert.ThrowsAsync<LoiNghiepVu>(()=>f.Service.GuiLienHeAsync(f.Input(new string('A',257))));
        Assert.Equal(400,ex.Status);f.AssertNoWrite();f.Store.VerifyNoOtherCalls();f.Current.VerifyNoOtherCalls();
    }
    [Fact,Trait("TestCaseId","UT-055")]
    public async Task UT_055_TuGuiLienHe_BiTuChoi()
    {
        using var f=new Fixture();f.Other.Id=f.User.Id;
        var ex=await Assert.ThrowsAsync<LoiNghiepVu>(()=>f.Service.GuiLienHeAsync(f.Input()));
        Assert.Equal(400,ex.Status);Assert.Equal("Liên hệ cần một gia sư và một bên học.",ex.Message);f.AssertNoWrite();
        f.Store.Verify(x=>x.BeginAsync(),Times.Never);
    }
    [Fact,Trait("TestCaseId","UT-056")]
    public async Task UT_056_HocSinhVaPhuHuynh_BiTuChoi()
    {
        using var f=new Fixture();f.Other.LoaiNguoi="PhuHuynh";
        var ex=await Assert.ThrowsAsync<LoiNghiepVu>(()=>f.Service.GuiLienHeAsync(f.Input()));
        Assert.Equal(400,ex.Status);Assert.Equal("Liên hệ cần một gia sư và một bên học.",ex.Message);f.AssertNoWrite();
        f.Store.Verify(x=>x.BeginAsync(),Times.Never);
    }
    [Fact,Trait("TestCaseId","UT-057")]
    public async Task UT_057_NguoiNhanKhongTonTai_BiTuChoi()
    {
        using var f=new Fixture();f.RecipientExists=false;
        var ex=await Assert.ThrowsAsync<LoiNghiepVu>(()=>f.Service.GuiLienHeAsync(f.Input()));
        Assert.Equal(400,ex.Status);Assert.Equal("Người nhận không tồn tại.",ex.Message);f.AssertNoWrite();
    }
    [Fact,Trait("TestCaseId","UT-058")]
    public async Task UT_058_QuanTriGuiGiaSu_BiCam()
    {
        using var f=new Fixture();f.User.LoaiNguoi="QuanTri";
        var ex=await Assert.ThrowsAsync<LoiNghiepVu>(()=>f.Service.GuiLienHeAsync(f.Input()));
        Assert.Equal(403,ex.Status);Assert.Equal("Vai trò không được gửi liên hệ.",ex.Message);f.AssertNoWrite();
        f.Store.Verify(x=>x.LockProfileAsync(It.IsAny<Guid>()),Times.Never);
    }
    [Fact,Trait("TestCaseId","UT-059")]
    public async Task UT_059_HoSoKhongDuDieuKien_KhongGhi()
    {
        using var f=new Fixture();f.Eligible=false;
        var ex=await Assert.ThrowsAsync<LoiNghiepVu>(()=>f.Service.GuiLienHeAsync(f.Input()));
        Assert.Equal(409,ex.Status);Assert.Equal("Hồ sơ gia sư đã ẩn hoặc chưa đủ điều kiện nhận liên hệ mới.",ex.Message);f.AssertNoWrite();
        f.Store.Verify(x=>x.LockProfileAsync(f.Other.Id),Times.Once);f.Store.Verify(x=>x.IsEligibleAsync(f.Other.Id),Times.Once);
    }
    [Fact,Trait("TestCaseId","UT-060")]
    public async Task UT_060_LienHeTrung_KhongGhi()
    {
        using var f=new Fixture();f.Duplicate=true;
        var ex=await Assert.ThrowsAsync<LoiNghiepVu>(()=>f.Service.GuiLienHeAsync(f.Input()));
        Assert.Equal(400,ex.Status);Assert.Equal("Hai bên đã liên hệ hoặc đang chờ phản hồi.",ex.Message);f.AssertNoWrite();
        f.Store.Verify(x=>x.ExistsAsync(f.Other.Id,f.User.Id),Times.Once);
    }
    [Fact,Trait("TestCaseId","UT-061")]
    public async Task UT_061_KhoaHoSoLoi_KhongGhiHayCommit()
    {
        using var f=new Fixture();var expected=new LoiNghiepVu("Không khóa được hồ sơ.",409);
        f.Store.Setup(x=>x.LockProfileAsync(f.Other.Id)).ThrowsAsync(expected);
        var ex=await Assert.ThrowsAsync<LoiNghiepVu>(()=>f.Service.GuiLienHeAsync(f.Input()));
        Assert.Same(expected,ex);f.AssertNoWrite();f.Store.Verify(x=>x.IsHiddenAsync(It.IsAny<Guid>()),Times.Never);
        f.Transaction.Verify(x=>x.DisposeAsync(),Times.Once);
    }
    [Fact,Trait("TestCaseId","UT-062")]
    public async Task UT_062_TrimLoiMoi_ThongBaoDungNguoiDich_LuuTruocCommit()
    {
        using var f=new Fixture();await f.Service.GuiLienHeAsync(f.Input("  Xin liên hệ  "));
        f.AssertSaved("Xin liên hệ");
        Assert.Equal(new[]{"add-contact","notify","save","commit"},f.Events);
        f.Transaction.Verify(x=>x.DisposeAsync(),Times.Once);
    }
    [Fact,Trait("TestCaseId","UT-063")]
    public async Task UT_063_LuuLoi_KhongCommit_KhongTraThanhCong()
    {
        using var f=new Fixture();var expected=new InvalidOperationException("Lỗi lưu giả lập.");
        f.Store.Setup(x=>x.SaveAsync()).Callback(()=>f.Events.Add("save-failed")).ThrowsAsync(expected);
        var ex=await Assert.ThrowsAsync<InvalidOperationException>(()=>f.Service.GuiLienHeAsync(f.Input()));
        Assert.Same(expected,ex);f.Store.Verify(x=>x.SaveAsync(),Times.Once);
        f.Transaction.Verify(x=>x.CommitAsync(It.IsAny<CancellationToken>()),Times.Never);f.Transaction.Verify(x=>x.DisposeAsync(),Times.Once);
        Assert.Equal(new[]{"add-contact","notify","save-failed"},f.Events);
    }
    [Fact(Skip="UT-064: kịch bản nói cần nhu cầu đang mở nhưng chưa chốt phạm vi áp dụng theo vai trò. GuiLienHeAsync hiện chưa có hợp đồng đọc nhu cầu mở; cần xác nhận điều kiện trước UC, không thêm policy mới chỉ để chạy test."),Trait("TestCaseId","UT-064")]
    public void UT_064_ChuaCoNhuCauDangMo_ChuaChotDieuKienTruoc()=>throw new InvalidOperationException("Chưa chốt điều kiện trước.");

    private sealed class Fixture:IDisposable
    {
        private readonly AppDbContext db=new(new DbContextOptionsBuilder<AppDbContext>().Options);
        public NguoiThamGia User {get;}=new(){Id=Guid.NewGuid(),HoTen="U1",LoaiNguoi="HocSinh"};
        public NguoiThamGia Other {get;}=new(){Id=Guid.NewGuid(),HoTen="U2",LoaiNguoi="GiaSu"};
        public bool RecipientExists {get;set;}=true;public bool Eligible {get;set;}=true;public bool Duplicate {get;set;}
        public YeuCauLienHe? Added {get;private set;}
        public List<string> Events {get;}=[];
        public Mock<ILienHeStore> Store {get;}=new(MockBehavior.Strict);
        public Mock<IDbContextTransaction> Transaction {get;}=new(MockBehavior.Strict);
        public Mock<INguoiDungHienTaiService> Current {get;}=new(MockBehavior.Strict);
        public KetNoiService Service {get;}
        public Fixture()
        {
            Current.Setup(x=>x.HienTaiAsync(default)).ReturnsAsync(User);
            Store.Setup(x=>x.ReadRecipientAsync(It.IsAny<Guid>())).ReturnsAsync((Guid id)=>RecipientExists&&id==Other.Id?Other:null);
            Store.Setup(x=>x.BeginAsync()).ReturnsAsync(Transaction.Object);
            Store.Setup(x=>x.LockProfileAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);
            Store.Setup(x=>x.IsHiddenAsync(It.IsAny<Guid>())).ReturnsAsync(false);
            Store.Setup(x=>x.IsEligibleAsync(It.IsAny<Guid>())).ReturnsAsync(()=>Eligible);
            Store.Setup(x=>x.ExistsAsync(It.IsAny<Guid>(),It.IsAny<Guid>())).ReturnsAsync(()=>Duplicate);
            Store.Setup(x=>x.AddContact(It.IsAny<YeuCauLienHe>())).Callback<YeuCauLienHe>(c=>{Added=c;Events.Add("add-contact");});
            Store.Setup(x=>x.Notify(It.IsAny<Guid>(),It.IsAny<string>(),It.IsAny<Guid>(),It.IsAny<string>())).Callback(()=>Events.Add("notify"));
            Store.Setup(x=>x.SaveAsync()).Callback(()=>Events.Add("save")).Returns(Task.CompletedTask);
            Transaction.Setup(x=>x.CommitAsync(default)).Callback(()=>Events.Add("commit")).Returns(Task.CompletedTask);
            Transaction.Setup(x=>x.DisposeAsync()).Returns(ValueTask.CompletedTask);
            var concreteCurrent=new NguoiDungHienTaiService(db,new HttpContextAccessor());
            Service=new(db,Current.Object,new ThongBaoService(db,concreteCurrent),Store.Object);
        }
        public GuiLienHeInput Input(string text="Xin liên hệ")=>new(){DoiTuongId=Other.Id,LoiNhan=text};
        public void AssertNoWrite()
        {
            Store.Verify(x=>x.AddContact(It.IsAny<YeuCauLienHe>()),Times.Never);
            Store.Verify(x=>x.Notify(It.IsAny<Guid>(),It.IsAny<string>(),It.IsAny<Guid>(),It.IsAny<string>()),Times.Never);
            Store.Verify(x=>x.SaveAsync(),Times.Never);Transaction.Verify(x=>x.CommitAsync(It.IsAny<CancellationToken>()),Times.Never);Assert.Null(Added);
        }
        public void AssertSaved(string text)
        {
            Assert.NotNull(Added);Assert.NotEqual(Guid.Empty,Added.Id);Assert.Equal(User.Id,Added.NguoiGuiId);Assert.Equal(User.Id,Added.BenHocId);Assert.Equal(Other.Id,Added.GiaSuId);Assert.Equal("ChoPhanHoi",Added.TrangThai);Assert.Equal(text,Added.LoiNhan);
            Store.Verify(x=>x.ReadRecipientAsync(Other.Id),Times.Once);Store.Verify(x=>x.BeginAsync(),Times.Once);Store.Verify(x=>x.LockProfileAsync(Other.Id),Times.Once);Store.Verify(x=>x.IsHiddenAsync(Other.Id),Times.Once);Store.Verify(x=>x.IsEligibleAsync(Other.Id),Times.Once);Store.Verify(x=>x.ExistsAsync(Other.Id,User.Id),Times.Once);
            Store.Verify(x=>x.AddContact(Added),Times.Once);Store.Verify(x=>x.Notify(Other.Id,"KetNoi",Added.Id,User.HoTen+" gửi yêu cầu liên hệ."),Times.Once);Store.Verify(x=>x.SaveAsync(),Times.Once);Store.VerifyNoOtherCalls();
            Transaction.Verify(x=>x.CommitAsync(default),Times.Once);Current.Verify(x=>x.HienTaiAsync(default),Times.Once);Current.VerifyNoOtherCalls();
        }
        public void Dispose()=>db.Dispose();
    }
}
