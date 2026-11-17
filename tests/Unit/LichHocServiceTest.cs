using EduConnect.Mvc.Areas.HocTap.Services;
using System.Text.Json;
using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Moq;
using Xunit;

namespace EduConnect.HocTap.UnitTests;

[Trait("Suite","LichHoc")]
public sealed class LichHocServiceTest
{
    [Fact,Trait("Suite","LichHoc"),Trait("TestCaseId","UT-070"),Trait("TestKind","DesignContract")]
    public void UT_070_ThietKe_GhiNhanBuoiHocAsync_PhaiCoTrongCode()
    {
        HocTapDesignContract.RequireMethod("UT-070", typeof(LichHocService), "GhiNhanBuoiHocAsync", null, "GhiNhanAsync");
    }
    [Fact,Trait("Suite","LichHoc"),Trait("TestCaseId","UT-071"),Trait("TestKind","DesignContract")]
    public void UT_071_ThietKe_GhiNhanBuoiHocAsync_PhaiCoTrongCode()
    {
        HocTapDesignContract.RequireMethod("UT-071", typeof(LichHocService), "GhiNhanBuoiHocAsync", null, "GhiNhanAsync");
    }
    [Fact,Trait("Suite","LichHoc"),Trait("TestCaseId","UT-072"),Trait("TestKind","DesignContract")]
    public void UT_072_ThietKe_GhiNhanBuoiHocAsync_PhaiCoTrongCode()
    {
        HocTapDesignContract.RequireMethod("UT-072", typeof(LichHocService), "GhiNhanBuoiHocAsync", null, "GhiNhanAsync");
    }
    [Fact,Trait("TestCaseId","UT-074")]
    public async Task UT_074_HaiBuoiHopLe_TruyenDuHaiBuoiVaThongBao()
    {
        using var f=new HocTapFixture();var model=new LichHangLoatInput{LopHocId=f.Lop.Id,Buoi=[f.Future(),f.Future(2)]};
        await f.Lich.ThemHangLoatAsync(model);var command=f.Procedure("usp_ThemLichHocHangLoat");var args=command.GetArguments();Assert.Equal(f.Lop.Id,args[0]);Assert.Equal(f.User.Id,args[1]);
        using var payload=JsonDocument.Parse(Assert.IsType<string>(args[2]));var rows=payload.RootElement.EnumerateArray().ToArray();Assert.Equal(2,rows.Length);
        for(int i=0;i<2;i++){Assert.Equal(model.Buoi[i].TieuDe,rows[i].GetProperty("TieuDe").GetString());Assert.Equal(Rules.Local(model.Buoi[i].BatDauLuc),rows[i].GetProperty("BatDauLuc").GetDateTimeOffset());Assert.Equal(Rules.Local(model.Buoi[i].KetThucLuc),rows[i].GetProperty("KetThucLuc").GetDateTimeOffset());}
        Assert.Equal("LichHoc",Assert.Single(f.NoticeCalls).Type);f.Data.Verify(x=>x.SaveAsync(),Times.Once);f.Transaction.Verify(x=>x.CommitAsync(default),Times.Once);
    }
    [Fact,Trait("Suite","LichHoc"),Trait("TestCaseId","UT-076"),Trait("TestKind","DesignContract")]
    public void UT_076_ThietKe_DoiLichAsync_PhaiCoTrongCode()
    {
        HocTapDesignContract.RequireMethod("UT-076", typeof(LichHocService), "DoiLichAsync", null, "SuaAsync");
    }
    [Fact,Trait("Suite","LichHoc"),Trait("TestCaseId","UT-077"),Trait("TestKind","DesignContract")]
    public void UT_077_ThietKe_GhiNhanBuoiHocAsync_PhaiCoTrongCode()
    {
        HocTapDesignContract.RequireMethod("UT-077", typeof(LichHocService), "GhiNhanBuoiHocAsync", null, "GhiNhanAsync");
    }
    [Fact,Trait("Suite","LichHoc"),Trait("TestCaseId","UT-078"),Trait("TestKind","DesignContract")]
    public void UT_078_ThietKe_GhiNhanBuoiHocAsync_PhaiCoTrongCode()
    {
        HocTapDesignContract.RequireMethod("UT-078", typeof(LichHocService), "GhiNhanBuoiHocAsync", null, "GhiNhanAsync");
    }
    [Fact,Trait("TestCaseId","UT-079")]
    public async Task UT_079_ThemBuoi_GiuNguyenBuoiBanDau()
    {
        using var f=new HocTapFixture();var before=(f.Schedule.BatDauLuc,f.Schedule.KetThucLuc,f.Schedule.TrangThai);
        await f.Lich.ThemHangLoatAsync(new(){LopHocId=f.Lop.Id,Buoi=[f.Future(3)]});
        Assert.Equal(before,(f.Schedule.BatDauLuc,f.Schedule.KetThucLuc,f.Schedule.TrangThai));Assert.Same(f.Schedule,Assert.Single(f.Rows<LichHoc>()));
        using var payload=JsonDocument.Parse(Assert.IsType<string>(f.Procedure("usp_ThemLichHocHangLoat").GetArguments()[2]));Assert.Single(payload.RootElement.EnumerateArray());
        f.Transaction.Verify(x=>x.CommitAsync(default),Times.Once);
    }
}
