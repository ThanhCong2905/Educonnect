using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using EduConnect.Mvc.Areas.KetNoi.Services;
using EduConnect.Mvc.Areas.KetNoi.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Xunit;

namespace EduConnect.ServiceYeuCauDeXuat.UnitTests;

internal sealed class FixedClock : TimeProvider
{
    public static readonly DateTimeOffset Now = new(2026,11,14,3,0,0,TimeSpan.Zero);
    public override DateTimeOffset GetUtcNow()=>Now;
}

internal sealed class ServiceYeuCauDeXuatFixture : IDisposable
{
    public static readonly Guid H=Guid.Parse("10000000-0000-0000-0000-000000000001"), G=Guid.Parse("20000000-0000-0000-0000-000000000002"), Other=Guid.Parse("30000000-0000-0000-0000-000000000003"), LH=Guid.Parse("40000000-0000-0000-0000-000000000004"), YC=Guid.Parse("50000000-0000-0000-0000-000000000005"), C=Guid.Parse("60000000-0000-0000-0000-000000000006");
    public byte[] RV=[1,2,3,4,5,6,7,8];
    public readonly CancellationTokenSource Cancellation=new();
    public CancellationToken Token=>Cancellation.Token;
    public NguoiThamGia User=new(){Id=H,HoTen="Bên học U1",LoaiNguoi="HocSinh"};
    public YeuCauLienHe? Contact=new(){Id=LH,GiaSuId=G,BenHocId=H,NguoiGuiId=H,TrangThai="ChapNhan"};
    public YeuCauKetNoiLopHoc Request=new(){Id=YC,LienHeId=LH,GiaSuId=G,BenHocId=H,NguoiGuiId=G,TrangThai="ChoBenHoc",HetHanLuc=FixedClock.Now.AddDays(2),RowVersion=[1,2,3,4,5,6,7,8]};
    public NhuCauHoc Need=new(){Id=Guid.Parse("70000000-0000-0000-0000-000000000007"),ChuSoHuuId=H,LoaiNhuCau="TimGiaSu",TrangThai="DangMo"};
    public List<TaiKhoan> Accounts=[new(){Id=H,TrangThai="HoatDong"},new(){Id=G,TrangThai="HoatDong"}];
    public GuiYeuCauViewModel Input=new(){LienHeId=LH,LoiNhan="Mong được học toán"};
    public DeXuatLopViewModel Proposal=new(){LienHeId=LH,TenLop="Toán lớp 9",MonHocId=1,MucTieu="Ôn tập",SoBuoiTuan=1,ThoiLuongPhut=60,BatDauHocLuc=new DateTime(2026,11,15,10,0,0),HocPhi=150000,DonViHocPhi="VND/Gio",HinhThucHoc="TrucTuyen",LichRanh=[new(){ThuTrongTuan=2,GioBatDau=new TimeOnly(18,0),GioKetThuc=new TimeOnly(19,0)}]};
    public readonly Mock<IKetNoiStore> Store=new(MockBehavior.Strict);
    public readonly Mock<IDbContextTransaction> Transaction=new(MockBehavior.Strict);
    public readonly Mock<INguoiDungHienTaiService> Current=new(MockBehavior.Strict);
    public readonly Mock<IThongBaoData> NoticeData=new(MockBehavior.Strict);
    public readonly List<string> Order=[];
    public readonly List<FormattableString> Commands=[];
    public readonly List<ThongBao> Notices=[];
    public readonly List<SuKienChoXuLy> Events=[];
    public Exception? CommandError, SaveError, LockError;
    public string? Version;
    public int Saves,Commits,AccountReads;
    private readonly AppDbContext db=new(new DbContextOptionsBuilder<AppDbContext>().Options);
    public KetNoiService Service {get;}
    public ServiceYeuCauDeXuatFixture()
    {
        Current.Setup(x=>x.HienTaiAsync(It.IsAny<CancellationToken>())).Returns((CancellationToken ct)=>Task.FromResult(User));
        Store.Setup(x=>x.SingleAsync<YeuCauLienHe>(It.IsAny<Expression<Func<YeuCauLienHe,bool>>>(),It.IsAny<CancellationToken>(),It.IsAny<bool>()))
          .Returns((Expression<Func<YeuCauLienHe,bool>> p,CancellationToken ct,bool tracking)=>{Order.Add("ReadContact");return Task.FromResult(Contact!=null&&p.Compile()(Contact)?Contact:null);});
        Store.Setup(x=>x.SingleAsync<YeuCauKetNoiLopHoc>(It.IsAny<Expression<Func<YeuCauKetNoiLopHoc,bool>>>(),It.IsAny<CancellationToken>(),It.IsAny<bool>()))
          .Returns((Expression<Func<YeuCauKetNoiLopHoc,bool>> p,CancellationToken ct,bool tracking)=>{Order.Add("ReadRequest");return Task.FromResult(p.Compile()(Request)?Request:null);});
        Store.Setup(x=>x.SingleAsync<TaiKhoan>(It.IsAny<Expression<Func<TaiKhoan,bool>>>(),It.IsAny<CancellationToken>(),It.IsAny<bool>()))
          .Returns((Expression<Func<TaiKhoan,bool>> p,CancellationToken ct,bool tracking)=>{AccountReads++;return Task.FromResult(Accounts.SingleOrDefault(p.Compile()));});
        Store.Setup(x=>x.AnyAsync<NhuCauHoc>(It.IsAny<Expression<Func<NhuCauHoc,bool>>>(),It.IsAny<CancellationToken>()))
          .Returns((Expression<Func<NhuCauHoc,bool>> p,CancellationToken ct)=>Task.FromResult(p.Compile()(Need)));
        Store.Setup(x=>x.FindAcceptedContactIdAsync(It.IsAny<Guid>(),It.IsAny<Guid>(),It.IsAny<CancellationToken>()))
          .Returns((Guid tutor,Guid learner,CancellationToken ct)=>{Order.Add("ResolveContact");return Task.FromResult(Contact!=null&&Contact.GiaSuId==tutor&&Contact.BenHocId==learner&&Contact.TrangThai=="ChapNhan"?Contact.Id:Guid.Empty);});
        Store.Setup(x=>x.LatestRequestIdAsync(It.IsAny<Guid>(),It.IsAny<string>(),It.IsAny<CancellationToken>()))
          .Returns((Guid contact,string state,CancellationToken ct)=>{Order.Add("Latest:"+contact+":"+state);return Task.FromResult(YC);});
        Store.Setup(x=>x.ClassIdForRequestAsync(It.IsAny<Guid>(),It.IsAny<CancellationToken>())).ReturnsAsync(C);
        Store.Setup(x=>x.RequiredAsync<LopHoc>(It.IsAny<Expression<Func<LopHoc,bool>>>(),It.IsAny<CancellationToken>()))
          .Returns((Expression<Func<LopHoc,bool>> p,CancellationToken ct)=>{var row=new LopHoc{Id=C,YeuCauKetNoiId=YC,GiaSuId=G,BenHocId=H};Assert.True(p.Compile()(row));return Task.FromResult(row);});
        Store.Setup(x=>x.LockContactAsync(It.IsAny<Guid>(),It.IsAny<CancellationToken>())).Returns((Guid id,CancellationToken ct)=>{Order.Add("LockContact:"+id);return Task.CompletedTask;});
        Store.Setup(x=>x.HasActiveClassAsync(It.IsAny<Guid>(),It.IsAny<CancellationToken>())).ReturnsAsync(false);
        Store.Setup(x=>x.BeginAsync(It.IsAny<CancellationToken>())).Returns((CancellationToken ct)=>{Order.Add("Begin");return Task.FromResult(Transaction.Object);});
        Store.Setup(x=>x.ExecuteAsync(It.IsAny<FormattableString>(),It.IsAny<CancellationToken>())).Returns((FormattableString cmd,CancellationToken ct)=>{
            Commands.Add(cmd);Order.Add(cmd.Format.Contains("sp_getapplock")?"AppLock":"BusinessCommand");
            var error=cmd.Format.Contains("sp_getapplock")?LockError:CommandError;
            return error==null?Task.CompletedTask:Task.FromException(error);
        });
        Store.Setup(x=>x.SetVersion(It.IsAny<object>(),It.IsAny<string>())).Callback((object entity,string v)=>{Version=v;Order.Add("SetVersion");});
        Store.Setup(x=>x.SaveAsync(It.IsAny<CancellationToken>())).Returns((CancellationToken ct)=>{Saves++;Order.Add("Save");return SaveError==null?Task.CompletedTask:Task.FromException(SaveError);});
        Transaction.Setup(x=>x.CommitAsync(It.IsAny<CancellationToken>())).Returns((CancellationToken ct)=>{Commits++;Order.Add("Commit");return Task.CompletedTask;});
        Transaction.Setup(x=>x.DisposeAsync()).Returns(ValueTask.CompletedTask);
        NoticeData.Setup(x=>x.AddEvent(It.IsAny<SuKienChoXuLy>())).Callback((SuKienChoXuLy ev)=>{Events.Add(ev);Order.Add("Event");});
        NoticeData.Setup(x=>x.AddNotice(It.IsAny<ThongBao>())).Callback((ThongBao n)=>{Notices.Add(n);Order.Add("Notice");});
        var concreteCurrent=new NguoiDungHienTaiService(db,new HttpContextAccessor());
        Service=new KetNoiService(db,Current.Object,new ThongBaoService(db,concreteCurrent,NoticeData.Object),store:Store.Object,timeProvider:new FixedClock());
    }
    public void Tutor(){User=new(){Id=G,HoTen="Gia sư U2",LoaiNguoi="GiaSu"};}
    public IEnumerable<FormattableString> Business=>Commands.Where(c=>!c.Format.Contains("sp_getapplock"));
    public void NoWrites(){Assert.Empty(Business);Assert.Equal(0,Saves);Assert.Equal(0,Commits);Assert.Empty(Notices);Assert.Empty(Events);}
    public void Saved(Guid recipient,Guid target){Assert.Equal(1,Saves);Assert.Equal(1,Commits);var n=Assert.Single(Notices);Assert.Equal(recipient,n.NguoiNhanId);Assert.Equal(target,n.DoiTuongId);Assert.Equal("KetNoi",n.LoaiThongBao);Assert.Equal(Assert.Single(Events).Id,n.MaSuKien);Assert.True(Order.IndexOf("Event")<Order.IndexOf("Save"));Assert.True(Order.IndexOf("Notice")<Order.IndexOf("Save"));Assert.True(Order.IndexOf("Save")<Order.IndexOf("Commit"));}
    public object Snapshot()=>new {actor=User.Id,role=User.LoaiNguoi,contact=Contact,requestState=Request.TrangThai,accountStates=Accounts.Select(a=>new{a.Id,a.TrangThai}),AccountReads,Version,Saves,Commits,commands=Commands.Select(c=>new{c.Format,args=c.GetArguments().Select(a=>a is SqlParameter p?p.Value:a)}),notices=Notices.Select(n=>new{n.NguoiNhanId,n.DoiTuongId,n.LoaiThongBao}),events=Events.Count,Order};
    public async Task<KetQua<YeuCauLienHe>> Accepted(bool tutor)=>await (Task<KetQua<YeuCauLienHe>>)typeof(KetNoiService).GetMethod("AcceptedAsync",BindingFlags.Instance|BindingFlags.NonPublic,null,[typeof(Guid),typeof(bool),typeof(Guid),typeof(CancellationToken)],null)!.Invoke(Service,[LH,tutor,User.Id,Token])!;
    public void Dispose(){db.Dispose();Cancellation.Dispose();}
    public static SqlException SqlError(int number)
    {
        var ctor=typeof(SqlError).GetConstructors(BindingFlags.Instance|BindingFlags.NonPublic).First(c=>c.GetParameters().Length==8);
        object? Value(ParameterInfo p)=>p.ParameterType==typeof(int)?(p.Name!.Contains("Number",StringComparison.OrdinalIgnoreCase)?number:1):p.ParameterType==typeof(byte)?(byte)16:p.ParameterType==typeof(uint)?(uint)0:p.ParameterType==typeof(string)?(p.Name!.Contains("message",StringComparison.OrdinalIgnoreCase)?"SQL giả lập "+number:"unit"):null;
        var error=ctor.Invoke(ctor.GetParameters().Select(Value).ToArray());
        var collection=(SqlErrorCollection)Activator.CreateInstance(typeof(SqlErrorCollection),true)!;
        typeof(SqlErrorCollection).GetMethod("Add",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(collection,[error]);
        var factory=typeof(SqlException).GetMethods(BindingFlags.Static|BindingFlags.NonPublic).First(m=>m.Name=="CreateException"&&m.GetParameters().Length==2);
        var ex=(SqlException)factory.Invoke(null,[collection,"16.0"])!;Assert.Equal(number,ex.Number);return ex;
    }
}
