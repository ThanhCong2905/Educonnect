using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Xunit;

namespace EduConnect.HocTap.UnitTests;

[Trait("Suite","BaiTap")]
public sealed class OutboxDeliveryTest
{
    [Fact,Trait("TestCaseId","UT-101")]
    public async Task UT_101_PhatThongBaoLoi_DanhDauThuLai_KhongLapSuKien()
    {
        var ev=new SuKienChoXuLy{Id=Guid.NewGuid(),TrangThai="DangXuLy",SoLanThu=1};
        var publishError=new IOException("Mat ket noi realtime.");int completed=0,retries=0;Exception? logged=null;
        await OutboxDelivery.RunAsync(ev,default,()=>Task.FromResult(true),()=>Task.FromException(publishError),()=>{completed++;return Task.CompletedTask;},seconds=>{Assert.Equal(2,seconds);retries++;ev.TrangThai="ChoThuLai";return Task.CompletedTask;},ex=>logged=ex);
        Assert.Equal(0,completed);Assert.Equal(1,retries);Assert.Same(publishError,logged);Assert.Equal("ChoThuLai",ev.TrangThai);
        // A retry publishes the same event; it does not repeat assignment creation.
        var originalEvent=ev.Id;int published=0;
        await OutboxDelivery.RunAsync(ev,default,()=>Task.FromResult(true),()=>{published++;return Task.CompletedTask;},()=>{completed++;ev.TrangThai="HoanTat";return Task.CompletedTask;},_=>throw new InvalidOperationException("Unexpected retry"),_=>throw new InvalidOperationException("Unexpected failure"));
        Assert.Equal(originalEvent,ev.Id);Assert.Equal(1,published);Assert.Equal(1,completed);Assert.Equal("HoanTat",ev.TrangThai);
    }
}
