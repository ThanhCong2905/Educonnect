using System.Reflection;
using System.Text.Json;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using EduConnect.Mvc.Areas.DanhGia.ViewModels;
using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Areas.QuanTri.ViewModels;
using EduConnect.Mvc.Areas.QuanTri.Services;
using EduConnect.Mvc.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using Xunit.Abstractions;
namespace EduConnect.Remaining.UnitTests;

public sealed partial class RemainingUnitTest(ITestOutputHelper output)
{
    readonly List<string> failures=[];readonly Dictionary<string,object?> observation=[];
    void Check(bool condition,string reason){if(!condition)failures.Add(reason);}
    void Note(string name,object? value)=>observation[name]=value;
    void Finish(){observation["assertionFailures"]=failures;output.WriteLine("OBSERVATION_JSON:"+JsonSerializer.Serialize(observation));Assert.True(failures.Count==0,string.Join("\n",failures));}
    static int Status(Exception? ex)=>ex is LoiNghiepVu e?e.Status:ex==null?200:500;
    static async Task<Exception?> Capture(Func<Task> action){try{await action();return null;}catch(Exception e){return e;}}
    async Task Run(int n)
    {
        Note("id",$"UT-{n:000}");Note("mode","Behavior");
        try
        {
            if(n<=303)await Filter(n);
            else if(n<=305)await Event(n);
            else if(n<=310)Contract("ClassAccessService", "Infrastructure/QuyenLopHocService.cs", "Không có lớp ClassAccessService theo thiết kế; QuyenLopHocService là lớp khác, không được tự thay để kết luận Pass.");
            else if(n<=317)await File(n);
            else if(n<=322)await Feedback(n);
            else if(n<=330)await Chat(n);
            else if(n<=348)await Administration(n);
            else if(n<=355)await Account(n);
            else if(n<=361)await Password(n);
            else Contract(n<=368?"LoginAttemptService":"UserSession","Infrastructure/"+(n<=368?"TaiKhoanService.cs":"PhienDangNhapService.cs"),"Thiếu đơn vị C# trong thiết kế. Quy tắc hiện được gọi qua stored procedure; unit này chỉ kết luận thiếu hợp đồng C#, chưa thực thi thuật toán SQL.");
        }
        catch(Exception e){Note("unexpectedException",e.ToString());Check(false,"Lỗi khi thực thi ca: "+e.GetType().Name+": "+e.Message);}
        Finish();
    }
    void Contract(string name,string source,string explanation)
    {
        var found=typeof(AppDbContext).Assembly.GetTypes().Where(x=>x.Name==name).Select(x=>x.FullName).ToArray();
        Note("mode","DesignContract");Note("requiredType",name);Note("found",found);Note("source",source);Note("explanation",explanation);Check(found.Length==1,explanation);
    }
    async Task Filter(int n)
    {
        var http=new DefaultHttpContext();http.Request.Headers["X-Requested-With"]="XMLHttpRequest";
        var temp=new TempDataDictionary(http,Mock.Of<ITempDataProvider>());var factory=new Mock<ITempDataDictionaryFactory>();factory.Setup(x=>x.GetTempData(http)).Returns(temp);
        var filter=new UiResponseFilter(factory.Object);var ac=new ActionContext(http,new RouteData(),new ActionDescriptor());
        Note("source",n==303?"Infrastructure/LoiNghiepVuMiddleware.cs":"Infrastructure/UiResponseFilter.cs");
        Note("mapping","UIResponseFilter trong bảng là UiResponseFilter trong code (khác viết hoa).");
        if(n==301)
        {
            var context=new ExceptionContext(ac,[]){Exception=new LoiNghiepVu("Lỗi nghiệp vụ mẫu",409)};filter.OnException(context);
            var r=context.Result as JsonResult;var json=JsonSerializer.SerializeToElement(r?.Value);
            Note("status",r?.StatusCode);Note("json",json);Note("handled",context.ExceptionHandled);
            Check(r?.StatusCode==409 && context.ExceptionHandled,"Phải giữ 409 và xử lý exception.");Check(!json.GetProperty("ok").GetBoolean()&&json.GetProperty("message").GetString()=="Lỗi nghiệp vụ mẫu","JSON phải có ok=false/message đúng.");
        }
        else if(n==302)
        {
            temp["Success"]="Đã gửi liên hệ.";var result=new RedirectResult("/KetNoi/LienHe");var context=new ResultExecutingContext(ac,[],result,new TestController());int calls=0;
            await filter.OnResultExecutionAsync(context,()=>{calls++;return Task.FromResult(new ResultExecutedContext(ac,[],context.Result,context.Controller));});
            var json=JsonSerializer.SerializeToElement((context.Result as JsonResult)?.Value);Note("json",json);Note("nextCalls",calls);
            Check(json.GetProperty("ok").GetBoolean()&&json.GetProperty("message").GetString()=="Đã gửi liên hệ."&&json.GetProperty("redirectUrl").GetString()=="/KetNoi/LienHe","Phải giữ success và URL liên hệ.");Check(calls==1,"Next chỉ gọi một lần.");
        }
        else
        {
            http.Response.Body=new MemoryStream();var m=new LoiNghiepVuMiddleware(_=>Task.FromException(new LoiNghiepVu("Không thấy <mẫu>",404)),NullLogger<LoiNghiepVuMiddleware>.Instance);await m.InvokeAsync(http);http.Response.Body.Position=0;var body=await new StreamReader(http.Response.Body).ReadToEndAsync();Note("status",http.Response.StatusCode);Note("body",body);Check(http.Response.StatusCode==404 && body.Contains("&lt;mẫu&gt;"),"Middleware phải trả 404, encode nội dung lỗi.");
        }
    }
    sealed class TestController:Controller{}
    async Task Event(int n)
    {
        using var f=new EduConnect.KetThucWorker.UnitTests.KetThucWorkerFixture();
        bool ok;Guid request;
        if(n==304){var r=await f.Service.TaoYeuCauHuyAsync(f.C,"Xin kết thúc",f.RV,f.H,f.Token);ok=r.ThanhCong;request=f.Added.Single().Id;}
        else {
            var current=new NguoiDungHienTaiService(f.Db,new HttpContextAccessor());
            var wrapper=new EduConnect.Mvc.Areas.HocTap.Services.LopHocService(f.Db,current,new QuyenLopHocService(f.Db,current),f.NoticeService,f.Service);
            var r=await wrapper.XacNhanKetThucAsync(f.R,f.RV,f.H,f.Token);ok=r.ThanhCong;request=f.R;
        }
        Note("source","Areas/KetNoi/Services/KetNoiService.KetThuc.cs");Note("resultSuccess",ok);Note("requestId",request);Note("classId",f.C);Note("events",f.Events);Note("commits",f.Commits);
        Check(ok,"Thao tác chính phải thành công.");Check(f.Events.Count==1,"Chỉ một sự kiện.");
        var expected=n==304?"YeuCauKetThucLop":"PhanHoiKetThucLop";
        Check(f.Events.All(x=>x.LoaiSuKien==expected),"Sai LoaiSuKien: kỳ vọng "+expected);
        Check(f.Events.All(x=>x.DuLieuToiThieu.Contains(request.ToString(),StringComparison.OrdinalIgnoreCase)),"Payload thiếu mã yêu cầu; không dùng mã lớp thay thế.");
        Check(f.Events.All(x=>x.TrangThai=="ChoXuLy"),"Trạng thái sự kiện phải ChoXuLy.");
        if(n==304)Check(f.Events.All(x=>x.TaoLuc==f.Now),"TaoLuc phải bằng đồng hồ T.");
    }
    async Task File(int n)
    {
        int size=n switch{311=>9999999,312 or 316=>10000000,313=>10000001,_=>1000000};int count=n switch{314 or 316=>5,315=>6,_=>1};
        var scanner=new Mock<IFileContentScanner>(MockBehavior.Strict);scanner.Setup(x=>x.ScanAsync(It.IsAny<byte[]>())).Returns(Task.CompletedTask);
        var env=new Mock<IWebHostEnvironment>();env.SetupGet(x=>x.EnvironmentName).Returns(Environments.Production);env.SetupGet(x=>x.ContentRootPath).Returns("unused-unit-path");
        var service=new TepService(null!,null!,null!,env.Object,scanner:scanner.Object);
        var files=new List<IFormFile>();for(int i=0;i<count;i++){var bytes=new byte[size];"%PDF-"u8.CopyTo(bytes);files.Add(new FormFile(new MemoryStream(bytes),0,bytes.Length,"Files",$"file{i}"+(n==317?".exe":".pdf")));}
        List<(string Name,string Mime,byte[] Bytes)>? rows=null;var ex=await Capture(async()=>rows=await service.KiemTraAsync(files));
        int scans=scanner.Invocations.Count;Note("source","Infrastructure/TepService.cs");Note("fileCount",count);Note("eachLength",size);Note("status",Status(ex));Note("message",ex?.Message);Note("field",(ex as LoiNghiepVu)?.Truong);Note("returnedLengths",rows?.Select(x=>x.Bytes.Length).ToArray());Note("scannerCalls",scans);
        bool reject=n is 313 or 315 or 317;Check(reject?ex is LoiNghiepVu:ex==null,"Sai kết quả chấp nhận/từ chối tệp.");Check(scans==(reject?0:count),"Số lần quét không đúng hoặc đã xử lý một phần ca bị từ chối.");Check(reject?rows==null:rows?.Count==count,"Kết quả tệp không đúng.");
        foreach(var file in files)((FormFile)file).OpenReadStream().Dispose();
    }
    async Task Feedback(int n)
    {
        using var f=new RemainingFixture();Note("source","Areas/DanhGia/Services/DanhGiaService.cs");
        if(n==321){Note("mode","DesignContract");var prop=typeof(GuiDanhGiaViewModel).GetProperty("MaThaoTac");Note("operationIdProperty",prop?.Name);Check(prop!=null,"GuiDanhGiaViewModel không có MaThaoTac; không truyền được mã thao tác đã thành công để kiểm retry theo thiết kế.");return;}
        var model=new GuiDanhGiaViewModel{MaLop=f.C,DotPhanHoiId=f.K,NoiDung=" Phản hồi hợp lệ ",MucDoHaiLong=4};
        if(n==319)model.NoiDung="";
        var other=new PhieuDanhGia{LopHocId=f.C,DotPhanHoiId=f.K,NguoiGuiId=f.G,NoiDung="Của gia sư",MucDoHaiLong=5,GuiLuc=f.Data.Now};f.Data.Seed(other);
        if(n==320)
        {
            f.Data.Seed(new PhieuDanhGia{LopHocId=f.C,DotPhanHoiId=f.K,NguoiGuiId=f.U,NoiDung="Học sinh"},new PhieuDanhGia{LopHocId=f.C,DotPhanHoiId=f.K,NguoiGuiId=f.P,NoiDung="Phụ huynh"},new PhieuDanhGia{LopHocId=f.C,DotPhanHoiId=f.M,NguoiGuiId=f.U,NoiDung="Kỳ khác"});
            var r=await f.Feedback().LayTheoLopAsync(f.C,f.U,default);Note("rows",r.DuLieu);Note("success",r.ThanhCong);Note("saves",f.Data.Saves);Check(r.ThanhCong,"Đọc thất bại.");Check(r.DuLieu?.Count==3 && r.DuLieu.All(x=>x.DotPhanHoiId==f.K),"Kỳ vọng ba nguồn của K1; thực tế truy vấn chỉ lọc lớp nên trộn cả kỳ K2.");Check(f.Data.Saves==0,"Truy vấn tổng hợp không được ghi.");return;
        }
        if(n==322){f.Data.Seed(new PhieuDanhGia{LopHocId=f.C,DotPhanHoiId=f.K,NguoiGuiId=f.U,NoiDung="Nội dung đã lưu",RowVersion=f.RV});model.RowVersion=Convert.ToBase64String(new byte[8]);f.Data.SaveError=new DbUpdateConcurrencyException("Stale RowVersion giả lập tại cổng Save");}
        var result=await f.Feedback().GuiAsync(model,f.U,default);Note("success",result.ThanhCong);Note("status",result.MaTrangThai);Note("message",result.ThongDiep);Note("added",f.Data.Added);Note("saves",f.Data.Saves);Note("commits",f.Data.Commits);Note("version",f.Data.Version);
        if(n==318){var added=f.Data.Added.OfType<PhieuDanhGia>().Single();Check(result.ThanhCong&&f.Data.Commits==1,"Phải lưu thành công và commit.");Check(added.NguoiGuiId==f.U&&added.LopHocId==f.C&&added.DotPhanHoiId==f.K&&added.NoiDung==model.NoiDung.Trim(),"Lưu sai người/lớp/kỳ/nội dung.");Check(other.NoiDung=="Của gia sư","Đã ghi đè phản hồi bên khác.");}
        if(n==319){var ex=await Capture(()=>{Rules.ValidateInput(model);return Task.CompletedTask;});Note("validatorField",(ex as LoiNghiepVu)?.Truong);Check(!result.ThanhCong&&f.Data.Saves==0&&(ex as LoiNghiepVu)?.Truong=="NoiDung","Phải có lỗi trường NoiDung và không Save.");}
        if(n==322){Check(!result.ThanhCong&&result.MaTrangThai==409&&f.Data.Commits==0,"RowVersion cũ phải trả 409 và không commit.");Check(f.Data.Version==model.RowVersion&&f.Data.Saves==1,"Phải chuyển RowVersion vào cổng lưu để phát hiện xung đột.");Note("scope","Mock Save ném concurrency; không khẳng định RAM được rollback hoặc SQL rowversion đã chạy.");}
    }
    async Task Chat(int n)
    {
        using var f=new RemainingFixture();Note("source","Areas/HocTap/Services/TinNhanService.cs");var room=f.Data.Rows<CuocHoiThoai>().Single();
        if(n==323){room.LoaiHoiThoai="TruocKetNoi";room.LienHeId=f.K;f.Data.Seed(new YeuCauLienHe{Id=f.K,TrangThai="ChoPhanHoi"});}
        if(n is 324 or 325)f.Data.Rows<LopHoc>().Single().TrangThai="DaKetThuc";
        var model=new GuiTinNhanViewModel{MaHoiThoai=f.Room,MaThaoTac=f.M,NoiDung=n==326?" ":n==329?"B":"A"};
        TinNhan? old=null;if(n is 325 or 328 or 329 or 330){old=new(){Id=f.K,CuocHoiThoaiId=f.Room,NguoiGuiId=f.U,MaThaoTac=f.M,NoiDung="A",GuiLuc=f.Data.Now};f.Data.Seed(old);}
        if(n==330){f.Rights.Setup(x=>x.KiemTraQuyenXemAsync(f.C,f.U)).ThrowsAsync(new LoiNghiepVu("Quyền đã thu hồi",403));}
        List<TinNhan>? history=null;var ex=await Capture(async()=>{if(n is 323 or 325)history=await f.Chat().LichSuAsync(f.Room);else await f.Chat().GuiAsync(model);});
        Note("status",Status(ex));Note("message",ex?.Message);Note("history",history);Note("added",f.Data.Added);Note("saves",f.Data.Saves);Note("commits",f.Data.Commits);Note("order",f.Data.Order);Note("oldText",old?.NoiDung);Note("mapping","ClientMessageId trong kịch bản được ánh xạ MaThaoTac của GuiTinNhanViewModel.");
        if(n is 323 or 324 or 330){Check(Status(ex)==403,"Phải từ chối với 403.");Check(f.Data.Added.Count==0&&f.Data.Saves==0&&history==null,"Không thêm tin hoặc trả lịch sử khi bị từ chối.");}
        if(n==325){Check(ex==null&&history?.Count==1&&history[0].Id==old!.Id,"Phải giữ lịch sử người đọc được phép xem.");Check(f.Data.Saves==0&&f.Data.Removed.Count==0,"Đọc không được sửa/xóa.");}
        if(n==326)Check(ex is LoiNghiepVu&&f.Data.Saves==0&&f.Data.Added.Count==0,"Tin rỗng phải lỗi trước khi ghi.");
        if(n==327){var msg=f.Data.Added.OfType<TinNhan>().Single();Check(ex==null&&msg.CuocHoiThoaiId==f.Room&&msg.NguoiGuiId==f.U&&msg.NoiDung=="A"&&msg.MaThaoTac==f.M,"Tin hợp lệ phải lưu đúng dữ liệu.");Check(f.Data.Commits==1&&f.Data.Saves==2&&f.Data.Order.Last()=="Commit","Chỉ hoàn tất sau Save/Commit.");Check(f.Data.Added.OfType<SuKienChoXuLy>().Count()==1,"Phải một outbox.");}
        if(n==328){Check(ex==null&&f.Data.Added.Count==0&&f.Data.Saves==0,"Retry cùng nội dung không được thêm tin/outbox.");Note("returnType",typeof(EduConnect.Mvc.Areas.HocTap.Services.TinNhanService).GetMethod("GuiAsync")!.ReturnType.Name);Check(typeof(EduConnect.Mvc.Areas.HocTap.Services.TinNhanService).GetMethod("GuiAsync")!.ReturnType!=typeof(Task),"Không thêm trùng đã đạt; nhưng GuiAsync trả Task không mang tin đã lưu như hợp đồng kịch bản.");}
        if(n==329)Check(Status(ex)==409&&old?.NoiDung=="A"&&f.Data.Added.Count==0&&f.Data.Saves==0,"Cùng mã khác nội dung phải 409 và giữ A.");
    }
    async Task Administration(int n)
    {
        using var f=new RemainingFixture();Note("source","Areas/QuanTri/Services/QuanTriService.cs");
        if(n<=333)
        {
            Note("source","Areas/QuanTri/Services/BaoCaoViPhamService.cs");f.Data.Seed(new IdentityUserClaim<Guid>{UserId=f.A,ClaimType=QuyenQuanTriService.ClaimType,ClaimValue="XuLyBaoCao"});
            var input=new TaoBaoCaoViewModel{MaThaoTac=f.M,LoaiDoiTuong="TaiKhoan",DoiTuongId=f.G,LyDo=n==333?"":"Sai thông tin",MoTa="Thông tin cần xem xét lại."};
            if(n==332)f.Data.Seed(new BaoCaoViPham{Id=f.K,NguoiBaoCaoId=f.U,DoiTuongId=f.G,LoaiDoiTuong="TaiKhoan",TrangThai="ChoXuLy",MaThaoTac=Guid.NewGuid(),LyDo="Lý do cũ",MoTa="Nội dung cũ"});
            var result=await f.Reports().GuiAsync(input,f.U,default);Note("success",result.ThanhCong);Note("status",result.MaTrangThai);Note("message",result.ThongDiep);if(result.ThanhCong)Note("returnedId",result.DuLieu);
            Note("added",f.Data.Added);Note("saves",f.Data.Saves);Note("notices",f.Sent.Select(x=>new{x.User,x.Type,x.Target,x.Text}).ToArray());
            if(n==331){var report=f.Data.Added.OfType<BaoCaoViPham>().Single();Check(result.ThanhCong&&report.NguoiBaoCaoId==f.U&&report.DoiTuongId==f.G&&report.TrangThai=="ChoXuLy"&&f.Data.Commits==1,"Báo cáo hợp lệ phải lưu đúng/commit.");Check(f.Sent.Any(x=>x.User==f.A&&x.Target==report.Id),"Thiếu thông báo admin.");Check(f.Sent.Any(x=>x.User==f.U&&x.Target==report.Id),"Thiếu xác nhận gửi báo cáo cho người gửi.");}
            if(n==332){Check(f.Data.Added.Count==0&&f.Data.Saves==0,"Báo cáo trùng không được ghi thêm.");Check(result.ThanhCong&&result.DuLieu==f.K,"Kỳ vọng thông tin/mã báo cáo cũ để bổ sung; hiện trả 409 và chỉ có message.");}
            if(n==333){var ex=await Capture(()=>{Rules.ValidateInput(input);return Task.CompletedTask;});Note("validatorField",(ex as LoiNghiepVu)?.Truong);Check(!result.ThanhCong&&f.Data.Saves==0&&(ex as LoiNghiepVu)?.Truong=="LyDo","Phải lỗi trường LyDo không lưu.");}return;
        }
        if(n is >=334 and <=337)
        {
            f.Data.Rows<LopHoc>().Clear();
            var user=f.Data.Rows<TaiKhoan>().Single(x=>x.Id==f.U);user.TrangThai=n is 335 or 336?"BiKhoa":"HoatDong";
            var session=new PhienDangNhap{Id=f.M,TaiKhoanId=f.U,ThuHoiLuc=n==335?f.Data.Now.AddMinutes(-2):null};f.Data.Seed(session);
            BienPhapQuanTri? measure=null;if(n is 335 or 336){measure=new(){Id=f.Room,Loai="KhoaTaiKhoan",LoaiDoiTuong="TaiKhoan",DoiTuongId=f.U,NguoiApDungId=f.A,ApDungLuc=f.Data.Now.AddDays(-1),LyDo="Lý do cũ",RowVersion=f.RV};f.Data.Seed(measure);}
            var ex=await Capture(()=>n==335?f.Management().GoBienPhapAsync(f.Room,"Đã xử lý",f.V):f.Management().KhoaTaiKhoanAsync(f.U,n==337?" ":"Vi phạm đã xác minh","STAMP"));
            Note("status",Status(ex));Note("message",ex?.Message);Note("state",user.TrangThai);Note("sessionRevoked",session.ThuHoiLuc);Note("measure",measure);Note("added",f.Data.Added);Note("audit",f.Audit);Note("saves",f.Data.Saves);Note("commits",f.Data.Commits);Note("notices",f.Sent.Select(x=>new{x.User,x.Type,x.Target,x.Text}).ToArray());
            if(n==334){Check(ex==null&&user.TrangThai=="BiKhoa"&&session.ThuHoiLuc!=null&&f.Data.Commits==1,"Khóa phải đổi trạng thái/thu hồi phiên và commit.");Check(f.Data.Added.OfType<BienPhapQuanTri>().Any(x=>x.LyDo=="Vi phạm đã xác minh")&&f.Audit.Contains("KhoaTaiKhoan"),"Thiếu lý do/nhật ký.");Check(f.Sent.Any(x=>x.User==f.U),"Không thông báo trực tiếp cho người dùng bị khóa (fixture không có lớp/yêu cầu đang chờ).");}
            if(n==335)Check(ex==null&&user.TrangThai=="HoatDong"&&measure?.GoLuc!=null&&session.ThuHoiLuc!=null&&f.Audit.Contains("GoBienPhap")&&f.Sent.Any(x=>x.User==f.U),"Gỡ khóa phải ghi nhật ký/thông báo, giữ phiên đã thu hồi.");
            if(n==336)Check(ex==null&&f.Data.Added.Count==0&&f.Data.Saves==0&&user.ConcurrencyStamp=="STAMP","Tài khoản đã BiKhoa phải no-op; hiện thêm biện pháp và Save lại.");
            if(n==337)Check(ex is LoiNghiepVu&&user.TrangThai=="HoatDong"&&f.Data.Saves==0&&f.Data.Added.Count==0,"Lý do trống phải lỗi và không đổi tài khoản.");return;
        }
        if(n is >=338 and <=340)
        {
            Note("source","Areas/NhuCau/Services/HoSoGiaSuService.cs");var h=f.SeedProfile(n==340?"CanChinhSua":"ChoDuyet");f.Me=new(){Id=n==340?f.G:f.A,LoaiNguoi=n==340?"GiaSu":"QuanTri",HoTen="Actor"};
            var ex=await Capture(async()=>{if(n==340){await f.Profile().CapNhatAsync(ProfileInput(f));await f.Profile().DoiTrangThaiAsync("GuiDuyet",f.V);}else await f.Profile().DuyetAsync(h.Id,f.V,n==338,n==339?"Bổ sung bằng cấp":null);});
            Note("status",Status(ex));Note("message",ex?.Message);Note("profile",h);Note("added",f.Data.Added);Note("notices",f.Sent.Select(x=>new{x.User,x.Type,x.Target,x.Text}).ToArray());Note("saves",f.Data.Saves);Note("commits",f.Data.Commits);
            Check(ex==null&&h.TrangThaiDuyet==(n==338?"DaDuyet":n==339?"CanChinhSua":"ChoDuyet")&&!h.DangHienThi,"Sai trạng thái hồ sơ hoặc đã tự công khai.");
            if(n!=340){Check(f.Data.Added.OfType<NhatKyQuanTri>().Any()&&f.Sent.Any(x=>x.User==f.G&&x.Target==h.Id),"Duyệt cần nhật ký và kết quả đúng gia sư.");if(n==339)Check(h.LyDoTuChoi=="Bổ sung bằng cấp","Mất nội dung cần bổ sung.");}
            else Note("workflow","CapNhatAsync lưu BanNhap; bước gửi lại thật là DoiTrangThaiAsync(GuiDuyet), không kết luận chỉ lưu là đã gửi.");return;
        }
        if(n is 341 or 342)
        {
            Note("source","Areas/QuanTri/Services/QuanTriService.CauHinh.cs");Note("mode","DesignContract");var props=typeof(PhanQuyenQuanTriViewModel).GetProperties().Select(x=>x.Name).ToArray();Note("inputProperties",props);
            Note("explanation","PhanQuyenAsync nhận Quyen quản trị (DuyetHoSo, XuLyBaoCao…), không nhận vai trò tài khoản. Không truyền tên vai trò vào Quyen rồi coi bị từ chối là kiểm đúng QT-12.");
            Check(props.Contains("VaiTro")||props.Contains("VaiTros"),"Thiếu hợp đồng đổi vai trò ở phương thức đích; chưa thể truyền một/hai vai trò để kiểm QT-12.");return;
        }
        if(n is 343 or 344)
        {
            Note("source","Areas/QuanTri/Services/QuanTriService.CauHinh.cs");if(n==343){f.Data.Seed(new NhatKyQuanTri{Id=f.K,NguoiThucHienId=f.A,HanhDong="Khoa",ThoiDiem=f.Data.Now},new NhatKyQuanTri{Id=f.M,NguoiThucHienId=f.U,HanhDong="Khoa",ThoiDiem=f.Data.Now},new NhatKyQuanTri{Id=f.Room,NguoiThucHienId=f.A,HanhDong="Khoa",ThoiDiem=f.Data.Now.AddDays(-3)});}
            var result=await f.Management().LayNhatKyAsync(tuNgay:f.Data.Now.AddHours(-1),denNgay:f.Data.Now);Note("items",result.Items);Note("total",result.Total);Note("saves",f.Data.Saves);Note("parameters",typeof(QuanTriService).GetMethod("LayNhatKyAsync")!.GetParameters().Select(x=>x.Name).ToArray());
            Check(f.Data.Saves==0&&f.Data.Added.Count==0&&f.Data.Removed.Count==0,"Không được cập nhật/xóa nhật ký.");
            if(n==343)Check(result.Items.Count==1&&result.Items[0].BanGhi.Id==f.K,"Thiếu lọc theo người thực hiện: không có tham số actor, kết quả có cả A và B khác người cùng thời gian.");else Check(result.Total==0&&result.Items.Count==0,"Nguồn rỗng phải trả danh sách rỗng.");return;
        }
        // Isolated permission service, not an MVC authorization pipeline test.
        var permissions=n switch{345=>QuyenQuanTriService.DanhSach,346=>["DuyetHoSo"],347=>["QuanLyQuyen"],_=>new[]{"XemNhatKy"}};
        Note("source","Areas/QuanTri/Services/QuyenQuanTriService.cs");Note("mapping","Các nhãn policy trong bảng ánh xạ guard CoQuyenAsync/KiemTraAsync của QuyenQuanTriService. Không khẳng định đã chạy ASP.NET authorization pipeline.");
        var cases=new List<object>();foreach(var role in new[]{"HocSinh","PhuHuynh","GiaSu"})foreach(var permission in permissions){f.Me=new(){Id=f.U,LoaiNguoi=role,HoTen="Actor"};var service=new QuyenQuanTriService(f.Db,f.Current.Object,new HttpContextAccessor());var allowed=await service.CoQuyenAsync(permission);var ex=await Capture(()=>service.KiemTraAsync(permission));cases.Add(new{role,permission,allowed,status=Status(ex)});Check(!allowed&&Status(ex)==403,"Vai trò thường không được quyền quản trị.");}
        Note("datasets",cases);Note("saves",f.Data.Saves);Check(f.Data.Saves==0,"Từ chối không được ghi dữ liệu.");
    }
    static HoSoInput ProfileInput(RemainingFixture f)=>new(){HoTen="Gia sư G",TrinhDo="Đại học",ChuyenMon="Toán",KinhNghiem="3 năm",MoTa="Giới thiệu đầy đủ cho hồ sơ",KhuVuc="Hà Nội",DayTrucTuyen=true,RowVersion=f.V,MonDay=[new(){MonHocId=1,PhamViDay="Lớp 10",HocPhiDeXuat=100000,DonViHocPhi="VND/Buoi"}]};
    async Task Account(int n)
    {
        using var f=new RemainingFixture();Note("source","Infrastructure/TaiKhoanService.cs");
        if(n>=354)
        {
            Note("source","Areas/NhuCau/Services/HoSoGiaSuService.cs");var h=f.SeedProfile("BanNhap");f.Me=new(){Id=f.G,LoaiNguoi="GiaSu",HoTen="G"};var input=ProfileInput(f);if(n==355)input.TrinhDo="";
            var ex=await Capture(async()=>{await f.Profile().CapNhatAsync(input);if(n==354)await f.Profile().DoiTrangThaiAsync("GuiDuyet",f.V);});Note("status",Status(ex));Note("field",(ex as LoiNghiepVu)?.Truong);Note("message",ex?.Message);Note("profile",h);Note("saves",f.Data.Saves);Note("notices",f.Sent.Select(x=>new{x.User,x.Type,x.Target,x.Text}).ToArray());
            if(n==354){Check(ex==null&&h.TrangThaiDuyet=="ChoDuyet"&&!h.DangHienThi,"Gửi duyệt phải ChoDuyet chưa công khai.");Check(f.Sent.Any(x=>x.User==f.A),"Gửi duyệt không tạo thông báo admin.");}
            else {Check(ex is LoiNghiepVu&&f.Data.Saves==0&&h.TrangThaiDuyet!="DaDuyet","Thiếu trường phải lỗi trước lưu.");Check((ex as LoiNghiepVu)?.Truong=="TrinhDo","Guard gộp báo thiếu thông tin nhưng không chỉ trường TrinhDo cần bổ sung.");}return;
        }
        var users=new Mock<UserManager<TaiKhoan>>(Mock.Of<IUserStore<TaiKhoan>>(),Microsoft.Extensions.Options.Options.Create(new IdentityOptions()),new PasswordHasher<TaiKhoan>(),Array.Empty<IUserValidator<TaiKhoan>>(),Array.Empty<IPasswordValidator<TaiKhoan>>(),new UpperInvariantLookupNormalizer(),new IdentityErrorDescriber(),new ServiceCollection().BuildServiceProvider(),NullLogger<UserManager<TaiKhoan>>.Instance);
        users.Setup(x=>x.CreateAsync(It.IsAny<TaiKhoan>(),It.IsAny<string>())).ReturnsAsync(n==349?IdentityResult.Failed(new IdentityError{Code="DuplicateEmail",Description="Email đã tồn tại"}):IdentityResult.Success);
        users.Setup(x=>x.AddToRoleAsync(It.IsAny<TaiKhoan>(),It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        var http=new DefaultHttpContext();var accessor=new HttpContextAccessor{HttpContext=http};var audit=new NhatKyAnToanService(accessor);http.RequestServices=new ServiceCollection().AddSingleton(audit).BuildServiceProvider();
        var service=new TaiKhoanService(f.Db,users.Object,null!,null!,new PhienCapNhat(),null!,new ConfigurationBuilder().Build(),f.Files.Object,audit,f.Data);
        if(n<=351)
        {
            var input=new DangKyInput{HoTen="Gia sư mới",Email="u@example.test",VaiTro=n==350?"Admin":"GiaSu",MatKhau="A-valid-password-123",XacNhanMatKhau="A-valid-password-123"};var r=await service.DangKyAsync(input);
            Note("success",r.Succeeded);Note("identityErrors",r.Errors);Note("added",f.Data.Added);Note("saves",f.Data.Saves);Note("commits",f.Data.Commits);Note("identityCalls",users.Invocations.Select(x=>x.Method.Name).ToArray());
            if(n==349){Check(!r.Succeeded&&r.Errors.Any(x=>x.Code=="DuplicateEmail")&&f.Data.Added.Count==0&&f.Data.Saves==0,"Identity trùng email phải lỗi không tạo bản ghi con.");users.Verify(x=>x.AddToRoleAsync(It.IsAny<TaiKhoan>(),It.IsAny<string>()),Times.Never());}
            if(n==350){Check(!r.Succeeded&&f.Data.Added.Count==0,"Vai trò công khai Admin phải từ chối.");users.Verify(x=>x.CreateAsync(It.IsAny<TaiKhoan>(),It.IsAny<string>()),Times.Never());}
            if(n==351){Check(r.Succeeded&&f.Data.Added.OfType<GiaSu>().Count()==1&&f.Data.Commits==1,"GiaSu phải tạo tài khoản/vai trò và dữ liệu tương ứng.");users.Verify(x=>x.AddToRoleAsync(It.IsAny<TaiKhoan>(),"GiaSu"),Times.Once());Check(f.Data.Added.OfType<HoSoGiaSu>().Any(x=>x.TrangThaiDuyet=="ChoDuyet"&&!x.DangHienThi),"Đăng ký chỉ tạo GiaSu, chưa tạo HoSoGiaSu ChoDuyet/ẩn như kỳ vọng.");}return;
        }
        var user=f.Data.Rows<TaiKhoan>().Single(x=>x.Id==f.U);if(n==353)user.TrangThai="BiKhoa";
        users.Setup(x=>x.NormalizeEmail(It.IsAny<string>())).Returns("U@EXAMPLE.TEST");users.Setup(x=>x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);users.Setup(x=>x.CheckPasswordAsync(user,It.IsAny<string>())).ReturnsAsync(n==353);
        var error=await service.DangNhapAsync(new DangNhapInput{Email="u@example.test",MatKhau="Secret-Not-In-Log-987"},http);
        var logs=http.Items.Values.OfType<List<NhatKyAnToan>>().SelectMany(x=>x).ToList();var logJson=JsonSerializer.Serialize(logs);Note("message",error);Note("commands",f.Data.Commands);Note("logs",logs);Note("saves",f.Data.Saves);Note("added",f.Data.Added);
        Check(!string.IsNullOrEmpty(error)&&f.Data.Added.OfType<PhienDangNhap>().Count()==0,"Không được tạo phiên khi xác thực sai/tài khoản khóa.");users.Verify(x=>x.GetRolesAsync(It.IsAny<TaiKhoan>()),Times.Never());
        if(n==352){Check(f.Data.Commands.Count==1&&f.Data.Commands[0].Contains("usp_GhiNhanDangNhapSai"),"Phải yêu cầu ghi lượt sai.");Check(logs.Any(x=>x.KetQua=="ThatBai")&&!logJson.Contains("Secret-Not-In-Log-987"),"Phải ghi log thất bại và không ghi mật khẩu.");}
        Note("scope","Chỉ xác nhận lời gọi ghi lượt sai; không thực thi quy tắc đếm/khóa SQL hoặc tạo cookie.");
    }
    async Task Password(int n)
    {
        var blocklist=new Mock<IPasswordBlocklist>(MockBehavior.Strict);blocklist.Setup(x=>x.Contains(It.IsAny<string>())).Returns(n==361);
        int length=n switch{356=>14,357=>15,358=>64,359=>65,_=>20};var password=n==360?"  Hôm nay trời đẹp  ":new string('x',length);
        var validator=new MatKhauValidator(new ConfigurationBuilder().Build(),blocklist.Object);var result=await validator.ValidateAsync(null!,new TaiKhoan(),password);
        Note("source","Infrastructure/MatKhauValidator.cs");Note("runeCount",password.EnumerateRunes().Count());Note("containsWhitespace",password.Contains(' '));Note("success",result.Succeeded);Note("errors",result.Errors);Note("blocklistCalls",blocklist.Invocations.Count);Note("externalRequests",0);
        Check(result.Succeeded==(n is 357 or 358 or 360),"Sai kết quả kiểm độ dài/khoảng trắng/blocklist.");if(n==361)blocklist.Verify(x=>x.Contains(password),Times.Once());
    }
}
