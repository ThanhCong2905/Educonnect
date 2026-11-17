using EduConnect.Mvc.Models;
namespace EduConnect.Mvc.Infrastructure;

public static class OutboxDelivery
{
    public static async Task RunAsync(SuKienChoXuLy ev,CancellationToken token,
        Func<Task<bool>> renew,Func<Task> publish,Func<Task> complete,
        Func<int,Task> retry,Action<Exception> failed)
    {
        try
        {
            if(!await renew())return;
            await publish();
            await complete();
        }
        catch(OperationCanceledException)when(token.IsCancellationRequested){throw;}
        catch(Exception ex)
        {
            var seconds=Math.Min(60,1<<Math.Min(ev.SoLanThu,5));
            await retry(seconds);
            failed(ex);
        }
    }
}
