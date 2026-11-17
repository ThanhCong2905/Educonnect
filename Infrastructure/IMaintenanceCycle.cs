namespace EduConnect.Mvc.Infrastructure;

public interface IMaintenanceCycle
{
    Task ExpireRequestsAsync(DateTimeOffset now,CancellationToken ct);
    Task AutoEndAsync(DateTimeOffset now,CancellationToken ct);
    Task RemindersAsync(CancellationToken ct);
    Task NeedsAsync(CancellationToken ct);
    Task FeedbackAsync(CancellationToken ct);
}
public interface IMaintenanceTicks:IDisposable
{
    ValueTask<bool> WaitAsync(CancellationToken ct);
}
public sealed class PeriodicMaintenanceTicks:IMaintenanceTicks
{
    private readonly PeriodicTimer timer=new(TimeSpan.FromSeconds(30));
    public ValueTask<bool> WaitAsync(CancellationToken ct)=>timer.WaitForNextTickAsync(ct);
    public void Dispose()=>timer.Dispose();
}
