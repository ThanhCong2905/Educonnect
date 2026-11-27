using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using HarmonyLib;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Areas.NhuCau.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;
namespace EduConnect.UnitTests.Current;

// Patches only framework dependencies in this test process. No application method is patched.
// No test copies the selected method's implementation. Dispose always removes the shim.
internal static class CurrentStaticBoundary
{
 private sealed class State{
  public DateTimeOffset Now;public int ClockCalls;public Assembly? Assembly;public bool ResourceMissing;public TrackingStream? Stream;public List<string> ResourceCalls=[];public List<string> Deleted=[];public IOException? DeleteError;
 }
 private static readonly AsyncLocal<State?> Active=new();
 private sealed class Shim:IDisposable{
  private readonly Harmony harmony=new("EduConnect.UnitTest.Framework."+Guid.NewGuid().ToString("N"));
  public Shim(State state,MethodInfo original,string prefix){harmony.Patch(original,prefix:new HarmonyMethod(typeof(CurrentStaticBoundary).GetMethod(prefix,BindingFlags.Static|BindingFlags.NonPublic)!));Active.Value=state;}
  public void Dispose(){Active.Value=null;harmony.UnpatchAll(harmony.Id);}
 }
 private static bool ClockPrefix(ref DateTimeOffset __result){var state=Active.Value;if(state==null)return true;state.ClockCalls++;__result=state.Now;return false;}
 private static bool ResourcePrefix(Assembly __instance,string __0,ref Stream? __result){var state=Active.Value;if(state?.Assembly!=__instance||__0!="EduConnect.Mvc.wwwroot.data.vietnam-wards.json")return true;state.ResourceCalls.Add(__0);__result=state.ResourceMissing?null:state.Stream;return false;}
 private static bool DeletePrefix(string __0){var state=Active.Value;if(state==null)return true;state.Deleted.Add(__0);if(state.DeleteError!=null)throw state.DeleteError;return false;}
 private sealed class TrackingStream(byte[] bytes):MemoryStream(bytes){public bool WasDisposed;protected override void Dispose(bool disposing){WasDisposed=true;base.Dispose(disposing);}}
 public static void Run(int n,string dl)=>RunAsync(n,dl).GetAwaiter().GetResult();
 private static async Task RunAsync(int n,string dl){
  if(n==8||n is >=121 and <=132){
   using var db=new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);_ = db.Model;
   var state=new State{Now=DateTimeOffset.Parse("2026-11-24T10:00:00Z")};var id=Guid.Parse("00000000-0000-0000-0000-000000000101");var owner=Guid.Parse("00000000-0000-0000-0000-000000000001");var operation=Guid.NewGuid();object sut;
   if(n==8){state.Now=DateTimeOffset.Parse("2026-11-24T17:00:00Z").AddTicks(dl=="DL01"?-1:dl=="DL02"?0:1);sut=new NhuCauService(db,null!,null!);}
   else{
    if(n!=126){var row=new TrangThaiThuDangNhap{LoaiKhoa=n is 122 or 125?"DiaChiIp":"TaiKhoan",GiaTriKhoa=n==123?"khac@example.vn":n is 122 or 124?"127.0.0.1":"a@example.vn",LanSaiCuoiLuc=state.Now.AddMinutes(-1),ChoDenLuc=state.Now.AddMinutes(1)};
     if(n is >=127 and <=129)row.LanSaiCuoiLuc=state.Now.AddMinutes(-15).AddTicks(n-128);
     if(n is >=130 and <=132)row.ChoDenLuc=state.Now.AddTicks(n-131);
     db.Add(row);db.SaveChanges();db.ChangeTracker.Clear();}
    sut=new TaiKhoanService(db,null!,null!,null!,null!,null!,null!,null!,null!);RuntimeHelpers.RunClassConstructor(typeof(TaiKhoanService).TypeHandle);
   }
   using(var shim=new Shim(state,typeof(DateTimeOffset).GetProperty(nameof(DateTimeOffset.UtcNow))!.GetMethod!,nameof(ClockPrefix))){
    Assert.Equal(state.Now,DateTimeOffset.UtcNow);state.ClockCalls=0;
    if(n==8){CurrentDesignTests.Call(typeof(NhuCauService),"GhiLuotDang",sut,id,owner,"Dang",operation);var entry=Assert.Single(db.ChangeTracker.Entries<LuotDangNhuCau>());var e=entry.Entity;Assert.Equal(EntityState.Added,entry.State);Assert.Equal(id,e.NhuCauId);Assert.Equal(owner,e.TaiKhoanId);Assert.Equal(operation,e.MaThaoTac);Assert.Equal("Dang",e.HanhDong);Assert.NotEqual(Guid.Empty,e.Id);Assert.Equal(state.Now,e.DangLuc);Assert.Equal(new DateOnly(2026,11,dl=="DL01"?24:25),e.NgayDang);}
    else{var task=(Task<bool>)CurrentDesignTests.Call(typeof(TaiKhoanService),"DangChoAsync",sut,"a@example.vn","127.0.0.1")!;Assert.Equal(n is 121 or 122 or 129 or 132,await task);Assert.Empty(db.ChangeTracker.Entries());}
    Assert.True(state.ClockCalls>0,"SUT must call the shimmed framework clock; do not silently use the real clock.");
   }return;
  }
  if(n is 280 or 281){
   // Warm only the application's static data during Arrange, before the resource shim is active.
   RuntimeHelpers.RunClassConstructor(typeof(DiaGioiHanhChinh).TypeHandle);var assembly=typeof(DiaGioiHanhChinh).Assembly;var stream=new TrackingStream(Encoding.UTF8.GetBytes("{\"provinces\":{\"Tỉnh A\":[\"Xã A1\",\"Xã A2\"],\"Tỉnh B\":[\"Xã B1\"]}}"));
   var state=new State{Assembly=assembly,ResourceMissing=n==280,Stream=stream};var original=assembly.GetType().GetMethod(nameof(Assembly.GetManifestResourceStream),[typeof(string)])!;
   using(var shim=new Shim(state,original,nameof(ResourcePrefix))){
    if(n==280){var e=Assert.Throws<InvalidOperationException>(()=>CurrentDesignTests.Call(typeof(DiaGioiHanhChinh),"DocDanhMuc",null));Assert.Equal("Thiếu danh mục địa giới hành chính.",e.Message);Assert.False(stream.WasDisposed);}
    else{var result=(Dictionary<string,string[]>)CurrentDesignTests.Call(typeof(DiaGioiHanhChinh),"DocDanhMuc",null)!;Assert.Equal(2,result.Count);Assert.Equal(new[]{"Xã A1","Xã A2"},result["Tỉnh A"]);Assert.Equal(new[]{"Xã B1"},result["Tỉnh B"]);Assert.False(result.ContainsKey("provinces"));Assert.True(stream.WasDisposed);}
    Assert.Equal("EduConnect.Mvc.wwwroot.data.vietnam-wards.json",Assert.Single(state.ResourceCalls));
   }stream.Dispose();return;
  }
  if(n is 330 or 331){
   var state=new State{DeleteError=n==331?new IOException("fixture-delete"):null};var paths=n==330?new[]{@"C:\UnitTest\root\a",@"C:\UnitTest\root\b"}:new[]{"a","b"};
   using(var shim=new Shim(state,typeof(File).GetMethod(nameof(File.Delete),[typeof(string)])!,nameof(DeletePrefix))){
    if(n==331){var e=Assert.Throws<IOException>(()=>TepService.DonDep(paths));Assert.Same(state.DeleteError,e);Assert.Equal(new[]{"a"},state.Deleted);}
    else{TepService.DonDep(paths);Assert.Equal(paths,state.Deleted);}
   }return;
  }
  throw new ArgumentOutOfRangeException(nameof(n));
 }
}
