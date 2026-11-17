using System.Reflection;
using System.Text.Json;
using EduConnect.Mvc.Areas.KetNoi.Services;
using EduConnect.Mvc.Areas.HocTap.Services;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EduConnect.KetThucWorker.UnitTests;
internal sealed class EndingClock:TimeProvider
{
    public static DateTimeOffset Now=DateTimeOffset.Parse("2026-11-14T10:00:00+07:00");
    public override DateTimeOffset GetUtcNow()=>Now;
}
internal sealed class KetThucWorkerFixture:IDisposable
{
    public Guid H=Guid.Parse("10000000-0000-0000-0000-000000000001"),G=Guid.Parse("20000000-0000-0000-0000-000000000002"),X=Guid.Parse("30000000-0000-0000-0000-000000000003"),C=Guid.Parse("40000000-0000-0000-0000-000000000004"),R=Guid.Parse("50000000-0000-0000-0000-000000000005");
    public byte[] RV=[1,2,3,4,5,6,7,8],Old=[0,0,0,0,0,0,0,1];
    public LopHoc? Class;public YeuCauHuyKetNoi Request;public NguoiThamGia User;
    public DateTimeOffset Now=EndingClock.Now;
    public bool Pending,MissingRequest;public List<(Guid Id,Guid Lop)> Candidates=[];
    public Exception? SaveError;
    public readonly Mock<IClassEndingStore> Store=new(MockBehavior.Strict);
    public readonly Mock<IDbContextTransaction> Tx=new(MockBehavior.Strict);
    public readonly Mock<INguoiDungHienTaiService> Current=new(MockBehavior.Strict);
    public readonly Mock<IThongBaoData> NoticeData=new(MockBehavior.Strict);
    public readonly Mock<IKetNoiService> Connections=new(MockBehavior.Strict);
    public readonly Mock<IMaintenanceCycle> Cycle=new(MockBehavior.Strict);
    public readonly Mock<IMaintenanceTicks> Ticks=new(MockBehavior.Strict);
    public readonly Mock<ILogger<BaoTriWorker>> Log=new();
    public readonly List<string> Order=[];
    public readonly List<ThongBao> Notices=[];public readonly List<SuKienChoXuLy> Events=[];public readonly List<YeuCauHuyKetNoi> Added=[];
    public int Begins,Saves,Commits,Disposals,Waits,VersionSets;public byte[]? Version;
    public CancellationTokenSource Stop=new();public CancellationToken Token=>Stop.Token;
    public readonly AppDbContext Db=new(new DbContextOptionsBuilder<AppDbContext>().Options);
    public KetNoiService Service;public LopHocService Classes;public ThongBaoService NoticeService;
    private readonly ServiceProvider provider;
    public KetThucWorkerFixture()
    {
        User=new(){Id=H,LoaiNguoi="HocSinh",HoTen="H"};Class=new(){Id=C,GiaSuId=G,BenHocId=H,TrangThai="DangHoatDong",RowVersion=RV.ToArray()};
        Request=new(){Id=R,LopHocId=C,NguoiGuiId=G,LyDo="Xin kết thúc",TrangThai="ChoPhanHoi",GuiLuc=Now.AddHours(-71),HetHanLuc=Now.AddSeconds(1),RowVersion=RV.ToArray()};
        Current.Setup(x=>x.HienTaiAsync(It.IsAny<CancellationToken>())).Returns((CancellationToken ct)=>Task.FromResult(User));
        Store.Setup(x=>x.BeginAsync(It.IsAny<CancellationToken>())).Returns((CancellationToken ct)=>{Begins++;Order.Add("Begin");return Task.FromResult(Tx.Object);});
        Store.Setup(x=>x.LockClassAsync(It.IsAny<Guid>(),It.IsAny<CancellationToken>())).Returns((Guid id,CancellationToken ct)=>{Assert.Equal(C,id);Order.Add("LockClass");return Task.FromResult<LopHoc?>(Class);});
        Store.Setup(x=>x.LockRequestAsync(It.IsAny<Guid>(),It.IsAny<CancellationToken>())).Returns((Guid id,CancellationToken ct)=>{Assert.Equal(R,id);Order.Add("LockRequest");return Task.FromResult(Request);});
        Store.Setup(x=>x.RequestClassIdAsync(It.IsAny<Guid>(),It.IsAny<CancellationToken>())).Returns((Guid id,CancellationToken ct)=>{Assert.Equal(R,id);Order.Add("FindRequest");return Task.FromResult(MissingRequest?(Guid?)null:C);});
        Store.Setup(x=>x.HasPendingAsync(It.IsAny<Guid>(),It.IsAny<CancellationToken>())).Returns((Guid id,CancellationToken ct)=>{Assert.Equal(C,id);return Task.FromResult(Pending);});
        Store.Setup(x=>x.NowAsync(It.IsAny<CancellationToken>())).Returns((CancellationToken ct)=>{Order.Add("SqlClock");return Task.FromResult(Now);});
        Store.Setup(x=>x.ExpiredAsync(It.IsAny<DateTimeOffset>(),It.IsAny<CancellationToken>())).Returns((DateTimeOffset now,CancellationToken ct)=>{Order.Add("Candidates");return Task.FromResult(Candidates.ToList());});
        Store.Setup(x=>x.Add(It.IsAny<YeuCauHuyKetNoi>())).Callback((YeuCauHuyKetNoi r)=>{Added.Add(r);Order.Add("AddRequest");});
        Store.Setup(x=>x.SetVersion(It.IsAny<YeuCauHuyKetNoi>(),It.IsAny<byte[]>())).Callback((YeuCauHuyKetNoi r,byte[] v)=>{VersionSets++;Version=v.ToArray();Order.Add("SetVersion");});
        Store.Setup(x=>x.SaveAsync(It.IsAny<CancellationToken>())).Returns((CancellationToken ct)=>{Saves++;Order.Add("Save");return SaveError==null?Task.CompletedTask:Task.FromException(SaveError);});
        Tx.Setup(x=>x.CommitAsync(It.IsAny<CancellationToken>())).Returns((CancellationToken ct)=>{Commits++;Order.Add("Commit");return Task.CompletedTask;});
        Tx.Setup(x=>x.DisposeAsync()).Callback(()=>{Disposals++;Order.Add("DisposeTx");}).Returns(ValueTask.CompletedTask);
        NoticeData.Setup(x=>x.AddEvent(It.IsAny<SuKienChoXuLy>())).Callback((SuKienChoXuLy e)=>{Events.Add(e);Order.Add("Event");});
        NoticeData.Setup(x=>x.AddNotice(It.IsAny<ThongBao>())).Callback((ThongBao n)=>{Notices.Add(n);Order.Add("Notice");});
        var concrete=new NguoiDungHienTaiService(Db,new HttpContextAccessor());NoticeService=new(Db,concrete,NoticeData.Object);
        Service=new(Db,Current.Object,NoticeService,endingStore:Store.Object);
        Classes=new(Db,concrete,new QuyenLopHocService(Db,concrete),NoticeService,Connections.Object);
        provider=new ServiceCollection().AddSingleton(Db).AddSingleton(Store.Object).AddSingleton(NoticeService).BuildServiceProvider();
        Cycle.Setup(x=>x.ExpireRequestsAsync(It.IsAny<DateTimeOffset>(),It.IsAny<CancellationToken>())).Returns((DateTimeOffset n,CancellationToken ct)=>{Order.Add("Expire");return Task.CompletedTask;});
        Cycle.Setup(x=>x.AutoEndAsync(It.IsAny<DateTimeOffset>(),It.IsAny<CancellationToken>())).Returns((DateTimeOffset n,CancellationToken ct)=>{Order.Add("AutoEnd");return Task.CompletedTask;});
        Cycle.Setup(x=>x.RemindersAsync(It.IsAny<CancellationToken>())).Returns((CancellationToken ct)=>{Order.Add("Reminders");return Task.CompletedTask;});
        Cycle.Setup(x=>x.NeedsAsync(It.IsAny<CancellationToken>())).Returns((CancellationToken ct)=>{Order.Add("Needs");return Task.CompletedTask;});
        Cycle.Setup(x=>x.FeedbackAsync(It.IsAny<CancellationToken>())).Returns((CancellationToken ct)=>{Order.Add("Feedback");return Task.CompletedTask;});
        Ticks.Setup(x=>x.WaitAsync(It.IsAny<CancellationToken>())).Returns((CancellationToken ct)=>{Waits++;Order.Add("Wait");return ValueTask.FromResult(false);});Ticks.Setup(x=>x.Dispose()).Callback(()=>Order.Add("DisposeTimer"));
    }
    public BaoTriWorker Worker(bool orchestration=false)=>new(provider.GetRequiredService<IServiceScopeFactory>(),Log.Object,orchestration?Cycle.Object:null,orchestration?Ticks.Object:null,new EndingClock());
    public async Task Execute(BaoTriWorker w)
    {
        var m=typeof(BaoTriWorker).GetMethod("ExecuteAsync",BindingFlags.Instance|BindingFlags.NonPublic)!;await ((Task)m.Invoke(w,[Token])!).WaitAsync(TimeSpan.FromSeconds(3));
    }
    public async Task Auto(BaoTriWorker w)
    {
        var m=typeof(BaoTriWorker).GetMethod("XuLyTuHuyKetNoiAsync",BindingFlags.Instance|BindingFlags.NonPublic)!;await ((Task)m.Invoke(w,[Now,Token])!).WaitAsync(TimeSpan.FromSeconds(3));
    }
    public int Warnings=>Log.Invocations.Count(i=>i.Method.Name=="Log"&&(LogLevel)i.Arguments[0]==LogLevel.Warning);
    public object Snapshot()=>new{actor=User.Id,Now,Class,Request,Added,Begins,Saves,Commits,Disposals,VersionSets,Version,notices=Notices.Select(n=>new{n.NguoiNhanId,n.LoaiThongBao,n.DoiTuongId,n.MaSuKien}),events=Events.Select(e=>new{e.Id,e.LoaiSuKien,e.DuLieuToiThieu}),Order,Waits,Warnings,forwardCalls=Connections.Invocations.Where(i=>i.Method.Name=="PhanHoiHuyAsync").Select(i=>new{id=i.Arguments[0],agree=i.Arguments[1],rv=i.Arguments[2],actor=i.Arguments[3],tokenMatches=((CancellationToken)i.Arguments[4])==Token})};
    public void NoSave(){Assert.Equal(0,Saves);Assert.Equal(0,Commits);Assert.Empty(Added);Assert.Empty(Notices);Assert.Empty(Events);}
    public void Saved(int noticeCount=1){Assert.Equal(1,Saves);Assert.Equal(1,Commits);Assert.Equal(1,Disposals);Assert.Equal(noticeCount,Notices.Count);Assert.Equal(noticeCount,Events.Count);Assert.True(Order.IndexOf("Event")<Order.IndexOf("Save"));Assert.True(Order.IndexOf("Save")<Order.IndexOf("Commit"));Assert.True(Order.IndexOf("Commit")<Order.IndexOf("DisposeTx"));foreach(var n in Notices)Assert.Contains(Events,e=>e.Id==n.MaSuKien);}
    public void Dispose(){provider.Dispose();Db.Dispose();Stop.Dispose();}
}

