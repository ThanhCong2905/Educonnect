using System.Collections.Concurrent;
using System.Data.Common;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

internal sealed class It89BindingProbe(It0203Case c):Microsoft.AspNetCore.Mvc.Filters.IActionFilter
{
 public void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context){if(context.ActionArguments.TryGetValue("m",out var arg)&&arg is EduConnect.Mvc.Areas.HocTap.ViewModels.ChamBaiViewModel m)c.Proof["mvcBinding"]=new{culture=System.Globalization.CultureInfo.CurrentCulture.Name,wire=context.HttpContext.Request.Form["Diem"].ToString(),bound=m.Diem,valid=context.ModelState.IsValid};}
 public void OnActionExecuted(Microsoft.AspNetCore.Mvc.Filters.ActionExecutedContext context){}
}
internal sealed class ScanSandbox:IFileContentScanner
{
    public bool Reject,Pause;public int Calls;public TaskCompletionSource Reached=new(TaskCreationOptions.RunContinuationsAsynchronously),Release=new(TaskCreationOptions.RunContinuationsAsynchronously);
    public async Task ScanAsync(byte[] bytes){Interlocked.Increment(ref Calls);if(Pause){Reached.TrySetResult();await Release.Task.WaitAsync(TimeSpan.FromSeconds(12));}if(Reject)throw new LoiNghiepVu("Scanner sandbox từ chối fixture vô hại."){Truong="Files"};}
}
internal sealed class CommitGate:DbTransactionInterceptor
{
    public int Armed;public TaskCompletionSource Reached=new(TaskCreationOptions.RunContinuationsAsynchronously),Release=new(TaskCreationOptions.RunContinuationsAsynchronously);
    public override async ValueTask<InterceptionResult> TransactionCommittingAsync(DbTransaction transaction,TransactionEventData data,InterceptionResult result,CancellationToken token=default){if(Interlocked.Exchange(ref Armed,0)==1){Reached.TrySetResult();await Release.Task.WaitAsync(TimeSpan.FromSeconds(12),token);}return result;}
}
internal sealed class It89Box
{
    public ScanSandbox Scanner=new();public CommitGate Commit=new();public string Root;
    public It89Box(It0203Case c,It04SqlTrace trace)
    {
        var allowed=Environment.GetEnvironmentVariable("IT0809_SANDBOX")??throw new InvalidOperationException("Cần đặt IT0809_SANDBOX trong workspace.");Root=Path.GetFullPath(Path.Combine(allowed,c.Tag));if(!Root.StartsWith(Path.GetFullPath(allowed)+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase))throw new InvalidOperationException("Sai sandbox path");
        c.Factory=new It0203Factory(trace,Commit){ExtraServices=services=>{services.Configure<Microsoft.AspNetCore.Mvc.MvcOptions>(options=>options.Filters.Add(new It89BindingProbe(c)));services.AddScoped<TepService>(sp=>new TepService(sp.GetRequiredService<AppDbContext>(),sp.GetRequiredService<NguoiDungHienTaiService>(),sp.GetRequiredService<QuyenLopHocService>(),new It0203Environment{ContentRootPath=Root},sp.GetRequiredService<ILogger<TepService>>(),new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string?>{{"FileScanning:AllowUnscannedUploads","false"}}).Build(),Scanner));}};
    }
    public object[] Physical()=>Directory.Exists(Path.Combine(Root,"uploads"))?Directory.GetFiles(Path.Combine(Root,"uploads")).Order().Select(p=>(object)new{name=Path.GetFileName(p),length=new FileInfo(p).Length,sha256=Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(p)))}).ToArray():[];
    public void Cleanup()
    {
        Scanner.Release.TrySetResult();Commit.Release.TrySetResult();var dir=Path.Combine(Root,"uploads");if(Directory.Exists(dir)){foreach(var file in Directory.GetFiles(dir)){if(!Path.GetFullPath(file).StartsWith(Root+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase))throw new InvalidOperationException("File outside sandbox");File.Delete(file);}Directory.Delete(dir);}if(Directory.Exists(Root))Directory.Delete(Root);
    }
}
internal sealed record It89Snapshot(It67Snapshot Base,List<KetQuaChamBai> Grades,List<TepDinhKem> Files,List<SuKienChoXuLy> ChatEvents,object[] Physical);
public sealed partial class Integration0203Tests
{
    static EduConnect.Mvc.Areas.HocTap.Services.BaiTapService TaskService(It0203Scope s)=>new(s.Db,s.Current,new QuyenLopHocService(s.Db,s.Current),new ThongBaoService(s.Db,s.Current),new TepService(s.Db,s.Current,new QuyenLopHocService(s.Db,s.Current),new It0203Environment()));
    static async Task<It89Snapshot> Snapshot89(It0203Case c,It89Box box)
    {
        var b=await Snapshot67(c);var ids=b.Submissions.Select(x=>x.Id).ToArray();var rooms=b.Class.Business.Rooms.Select(x=>x.Id.ToString()).ToArray();await using var db=c.Db();var ev=await db.Set<SuKienChoXuLy>().AsNoTracking().Where(x=>x.LoaiSuKien=="TinNhan").OrderBy(x=>x.Id).ToListAsync();
        return new(b,await db.Set<KetQuaChamBai>().AsNoTracking().Where(x=>ids.Contains(x.BaiTapNopId)).OrderBy(x=>x.Id).ToListAsync(),await db.Set<TepDinhKem>().AsNoTracking().Where(x=>c.Users.Contains(x.NguoiTaiLenId)).OrderBy(x=>x.Id).ToListAsync(),ev.Where(x=>rooms.Any(r=>x.DuLieuToiThieu.Contains(r))).ToList(),box.Physical());
    }
    static byte[] PdfBytes(int length=256)
    {
        using var stream=new MemoryStream();void Put(string s){var b=Encoding.ASCII.GetBytes(s);stream.Write(b);}Put("%PDF-1.4\n");var offsets=new List<long>{0};foreach(var obj in new[]{"<< /Type /Catalog /Pages 2 0 R >>","<< /Type /Pages /Kids [3 0 R] /Count 1 >>","<< /Type /Page /Parent 2 0 R /MediaBox [0 0 200 200] /Contents 4 0 R >>","<< /Length 4 >>\nstream\nq\nQ\nendstream"}){offsets.Add(stream.Position);Put($"{offsets.Count-1} 0 obj\n{obj}\nendobj\n");}var xref=stream.Position;Put("xref\n0 5\n0000000000 65535 f \n");foreach(var n in offsets.Skip(1))Put(n.ToString("D10")+" 00000 n \n");Put($"trailer\n<< /Size 5 /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF\n");var raw=stream.ToArray();if(length<=raw.Length)return raw;var result=new byte[length];Array.Fill(result,(byte)' ');raw.CopyTo(result,0);Encoding.ASCII.GetBytes("\n%%EOF\n").CopyTo(result,length-7);return result;
    }
    static async Task<HttpResponseMessage> Multipart(HttpClient client,string path,Dictionary<string,string> fields,params (string Name,byte[] Bytes)[] files)
    {
        fields["__RequestVerificationToken"]=await It0203Case.Token(client,"/HocTap/LopHoc");using var body=new MultipartFormDataContent();foreach(var x in fields)body.Add(new StringContent(x.Value),x.Key);foreach(var file in files){var content=new ByteArrayContent(file.Bytes);content.Headers.ContentType=new MediaTypeHeaderValue("application/pdf");body.Add(content,"Files",file.Name);}using var req=new HttpRequestMessage(HttpMethod.Post,path){Content=body};req.Headers.Add("X-Requested-With","XMLHttpRequest");return await client.SendAsync(req);
    }
    static async Task<BaiTapGiao> ReadAssignment(It0203Case c,Guid id){await using var db=c.Db();return await db.Set<BaiTapGiao>().AsNoTracking().SingleAsync(x=>x.Id==id);}
    static async Task<KetQuaChamBai> ReadGrade(It0203Case c,Guid submission){await using var db=c.Db();return await db.Set<KetQuaChamBai>().AsNoTracking().SingleAsync(x=>x.BaiTapNopId==submission);}
    static async Task<Guid> SeedAssignment(It0203Case c,Guid cl,bool draft=false)
    {
        var first=await FirstLesson(c,cl);await SetLessonTime(c,first.Lich.Id,false);await using var s=c.Scope(c.GS);await CalendarService(s).BatDauBuoiAsync(first.Lich.Id,Convert.ToBase64String(first.Buoi.RowVersion));await TaskService(s).GiaoAsync(new(){LopHocId=cl,BuoiHocId=first.Buoi.Id,TieuDe="Bài tập fixture",NoiDung="Nội dung fixture",HanNopLuc=DateTime.Today.AddDays(10),LuuNhap=draft});return await s.Db.Set<BaiTapGiao>().Where(x=>x.LopHocId==cl).Select(x=>x.Id).SingleAsync();
    }
    static async Task<Guid> SeedSubmission(It0203Case c,Guid task,Guid? operation=null,string text="Nội dung A")
    {var op=operation??Guid.NewGuid();await using var s=c.Scope(c.HS);await TaskService(s).NopAsync(new(){BaiTapGiaoId=task,MaThaoTac=op,NoiDung=text});return await s.Db.Set<BaiTapNop>().Where(x=>x.MaThaoTac==op&&x.NguoiNopId==c.HS).Select(x=>x.Id).SingleAsync();}
    async Task Finish89(It0203Case? c,It89Box? box,Exception? error,It04SqlTrace trace,int n)
    {
        if(box!=null){box.Scanner.Release.TrySetResult();box.Commit.Release.TrySetResult();}if(c==null){failures.Add("Fixture lỗi: "+error);return;}c.Proof["sqlCommands"]=trace.Commands.ToArray();c.Proof["sqlErrorNumbers"]=trace.Errors.ToArray();c.Proof["scannerCalls"]=box?.Scanner.Calls;c.Proof["scannerSandbox"]=true;
        try{await c.DisposeAsync();box?.Cleanup();c.Proof["fileCleanup"]="Đã xóa riêng tệp và thư mục sandbox của ca, không xóa đệ quy.";}catch(Exception ex){c.Proof["cleanupError"]=ex.ToString();failures.Add("Cleanup: "+ex.Message);}c.Proof["assertionDifferences"]=failures;c.Proof["exception"]=error?.ToString();var dir=Environment.GetEnvironmentVariable("IT0809_EVIDENCE");if(dir!=null){Directory.CreateDirectory(dir);await File.WriteAllTextAsync(Path.Combine(dir,$"IT-{n:000}.json"),JsonSerializer.Serialize(c.Proof,new JsonSerializerOptions{WriteIndented=true}));}output.WriteLine("OBSERVATION_JSON:"+JsonSerializer.Serialize(c.Proof));
    }
}
