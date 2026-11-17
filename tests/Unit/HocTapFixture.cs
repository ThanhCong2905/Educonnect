using System.Data;
using System.Linq.Expressions;
using EduConnect.Mvc.Areas.HocTap.Services;
using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Xunit;

namespace EduConnect.HocTap.UnitTests;

internal sealed class HocTapFixture:IDisposable
{
    public static readonly byte[] RV=[1,2,3,4,5,6,7,8];
    public static string Version=>Convert.ToBase64String(RV);
    public DateTimeOffset Now {get;}=DateTimeOffset.Parse("2026-11-14T19:00:00+07:00");
    public Guid H {get;}=Guid.NewGuid();public Guid P {get;}=Guid.NewGuid();public Guid X {get;}=Guid.NewGuid();
    public NguoiThamGia User {get;}=new(){Id=Guid.NewGuid(),HoTen="Gia su G",LoaiNguoi="GiaSu"};
    public LopHoc Lop {get;}
    public LichHoc Schedule {get;}
    public BuoiHoc Lesson {get;}
    public BaiTapGiao Assignment {get;}
    public BaiTapNop Submission {get;}
    public Mock<IHocTapData> Data {get;}=new(MockBehavior.Strict);
    public Mock<IDbContextTransaction> Transaction {get;}=new(MockBehavior.Strict);
    public Mock<INguoiDungHienTaiService> Current {get;}=new(MockBehavior.Strict);
    public Mock<IQuyenLopHocService> Rights {get;}=new(MockBehavior.Strict);
    public Mock<IThongBaoService> Notices {get;}=new(MockBehavior.Strict);
    public Mock<ITepService> Files {get;}=new(MockBehavior.Strict);
    public Mock<IThongBaoData> NoticeData {get;}=new(MockBehavior.Strict);
    public List<ThongBao> Delivered {get;}=[];public List<SuKienChoXuLy> Outbox {get;}=[];
    public List<(Guid Lop,string Type,Guid Target,string Text,Guid? Exclude)> NoticeCalls {get;}=[];
    public List<FormattableString> Sql {get;}=[];public List<object> Added {get;}=[];public List<string> Events {get;}=[];
    private readonly Dictionary<Type,object> tables=[];
    private readonly AppDbContext db=new(new DbContextOptionsBuilder<AppDbContext>().Options);
    public IThongBaoService RealNotices {get;}
    public LichHocService Lich {get;}
    public BaiTapService Tasks {get;}
    public bool DenyWrite {get;set;}public bool ClosedClass {get;set;}
    public HocTapFixture(bool realNotices=false)
    {
        Lop=new(){Id=Guid.NewGuid(),GiaSuId=User.Id,BenHocId=H,TrangThai="DangHoatDong",RowVersion=[..RV]};
        Schedule=new(){Id=Guid.NewGuid(),LopHocId=Lop.Id,TieuDe="Buoi cu",BatDauLuc=Now.AddHours(-2),KetThucLuc=Now.AddHours(-1),TrangThai="DaLenLich",RowVersion=[..RV]};
        Lesson=new(){Id=Guid.NewGuid(),LichHocId=Schedule.Id,TieuDe=Schedule.TieuDe,TrangThai="DaHoc",RowVersion=[..RV],NhanXet="Nhan xet cu",NoiDungGiangDay="Noi dung cu"};
        Assignment=new(){Id=Guid.NewGuid(),LopHocId=Lop.Id,BuoiHocId=Lesson.Id,NguoiGiaoId=User.Id,TrangThai="DaGiao",TieuDe="Bai cu",NoiDung="Noi dung cu",HanNopLuc=Now.AddHours(1),RowVersion=[..RV]};
        Submission=new(){Id=Guid.NewGuid(),BaiTapGiaoId=Assignment.Id,NguoiNopId=H,NoiDung="Bai lam",NopLuc=Now,MaThaoTac=Guid.NewGuid(),LanNop=1,RowVersion=[..RV]};
        Register(Lop);Register(Schedule);Register(Lesson);Register(Assignment);Register(Submission);
        Register<KetQuaChamBai>();Register<TepDinhKem>();Register<ThanhVienLopHoc>();Register<ThongBao>();Register<LichSuDieuChinhLich>();
        Current.Setup(x=>x.HienTaiAsync(It.IsAny<CancellationToken>())).ReturnsAsync(User);
        Rights.Setup(x=>x.KiemTraQuyenXemAsync(Lop.Id)).ReturnsAsync(Lop);
        Rights.Setup(x=>x.KiemTraQuyenGhiAsync(Lop.Id,It.IsAny<bool>())).Returns((Guid id,bool tutor)=>
            ClosedClass?Task.FromException<LopHoc>(new LoiNghiepVu("Lớp không còn hoạt động.",409)):
            DenyWrite?Task.FromException<LopHoc>(new LoiNghiepVu("Bạn không có quyền thực hiện thao tác này.",403)):Task.FromResult(Lop));
        Data.Setup(x=>x.BeginAsync(It.IsAny<IsolationLevel?>())).ReturnsAsync(Transaction.Object);
        Data.Setup(x=>x.ExecuteAsync(It.IsAny<FormattableString>())).Callback<FormattableString>(s=>{Sql.Add(s);Events.Add("sql");}).Returns(Task.CompletedTask);
        Data.Setup(x=>x.ReloadAsync(It.IsAny<object>())).Returns(Task.CompletedTask);
        Data.Setup(x=>x.SetVersion(It.IsAny<object>(),It.IsAny<string?>()));
        Data.Setup(x=>x.SaveAsync()).Callback(()=>Events.Add("save")).Returns(Task.CompletedTask);
        Data.Setup(x=>x.OverlapAsync(Lop,Schedule.Id,It.IsAny<DateTimeOffset>(),It.IsAny<DateTimeOffset>())).ReturnsAsync(false);
        Data.Setup(x=>x.LessonInClassAsync(Lesson.Id,Lop.Id)).ReturnsAsync(true);
        Transaction.Setup(x=>x.CommitAsync(default)).Callback(()=>Events.Add("commit")).Returns(Task.CompletedTask);
        Transaction.Setup(x=>x.DisposeAsync()).Returns(ValueTask.CompletedTask);
        Notices.Setup(x=>x.TaoChoLopAsync(It.IsAny<Guid>(),It.IsAny<string>(),It.IsAny<Guid>(),It.IsAny<string>(),It.IsAny<Guid?>()))
            .Callback<Guid,string,Guid,string,Guid?>((l,t,target,text,exclude)=>NoticeCalls.Add((l,t,target,text,exclude))).Returns(Task.CompletedTask);
        Notices.Setup(x=>x.Tao(It.IsAny<Guid>(),It.IsAny<string>(),It.IsAny<Guid>(),It.IsAny<string>()))
            .Callback<Guid,string,Guid,string>((recipient,type,target,text)=>Delivered.Add(new(){NguoiNhanId=recipient,LoaiThongBao=type,DoiTuongId=target,NoiDungTomTat=text}));
        Files.Setup(x=>x.KiemTraAsync(It.IsAny<List<IFormFile>>())).ReturnsAsync([]);
        Files.Setup(x=>x.LuuAsync(It.IsAny<List<(string Name,string Mime,byte[] Bytes)>>(),It.IsAny<Guid?>(),It.IsAny<Guid?>(),It.IsAny<Guid?>(),It.IsAny<Guid?>(),It.IsAny<Guid?>())).ReturnsAsync([]);
        NoticeData.Setup(x=>x.ReadClassAsync(Lop.Id)).ReturnsAsync(Lop);
        NoticeData.Setup(x=>x.ReadEligibleMemberIdsAsync(Lop.Id)).ReturnsAsync(()=>new List<Guid>{P,H,P});
        NoticeData.Setup(x=>x.AddEvent(It.IsAny<SuKienChoXuLy>())).Callback<SuKienChoXuLy>(e=>Outbox.Add(e));
        NoticeData.Setup(x=>x.AddNotice(It.IsAny<ThongBao>())).Callback<ThongBao>(n=>Delivered.Add(n));
        RealNotices=new ThongBaoService(db,new NguoiDungHienTaiService(db,new HttpContextAccessor()),NoticeData.Object);
        IThongBaoService writer=realNotices?RealNotices:Notices.Object;
        var clock=new FixedTime(Now);
        Lich=new(db,Current.Object,Rights.Object,writer,Data.Object,clock);
        Tasks=new(db,Current.Object,Rights.Object,writer,Files.Object,Data.Object,clock);
    }
    public List<T> Rows<T>()where T:class=>(List<T>)tables[typeof(T)];
    private void Register<T>(params T[] values)where T:class
    {
        tables[typeof(T)]=values.ToList();
        Data.Setup(x=>x.SingleAsync<T>(It.IsAny<Expression<Func<T,bool>>>(),It.IsAny<bool>())).ReturnsAsync((Expression<Func<T,bool>> p,bool track)=>Rows<T>().SingleOrDefault(p.Compile()));
        Data.Setup(x=>x.RequiredAsync<T>(It.IsAny<Expression<Func<T,bool>>>())).ReturnsAsync((Expression<Func<T,bool>> p)=>Rows<T>().Single(p.Compile()));
        Data.Setup(x=>x.ListAsync<T>(It.IsAny<Expression<Func<T,bool>>>())).ReturnsAsync((Expression<Func<T,bool>> p)=>Rows<T>().Where(p.Compile()).ToList());
        Data.Setup(x=>x.AnyAsync<T>(It.IsAny<Expression<Func<T,bool>>>())).ReturnsAsync((Expression<Func<T,bool>> p)=>Rows<T>().Any(p.Compile()));
        Data.Setup(x=>x.Add(It.IsAny<T>())).Callback<T>(x=>{Added.Add(x);Rows<T>().Add(x);Events.Add("add");});
    }
    public GhiNhanBuoiHocViewModel Record()=>new(){LichHocId=Schedule.Id,RowVersion=Version,KetQuaBuoiHoc="DaDienRa",NoiDung="Noi dung moi",NhanXet="Nhan xet moi",TrangThaiGiaSu="CoMat",TrangThaiBenHoc="CoMat"};
    public LichInput Future(int days=1)=>new(){TieuDe="Buoi moi",BatDauLuc=Local(Now.AddDays(days)),KetThucLuc=Local(Now.AddDays(days).AddHours(1))};
    public GiaoBaiTapViewModel Assign()=>new(){LopHocId=Lop.Id,BuoiHocId=Lesson.Id,TieuDe="Bai moi",NoiDung="Noi dung bai moi",HanNopLuc=Local(Now.AddDays(2))};
    public ChamBaiViewModel Grade()=>new(){BaiTapGiaoId=Assignment.Id,BaiTapNopId=Submission.Id,RowVersion=Version,Diem=8.25m,NhanXet="Lam tot"};
    public NopBaiViewModel Submit(DateTimeOffset time)
    {
        User.Id=H;User.LoaiNguoi="HocSinh";User.HoTen="H";
        Rows<BaiTapNop>().Clear();Submission.NopLuc=time;
        var input=new NopBaiViewModel{BaiTapGiaoId=Assignment.Id,NoiDung="Bai lam moi",MaThaoTac=Submission.MaThaoTac};
        Data.Setup(x=>x.RequiredAsync<BaiTapNop>(It.IsAny<Expression<Func<BaiTapNop,bool>>>())).ReturnsAsync(Submission);
        return input;
    }
    public FormattableString Procedure(string name)=>Assert.Single(Sql,s=>s.Format.Contains(name,StringComparison.Ordinal));
    public void AssertNoWrite()
    {
        Assert.Empty(Added);Assert.Empty(Delivered);Assert.Empty(NoticeCalls);
        Data.Verify(x=>x.SaveAsync(),Times.Never);Transaction.Verify(x=>x.CommitAsync(It.IsAny<CancellationToken>()),Times.Never);
    }
    public static DateTime Local(DateTimeOffset value)=>value.ToOffset(TimeSpan.FromHours(7)).DateTime;
    public void Dispose()=>db.Dispose();
    private sealed class FixedTime(DateTimeOffset now):TimeProvider{public override DateTimeOffset GetUtcNow()=>now.ToUniversalTime();}
}
