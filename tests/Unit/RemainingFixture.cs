using System.Collections;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using EduConnect.Mvc.Areas.DanhGia.Services;
using EduConnect.Mvc.Areas.HocTap.Services;
using EduConnect.Mvc.Areas.QuanTri.Services;
using EduConnect.Mvc.Areas.NhuCau.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
namespace EduConnect.Remaining.UnitTests;

internal sealed class AsyncRows<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public AsyncRows(IEnumerable<T> rows):base(rows){}
    public AsyncRows(Expression expression):base(expression){}
    IQueryProvider IQueryable.Provider => new AsyncProvider(this);
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken token=default) => new AsyncIterator<T>(this.AsEnumerable().GetEnumerator(),token);
}
internal sealed class AsyncIterator<T>(IEnumerator<T> rows,CancellationToken token) : IAsyncEnumerator<T>
{
    public T Current=>rows.Current;
    public ValueTask<bool> MoveNextAsync(){token.ThrowIfCancellationRequested();return ValueTask.FromResult(rows.MoveNext());}
    public ValueTask DisposeAsync(){rows.Dispose();return ValueTask.CompletedTask;}
}
internal sealed class AsyncProvider(IQueryProvider inner):IAsyncQueryProvider
{
    public IQueryable CreateQuery(Expression expression)=> (IQueryable)Activator.CreateInstance(typeof(AsyncRows<>).MakeGenericType(expression.Type.GetGenericArguments()[0]),expression)!;
    public IQueryable<T> CreateQuery<T>(Expression expression)=>new AsyncRows<T>(expression);
    public object? Execute(Expression expression)=>inner.Execute(expression);
    public T Execute<T>(Expression expression)=>inner.Execute<T>(expression);
    public TResult ExecuteAsync<TResult>(Expression expression,CancellationToken token=default)
    {
        token.ThrowIfCancellationRequested();var type=typeof(TResult).GetGenericArguments()[0];
        var value=typeof(IQueryProvider).GetMethod(nameof(Execute),1,[typeof(Expression)])!.MakeGenericMethod(type).Invoke(inner,[expression]);
        return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))!.MakeGenericMethod(type).Invoke(null,[value])!;
    }
}
internal sealed class MemoryData : IServiceDataStore
{
    readonly Dictionary<Type,IList> rows=[];
    public List<object> Added=[],Removed=[];public List<string> Order=[],Commands=[];
    public int Saves,Commits,Begins,Disposals,VersionSets;public Exception? SaveError;
    public string? Version;public DateTimeOffset Now=DateTimeOffset.Parse("2026-11-14T10:00:00+07:00");
    public List<T> Rows<T>() where T:class
    {if(!rows.TryGetValue(typeof(T),out var list))rows[typeof(T)]=list=new List<T>();return (List<T>)list;}
    public IQueryable<T> Query<T>()where T:class=>new AsyncRows<T>(Rows<T>());
    public void Seed<T>(params T[] items)where T:class=>Rows<T>().AddRange(items);
    public void Add(object item){Added.Add(item);if(!rows.TryGetValue(item.GetType(),out var list))rows[item.GetType()]=list=(IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(item.GetType()))!;list.Add(item);Order.Add("Add:"+item.GetType().Name);}
    public void RemoveRange(IEnumerable<object> items){foreach(var item in items.ToArray()){Removed.Add(item);rows[item.GetType()].Remove(item);}}
    public Task<int> SaveAsync(CancellationToken token=default){token.ThrowIfCancellationRequested();Saves++;Order.Add("Save");return SaveError==null?Task.FromResult(1):Task.FromException<int>(SaveError);}
    public Task<IDbContextTransaction> BeginAsync(IsolationLevel? isolation=null,CancellationToken token=default){Begins++;Order.Add("Begin");var tx=new Mock<IDbContextTransaction>(MockBehavior.Strict);tx.Setup(x=>x.CommitAsync(It.IsAny<CancellationToken>())).Callback(()=>{Commits++;Order.Add("Commit");}).Returns(Task.CompletedTask);tx.Setup(x=>x.DisposeAsync()).Callback(()=>Disposals++).Returns(ValueTask.CompletedTask);return Task.FromResult(tx.Object);}
    public void SetVersion(object entity,string? version){VersionSets++;Version=version;Rules.Version(version);Order.Add("SetVersion");}
    public void MarkModified(object entity,string property)=>Order.Add("Mark:"+property);
    public Task<DateTimeOffset> NowAsync(CancellationToken token=default)=>Task.FromResult(Now);
    public Task<LopHoc> LockClassAsync(Guid id,CancellationToken token=default)=>Task.FromResult(Rows<LopHoc>().Single(x=>x.Id==id));
    public Task LockProfileAsync(Guid id)=>Task.CompletedTask;
    public Task LockAdminAsync(Guid? actor=null,string? permission=null)=>Task.CompletedTask;
    public Task CheckAccountAsync(Guid actor)=>Task.CompletedTask;
    public Task CheckAdminAsync(Guid actor,string permission)=>Task.CompletedTask;
    public Task<int> ExecuteAsync(FormattableString command,CancellationToken token=default){Commands.Add(command.Format);Order.Add("Command");return Task.FromResult(1);}
}
internal sealed class RemainingFixture:IDisposable
{
    public Guid U=Guid.Parse("10000000-0000-0000-0000-000000000001"),G=Guid.Parse("20000000-0000-0000-0000-000000000002"),P=Guid.Parse("30000000-0000-0000-0000-000000000003"),A=Guid.Parse("40000000-0000-0000-0000-000000000004"),C=Guid.Parse("50000000-0000-0000-0000-000000000005"),K=Guid.Parse("60000000-0000-0000-0000-000000000006"),Room=Guid.Parse("70000000-0000-0000-0000-000000000007"),M=Guid.Parse("80000000-0000-0000-0000-000000000008");
    public readonly byte[] RV=[1,2,3,4,5,6,7,8];public string V=>Convert.ToBase64String(RV);
    public readonly MemoryData Data=new();public readonly AppDbContext Db=new(new DbContextOptionsBuilder<AppDbContext>().Options);
    public readonly Mock<INguoiDungHienTaiService> Current=new(MockBehavior.Strict);
    public readonly Mock<IQuyenLopHocService> Rights=new(MockBehavior.Strict);
    public readonly Mock<IThongBaoService> Notices=new(MockBehavior.Loose);
    public readonly Mock<ITepService> Files=new(MockBehavior.Strict);
    public readonly Mock<IQuyenQuanTriService> Admin=new(MockBehavior.Strict);
    public readonly Mock<IPhienDangNhapService> Sessions=new(MockBehavior.Strict);
    public readonly List<(Guid User,string Type,Guid Target,string Text)> Sent=[];
    public readonly List<string> Audit=[];public NguoiThamGia Me;
    public RemainingFixture()
    {
        Me=new(){Id=U,LoaiNguoi="HocSinh",HoTen="U1"};
        Current.Setup(x=>x.HienTaiAsync(It.IsAny<CancellationToken>())).Returns((CancellationToken ct)=>Task.FromResult(Me));
        Current.Setup(x=>x.KiemTraNguoiThucHienAsync(It.IsAny<Guid?>(),It.IsAny<CancellationToken>())).Returns((Guid? id,CancellationToken ct)=>Task.FromResult(Me));
        Rights.Setup(x=>x.KiemTraQuyenXemAsync(It.IsAny<Guid>())).Returns(() => Task.FromResult(Data.Rows<LopHoc>().Single()));
        Rights.Setup(x=>x.KiemTraQuyenXemAsync(It.IsAny<Guid>(),It.IsAny<Guid>())).Returns(() => Task.FromResult(Data.Rows<LopHoc>().Single()));
        Notices.Setup(x=>x.Tao(It.IsAny<Guid>(),It.IsAny<string>(),It.IsAny<Guid>(),It.IsAny<string>())).Callback((Guid u,string t,Guid id,string text)=>Sent.Add((u,t,id,text)));
        Notices.Setup(x=>x.TaoChoLopAsync(It.IsAny<Guid>(),It.IsAny<string>(),It.IsAny<Guid>(),It.IsAny<string>(),It.IsAny<Guid?>())).Returns(Task.CompletedTask);
        Files.Setup(x=>x.KiemTraAsync(It.IsAny<List<IFormFile>>())).ReturnsAsync(new List<(string Name,string Mime,byte[] Bytes)>());
        Files.Setup(x=>x.LuuAsync(It.IsAny<List<(string Name,string Mime,byte[] Bytes)>>(),It.IsAny<Guid?>(),It.IsAny<Guid?>(),It.IsAny<Guid?>(),It.IsAny<Guid?>(),It.IsAny<Guid?>())).ReturnsAsync(new List<string>());
        Files.Setup(x=>x.DonDepDaXoa(It.IsAny<IEnumerable<string>>()));
        Admin.Setup(x=>x.KiemTraAsync(It.IsAny<string>())).ReturnsAsync(A);
        Admin.Setup(x=>x.CoQuyenAsync(It.IsAny<string>())).ReturnsAsync(true);
        Admin.Setup(x=>x.GhiNhatKy(It.IsAny<Guid>(),It.IsAny<string>(),It.IsAny<string>(),It.IsAny<object>(),It.IsAny<string>(),It.IsAny<string?>())).Callback((Guid a,string h,string l,object id,string k,string? c)=>Audit.Add(h));
        Sessions.Setup(x=>x.ThuHoiTatCaAsync(It.IsAny<Guid>(),It.IsAny<string>())).Returns(Task.CompletedTask);
        Data.Seed(new LopHoc{Id=C,GiaSuId=G,BenHocId=U,TrangThai="DangHoatDong",RowVersion=RV});
        Data.Seed(new TaiKhoan{Id=U,HoTen="U1",TrangThai="HoatDong",ConcurrencyStamp="STAMP"},new TaiKhoan{Id=G,HoTen="G",TrangThai="HoatDong"},new TaiKhoan{Id=A,HoTen="Admin",TrangThai="HoatDong"});
        Data.Seed(new IdentityRole<Guid>{Id=K,Name="HocSinh"});Data.Seed(new IdentityUserRole<Guid>{UserId=U,RoleId=K});
        Data.Seed(new DotPhanHoi{Id=K,LopHocId=C,Ten="K1",HanGui=Data.Now.AddDays(2),TrangThai="DangMo"});
        Data.Seed(new CuocHoiThoai{Id=Room,LopHocId=C,GiaSuId=G,BenHocId=U,LoaiHoiThoai="TheoLop"});
    }
    public DanhGiaService Feedback()=>new(Db,Current.Object,Rights.Object,Notices.Object,Data);
    public TinNhanService Chat()=>new(Db,Current.Object,Rights.Object,Files.Object,Data);
    public QuanTriService Management()=>new(Db,Current.Object,Admin.Object,Sessions.Object,Notices.Object,new PhienCapNhat(),Data);
    public BaoCaoViPhamService Reports()=>new(Db,Current.Object,Rights.Object,Chat(),Files.Object,new QuyenQuanTriService(Db,Current.Object,new HttpContextAccessor()),Notices.Object,Data);
    public HoSoGiaSuService Profile()=>new(Db,Current.Object,new MonHocCatalog(Db),Files.Object,Notices.Object,dataStore:Data);
    public HoSoGiaSu SeedProfile(string state="ChoDuyet")
    {
        var h=new HoSoGiaSu{Id=M,GiaSuId=G,TrangThaiDuyet=state,TrinhDo="Dai hoc",ChuyenMon="Toan",KinhNghiem="3 nam",MoTa="Gioi thieu day du",KhuVuc="Ha Noi",DayTrucTuyen=true,RowVersion=RV};Data.Seed(h);
        Data.Seed(new MonHoc{Id=1,TenMon="Toan",DangSuDung=true});Data.Seed(new HoSoGiaSuMonDay{HoSoGiaSuId=h.Id,MonHocId=1,PhamViDay="Lop 10",HocPhiDeXuat=100000,DonViHocPhi="VND/Buoi"});
        Data.Seed(new IdentityUserClaim<Guid>{UserId=A,ClaimType=QuyenQuanTriService.ClaimType,ClaimValue="DuyetHoSo"});return h;
    }
    public void Dispose()=>Db.Dispose();
}
