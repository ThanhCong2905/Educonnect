using EduConnect.Mvc.Areas.TimKiem.Services;
using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace EduConnect.SearchContact.UnitTests;

[Trait("Suite", "TimKiemLienHe")]
public sealed class TimKiemServiceTest
{
    [Fact, Trait("TestCaseId", "UT-021")]
    public async Task UT_021_HocSinh_TimHoSo_KhongTimNhuCau() => await AssertRole("HocSinh");
    [Fact, Trait("TestCaseId", "UT-022")]
    public async Task UT_022_PhuHuynh_TimHoSo_KhongTimNhuCau() => await AssertRole("PhuHuynh");
    [Fact, Trait("TestCaseId", "UT-023")]
    public async Task UT_023_GiaSu_TimNhuCau_KhongTimHoSo() => await AssertRole("GiaSu");
    private static async Task AssertRole(string role)
    {
        using var f=new Fixture(role);var filter=f.Filter();
        var expected=role=="GiaSu" ? (UngVienViewModel)f.Need() : f.Profile();
        f.Search(filter,[expected],12);
        var result=await f.Service.TimKiemAsync(filter,f.User.Id,f.Token);
        Assert.True(result.ThanhCong,result.ThongBao);
        Assert.Same(expected,Assert.Single(result.DuLieu.Items));
        if(role=="GiaSu")
        {
            Assert.IsType<NhuCauCongKhaiViewModel>(result.DuLieu.Items[0]);
            f.Store.Verify(x=>x.TimNhuCauAsync(filter,f.User.Id,f.Token),Times.Once);
            f.Store.Verify(x=>x.TimHoSoAsync(It.IsAny<BoLocTimKiemViewModel>(),It.IsAny<CancellationToken>()),Times.Never);
        }
        else
        {
            Assert.IsType<HoSoGiaSuCongKhaiViewModel>(result.DuLieu.Items[0]);
            f.Store.Verify(x=>x.TimHoSoAsync(filter,f.Token),Times.Once);
            f.Store.Verify(x=>x.TimNhuCauAsync(It.IsAny<BoLocTimKiemViewModel>(),It.IsAny<Guid>(),It.IsAny<CancellationToken>()),Times.Never);
        }
        f.Store.VerifyNoOtherCalls();f.VerifyActor();
    }
    [Fact, Trait("TestCaseId", "UT-024")]
    public async Task UT_024_PhiAm_KhongTruyVan()
    {using var f=new Fixture();var filter=f.Filter();filter.HocPhiToiDa=-1;await InvalidFilter(f,filter);}
    [Fact, Trait("TestCaseId", "UT-025")]
    public async Task UT_025_DonViSai_KhongTruyVan()
    {using var f=new Fixture();var filter=f.Filter();filter.DonViHocPhi="USD/Thang";await InvalidFilter(f,filter);}
    [Fact, Trait("TestCaseId", "UT-026")]
    public async Task UT_026_TrangKhong_KhongTruyVan()
    {using var f=new Fixture();var filter=f.Filter();filter.Trang=0;await InvalidFilter(f,filter);}
    private static async Task InvalidFilter(Fixture f,BoLocTimKiemViewModel filter)
    {
        var result=await f.Service.TimKiemAsync(filter,f.User.Id,f.Token);
        Assert.False(result.ThanhCong);Assert.Equal(400,result.MaTrangThai);Assert.False(string.IsNullOrWhiteSpace(result.ThongBao));
        f.Store.VerifyNoOtherCalls();f.VerifyActor();
    }
    [Fact, Trait("TestCaseId", "UT-027")]
    public async Task UT_027_Page_HaiDongTongMuoiHai_GiuKichThuocHai()
    {
        using var f=new Fixture();var filter=f.Filter();filter.Trang=2;
        f.Search(filter,[f.Profile(),f.Profile()],12,size:2);
        var result=await f.Service.TimKiemAsync(filter,f.User.Id,f.Token);
        Assert.True(result.ThanhCong,result.ThongBao);Assert.Equal(12,result.DuLieu.Total);
        Assert.Equal(2,result.DuLieu.Items.Count);Assert.Equal(2,result.DuLieu.Trang);
        Assert.Equal(2,result.DuLieu.KichThuocTrang);
    }
    [Fact(Skip="UT-028: model hiện không có DiemPhuHop hoặc hợp đồng bộ tính điểm. Tài liệu MT1 yêu cầu lý do gợi ý, không phần trăm; cần chốt lại yêu cầu trước khi thực thi ca điểm."),Trait("TestCaseId","UT-028")]
    public void UT_028_AnhXaDiemPhuHop_ChuaCoHopDong() => throw new InvalidOperationException("Chưa có hợp đồng điểm phù hợp.");
    [Fact,Trait("TestCaseId","UT-033")]
    public async Task UT_033_ChiTietHoSoCongKhai_DungModel()
    {
        using var f=new Fixture();var row=f.Profile();f.Read(row);
        var result=await f.Service.ChiTietAsync(row.Id,f.Token);
        Assert.Same(row,result);Assert.IsType<HoSoGiaSuCongKhaiViewModel>(result);
        f.Store.Verify(x=>x.ReadProfileAsync(row.Id,f.Token),Times.Once);
        f.Store.VerifyNoOtherCalls();
    }
    [Fact,Trait("TestCaseId","UT-034")]
    public async Task UT_034_ChiTietNhuCau_SauKhiKhongCoHoSo()
    {
        using var f=new Fixture();var row=f.Need();f.Read(row);
        var result=await f.Service.ChiTietAsync(row.Id,f.Token);
        Assert.Same(row,result);Assert.IsType<NhuCauCongKhaiViewModel>(result);
        f.Store.Verify(x=>x.ReadProfileAsync(row.Id,f.Token),Times.Once);
        f.Store.Verify(x=>x.ReadNeedAsync(row.Id,f.Token),Times.Once);f.Store.VerifyNoOtherCalls();
    }
    [Fact,Trait("TestCaseId","UT-035")]
    public async Task UT_035_KhongCongKhai_404_KhongLoDuLieu()
    {
        using var f=new Fixture();var id=Guid.NewGuid();f.Missing(id);
        var ex=await Assert.ThrowsAsync<LoiNghiepVu>(()=>f.Service.ChiTietAsync(id,f.Token));
        Assert.Equal(404,ex.Status);Assert.Contains("không còn công khai",ex.Message);
        f.Store.Verify(x=>x.ReadProfileAsync(id,f.Token),Times.Once);f.Store.Verify(x=>x.ReadNeedAsync(id,f.Token),Times.Once);f.Store.VerifyNoOtherCalls();
    }
    [Fact,Trait("TestCaseId","UT-038")]
    public async Task UT_038_SoSanhMotId_ThanhCongTheoBangGoc()
    {using var f=new Fixture();var a=f.Profile();f.Read(a);await CompareSuccess(f,[a.Id],[a]);}
    [Fact,Trait("TestCaseId","UT-039")]
    public async Task UT_039_SoSanhHaiId_ThanhCong()
    {using var f=new Fixture();var a=f.Profile();var b=f.Profile();f.Read(a);f.Read(b);await CompareSuccess(f,[a.Id,b.Id],[a,b]);}
    [Fact,Trait("TestCaseId","UT-040")]
    public async Task UT_040_SoSanhBaId_ThanhCong()
    {using var f=new Fixture();var a=f.Profile();var b=f.Profile();var c=f.Profile();f.Read(a);f.Read(b);f.Read(c);await CompareSuccess(f,[a.Id,b.Id,c.Id],[a,b,c]);}
    [Fact,Trait("TestCaseId","UT-041")]
    public async Task UT_041_SoSanhKhongId_TuChoiTruocDoc()
    {using var f=new Fixture();await CompareRejected(f,[]);}
    [Fact,Trait("TestCaseId","UT-042")]
    public async Task UT_042_SoSanhBonId_TuChoiTruocDoc()
    {using var f=new Fixture();await CompareRejected(f,[Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid()]);}
    private static async Task CompareRejected(Fixture f,Guid[] ids)
    {
        var result=await f.Service.SoSanhAsync(ids,f.User.Id,f.Token);
        Assert.False(result.ThanhCong);Assert.Equal(400,result.MaTrangThai);f.Store.VerifyNoOtherCalls();f.VerifyActor();
    }
    private static async Task CompareSuccess(Fixture f,Guid[] ids,UngVienViewModel[] expected)
    {
        var result=await f.Service.SoSanhAsync(ids,f.User.Id,f.Token);
        Assert.True(result.ThanhCong,result.ThongBao);
        Assert.Equal(expected.Select(x=>x.Id),result.DuLieu.MaDoiTuong);
        Assert.Equal(expected,result.DuLieu.Items);
        foreach(var id in expected.Select(x=>x.Id))f.Store.Verify(x=>x.ReadProfileAsync(id,f.Token),Times.Once);
        f.Store.VerifyNoOtherCalls();f.VerifyActor();
    }
    [Fact,Trait("TestCaseId","UT-043")]
    public async Task UT_043_LoaiTrungTruocDem_VaDocMoiIdMotLan()
    {using var f=new Fixture();var a=f.Profile();var b=f.Profile();var c=f.Profile();f.Read(a);f.Read(b);f.Read(c);await CompareSuccess(f,[a.Id,a.Id,b.Id,c.Id],[a,b,c]);}
    [Fact,Trait("TestCaseId","UT-044")]
    public async Task UT_044_BiAnMotDoiTuong_GiuHaiDoiTuongConLaiTheoBangGoc()
    {
        using var f=new Fixture();var a=f.Profile();var b=Guid.NewGuid();var c=f.Profile();f.Read(a);f.Missing(b);f.Read(c);
        var result=await f.Service.SoSanhAsync(new[]{a.Id,b,c.Id},f.User.Id,f.Token);
        Assert.True(result.ThanhCong,result.ThongBao);Assert.Equal(new[]{a.Id,c.Id},result.DuLieu.Items.Select(x=>x.Id));
    }
    [Fact,Trait("TestCaseId","UT-047")]
    public async Task UT_047_LienQuan_KhongLapNhuCauDangXem()
    {
        using var f=new Fixture();var owner=Guid.NewGuid();var current=f.Need(owner);var other=f.Need(owner);
        // Store contract returns all public needs for this owner, including the current one.
        f.Store.Setup(x=>x.LienQuanAsync(owner)).ReturnsAsync([current,other]);
        var result=await f.Service.LienQuanAsync(owner);
        f.Store.Verify(x=>x.LienQuanAsync(owner),Times.Once);f.Store.VerifyNoOtherCalls();
        Assert.DoesNotContain(result,x=>x.Id==current.Id);Assert.Same(other,Assert.Single(result));
    }
    [Fact,Trait("TestCaseId","UT-048")]
    public void UT_048_AnhXaDayDuHoSo_HaiMonHaiHocPhi()
    {
        var h=new HoSoGiaSu {Id=Guid.NewGuid(),GiaSuId=Guid.NewGuid(),TrinhDo="Thac si",ChuyenMon="Toan",KinhNghiem="5 nam",MoTa="Gioi thieu",KhuVuc="Ha Noi",DayTrucTuyen=true,DayTrucTiep=true,DuyetLuc=DateTimeOffset.Parse("2026-01-01T00:00:00Z")};
        List<MonDayCongKhai> subjects=[new(){MonHocId=1,TenMon="Toan",PhamViDay="10",HocPhi=150000,DonVi="VND/Gio"},new(){MonHocId=2,TenMon="Ly",PhamViDay="11",HocPhi=200000,DonVi="VND/Buoi"}];
        var result=HoSoCongKhaiMapper.Map(h,"Gia su A","avatar-key",subjects);
        Assert.Equal(h.Id,result.Id);Assert.Equal(h.GiaSuId,result.ChuSoHuuId);Assert.Equal("Gia su A",result.HoTen);
        Assert.Equal(h.TrinhDo,result.TrinhDo);Assert.Equal(h.ChuyenMon,result.ChuyenMon);Assert.Equal(h.KinhNghiem,result.KinhNghiem);Assert.Equal(h.MoTa,result.MoTa);
        Assert.Equal(h.KhuVuc,result.KhuVuc);Assert.Equal("CaHai",result.HinhThucHoc);Assert.True(result.DayTrucTuyen);Assert.True(result.DayTrucTiep);
        Assert.Equal(subjects,result.MonTheoHoSo);Assert.Equal(150000m,result.MonTheoHoSo.Single(x=>x.MonHocId==1).HocPhi);Assert.Equal(200000m,result.MonTheoHoSo.Single(x=>x.MonHocId==2).HocPhi);
        Assert.Equal("VND/Gio",result.MonTheoHoSo[0].DonVi);Assert.Equal("VND/Buoi",result.MonTheoHoSo[1].DonVi);Assert.Equal("10",result.MonTheoHoSo[0].PhamViDay);Assert.Equal(new[]{"Toan","Ly"},result.MonDay);
    }
    [Fact,Trait("TestCaseId","UT-049")]
    public async Task UT_049_SoSanhChiDoc_KhongTruyCapDbHoacGhi()
    {using var f=new Fixture();var a=f.Profile();var b=f.Profile();f.Read(a);f.Read(b);await CompareSuccess(f,[a.Id,b.Id],[a,b]);}

    private sealed class Fixture:IDisposable
    {
        public AppDbContext Db {get;}=new(new DbContextOptionsBuilder<AppDbContext>().Options);
        private readonly CancellationTokenSource cancellation=new();public CancellationToken Token=>cancellation.Token;
        public NguoiThamGia User {get;}=new(){Id=Guid.NewGuid(),HoTen="U1",LoaiNguoi="HocSinh"};
        public Mock<INguoiDungHienTaiService> Current {get;}=new(MockBehavior.Strict);
        public Mock<ITimKiemStore> Store {get;}=new(MockBehavior.Strict);
        public TimKiemService Service {get;}
        public Fixture(string role="HocSinh")
        {
            User.LoaiNguoi=role;Current.Setup(x=>x.KiemTraNguoiThucHienAsync(User.Id,Token)).ReturnsAsync(User);
            Service=new(Db,Current.Object,Store.Object);
        }
        public BoLocTimKiemViewModel Filter()=>new(){MonHocId=1,KhoiLop="10",HinhThucHoc="TrucTuyen",Trang=1};
        public HoSoGiaSuCongKhaiViewModel Profile()=>new(){Id=Guid.NewGuid(),ChuSoHuuId=Guid.NewGuid(),LoaiNguoi="GiaSu",HoTen="GS",MonHocId=1,TenMon="Toan"};
        public NhuCauCongKhaiViewModel Need(Guid? owner=null)=>new(){Id=Guid.NewGuid(),ChuSoHuuId=owner??Guid.NewGuid(),LoaiNguoi="HocSinh",HoTen="HS",MonHocId=1,TenMon="Toan"};
        public void Search(BoLocTimKiemViewModel filter,List<UngVienViewModel> rows,int total,int size=9)
        {
            var data=new Page<UngVienViewModel>{Items=rows,Total=total,Trang=filter.Trang,KichThuocTrang=size};
            if(User.LoaiNguoi=="GiaSu")Store.Setup(x=>x.TimNhuCauAsync(filter,User.Id,Token)).ReturnsAsync(data);
            else Store.Setup(x=>x.TimHoSoAsync(filter,Token)).ReturnsAsync(data);
        }
        public void Read(UngVienViewModel row)
        {
            Store.Setup(x=>x.ReadProfileAsync(row.Id,Token)).ReturnsAsync(row as HoSoGiaSuCongKhaiViewModel);
            if(row is not HoSoGiaSuCongKhaiViewModel)Store.Setup(x=>x.ReadNeedAsync(row.Id,Token)).ReturnsAsync(row);
        }
        public void Missing(Guid id)
        {Store.Setup(x=>x.ReadProfileAsync(id,Token)).ReturnsAsync((HoSoGiaSuCongKhaiViewModel?)null);Store.Setup(x=>x.ReadNeedAsync(id,Token)).ReturnsAsync((UngVienViewModel?)null);}
        public void VerifyActor(){Current.Verify(x=>x.KiemTraNguoiThucHienAsync(User.Id,Token),Times.Once);Current.VerifyNoOtherCalls();}
        public void Dispose(){Db.Dispose();cancellation.Dispose();}
    }
}
