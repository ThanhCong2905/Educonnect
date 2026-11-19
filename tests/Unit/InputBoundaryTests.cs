using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Areas.KetNoi.ViewModels;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.Extensions.Configuration;

using Xunit;

public sealed class InputBoundaryTests
{
    [Fact]
    public void Input_boundary_001() => Accept(new BoLocTimKiemViewModel { TuKhoa = new string('a', 100), KhoiLop = new string('b', 256) });

    [Fact]
    public void Input_boundary_002() => Reject(new BoLocTimKiemViewModel { TuKhoa = new string('a', 101) }, "TuKhoa");

    [Fact]
    public void Input_boundary_003() => Reject(new BoLocTimKiemViewModel { KhoiLop = new string('a', 257) }, "KhoiLop");

    [Fact]
    public void Input_boundary_004() => Reject(new BoLocTimKiemViewModel { KhuVuc = new string('a', 257) }, "KhuVuc");

    [Fact]
    public void Input_boundary_005() => Reject(new BoLocTimKiemViewModel { HinhThucHoc = "Khac" }, "HinhThucHoc");

    [Fact]
    public void Input_boundary_006() => Reject(new BoLocTimKiemViewModel { SapXep = "KhongCo" }, "SapXep");

    [Fact]
    public void Input_boundary_007() => Reject(new BoLocTimKiemViewModel { Trang = 0 }, "Trang");

    [Fact]
    public void Input_boundary_008() => Reject(new BoLocTimKiemViewModel { Trang = 100001 }, "Trang");

    [Fact]
    public void Input_boundary_009() => Accept(new BoLocTimKiemViewModel { Trang = 100000 });

    [Fact]
    public void Input_boundary_010() => Reject(new BoLocTimKiemViewModel { MonHocIds = Enumerable.Range(1, 11).ToList() }, "MonHocIds");

    [Fact]
    public void Input_boundary_011() => Accept(new NhuCauInput { MonHocIds = [1], MucTieu = new string('a', 2000) });

    [Fact]
    public void Input_boundary_012() => Accept(new TaoNhuCauBenHocViewModel { MonHocIds = [1], MucTieu = "Ôn tập" }.ToInput());

    [Fact]
    public void Input_boundary_013() => Reject(new NhuCauInput { MucTieu = new string('a', 2001) }, "MucTieu");

    [Fact]
    public void Input_boundary_014() => Reject(new NhuCauInput { SoBuoiTuan = 8 }, "SoBuoiTuan");

    [Fact]
    public void Input_boundary_015() => Reject(new NhuCauInput { TuoiNguoiHoc = 121 }, "TuoiNguoiHoc");

    [Fact]
    public void Input_boundary_016() => Accept(new GuiLienHeInput { LoiNhan = new string('a', 256) });

    [Fact]
    public void Input_boundary_017() => Reject(new GuiLienHeInput { LoiNhan = new string('a', 257) }, "LoiNhan");

    [Fact]
    public void Input_boundary_018() => Reject(new GuiLienHeInput { LoiNhan = "   " }, "LoiNhan");

    [Fact]
    public void Input_boundary_019() => Accept(new GiaoBaiTapViewModel { TieuDe = new string('a', 256), NoiDung = new string('a', 10000), TieuChiCham = new string('a', 5000) });

    [Fact]
    public void Input_boundary_020() => Reject(new GiaoBaiTapViewModel { TieuDe = "Bài", NoiDung = new string('a', 10001) }, "NoiDung");

    [Fact]
    public void Input_boundary_021() => Reject(new ChamBaiViewModel { NhanXet = "Nhận xét", Diem = 10.01m }, "Diem");

    [Fact]
    public void Input_boundary_022() => Accept(new ChamBaiViewModel { NhanXet = new string('a', 2000), Diem = 10 });

    [Fact]
    public void Input_boundary_023() => Reject(new ChamBaiViewModel { NhanXet = new string('a', 2001) }, "NhanXet");

    [Fact]
    public void Input_boundary_024() => Reject(new SuaLichInput { LyDo = new string('a', 2001) }, "LyDo");

    [Theory]
    [InlineData(14, false, false)]
    [InlineData(15, false, true)]
    [InlineData(64, false, true)]
    [InlineData(65, false, false)]
    [InlineData(64, true, true)]
    [InlineData(65, true, false)]
    public async Task Password_length_counts_Unicode_scalars(int length, bool emoji, bool expected)
    {
        var input = emoji ? string.Concat(Enumerable.Repeat("😀", length)) : new string('z', length);
        var validator = new MatKhauValidator(new ConfigurationBuilder().Build());
        var result = await validator.ValidateAsync(null!, new TaiKhoan(), input);
        Assert.Equal(expected, result.Succeeded);
    }
    private static void Accept(object input) => Rules.ValidateInput(input);
    private static void Reject(object input, string field)
    {
        var error = Assert.Throws<LoiNghiepVu>(() => Rules.ValidateInput(input));
        Assert.Equal(400, error.Status);
        Assert.Equal(field, error.Truong);
    }
}
