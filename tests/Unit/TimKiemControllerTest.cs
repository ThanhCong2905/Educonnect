using EduConnect.Mvc.Areas.TimKiem.Controllers;
using EduConnect.Mvc.Areas.TimKiem.Services;
using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Areas.KetNoi.Services;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Xunit;

namespace EduConnect.SearchContact.UnitTests;

[Trait("Suite","TimKiemLienHe")]
public sealed class TimKiemControllerTest
{
    [Fact,Trait("TestCaseId","UT-029")]
    public async Task UT_029_CoKetQua_TraDungHaiUngVienVaTongTrang()
    {
        using var f=new Fixture();var filter=new BoLocTimKiemViewModel{Trang=2};
        List<UngVienViewModel> rows=[f.Need(),f.Need()];f.Search(filter,rows,12);
        var result=Assert.IsType<ViewResult>(await f.Controller.Tim(filter));
        Assert.Equal("Index",result.ViewName);var model=Assert.IsType<SearchPage>(result.Model);
        Assert.Same(filter,model.Filter);Assert.Equal(2,model.Filter.Trang);Assert.Equal(12,model.Total);Assert.Equal(rows,model.Items);f.VerifySearch(filter);
    }
    [Fact,Trait("TestCaseId","UT-030")]
    public async Task UT_030_KetQuaRong_KhongException()
    {
        using var f=new Fixture();var filter=new BoLocTimKiemViewModel();f.Search(filter,[],0);
        var view=Assert.IsType<ViewResult>(await f.Controller.Tim(filter));var model=Assert.IsType<SearchPage>(view.Model);
        Assert.Equal("Index",view.ViewName);Assert.Empty(model.Items);Assert.Equal(0,model.Total);f.VerifySearch(filter);
        // Text rendered by Razor is outside this controller unit test.
    }
    [Fact,Trait("TestCaseId","UT-031")]
    public async Task UT_031_ServiceTraLoi_TraIndexKemLoiTheoBangGoc()
    {
        using var f=new Fixture();var filter=new BoLocTimKiemViewModel();
        f.Service.Setup(x=>x.TimKiemAsync(filter,f.User.Id,f.Token)).Returns(Error<Page<UngVienViewModel>>("Tiêu chí không hợp lệ.",400));
        var view=Assert.IsType<ViewResult>(await f.Controller.Tim(filter));
        Assert.Equal("Index",view.ViewName);Assert.False(f.Controller.ModelState.IsValid);
        Assert.Contains(f.Controller.ModelState.Values.SelectMany(x=>x.Errors),x=>x.ErrorMessage=="Tiêu chí không hợp lệ.");
    }
    [Fact,Trait("TestCaseId","UT-032")]
    public async Task UT_032_TruyenDayDuBoLocMoi_KichThuocTrangMuoiTheoBangGoc()
    {
        using var f=new Fixture();var filter=new BoLocTimKiemViewModel{MonHocId=2,MonHocIds=[2,3],TrinhDo="Dai hoc",TuKhoa="Ly",KhoiLop="11",HinhThucHoc="TrucTiep",KhuVuc="Ha Noi",HocPhiToiDa=200000,DonViHocPhi="VND/Buoi",ThuTrongTuan=2,GioBatDau=new(18,0),GioKetThuc=new(19,0),SapXep="HocPhiTang",Trang=1};
        var sizeProperty=typeof(BoLocTimKiemViewModel).GetProperty("KichThuocTrang");
        sizeProperty?.SetValue(filter,10);
        var row=f.Need();f.Search(filter,[row],1);
        var view=Assert.IsType<ViewResult>(await f.Controller.Tim(filter));var model=Assert.IsType<SearchPage>(view.Model);
        Assert.Same(filter,model.Filter);Assert.Same(row,Assert.Single(model.Items));f.VerifySearch(filter);
        Assert.NotNull(sizeProperty);Assert.Equal(10,sizeProperty.GetValue(model.Filter));
    }
    [Fact,Trait("TestCaseId","UT-036")]
    public async Task UT_036_ChiTietChuaLienHe_KhongHoiHoiThoai()
    {
        using var f=new Fixture();var row=f.Need();List<UngVienViewModel> related=[f.Need(),f.Need()];
        f.Service.Setup(x=>x.ChiTietAsync(row.Id,default)).ReturnsAsync(row);
        f.Contacts.Setup(x=>x.LienHeVoiAsync(row.ChuSoHuuId)).ReturnsAsync((YeuCauLienHe?)null);
        f.Service.Setup(x=>x.LienQuanAsync(row.ChuSoHuuId)).ReturnsAsync(related);
        var view=Assert.IsType<ViewResult>(await f.Controller.ChiTiet(row.Id));Assert.Same(row,view.Model);
        Assert.Null(view.ViewData["Contact"]);Assert.Null(view.ViewData["ContactChat"]);Assert.Same(related,view.ViewData["Related"]);
        f.Contacts.Verify(x=>x.LienHeVoiAsync(row.ChuSoHuuId),Times.Once);f.Contacts.Verify(x=>x.HoiThoaiAsync(It.IsAny<Guid>()),Times.Never);f.Contacts.VerifyNoOtherCalls();
        f.Service.Verify(x=>x.ChiTietAsync(row.Id,default),Times.Once);f.Service.Verify(x=>x.LienQuanAsync(row.ChuSoHuuId),Times.Once);f.Service.VerifyNoOtherCalls();
    }
    [Fact,Trait("TestCaseId","UT-037")]
    public async Task UT_037_ChiTietLienHeChapNhan_CoHoiThoai()
    {
        using var f=new Fixture();var row=f.Need();var contact=new YeuCauLienHe{Id=Guid.NewGuid(),TrangThai="ChapNhan"};var chat=Guid.NewGuid();List<UngVienViewModel> related=[f.Need(),f.Need()];
        f.Service.Setup(x=>x.ChiTietAsync(row.Id,default)).ReturnsAsync(row);f.Contacts.Setup(x=>x.LienHeVoiAsync(row.ChuSoHuuId)).ReturnsAsync(contact);
        f.Contacts.Setup(x=>x.HoiThoaiAsync(contact.Id)).ReturnsAsync(chat);f.Service.Setup(x=>x.LienQuanAsync(row.ChuSoHuuId)).ReturnsAsync(related);
        var view=Assert.IsType<ViewResult>(await f.Controller.ChiTiet(row.Id));Assert.Same(row,view.Model);
        Assert.Same(contact,view.ViewData["Contact"]);Assert.Equal(chat,Assert.IsType<Guid>(view.ViewData["ContactChat"]));Assert.Same(related,view.ViewData["Related"]);
        f.Contacts.Verify(x=>x.LienHeVoiAsync(row.ChuSoHuuId),Times.Once);f.Contacts.Verify(x=>x.HoiThoaiAsync(contact.Id),Times.Once);f.Contacts.VerifyNoOtherCalls();
        f.Service.Verify(x=>x.ChiTietAsync(row.Id,default),Times.Once);f.Service.Verify(x=>x.LienQuanAsync(row.ChuSoHuuId),Times.Once);f.Service.VerifyNoOtherCalls();
    }
    [Fact,Trait("TestCaseId","UT-045")]
    public async Task UT_045_SoSanhThanhCong_ModelDuHaiUngVien()
    {
        using var f=new Fixture();var a=new HoSoGiaSuCongKhaiViewModel{Id=Guid.NewGuid()};var b=new HoSoGiaSuCongKhaiViewModel{Id=Guid.NewGuid()};Guid[] ids=[a.Id,b.Id];
        var model=new SoSanhViewModel{MaDoiTuong=ids,HoSo=[a,b]};
        f.Service.Setup(x=>x.SoSanhAsync(ids,f.User.Id,f.Token)).Returns(Success(model));
        var view=Assert.IsType<ViewResult>(await f.Controller.SoSanh(ids));
        f.Service.Verify(x=>x.SoSanhAsync(ids,f.User.Id,f.Token),Times.Once);f.Service.VerifyNoOtherCalls();
        Assert.True(view.ViewName==null || view.ViewName=="SoSanh");
        Assert.Equal(new UngVienViewModel[]{a,b},Assert.IsAssignableFrom<IEnumerable<UngVienViewModel>>(view.Model));
    }
    [Fact,Trait("TestCaseId","UT-046")]
    public async Task UT_046_SoSanhLoi_TraIndexKemLoiTheoBangGoc()
    {
        using var f=new Fixture();Guid[] ids=[];
        f.Service.Setup(x=>x.SoSanhAsync(ids,f.User.Id,f.Token)).Returns(Error<SoSanhViewModel>("Số lượng không hợp lệ.",400));
        var view=Assert.IsType<ViewResult>(await f.Controller.SoSanh(ids));
        Assert.Equal("Index",view.ViewName);Assert.False(f.Controller.ModelState.IsValid);
    }
    private static Task<KetQua<T>> Success<T>(T value)=>KetQua<T>.ThucHienAsync(()=>Task.FromResult(value));
    private static Task<KetQua<T>> Error<T>(string text,int status)=>KetQua<T>.ThucHienAsync(()=>throw new LoiNghiepVu(text,status));
    private sealed class Fixture:IDisposable
    {
        private readonly CancellationTokenSource cancellation=new();public CancellationToken Token=>cancellation.Token;
        public NguoiThamGia User {get;}=new(){Id=Guid.NewGuid(),HoTen="U1",LoaiNguoi="HocSinh"};
        public Mock<ITimKiemService> Service {get;}=new(MockBehavior.Strict);
        public Mock<INhuCauService> Needs {get;}=new(MockBehavior.Strict);
        public Mock<IKetNoiService> Contacts {get;}=new(MockBehavior.Strict);
        public Mock<INguoiDungHienTaiService> Current {get;}=new(MockBehavior.Strict);
        public TimKiemController Controller {get;}
        public Fixture()
        {
            Current.Setup(x=>x.HienTaiAsync(It.IsAny<CancellationToken>())).ReturnsAsync(User);
            Needs.Setup(x=>x.MonAsync()).ReturnsAsync([new MonHoc{Id=1,TenMon="Toan",DangSuDung=true}]);
            var context=new DefaultHttpContext{RequestAborted=Token};
            Controller=new(Service.Object,Needs.Object,Contacts.Object,Current.Object){ControllerContext=new(){HttpContext=context},TempData=new TempDataDictionary(context,Mock.Of<ITempDataProvider>())};
        }
        public NhuCauCongKhaiViewModel Need()=>new(){Id=Guid.NewGuid(),ChuSoHuuId=Guid.NewGuid(),LoaiNguoi="HocSinh",HoTen="HS",MonHocId=1};
        public void Search(BoLocTimKiemViewModel filter,List<UngVienViewModel> rows,int total)=>Service.Setup(x=>x.TimKiemAsync(filter,User.Id,Token)).Returns(Success(new Page<UngVienViewModel>{Items=rows,Total=total,Trang=filter.Trang}));
        public void VerifySearch(BoLocTimKiemViewModel filter)
        {
            Service.Verify(x=>x.TimKiemAsync(filter,User.Id,Token),Times.Once);Service.VerifyNoOtherCalls();Needs.Verify(x=>x.MonAsync(),Times.Once);Needs.VerifyNoOtherCalls();Current.Verify(x=>x.HienTaiAsync(It.IsAny<CancellationToken>()),Times.Once);Current.VerifyNoOtherCalls();Contacts.VerifyNoOtherCalls();
        }
        public void Dispose()=>cancellation.Dispose();
    }
}
