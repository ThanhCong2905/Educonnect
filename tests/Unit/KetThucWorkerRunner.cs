using System.Text.Json;
using EduConnect.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Xunit.Abstractions;
namespace EduConnect.KetThucWorker.UnitTests;

public sealed partial class KetThucWorkerTest(ITestOutputHelper output)
{
    private async Task Run(int n)
    {
        using var f=new KetThucWorkerFixture();Exception? exception=null;object? raw=null;bool? success=null;string? message=null;int status=0;Guid? id=null;int? expected=null;
        var rv=f.RV.ToArray();var reason=" Xin kết thúc ";bool agree=n!=244;
        if(n<=223)
        {
            var result=await KetQua.ThucHienAsync(()=>n==222?Task.FromException(new LoiNghiepVu("Xung đột mẫu",409)):Task.CompletedTask);raw=result;
            f.Connections.Setup(x=>x.PhanHoiHuyAsync(f.R,true,It.IsAny<byte[]>(),f.H,It.IsAny<CancellationToken>())).ReturnsAsync(result);
            if(n==223)f.Stop.Cancel();
        }
        switch(n)
        {
            case 224:reason=" ";expected=400;break;
            case 225:reason=new string('a',2001);expected=400;break;
            case 226:reason=new string('a',2000);break;
            case 227:f.Class=null;expected=404;break;
            case 228:f.User.Id=f.X;expected=403;break;
            case 229:f.Class!.TrangThai="DaKetThuc";expected=403;break;
            case 230:f.Pending=true;expected=409;break;
            case 231:rv=f.Old;expected=409;break;
            case 233 or 245:f.SaveError=new DbUpdateConcurrencyException("Concurrency giả lập tại Save");expected=409;break;
            case 234 or 246:f.SaveError=new DbUpdateException("Lỗi ghi giả lập tại Save");expected=500;break;
            case 235:f.MissingRequest=true;expected=404;break;
            case 236:f.User.Id=f.G;expected=403;break;
            case 237:f.User.Id=f.X;expected=403;break;
            case 238:f.Request.TrangThai="TuChoi";expected=409;break;
            case 239:rv=f.Old;expected=409;break;
            case 240:f.Class!.RowVersion=f.Old;rv=f.Class.RowVersion;expected=409;break;
            case 241:f.Now=f.Request.HetHanLuc;expected=409;break;
            case 242:f.Now=f.Request.HetHanLuc.AddSeconds(1);expected=409;break;
            case 247:f.Stop.Cancel();break;
            case 249:f.Cycle.Setup(x=>x.ExpireRequestsAsync(It.IsAny<DateTimeOffset>(),It.IsAny<CancellationToken>())).Returns((DateTimeOffset t,CancellationToken ct)=>{f.Order.Add("Expire");f.Stop.Cancel();return Task.FromException(new OperationCanceledException(ct));});break;
            case 250:
                int calls=0;f.Cycle.Setup(x=>x.ExpireRequestsAsync(It.IsAny<DateTimeOffset>(),It.IsAny<CancellationToken>())).Returns((DateTimeOffset t,CancellationToken ct)=>{f.Order.Add("Expire");return ++calls==1?Task.FromException(new InvalidOperationException("Vòng đầu lỗi mẫu")):Task.CompletedTask;});
                f.Ticks.Setup(x=>x.WaitAsync(It.IsAny<CancellationToken>())).Returns((CancellationToken ct)=>{f.Waits++;f.Order.Add("Wait");return ValueTask.FromResult(f.Waits==1);});break;
        }
        if(n>=252)
        {
            f.Candidates=[(f.R,f.C)];f.Request.HetHanLuc=f.Now;
            if(n==252)f.Request.HetHanLuc=f.Now.AddSeconds(1);
            if(n==253)f.Request.TrangThai="TuChoi";
            if(n==254){f.Class!.TrangThai="DaKetThuc";f.Class.CachKetThuc="DongY";f.Class.KetThucLuc=f.Now.AddDays(-1);}
            if(n==256)f.Request.HetHanLuc=f.Now.AddSeconds(-1);
        }
        var classBefore=JsonSerializer.Serialize(f.Class);var requestBefore=JsonSerializer.Serialize(f.Request);
        try
        {
            if(n<=223){var r=await f.Classes.XacNhanKetThucAsync(f.R,rv,f.H,f.Token);success=r.ThanhCong;message=r.ThongDiep;status=r.ThanhCong?200:int.Parse(r.MaLoi!);if(n!=223)Assert.Same(raw,r);}
            else if(n<=234){var r=await f.Service.TaoYeuCauHuyAsync(f.C,reason,rv,f.User.Id,f.Token);success=r.ThanhCong;message=r.ThongDiep;status=r.MaTrangThai;if(r.ThanhCong)id=r.DuLieu;}
            else if(n<=246){var r=await f.Service.PhanHoiHuyAsync(f.R,agree,rv,f.User.Id,f.Token);success=r.ThanhCong;message=r.ThongDiep;status=r.ThanhCong?200:int.Parse(r.MaLoi!);}
            else if(n<=250){using var worker=f.Worker(true);await f.Execute(worker);success=true;}
            else{using var worker=f.Worker();await f.Auto(worker);if(n==257){var before=new{f.Saves,f.Commits,notices=f.Notices.Count,events=f.Events.Count};await f.Auto(worker);Assert.Equal(before.Saves,f.Saves);Assert.Equal(before.Commits,f.Commits);Assert.Equal(before.notices,f.Notices.Count);Assert.Equal(before.events,f.Events.Count);}success=true;}
        }
        catch(Exception ex){exception=ex;}
        output.WriteLine("OBSERVATION_JSON:"+JsonSerializer.Serialize(new{id=$"UT-{n}",expectedStatus=expected,actualStatus=status,success,message,data=id,exception=exception?.GetType().FullName,exceptionMessage=exception?.Message,classBefore,requestBefore,actual=f.Snapshot()}));
        if(n==223){Assert.IsAssignableFrom<OperationCanceledException>(exception);f.Connections.Verify(x=>x.PhanHoiHuyAsync(It.IsAny<Guid>(),It.IsAny<bool>(),It.IsAny<byte[]>(),It.IsAny<Guid>(),It.IsAny<CancellationToken>()),Times.Never());return;}
        Assert.Null(exception);
        if(n<=222)
        {
            Assert.Equal(n==221,success);Assert.Equal(n==221?200:409,status);if(n==222)Assert.Equal("Xung đột mẫu",message);
            f.Connections.Verify(x=>x.PhanHoiHuyAsync(f.R,true,It.Is<byte[]>(v=>v.SequenceEqual(rv)),f.H,f.Token),Times.Once());f.NoSave();return;
        }
        if(expected!=null)
        {
            Assert.False(success);Assert.Equal(expected,status);Assert.Equal(0,f.Commits);Assert.Equal(f.Begins,f.Disposals);
            if(n is 233 or 245){Assert.Equal(1,f.Saves);}
            else{f.NoSave();Assert.Equal(classBefore,JsonSerializer.Serialize(f.Class));Assert.Equal(requestBefore,JsonSerializer.Serialize(f.Request));}
            if(n is 224 or 225)Assert.Equal(0,f.Begins);return;
        }
        Assert.True(success);
        if(n<=234)
        {
            f.Saved();var r=Assert.Single(f.Added);Assert.Equal(r.Id,id);Assert.Equal(reason.Trim(),r.LyDo);Assert.Equal(f.C,r.LopHocId);Assert.Equal(f.User.Id,r.NguoiGuiId);Assert.Equal("ChoPhanHoi",r.TrangThai);Assert.Equal(f.Now,r.GuiLuc);Assert.Equal(f.Now.AddHours(72),r.HetHanLuc);Assert.Equal(classBefore,JsonSerializer.Serialize(f.Class));Assert.Equal(f.G,Assert.Single(f.Notices).NguoiNhanId);
            if(n==232){var e=Assert.Single(f.Events);Assert.Equal("YeuCauKetThucLop",e.LoaiSuKien);Assert.Contains(r.Id.ToString(),e.DuLieuToiThieu);}return;
        }
        if(n<=246)
        {
            f.Saved();Assert.Equal(f.RV,f.Version);Assert.Equal(agree?"ChapNhan":"TuChoi",f.Request.TrangThai);Assert.Equal(f.Now,f.Request.PhanHoiLuc);Assert.Equal(f.User.Id,f.Request.NguoiPhanHoiId);
            if(agree){Assert.Equal("DaKetThuc",f.Class!.TrangThai);Assert.Equal(f.Now,f.Class.KetThucLuc);Assert.Equal("DongY",f.Class.CachKetThuc);}else Assert.Equal(classBefore,JsonSerializer.Serialize(f.Class));
            Assert.Equal(f.G,Assert.Single(f.Notices).NguoiNhanId);Assert.Equal("PhanHoiKetThucLop",Assert.Single(f.Events).LoaiSuKien);return;
        }
        if(n<=250)
        {
            f.NoSave();if(n==247){Assert.Empty(f.Cycle.Invocations);Assert.Equal(0,f.Waits);}
            if(n==248){Assert.Equal(new[]{"Expire","AutoEnd","Reminders","Needs","Feedback","Wait","DisposeTimer"},f.Order);}
            if(n==249){Assert.Equal(new[]{"Expire","DisposeTimer"},f.Order);Assert.Equal(0,f.Warnings);}
            if(n==250){Assert.Equal(1,f.Warnings);Assert.Equal(2,f.Order.Count(x=>x=="Expire"));Assert.Equal(1,f.Order.Count(x=>x=="AutoEnd"));Assert.Equal(2,f.Waits);}return;
        }
        if(n<=254){f.NoSave();Assert.Equal(classBefore,JsonSerializer.Serialize(f.Class));Assert.Equal(requestBefore,JsonSerializer.Serialize(f.Request));Assert.Equal(n==251?0:1,f.Begins);Assert.Equal(f.Begins,f.Disposals);return;}
        Assert.Equal("TuDongChapNhan",f.Request.TrangThai);Assert.Equal(f.Now,f.Request.PhanHoiLuc);Assert.Null(f.Request.NguoiPhanHoiId);Assert.Equal("DaKetThuc",f.Class!.TrangThai);Assert.Equal(f.Now,f.Class.KetThucLuc);Assert.Equal("HetHan",f.Class.CachKetThuc);Assert.Equal(new[]{f.H,f.G}.Order(),f.Notices.Select(x=>x.NguoiNhanId).Order());Assert.All(f.Notices,x=>Assert.Equal(f.C,x.DoiTuongId));
        Assert.Equal(1,f.Saves);Assert.Equal(1,f.Commits);Assert.True(f.Order.IndexOf("Save")<f.Order.IndexOf("Commit"));Assert.Equal(f.Begins,f.Disposals);Assert.Equal(2,f.Events.Count);
    }
}
