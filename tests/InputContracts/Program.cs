using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Areas.KetNoi.ViewModels;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.Extensions.Configuration;

var passed = 0;
void Accept(object input)
{
    Rules.ValidateInput(input);
    passed++;
}
void Reject(object input, string field)
{
    try { Rules.ValidateInput(input); }
    catch (LoiNghiepVu ex) when (ex.Status == 400 && ex.Truong == field) { passed++; return; }
    throw new Exception("Expected field error: " + field);
}
Accept(new BoLocTimKiemViewModel { TuKhoa = new string('a', 100), KhoiLop = new string('b', 256) });
Reject(new BoLocTimKiemViewModel { TuKhoa = new string('a', 101) }, "TuKhoa");
Reject(new BoLocTimKiemViewModel { KhoiLop = new string('a', 257) }, "KhoiLop");
Reject(new BoLocTimKiemViewModel { KhuVuc = new string('a', 257) }, "KhuVuc");
Reject(new BoLocTimKiemViewModel { HinhThucHoc = "Khac" }, "HinhThucHoc");
Reject(new BoLocTimKiemViewModel { SapXep = "KhongCo" }, "SapXep");
Reject(new BoLocTimKiemViewModel { Trang = 0 }, "Trang");
Reject(new BoLocTimKiemViewModel { Trang = 100001 }, "Trang");
Accept(new BoLocTimKiemViewModel { Trang = 100000 });
Reject(new BoLocTimKiemViewModel { MonHocIds = Enumerable.Range(1, 11).ToList() }, "MonHocIds");
Accept(new NhuCauInput { MonHocIds = [1], MucTieu = new string('a', 256) });
Accept(new TaoNhuCauBenHocViewModel { MonHocIds = [1], MucTieu = "Ôn tập" }.ToInput());
Reject(new NhuCauInput { MucTieu = new string('a', 257) }, "MucTieu");
Reject(new NhuCauInput { SoBuoiTuan = 8 }, "SoBuoiTuan");
Reject(new NhuCauInput { TuoiNguoiHoc = 121 }, "TuoiNguoiHoc");
Accept(new GuiLienHeInput { LoiNhan = new string('a', 256) });
Reject(new GuiLienHeInput { LoiNhan = new string('a', 257) }, "LoiNhan");
Reject(new GuiLienHeInput { LoiNhan = "   " }, "LoiNhan");
Accept(new GiaoBaiTapViewModel { TieuDe = new string('a', 256), NoiDung = new string('a', 10000), TieuChiCham = new string('a', 5000) });
Reject(new GiaoBaiTapViewModel { TieuDe = "Bài", NoiDung = new string('a', 10001) }, "NoiDung");
Reject(new ChamBaiViewModel { NhanXet = "Nhận xét", Diem = 10.01m }, "Diem");
Accept(new ChamBaiViewModel { NhanXet = new string('a', 2000), Diem = 10 });
Reject(new ChamBaiViewModel { NhanXet = new string('a', 2001) }, "NhanXet");
Reject(new SuaLichInput { LyDo = new string('a', 2001) }, "LyDo");

var passwords = new MatKhauValidator(new ConfigurationBuilder().Build());
foreach (var (value, valid) in new[] {
    (new string('z', 14), false), (new string('z', 15), true),
    (new string('z', 64), true), (new string('z', 65), false),
    (string.Concat(Enumerable.Repeat("😀", 64)), true),
    (string.Concat(Enumerable.Repeat("😀", 65)), false)
})
{
    var result = await passwords.ValidateAsync(null!, new TaiKhoan(), value);
    if (result.Succeeded != valid) throw new Exception("Password boundary mismatch");
    passed++;
}
Console.WriteLine($"PASS {passed} input boundary checks");
