using System.Net;
using System.Security.Claims;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
namespace EduConnect.UnitTests.Current;
internal static class CurrentAuditBoundary
{
 public static void Run(int n)=>RunAsync(n).GetAwaiter().GetResult();
 private static async Task RunAsync(int n){
  var spy=new SaveSpy(n==298);using var db=new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).AddInterceptors(spy).Options);_=db.Model;
  var scope=new Scope(db);var logger=new Logger();var sut=new NhatKyAnToanHubFilter(scope,logger);var user=Guid.NewGuid();var target=Guid.NewGuid();var h=new DefaultHttpContext();h.Connection.RemoteIpAddress=IPAddress.Loopback;
  var caller=new Caller{UserValue=new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier,n==298?user.ToString():"bad-guid")],"Test"))};if(n==298)caller.Features.Set<IHttpContextFeature>(new HttpFeature(h));
  var task=(Task)CurrentDesignTests.Call(typeof(NhatKyAnToanHubFilter),"GhiAsync",sut,caller,"Gui",n==298?"TuChoi":"ThanhCong",n==298?target:null,n==298?"X":"Authorization")!;await task;
  var record=Assert.IsType<NhatKyAnToan>(spy.Record);Assert.Equal(n==298?(Guid?)user:null,record.TaiKhoanId);Assert.Equal(n==298?"X":"Authorization",record.Nhom);Assert.Equal("SignalR.Gui",record.SuKien);Assert.Equal(n==298?"TuChoi":"ThanhCong",record.KetQua);Assert.Equal(n==298?"PhienHoacQuyenHoiThoaiKhongHopLe":"",record.LyDo);Assert.Equal(n==298?target.ToString():null,record.DoiTuongId);Assert.Equal(n==298?"127.0.0.1":"",record.DiaChiIp);Assert.Equal("/cap-nhat",record.DuongDan);Assert.Equal("SignalR",record.PhuongThuc);Assert.True(Guid.TryParseExact(record.MaYeuCau,"D",out _));Assert.Equal(1,spy.Count);Assert.True(spy.Token.CanBeCanceled);Assert.True(scope.Disposed);
  if(n==298){var entry=Assert.Single(logger.Errors);Assert.Same(spy.Failure,entry);}else Assert.Empty(logger.Errors);
 }
 private sealed class SaveSpy(bool fail):SaveChangesInterceptor{
  public readonly Exception Failure=new InvalidOperationException("E");public NhatKyAnToan? Record;public int Count;public CancellationToken Token;
  public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData e,InterceptionResult<int> r,CancellationToken ct=default){Count++;Token=ct;Record=Assert.Single(e.Context!.ChangeTracker.Entries<NhatKyAnToan>()).Entity;if(fail)throw Failure;return ValueTask.FromResult(InterceptionResult<int>.SuppressWithResult(1));}
 }
 private sealed class Scope(AppDbContext db):IServiceScopeFactory,IServiceScope,IAsyncDisposable,IServiceProvider{
  public bool Disposed;public IServiceProvider ServiceProvider=>this;public IServiceScope CreateScope()=>this;public object? GetService(Type t)=>t==typeof(AppDbContext)?db:null;public void Dispose()=>Disposed=true;public ValueTask DisposeAsync(){Disposed=true;return ValueTask.CompletedTask;}
 }
 private sealed class Logger:ILogger<NhatKyAnToanHubFilter>{public List<Exception?> Errors=[];public IDisposable? BeginScope<T>(T state)where T:notnull=>null;public bool IsEnabled(LogLevel l)=>true;public void Log<T>(LogLevel l,EventId id,T state,Exception? e,Func<T,Exception?,string> formatter){if(l==LogLevel.Error)Errors.Add(e);}}
 private sealed class HttpFeature(HttpContext h):IHttpContextFeature{public HttpContext HttpContext{get;set;}=h;}
 private sealed class Caller:HubCallerContext{
  public ClaimsPrincipal? UserValue;public override string ConnectionId=>"C1";public override string? UserIdentifier=>null;public override ClaimsPrincipal? User=>UserValue;public override IDictionary<object,object?> Items{get;}=new Dictionary<object,object?>();public override IFeatureCollection Features{get;}=new FeatureCollection();public override CancellationToken ConnectionAborted=>CancellationToken.None;public override void Abort(){}
 }
}
