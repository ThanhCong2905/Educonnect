using System.Data;
using System.Data.Common;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.KetNoi.Services;
using EduConnect.Mvc.Areas.HocTap.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
namespace EduConnect.UnitTests.Current;

// Every connection open is suppressed; every command is intercepted before execution.
// This only tests command construction/materialization and token forwarding, never SQL locks.
internal static class CurrentRelationalBoundary
{
 private static readonly DateTimeOffset T=DateTimeOffset.Parse("2026-11-24T10:00:00Z");
 public static void Run(int n)=>RunAsync(n).GetAwaiter().GetResult();
 private static async Task<object?> Await(object? task){await(Task)task!;return task!.GetType().GetProperty("Result")?.GetValue(task);}
 private static async Task RunAsync(int n){
  var fake=new Boundary();using var db=new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlServer("Server=unit.invalid;Database=Unit;Integrated Security=true;TrustServerCertificate=true").AddInterceptors(fake,new NoConnection()).Options);_ = db.Model;
  using var source=new CancellationTokenSource();var ct=source.Token;var id=Guid.Parse("ABCDEFAB-1234-1234-1234-ABCDEFABCDEF");
  if(n is 45 or 282){fake.Table.Columns.Add("Value",typeof(DateTimeOffset));fake.Table.Rows.Add(T);}else if(n is 283 or 284){
   var type=db.Model.FindEntityType(typeof(LopHoc))!;var sample=new LopHoc{Id=id,TenLop="Lớp fixture"};
   foreach(var p in type.GetProperties()){var clr=Nullable.GetUnderlyingType(p.ClrType)??p.ClrType;fake.Table.Columns.Add(p.GetColumnName(),clr);}
   if(n==284)fake.Table.Rows.Add(type.GetProperties().Select(p=>p.PropertyInfo?.GetValue(sample)??(Nullable.GetUnderlyingType(p.ClrType)!=null||!p.ClrType.IsValueType?DBNull.Value:Activator.CreateInstance(p.ClrType))!).ToArray());
  }
  if(n==5)await Await(CurrentDesignTests.Call(typeof(NhuCauService),"KhoaTaiKhoanAsync",new NhuCauService(db,null!,null!),id,ct));
  else if(n==45)Assert.Equal(T,await Await(CurrentDesignTests.Call(typeof(KetNoiService),"GioMayChuAsync",new KetNoiService(db,null!,null!),ct)));
  else if(n==282)Assert.Equal(T,await GiaoDichNghiepVu.GioMayChuAsync(db,ct));
  else if(n==283){var e=await Assert.ThrowsAsync<LoiNghiepVu>(()=>GiaoDichNghiepVu.KhoaLopAsync(db,id,ct));Assert.Equal(404,e.Status);Assert.Equal("Không tìm thấy lớp học.",e.Message);}
  else if(n==284){var row=await GiaoDichNghiepVu.KhoaLopAsync(db,id,ct);Assert.Equal(id,row.Id);Assert.Equal("Lớp fixture",row.TenLop);}
  else if(n==285)await HoSoLock.KhoaAsync(db,id);
  else if(n==360)await Await(CurrentDesignTests.Call(typeof(LopHocService),"KhoaThanhVienLopAsync",new LopHocService(db,null!,null!,null!,null!),id));
  else if(n==47){var scope=new Scope(db);var worker=new BaoTriWorker(scope,NullLogger<BaoTriWorker>.Instance);await Await(CurrentDesignTests.Call(typeof(BaoTriWorker),"XuLyHetHanYeuCauAsync",worker,T,ct));Assert.True(scope.Disposed);}
  else throw new InvalidOperationException();
  var command=Assert.Single(fake.Commands);Assert.NotNull(command.Text);
  if(n is 5 or 283 or 284 or 360){Assert.Contains(n==5?"dbo.AspNetUsers":"dbo.LopHoc",command.Text);Assert.Contains("UPDLOCK",command.Text);Assert.Contains("HOLDLOCK",command.Text);Assert.Equal(id,Assert.Single(command.Values));}
  if(n==285){Assert.Contains("sys.sp_getapplock",command.Text);Assert.Contains("@LockMode='Exclusive'",command.Text);Assert.Contains("@LockOwner='Transaction'",command.Text);Assert.Contains("@LockTimeout=15000",command.Text);Assert.Equal("EduConnect:profile:abcdefab-1234-1234-1234-abcdefabcdef",Assert.Single(command.Values));}
  if(n==45)Assert.Contains("TODATETIMEOFFSET(SYSUTCDATETIME(), '+00:00') AS Value",command.Text);
  if(n==282)Assert.Contains("SYSDATETIMEOFFSET() AS Value",command.Text);
  if(n==47)Assert.Equal("UPDATE dbo.YeuCauKetNoiLopHoc SET TrangThai='HetHan' WHERE TrangThai IN ('ChoGiaSu','ChoBenHoc') AND HetHanLuc<=SYSDATETIMEOFFSET()",command.Text);
  if(n is not(285 or 360))Assert.Equal(ct,command.Token);
 }
 private sealed class Scope(AppDbContext db):IServiceScopeFactory,IServiceScope,IServiceProvider{
  public bool Disposed;public IServiceProvider ServiceProvider=>this;public IServiceScope CreateScope()=>this;public object? GetService(Type t)=>t==typeof(AppDbContext)?db:null;public void Dispose()=>Disposed=true;
 }
 private sealed class NoConnection:DbConnectionInterceptor{
  public override ValueTask<InterceptionResult> ConnectionOpeningAsync(DbConnection c,ConnectionEventData e,InterceptionResult r,CancellationToken ct=default)=>ValueTask.FromResult(InterceptionResult.Suppress());
  public override InterceptionResult ConnectionOpening(DbConnection c,ConnectionEventData e,InterceptionResult r)=>InterceptionResult.Suppress();
 }
 private sealed class Boundary:DbCommandInterceptor{
  public DataTable Table=new();public List<(string Text,object?[] Values,CancellationToken Token)> Commands=[];
  private void Record(DbCommand c,CancellationToken ct)=>Commands.Add((c.CommandText,c.Parameters.Cast<DbParameter>().Select(p=>p.Value).ToArray(),ct));
  public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand c,CommandEventData e,InterceptionResult<DbDataReader> r,CancellationToken ct=default){Record(c,ct);return ValueTask.FromResult(InterceptionResult<DbDataReader>.SuppressWithResult(Table.CreateDataReader()));}
  public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand c,CommandEventData e,InterceptionResult<int> r,CancellationToken ct=default){Record(c,ct);return ValueTask.FromResult(InterceptionResult<int>.SuppressWithResult(1));}
 }
}
