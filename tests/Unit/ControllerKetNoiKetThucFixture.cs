using System.Reflection;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using EduConnect.Mvc.Areas.KetNoi.Controllers;
using EduConnect.Mvc.Areas.KetNoi.Services;
using EduConnect.Mvc.Areas.KetNoi.ViewModels;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.HocTap.Controllers;
using EduConnect.Mvc.Areas.HocTap.Services;
using EduConnect.Mvc.Areas.DanhGia.Services;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;

namespace EduConnect.KetNoiKetThuc.UnitTests;

internal enum ConnectAction { GuiYeuCau,GuiDeXuat,ChapNhan,TuChoi,Rut,XacNhanKetThuc,HuyKetNoi,PhanHoiHuy }

internal sealed class ControllerKetNoiKetThucFixture:IDisposable
{
    public const string ErrorText="Lỗi mẫu";
    public static readonly JsonSerializerOptions JsonOptions=new(){Encoder=JavaScriptEncoder.UnsafeRelaxedJsonEscaping};
    private readonly CancellationTokenSource cancellation=new();
    private readonly AppDbContext db=new(new DbContextOptionsBuilder<AppDbContext>().Options);
    private readonly ServiceProvider provider;
    public CancellationToken Token=>cancellation.Token;
    public NguoiThamGia User {get;}=new(){Id=Guid.NewGuid(),HoTen="U1",LoaiNguoi="HocSinh"};
    public Guid RequestId {get;}=Guid.NewGuid();public Guid ContactId {get;}=Guid.NewGuid();
    public Guid NeedId {get;}=Guid.NewGuid();public Guid ClassId {get;}=Guid.NewGuid();public Guid C1 {get;}=Guid.NewGuid();
    public Guid ForgedActor {get;}=Guid.NewGuid();
    public byte[] RV {get;}=[1,2,3,4,5,6,7,8];private readonly byte[] expectedRV=[1,2,3,4,5,6,7,8];
    public string Reason {get;}="Lý do kiểm thử giữ nguyên";
    public Mock<IKetNoiService> Connect {get;}=new(MockBehavior.Strict);
    public Mock<INhuCauService> Needs {get;}=new(MockBehavior.Strict);
    public Mock<ILopHocService> Classes {get;}=new(MockBehavior.Strict);
    public Mock<IBaiTapService> Tasks {get;}=new(MockBehavior.Strict);
    public Mock<INguoiDungHienTaiService> Current {get;}=new(MockBehavior.Strict);
    public KetNoiController Controller {get;}public LopHocController ClassController {get;}
    public GuiYeuCauViewModel Request {get;}
    public DeXuatLopViewModel Proposal {get;}
    public ChapNhanDeXuatViewModel Acceptance {get;}
    public PhanHoiHuyViewModel Reply {get;}
    public object ServiceResult {get;private set;}=null!;
    public ConnectAction Action {get;private set;}
    public int Phase {get;private set;}
    public Controller ActiveController=>Action==ConnectAction.XacNhanKetThuc?ClassController:Controller;
    public string ServiceMethod=>Action switch{
        ConnectAction.GuiYeuCau=>"GuiYeuCauAsync",ConnectAction.GuiDeXuat=>"GuiDeXuatAsync",ConnectAction.ChapNhan=>"ChapNhanDeXuatAsync",ConnectAction.TuChoi=>"TuChoiDeXuatAsync",ConnectAction.Rut=>"RutYeuCauAsync",ConnectAction.XacNhanKetThuc=>"XacNhanKetThucAsync",ConnectAction.HuyKetNoi=>"TaoYeuCauHuyAsync",_=>"PhanHoiHuyAsync"};
    public ControllerKetNoiKetThucFixture()
    {
        Request=new(){LienHeId=ContactId,NhuCauId=NeedId,MaDoiTuong=Guid.NewGuid(),LoiNhan="Lời nhắn gốc"};
        var requestVersion=typeof(GuiYeuCauViewModel).GetProperty("RowVersion");
        if(requestVersion?.CanWrite==true)
        {
            if(requestVersion.PropertyType==typeof(byte[]))requestVersion.SetValue(Request,RV);
            else if(requestVersion.PropertyType==typeof(string))requestVersion.SetValue(Request,Convert.ToBase64String(RV));
        }
        Proposal=new(){Id=RequestId,LienHeId=ContactId,NhuCauId=NeedId,RowVersion=Convert.ToBase64String(RV),TenLop="Lớp kiểm thử",MonHocId=7,MucTieu="Mục tiêu gốc",SoBuoiTuan=2,ThoiLuongPhut=60,BatDauHocLuc=new(2030,1,2,18,0,0),HinhThucHoc="TrucTiep",DiaDiem="Địa điểm gốc",HocPhi=125000m,DonViHocPhi="VND/Buoi",LichRanh=[new(){ThuTrongTuan=1,GioBatDau=new(18,0),GioKetThuc=new(19,0)},new(){ThuTrongTuan=3,GioBatDau=new(18,0),GioKetThuc=new(19,0)}]};
        Acceptance=new(){MaYeuCau=RequestId,RowVersion=RV};Reply=new(){MaYeuCauHuy=RequestId,DongY=true,RowVersion=RV};
        Current.Setup(x=>x.HienTaiAsync(It.IsAny<CancellationToken>())).ReturnsAsync(User);
        provider=new ServiceCollection().AddSingleton<INguoiDungHienTaiService>(Current.Object).BuildServiceProvider();
        var context=Context();Controller=new(Connect.Object,Needs.Object){ControllerContext=new(){HttpContext=context},TempData=new TempDataDictionary(context,Mock.Of<ITempDataProvider>())};
        Controller.Url=Mock.Of<IUrlHelper>();
        var env=new Mock<IWebHostEnvironment>();env.SetupGet(x=>x.EnvironmentName).Returns("UnitTest");env.SetupGet(x=>x.ContentRootPath).Returns(Path.GetTempPath());
        var concreteCurrent=new NguoiDungHienTaiService(db,new HttpContextAccessor());
        var rights=new QuyenLopHocService(db,concreteCurrent);var files=new TepService(db,concreteCurrent,rights,env.Object);
        var chat=new TinNhanService(db,concreteCurrent,rights,files);
        var classContext=Context();ClassController=new(Classes.Object,Tasks.Object,chat,new TienDoService(db,rights)){ControllerContext=new(){HttpContext=classContext},TempData=new TempDataDictionary(classContext,Mock.Of<ITempDataProvider>())};
        ClassController.Url=Mock.Of<IUrlHelper>();
        Needs.Setup(x=>x.MonDuocChonAsync()).ReturnsAsync([new MonHoc{Id=7,TenMon="Toán",DangSuDung=true}]);
        Connect.Setup(x=>x.LayAsync()).ReturnsAsync((new List<YeuCauLienHe>{new(){Id=ContactId,GiaSuId=Guid.NewGuid(),BenHocId=User.Id,TrangThai="ChapNhan"}},new List<YeuCauKetNoiLopHoc>()));
    }
    private DefaultHttpContext Context()
    {
        var context=new DefaultHttpContext{RequestAborted=Token,RequestServices=provider};
        context.User=new ClaimsPrincipal(new ClaimsIdentity(new[]{new Claim(ClaimTypes.NameIdentifier,User.Id.ToString())},"UnitTest"));
        context.Request.ContentType="application/x-www-form-urlencoded";
        context.Request.Form=new FormCollection(new Dictionary<string,StringValues>{["maNguoiThucHien"]=ForgedActor.ToString(),["LichRanh[0].ThuTrongTuan"]="1",["LichRanh[0].GioBatDau"]="18:00",["LichRanh[0].GioKetThuc"]="19:00"});
        return context;
    }
    public async Task ArrangeAsync(ConnectAction action,int phase)
    {
        Action=action;Phase=phase;int? code=phase is >=2 and <=6?new[]{403,404,409,400,500}[phase-2]:null;
        if(phase==0)ActiveController.ModelState.AddModelError("field",ErrorText);
        if(action is ConnectAction.GuiYeuCau or ConnectAction.GuiDeXuat or ConnectAction.ChapNhan or ConnectAction.HuyKetNoi)
        {
            var result=await KetQua<Guid>.ThucHienAsync(()=>code.HasValue?throw new LoiNghiepVu(ErrorText,code.Value):Task.FromResult(C1));ServiceResult=result;
            switch(action)
            {
                case ConnectAction.GuiYeuCau:Connect.Setup(x=>x.GuiYeuCauAsync(It.IsAny<GuiYeuCauViewModel>(),It.IsAny<Guid>(),It.IsAny<CancellationToken>())).ReturnsAsync(result);break;
                case ConnectAction.GuiDeXuat:Connect.Setup(x=>x.GuiDeXuatAsync(It.IsAny<DeXuatLopViewModel>(),It.IsAny<Guid>(),It.IsAny<CancellationToken>())).ReturnsAsync(result);break;
                case ConnectAction.ChapNhan:Connect.Setup(x=>x.ChapNhanDeXuatAsync(It.IsAny<Guid>(),It.IsAny<byte[]>(),It.IsAny<Guid>(),It.IsAny<CancellationToken>())).ReturnsAsync(result);break;
                case ConnectAction.HuyKetNoi:Connect.Setup(x=>x.TaoYeuCauHuyAsync(It.IsAny<Guid>(),It.IsAny<string>(),It.IsAny<byte[]>(),It.IsAny<Guid>(),It.IsAny<CancellationToken>())).ReturnsAsync(result);break;
            }
        }
        else
        {
            var result=await KetQua.ThucHienAsync(()=>code.HasValue?throw new LoiNghiepVu(ErrorText,code.Value):Task.CompletedTask);ServiceResult=result;
            switch(action)
            {
                case ConnectAction.TuChoi:Connect.Setup(x=>x.TuChoiDeXuatAsync(It.IsAny<Guid>(),It.IsAny<string>(),It.IsAny<byte[]>(),It.IsAny<Guid>(),It.IsAny<CancellationToken>())).ReturnsAsync(result);break;
                case ConnectAction.Rut:Connect.Setup(x=>x.RutYeuCauAsync(It.IsAny<Guid>(),It.IsAny<byte[]>(),It.IsAny<Guid>(),It.IsAny<CancellationToken>())).ReturnsAsync(result);break;
                case ConnectAction.XacNhanKetThuc:Classes.Setup(x=>x.XacNhanKetThucAsync(It.IsAny<Guid>(),It.IsAny<byte[]>(),It.IsAny<Guid>(),It.IsAny<CancellationToken>())).ReturnsAsync(result);break;
                case ConnectAction.PhanHoiHuy:Connect.Setup(x=>x.PhanHoiHuyAsync(It.IsAny<Guid>(),It.IsAny<bool>(),It.IsAny<byte[]>(),It.IsAny<Guid>(),It.IsAny<CancellationToken>())).ReturnsAsync(result);break;
            }
        }
    }
    public void VerifyDesignedAction()
    {
        Type[] types=Action switch{ConnectAction.GuiYeuCau=>[typeof(GuiYeuCauViewModel)],ConnectAction.GuiDeXuat=>[typeof(DeXuatLopViewModel)],ConnectAction.ChapNhan=>[typeof(ChapNhanDeXuatViewModel)],ConnectAction.TuChoi or ConnectAction.HuyKetNoi=>[typeof(Guid),typeof(string),typeof(byte[])],ConnectAction.Rut or ConnectAction.XacNhanKetThuc=>[typeof(Guid),typeof(byte[])],_=>[typeof(PhanHoiHuyViewModel)]};
        var method=ActiveController.GetType().GetMethod(Action.ToString(),types);
        Assert.True(method!=null,$"Thiếu action {ActiveController.GetType().Name}.{Action} với đúng overload đầu vào theo thiết kế.");
        Assert.Equal(typeof(Task<IActionResult>),method.ReturnType);Assert.NotNull(method.GetCustomAttribute<HttpPostAttribute>());
    }
    public Task<IActionResult> InvokeAsync()=>Action switch{
        ConnectAction.GuiYeuCau=>Controller.GuiYeuCau(Request),ConnectAction.GuiDeXuat=>Controller.GuiDeXuat(Proposal),ConnectAction.ChapNhan=>Controller.ChapNhan(Acceptance),ConnectAction.TuChoi=>Controller.TuChoi(RequestId,Reason,RV),ConnectAction.Rut=>Controller.Rut(RequestId,RV),ConnectAction.XacNhanKetThuc=>ClassController.XacNhanKetThuc(RequestId,RV),ConnectAction.HuyKetNoi=>Controller.HuyKetNoi(ClassId,Reason,RV),_=>Controller.PhanHoiHuy(Reply)};
    public string InputSnapshot()=>JsonSerializer.Serialize(Action switch{ConnectAction.GuiYeuCau=>(object)Request,ConnectAction.GuiDeXuat=>Proposal,ConnectAction.ChapNhan=>Acceptance,ConnectAction.PhanHoiHuy=>Reply,ConnectAction.HuyKetNoi=>new{maLop=ClassId,lyDo=Reason,rowVersion=RV},ConnectAction.TuChoi=>new{maYeuCau=RequestId,lyDo=Reason,rowVersion=RV},_=>new{maYeuCau=RequestId,rowVersion=RV}},JsonOptions);
    public object Observe(IActionResult? result,Exception? exception)=>new{
        action=ActiveController.GetType().Name+"."+Action,
        input=JsonSerializer.Deserialize<JsonElement>(InputSnapshot()),actor=User.Id,forgedActor=ForgedActor,
        resultType=result?.GetType().Name,viewName=(result as ViewResult)?.ViewName,
        redirectAction=(result as RedirectToActionResult)?.ActionName,redirectUrl=(result as RedirectResult)?.Url,
        objectStatus=(result as ObjectResult)?.StatusCode,
        exceptionType=exception?.GetType().Name,exceptionMessage=exception?.Message,exceptionStatus=(exception as LoiNghiepVu)?.Status,exceptionStack=exception?.StackTrace,
        success=ActiveController.TempData.TryGetValue("Success",out var success)?success:null,
        modelState=ActiveController.ModelState.ToDictionary(p=>p.Key,p=>p.Value!.Errors.Select(e=>e.ErrorMessage).ToArray()),
        coreServiceMethod=ServiceMethod,coreServiceCalls=CoreCalls,
        serviceCalls=Connect.Invocations.Select(i=>new{method=i.Method.Name,arguments=i.Arguments.Select(SafeArg).ToArray()}).ToArray(),
        classServiceCalls=Classes.Invocations.Select(i=>new{method=i.Method.Name,arguments=i.Arguments.Select(SafeArg).ToArray()}).ToArray(),
        currentCalls=Current.Invocations.Select(i=>new{method=i.Method.Name,tokenMatches=i.Arguments.OfType<CancellationToken>().Any(t=>t==Token)}).ToArray()};
    private object? SafeArg(object? value)=>value is CancellationToken t?new{type="CancellationToken",matchesRequestAborted=t==Token}:value;
    public int CoreCalls=>Action==ConnectAction.XacNhanKetThuc?Classes.Invocations.Count(i=>i.Method.Name==ServiceMethod):Connect.Invocations.Count(i=>i.Method.Name==ServiceMethod);
    public void VerifyCore(Times times)
    {
        switch(Action)
        {
            case ConnectAction.GuiYeuCau:Connect.Verify(x=>x.GuiYeuCauAsync(Request,User.Id,Token),times);break;
            case ConnectAction.GuiDeXuat:Connect.Verify(x=>x.GuiDeXuatAsync(Proposal,User.Id,Token),times);break;
            case ConnectAction.ChapNhan:Connect.Verify(x=>x.ChapNhanDeXuatAsync(RequestId,It.Is<byte[]>(v=>v.SequenceEqual(expectedRV)),User.Id,Token),times);break;
            case ConnectAction.TuChoi:Connect.Verify(x=>x.TuChoiDeXuatAsync(RequestId,Reason,It.Is<byte[]>(v=>v.SequenceEqual(expectedRV)),User.Id,Token),times);break;
            case ConnectAction.Rut:Connect.Verify(x=>x.RutYeuCauAsync(RequestId,It.Is<byte[]>(v=>v.SequenceEqual(expectedRV)),User.Id,Token),times);break;
            case ConnectAction.XacNhanKetThuc:Classes.Verify(x=>x.XacNhanKetThucAsync(RequestId,It.Is<byte[]>(v=>v.SequenceEqual(expectedRV)),User.Id,Token),times);break;
            case ConnectAction.HuyKetNoi:Connect.Verify(x=>x.TaoYeuCauHuyAsync(ClassId,Reason,It.Is<byte[]>(v=>v.SequenceEqual(expectedRV)),User.Id,Token),times);break;
            case ConnectAction.PhanHoiHuy:Connect.Verify(x=>x.PhanHoiHuyAsync(RequestId,true,It.Is<byte[]>(v=>v.SequenceEqual(expectedRV)),User.Id,Token),times);break;
        }
    }
    public void VerifyArgumentsAndToken()
    {
        VerifyCore(Times.Once());Current.Verify(x=>x.HienTaiAsync(Token),Times.Once);Current.VerifyNoOtherCalls();
        Connect.VerifyNoOtherCalls();Classes.VerifyNoOtherCalls();Needs.VerifyNoOtherCalls();Tasks.VerifyNoOtherCalls();
    }
    public void Dispose(){provider.Dispose();cancellation.Dispose();db.Dispose();}
}
