using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace EduConnect.KetNoiKetThuc.UnitTests;

internal static class ControllerKetNoiKetThucRunner
{
    public static async Task RunAsync(int id,ITestOutputHelper output)
    {
        Assert.InRange(id,106,169);var action=(ConnectAction)((id-106)/8);var phase=(id-106)%8;
        using var f=new ControllerKetNoiKetThucFixture();await f.ArrangeAsync(action,phase);f.VerifyDesignedAction();
        var inputBefore=f.InputSnapshot();IActionResult? result=null;
        var error=await Record.ExceptionAsync(async()=>result=await f.InvokeAsync());
        output.WriteLine("OBSERVATION_JSON:"+JsonSerializer.Serialize(f.Observe(result,error),ControllerKetNoiKetThucFixture.JsonOptions));
        if(phase==0)
        {
            Assert.False(f.ActiveController.TempData.ContainsKey("Success"));
            Assert.Equal(0,f.CoreCalls);f.VerifyCore(Times.Never());
            Assert.Null(error);var bad=Assert.IsType<BadRequestObjectResult>(result);Assert.NotNull(bad.Value);
            Assert.Contains(ControllerKetNoiKetThucFixture.ErrorText,JsonSerializer.Serialize(bad.Value,ControllerKetNoiKetThucFixture.JsonOptions));
            Assert.False(f.ActiveController.ModelState.IsValid);
        }
        else if(phase==1)
        {
            Assert.Null(error);Assert.Equal(1,f.CoreCalls);f.VerifyCore(Times.Once());
            if(action==ConnectAction.ChapNhan)Assert.Equal($"/HocTap/LopHoc/ChiTiet/{f.C1}",Assert.IsType<RedirectResult>(result).Url);
            else
            {
                Assert.Equal("Index",Assert.IsType<RedirectToActionResult>(result).ActionName);
                Assert.Equal("Đã xử lý yêu cầu.",f.ActiveController.TempData["Success"]);
            }
        }
        else if(phase is >=2 and <=6)
        {
            Assert.False(f.ActiveController.TempData.ContainsKey("Success"));Assert.Equal(1,f.CoreCalls);f.VerifyCore(Times.Once());
            Assert.Null(error);
            var code=new[]{403,404,409,400,500}[phase-2];
            switch(code)
            {
                case 403:Assert.IsType<ForbidResult>(result);break;
                case 404:Assert.Same(f.ServiceResult,Assert.IsType<NotFoundObjectResult>(result).Value);break;
                case 409:Assert.Same(f.ServiceResult,Assert.IsType<ConflictObjectResult>(result).Value);break;
                default:Assert.Same(f.ServiceResult,Assert.IsType<BadRequestObjectResult>(result).Value);break;
            }
            Assert.True(f.ActiveController.ModelState.TryGetValue("",out var state),"Thiếu lỗi ModelState chung theo thiết kế.");
            Assert.Contains(state!.Errors,e=>e.ErrorMessage==ControllerKetNoiKetThucFixture.ErrorText);
        }
        else
        {
            Assert.Null(error);Assert.Equal(inputBefore,f.InputSnapshot());f.VerifyArgumentsAndToken();
            if(action==ConnectAction.GuiYeuCau)
            {
                var property=f.Request.GetType().GetProperty("RowVersion");
                Assert.True(property!=null,"UT-113: bảng TC yêu cầu truyền RV1 nhưng GuiYeuCauViewModel hiện không khai báo RowVersion. Các trường hiện có, actor và token đã truyền đúng; chưa đáp ứng đầy đủ yêu cầu RV1 của TC.");
                var value=property.GetValue(f.Request);
                Assert.True(value is byte[] bytes&&bytes.SequenceEqual(f.RV)||value is string text&&text==Convert.ToBase64String(f.RV),"UT-113: RowVersion không giữ đúng RV1 của fixture.");
            }
        }
    }
}
