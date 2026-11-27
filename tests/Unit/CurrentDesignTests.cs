using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.IO.Compression;
using System.Security.Claims;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Controllers;
using EduConnect.Mvc.ViewModels;
using EduConnect.Mvc.Models;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Areas.HocTap.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Xunit;
namespace EduConnect.UnitTests.Current;
public sealed class CurrentDesignTests {
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-001"),Trait("Dataset","DL01")]
public void UT_001_DL01(){
CurrentEfBoundary.Run(1,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-002"),Trait("Dataset","DL01")]
public void UT_002_DL01(){
CurrentEfBoundary.Run(2,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-003"),Trait("Dataset","DL01")]
public void UT_003_DL01(){
CurrentEfBoundary.Run(3,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-004"),Trait("Dataset","DL01")]
public void UT_004_DL01(){
CurrentEfBoundary.Run(4,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-005"),Trait("Dataset","DL01")]
public void UT_005_DL01(){
CurrentRelationalBoundary.Run(5);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-006"),Trait("Dataset","DL01")]
public void UT_006_DL01(){
CurrentEfBoundary.Run(6,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-007"),Trait("Dataset","DL01")]
public void UT_007_DL01(){
CurrentEfBoundary.Run(7,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-008"),Trait("Dataset","DL01")]
public void UT_008_DL01(){
CurrentStaticBoundary.Run(8,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-008"),Trait("Dataset","DL02")]
public void UT_008_DL02(){
CurrentStaticBoundary.Run(8,"DL02");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-008"),Trait("Dataset","DL03")]
public void UT_008_DL03(){
CurrentStaticBoundary.Run(8,"DL03");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-009"),Trait("Dataset","DL01")]
public void UT_009_DL01(){
CurrentEfBoundary.Run(9,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-010"),Trait("Dataset","DL01")]
public void UT_010_DL01(){
CurrentEfBoundary.Run(10,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-011"),Trait("Dataset","DL01")]
public void UT_011_DL01(){
CurrentEfBoundary.Run(11,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-012"),Trait("Dataset","DL01")]
public void UT_012_DL01(){
CurrentEfBoundary.Run(12,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-013"),Trait("Dataset","DL01")]
public void UT_013_DL01(){
CurrentEfBoundary.Run(13,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-014"),Trait("Dataset","DL01")]
public void UT_014_DL01(){
CurrentEfBoundary.Run(14,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-015"),Trait("Dataset","DL01")]
public void UT_015_DL01(){
CurrentEfBoundary.Run(15,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-016"),Trait("Dataset","DL01")]
public void UT_016_DL01(){
CurrentEfBoundary.Run(16,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-017"),Trait("Dataset","DL01")]
public void UT_017_DL01(){
CurrentEfBoundary.Run(17,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-018"),Trait("Dataset","DL01")]
public void UT_018_DL01(){
CurrentEfBoundary.Run(18,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-019"),Trait("Dataset","DL01")]
public void UT_019_DL01(){
CurrentEfBoundary.Run(19,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-020"),Trait("Dataset","DL01")]
public void UT_020_DL01(){
CurrentEfBoundary.Run(20,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-021"),Trait("Dataset","DL01")]
public void UT_021_DL01(){
CurrentEfBoundary.Run(21,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-022"),Trait("Dataset","DL01")]
public void UT_022_DL01(){
CurrentEfBoundary.Run(22,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-023"),Trait("Dataset","DL01")]
public void UT_023_DL01(){
CurrentEfBoundary.Run(23,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-024"),Trait("Dataset","DL01")]
public void UT_024_DL01(){
CurrentEfBoundary.Run(24,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-025"),Trait("Dataset","DL01")]
public void UT_025_DL01(){
CurrentEfBoundary.Run(25,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-026"),Trait("Dataset","DL01")]
public void UT_026_DL01(){
CurrentEfBoundary.Run(26,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-027"),Trait("Dataset","DL01")]
public void UT_027_DL01(){
CurrentEfBoundary.Run(27,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-028"),Trait("Dataset","DL01")]
public void UT_028_DL01(){
CurrentEfBoundary.Run(28,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-029"),Trait("Dataset","DL01")]
public void UT_029_DL01(){
CurrentEfBoundary.Run(29,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-030"),Trait("Dataset","DL01")]
public void UT_030_DL01(){
CurrentEfBoundary.Run(30,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-031"),Trait("Dataset","DL01")]
public void UT_031_DL01(){
CurrentEfBoundary.Run(31,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-032"),Trait("Dataset","DL01")]
public void UT_032_DL01(){
CurrentEfBoundary.Run(32,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-033"),Trait("Dataset","DL01")]
public void UT_033_DL01(){
CurrentEfBoundary.Run(33,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-034"),Trait("Dataset","DL01")]
public void UT_034_DL01(){
CurrentEfBoundary.Run(34,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-035"),Trait("Dataset","DL01")]
public void UT_035_DL01(){
CurrentEfBoundary.Run(35,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-036"),Trait("Dataset","DL01")]
public void UT_036_DL01(){
CurrentEfBoundary.Run(36,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-037"),Trait("Dataset","DL01")]
public void UT_037_DL01(){
CurrentEfBoundary.Run(37,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-038"),Trait("Dataset","DL01")]
public void UT_038_DL01(){
CurrentEfBoundary.Run(38,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-039"),Trait("Dataset","DL01")]
public void UT_039_DL01(){
CurrentEfBoundary.Run(39,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-040"),Trait("Dataset","DL01")]
public void UT_040_DL01(){
CurrentEfBoundary.Run(40,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-041"),Trait("Dataset","DL01")]
public void UT_041_DL01(){
CurrentEfBoundary.Run(41,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","01"),Trait("TestCaseId","UT-042"),Trait("Dataset","DL01")]
public void UT_042_DL01(){
CurrentEfBoundary.Run(42,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","02"),Trait("TestCaseId","UT-043"),Trait("Dataset","DL01")]
public void UT_043_DL01(){
CurrentEfBoundary.Run(43,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","02"),Trait("TestCaseId","UT-044"),Trait("Dataset","DL01")]
public void UT_044_DL01(){
CurrentEfBoundary.Run(44,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","02"),Trait("TestCaseId","UT-045"),Trait("Dataset","DL01")]
public void UT_045_DL01(){
CurrentRelationalBoundary.Run(45);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","02"),Trait("TestCaseId","UT-046"),Trait("Dataset","DL01")]
public void UT_046_DL01(){
CurrentEfBoundary.Run(46,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","02"),Trait("TestCaseId","UT-047"),Trait("Dataset","DL01")]
public void UT_047_DL01(){
CurrentRelationalBoundary.Run(47);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","03"),Trait("TestCaseId","UT-048"),Trait("Dataset","DL01")]
public void UT_048_DL01(){
var events=new List<SuKienLichViewModel>{};var original=events.ToArray();var sourceOrder=events.ToArray();Call(typeof(LichTuanViewModel),"XepCot",null,events);Assert.Equal(sourceOrder,events);Assert.Equal(0,events.Count);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","03"),Trait("TestCaseId","UT-049"),Trait("Dataset","DL01")]
public void UT_049_DL01(){
var events=new List<SuKienLichViewModel>{Event(1,0,1080,1140)};var original=events.ToArray();var sourceOrder=events.ToArray();Call(typeof(LichTuanViewModel),"XepCot",null,events);Assert.Equal(sourceOrder,events);Assert.Equal(1,events.Count);Assert.Equal(0,original[0].CotTrongNgay);Assert.Equal(1,original[0].SoCot);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","03"),Trait("TestCaseId","UT-050"),Trait("Dataset","DL01")]
public void UT_050_DL01(){
var events=new List<SuKienLichViewModel>{Event(1,0,1080,1140),Event(2,0,1110,1170)};var original=events.ToArray();var sourceOrder=events.ToArray();Call(typeof(LichTuanViewModel),"XepCot",null,events);Assert.Equal(sourceOrder,events);Assert.Equal(2,events.Count);Assert.Equal(0,original[0].CotTrongNgay);Assert.Equal(2,original[0].SoCot);Assert.Equal(1,original[1].CotTrongNgay);Assert.Equal(2,original[1].SoCot);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","03"),Trait("TestCaseId","UT-051"),Trait("Dataset","DL01")]
public void UT_051_DL01(){
var events=new List<SuKienLichViewModel>{Event(1,0,1080,1140),Event(2,0,1140,1200)};var original=events.ToArray();var sourceOrder=events.ToArray();Call(typeof(LichTuanViewModel),"XepCot",null,events);Assert.Equal(sourceOrder,events);Assert.Equal(2,events.Count);Assert.Equal(0,original[0].CotTrongNgay);Assert.Equal(1,original[0].SoCot);Assert.Equal(0,original[1].CotTrongNgay);Assert.Equal(1,original[1].SoCot);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","03"),Trait("TestCaseId","UT-052"),Trait("Dataset","DL01")]
public void UT_052_DL01(){
var events=new List<SuKienLichViewModel>{Event(1,0,100,110),Event(2,0,124,130)};var original=events.ToArray();var sourceOrder=events.ToArray();Call(typeof(LichTuanViewModel),"XepCot",null,events);Assert.Equal(sourceOrder,events);Assert.Equal(2,events.Count);Assert.Equal(0,original[0].CotTrongNgay);Assert.Equal(2,original[0].SoCot);Assert.Equal(1,original[1].CotTrongNgay);Assert.Equal(2,original[1].SoCot);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","03"),Trait("TestCaseId","UT-053"),Trait("Dataset","DL01")]
public void UT_053_DL01(){
var events=new List<SuKienLichViewModel>{Event(1,0,100,110),Event(2,0,125,150)};var original=events.ToArray();var sourceOrder=events.ToArray();Call(typeof(LichTuanViewModel),"XepCot",null,events);Assert.Equal(sourceOrder,events);Assert.Equal(2,events.Count);Assert.Equal(0,original[0].CotTrongNgay);Assert.Equal(1,original[0].SoCot);Assert.Equal(0,original[1].CotTrongNgay);Assert.Equal(1,original[1].SoCot);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","03"),Trait("TestCaseId","UT-054"),Trait("Dataset","DL01")]
public void UT_054_DL01(){
var events=new List<SuKienLichViewModel>{Event(1,0,100,160),Event(2,1,100,160)};var original=events.ToArray();var sourceOrder=events.ToArray();Call(typeof(LichTuanViewModel),"XepCot",null,events);Assert.Equal(sourceOrder,events);Assert.Equal(2,events.Count);Assert.Equal(0,original[0].CotTrongNgay);Assert.Equal(1,original[0].SoCot);Assert.Equal(0,original[1].CotTrongNgay);Assert.Equal(1,original[1].SoCot);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","03"),Trait("TestCaseId","UT-055"),Trait("Dataset","DL01")]
public void UT_055_DL01(){
var events=new List<SuKienLichViewModel>{Event(1,0,1080,1140),Event(2,0,1110,1200),Event(3,0,1140,1170)};var original=events.ToArray();var sourceOrder=events.ToArray();Call(typeof(LichTuanViewModel),"XepCot",null,events);Assert.Equal(sourceOrder,events);Assert.Equal(3,events.Count);Assert.Equal(0,original[0].CotTrongNgay);Assert.Equal(2,original[0].SoCot);Assert.Equal(1,original[1].CotTrongNgay);Assert.Equal(2,original[1].SoCot);Assert.Equal(0,original[2].CotTrongNgay);Assert.Equal(2,original[2].SoCot);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","03"),Trait("TestCaseId","UT-056"),Trait("Dataset","DL01")]
public void UT_056_DL01(){
var events=new List<SuKienLichViewModel>{Event(1,0,100,160),Event(2,0,100,160)};var original=events.ToArray();events.Reverse();var sourceOrder=events.ToArray();Call(typeof(LichTuanViewModel),"XepCot",null,events);Assert.Equal(sourceOrder,events);Assert.Equal(2,events.Count);Assert.Equal(0,original[0].CotTrongNgay);Assert.Equal(2,original[0].SoCot);Assert.Equal(1,original[1].CotTrongNgay);Assert.Equal(2,original[1].SoCot);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","03"),Trait("TestCaseId","UT-057"),Trait("Dataset","DL01")]
public void UT_057_DL01(){
var now=DateTimeOffset.Parse("2026-11-24T00:00:00Z");var lich=new LichHoc{TrangThai="DaLenLich",KetThucLuc=now+TimeSpan.FromHours(1)};BuoiHoc? buoi=new(){TrangThai="DaNghi"};Assert.Equal("Đã nghỉ",SuKienLichViewModel.TrangThaiBuoi(lich,buoi,now));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","03"),Trait("TestCaseId","UT-058"),Trait("Dataset","DL01")]
public void UT_058_DL01(){
var now=DateTimeOffset.Parse("2026-11-24T00:00:00Z");var lich=new LichHoc{TrangThai="DaLenLich",KetThucLuc=now+TimeSpan.FromHours(1)};BuoiHoc? buoi=new(){TrangThai="DaHoc"};Assert.Equal("Đã học",SuKienLichViewModel.TrangThaiBuoi(lich,buoi,now));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","03"),Trait("TestCaseId","UT-059"),Trait("Dataset","DL01")]
public void UT_059_DL01(){
var now=DateTimeOffset.Parse("2026-11-24T00:00:00Z");var lich=new LichHoc{TrangThai="DaLenLich",KetThucLuc=now+TimeSpan.FromHours(1)};BuoiHoc? buoi=new(){TrangThai="DangDienRa"};Assert.Equal("Đang diễn ra",SuKienLichViewModel.TrangThaiBuoi(lich,buoi,now));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","03"),Trait("TestCaseId","UT-060"),Trait("Dataset","DL01")]
public void UT_060_DL01(){
var now=DateTimeOffset.Parse("2026-11-24T00:00:00Z");var lich=new LichHoc{TrangThai="DaLenLich",KetThucLuc=now+TimeSpan.FromHours(1)};BuoiHoc? buoi=new(){TrangThai="DaHuy"};Assert.Equal("Đã hủy",SuKienLichViewModel.TrangThaiBuoi(lich,buoi,now));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","03"),Trait("TestCaseId","UT-061"),Trait("Dataset","DL01")]
public void UT_061_DL01(){
var now=DateTimeOffset.Parse("2026-11-24T00:00:00Z");var lich=new LichHoc{TrangThai="DaHuy",KetThucLuc=now+TimeSpan.FromHours(1)};BuoiHoc? buoi=new(){TrangThai="DaHoc"};Assert.Equal("Đã hủy",SuKienLichViewModel.TrangThaiBuoi(lich,buoi,now));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","03"),Trait("TestCaseId","UT-062"),Trait("Dataset","DL01")]
public void UT_062_DL01(){
var now=DateTimeOffset.Parse("2026-11-24T00:00:00Z");var lich=new LichHoc{TrangThai="DaDieuChinh",KetThucLuc=now+TimeSpan.FromHours(1)};BuoiHoc? buoi=new(){TrangThai="DaHoc"};Assert.Equal("Đã đổi lịch",SuKienLichViewModel.TrangThaiBuoi(lich,buoi,now));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","03"),Trait("TestCaseId","UT-063"),Trait("Dataset","DL01")]
public void UT_063_DL01(){
var now=DateTimeOffset.Parse("2026-11-24T00:00:00Z");var lich=new LichHoc{TrangThai="DaLenLich",KetThucLuc=now+TimeSpan.Zero};BuoiHoc? buoi=null;Assert.Equal("Chưa ghi nhận",SuKienLichViewModel.TrangThaiBuoi(lich,buoi,now));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","03"),Trait("TestCaseId","UT-064"),Trait("Dataset","DL01")]
public void UT_064_DL01(){
var now=DateTimeOffset.Parse("2026-11-24T00:00:00Z");var lich=new LichHoc{TrangThai="DaLenLich",KetThucLuc=now+TimeSpan.FromTicks(1)};BuoiHoc? buoi=null;Assert.Equal("Dự kiến",SuKienLichViewModel.TrangThaiBuoi(lich,buoi,now));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","03"),Trait("TestCaseId","UT-065"),Trait("Dataset","DL01")]
public void UT_065_DL01(){
var now=DateTimeOffset.Parse("2026-11-24T00:00:00Z");var lich=new LichHoc{TrangThai="DaLenLich",KetThucLuc=now+TimeSpan.FromTicks(-1)};BuoiHoc? buoi=new(){TrangThai="Khac"};Assert.Equal("Chưa ghi nhận",SuKienLichViewModel.TrangThaiBuoi(lich,buoi,now));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","04"),Trait("TestCaseId","UT-066"),Trait("Dataset","DL01")]
public void UT_066_DL01(){
CurrentEfBoundary.Run(66,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","04"),Trait("TestCaseId","UT-067"),Trait("Dataset","DL01")]
public void UT_067_DL01(){
CurrentEfBoundary.Run(67,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","04"),Trait("TestCaseId","UT-068"),Trait("Dataset","DL01")]
public void UT_068_DL01(){
CurrentEfBoundary.Run(68,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","06"),Trait("TestCaseId","UT-069"),Trait("Dataset","DL01")]
public void UT_069_DL01(){
CurrentEfBoundary.Run(69,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","06"),Trait("TestCaseId","UT-070"),Trait("Dataset","DL01")]
public void UT_070_DL01(){
CurrentEfBoundary.Run(70,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","06"),Trait("TestCaseId","UT-071"),Trait("Dataset","DL01")]
public void UT_071_DL01(){
CurrentEfBoundary.Run(71,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","06"),Trait("TestCaseId","UT-072"),Trait("Dataset","DL01")]
public void UT_072_DL01(){
CurrentEfBoundary.Run(72,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","06"),Trait("TestCaseId","UT-073"),Trait("Dataset","DL01")]
public void UT_073_DL01(){
CurrentEfBoundary.Run(73,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","06"),Trait("TestCaseId","UT-074"),Trait("Dataset","DL01")]
public void UT_074_DL01(){
CurrentEfBoundary.Run(74,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","06"),Trait("TestCaseId","UT-075"),Trait("Dataset","DL01")]
public void UT_075_DL01(){
CurrentEfBoundary.Run(75,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","06"),Trait("TestCaseId","UT-076"),Trait("Dataset","DL01")]
public void UT_076_DL01(){
CurrentEfBoundary.Run(76,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","06"),Trait("TestCaseId","UT-077"),Trait("Dataset","DL01")]
public void UT_077_DL01(){
CurrentEfBoundary.Run(77,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","06"),Trait("TestCaseId","UT-078"),Trait("Dataset","DL01")]
public void UT_078_DL01(){
CurrentEfBoundary.Run(78,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","06"),Trait("TestCaseId","UT-079"),Trait("Dataset","DL01")]
public void UT_079_DL01(){
CurrentEfBoundary.Run(79,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","06"),Trait("TestCaseId","UT-080"),Trait("Dataset","DL01")]
public void UT_080_DL01(){
CurrentEfBoundary.Run(80,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","06"),Trait("TestCaseId","UT-081"),Trait("Dataset","DL01")]
public void UT_081_DL01(){
CurrentEfBoundary.Run(81,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","06"),Trait("TestCaseId","UT-082"),Trait("Dataset","DL01")]
public void UT_082_DL01(){
CurrentEfBoundary.Run(82,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","06"),Trait("TestCaseId","UT-082"),Trait("Dataset","DL02")]
public void UT_082_DL02(){
CurrentEfBoundary.Run(82,"DL02");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-083"),Trait("Dataset","DL01")]
public void UT_083_DL01(){
var c=Account(); var view=Assert.IsType<ViewResult>(c.DangKy()); Assert.Null(view.ViewName); Assert.IsType<DangKyInput>(view.Model);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-084"),Trait("Dataset","DL01")]
public void UT_084_DL01(){
var r=(IdentityResult)Call(typeof(MatKhauValidator),"Failure",null,"Mật khẩu phải có chữ số.")!; Assert.False(r.Succeeded); var e=Assert.Single(r.Errors); Assert.Equal("PasswordPolicy",e.Code); Assert.Equal("Mật khẩu phải có chữ số.",e.Description);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-085"),Trait("Dataset","DL01")]
public void UT_085_DL01(){
var r=(IdentityResult)Call(typeof(MatKhauValidator),"Failure",null,"")!; Assert.False(r.Succeeded); var e=Assert.Single(r.Errors); Assert.Equal("PasswordPolicy",e.Code); Assert.Equal("",e.Description);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-086"),Trait("Dataset","DL01")]
public void UT_086_DL01(){
var sut=new LoiTaiKhoan(); var e=sut.DuplicateEmail("a@example.vn"); Assert.Equal("DuplicateEmail",e.Code); Assert.Equal("Email đã được đăng ký.",e.Description); Assert.NotSame(e,sut.DuplicateEmail("a@example.vn"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-087"),Trait("Dataset","DL01")]
public void UT_087_DL01(){
var sut=new LoiTaiKhoan(); var e=sut.DuplicateUserName("a@example.vn"); Assert.Equal("DuplicateUserName",e.Code); Assert.Equal("Email đã được đăng ký.",e.Description); Assert.NotSame(e,sut.DuplicateUserName("a@example.vn"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-088"),Trait("Dataset","DL01")]
public void UT_088_DL01(){
var sut=new LoiTaiKhoan(); var e=sut.InvalidEmail("sai-email"); Assert.Equal("InvalidEmail",e.Code); Assert.Equal("Email chưa đúng định dạng.",e.Description); Assert.NotSame(e,sut.InvalidEmail("sai-email"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-089"),Trait("Dataset","DL01")]
public void UT_089_DL01(){
var sut=new LoiTaiKhoan(); var e=sut.InvalidEmail(null); Assert.Equal("InvalidEmail",e.Code); Assert.Equal("Email chưa đúng định dạng.",e.Description); Assert.NotSame(e,sut.InvalidEmail(null));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-090"),Trait("Dataset","DL01")]
public void UT_090_DL01(){
var sut=new LoiTaiKhoan(); var e=sut.InvalidToken(); Assert.Equal("InvalidToken",e.Code); Assert.Equal("Liên kết đã hết hạn hoặc đã được sử dụng. Hãy yêu cầu liên kết mới.",e.Description); Assert.NotSame(e,sut.InvalidToken());
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-091"),Trait("Dataset","DL01")]
public void UT_091_DL01(){
var sut=new LoiTaiKhoan(); var e=sut.ConcurrencyFailure(); Assert.Equal("ConcurrencyFailure",e.Code); Assert.Equal("Tài khoản vừa thay đổi. Vui lòng thử lại.",e.Description); Assert.NotSame(e,sut.ConcurrencyFailure());
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-092"),Trait("Dataset","DL01")]
public void UT_092_DL01(){
var c=Account(); c.HttpContext.User=new ClaimsPrincipal(new ClaimsIdentity([],"fixture")); c.Url=new UrlFixture(c.ControllerContext,false); var result=c.DangNhap((string?)"/NhuCau"); var r=Assert.IsType<RedirectToActionResult>(result); Assert.Equal("Index",r.ActionName); Assert.Equal("Home",r.ControllerName);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-093"),Trait("Dataset","DL01")]
public void UT_093_DL01(){
var c=Account(); c.HttpContext.User=new ClaimsPrincipal(); c.Url=new UrlFixture(c.ControllerContext,true); var result=c.DangNhap((string?)"/NhuCau"); var r=Assert.IsType<ViewResult>(result); Assert.Equal("/NhuCau",Assert.IsType<DangNhapInput>(r.Model).ReturnUrl);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-094"),Trait("Dataset","DL01")]
public void UT_094_DL01(){
var c=Account(); c.HttpContext.User=new ClaimsPrincipal(); c.Url=new UrlFixture(c.ControllerContext,false); var result=c.DangNhap((string?)"https://external.example"); var r=Assert.IsType<ViewResult>(result); Assert.Equal(null,Assert.IsType<DangNhapInput>(r.Model).ReturnUrl);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-095"),Trait("Dataset","DL01")]
public void UT_095_DL01(){
var c=Account(); c.HttpContext.User=new ClaimsPrincipal(); c.Url=new UrlFixture(c.ControllerContext,false); var result=c.DangNhap((string?)null); var r=Assert.IsType<ViewResult>(result); Assert.Equal(null,Assert.IsType<DangNhapInput>(r.Model).ReturnUrl);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-096"),Trait("Dataset","DL01")]
public void UT_096_DL01(){
var c=Account(); var view=Assert.IsType<ViewResult>(c.QuenMatKhau()); Assert.Null(view.ViewName); Assert.IsType<QuenMatKhauInput>(view.Model);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-097"),Trait("Dataset","DL01")]
public void UT_097_DL01(){
var c=Account(); var v=Assert.IsType<ViewResult>(c.DatLaiMatKhau("a@example.vn"!,"TOKEN1"!)); var m=Assert.IsType<DatLaiMatKhauInput>(v.Model); Assert.Equal("a@example.vn",m.Email); Assert.Equal("TOKEN1",m.Ma); Assert.Null(v.ViewName);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-098"),Trait("Dataset","DL01")]
public void UT_098_DL01(){
var c=Account(); var v=Assert.IsType<ViewResult>(c.DatLaiMatKhau(null!,null!)); var m=Assert.IsType<DatLaiMatKhauInput>(v.Model); Assert.Equal("",m.Email); Assert.Equal("",m.Ma); Assert.Null(v.ViewName);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-099"),Trait("Dataset","DL01")]
public void UT_099_DL01(){
var c=Account(); var v=Assert.IsType<ViewResult>(c.DatLaiMatKhau(null!,"TOKEN1"!)); var m=Assert.IsType<DatLaiMatKhauInput>(v.Model); Assert.Equal("",m.Email); Assert.Equal("TOKEN1",m.Ma); Assert.Null(v.ViewName);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-100"),Trait("Dataset","DL01")]
public void UT_100_DL01(){
var c=Account(); var v=Assert.IsType<ViewResult>(c.DatLaiMatKhau("a@example.vn"!,null!)); var m=Assert.IsType<DatLaiMatKhauInput>(v.Model); Assert.Equal("a@example.vn",m.Email); Assert.Equal("",m.Ma); Assert.Null(v.ViewName);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-101"),Trait("Dataset","DL01")]
public void UT_101_DL01(){
var c=Account(); var v=Assert.IsType<ViewResult>(c.KhongCoQuyen()); Assert.Equal(403,c.Response.StatusCode); Assert.Null(v.ViewName);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-102"),Trait("Dataset","DL01")]
public void UT_102_DL01(){
var c=Account(); Call(typeof(TaiKhoanController),"AddErrors",c,IdentityResult.Failed()); Assert.Empty(c.ModelState);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-103"),Trait("Dataset","DL01")]
public void UT_103_DL01(){
var c=Account(); Call(typeof(TaiKhoanController),"AddErrors",c,IdentityResult.Failed(new IdentityError{Code="1",Description="Lỗi A"},new IdentityError{Code="2",Description="Lỗi A"},new IdentityError{Description="Lỗi B"})); Assert.Equal(new[]{"Lỗi A","Lỗi B"},c.ModelState["" ]!.Errors.Select(e=>e.ErrorMessage));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-104"),Trait("Dataset","DL01")]
public void UT_104_DL01(){
var c=Account(); c.ModelState.SetModelValue("Email","a@example.vn","a@example.vn"); c.ModelState.SetModelValue("MatKhau","abc","abc");c.ModelState.AddModelError("MatKhau","E");c.ModelState.SetModelValue("XacNhanMatKhau","abc","abc");c.ModelState.AddModelError("XacNhanMatKhau","E"); Call(typeof(TaiKhoanController),"ClearPassword",c); Assert.Null(c.ModelState["MatKhau"]!.RawValue);Assert.Null(c.ModelState["MatKhau"]!.AttemptedValue);Assert.Equal("E",Assert.Single(c.ModelState["MatKhau"]!.Errors).ErrorMessage);Assert.Null(c.ModelState["XacNhanMatKhau"]!.RawValue);Assert.Null(c.ModelState["XacNhanMatKhau"]!.AttemptedValue);Assert.Equal("E",Assert.Single(c.ModelState["XacNhanMatKhau"]!.Errors).ErrorMessage); Assert.Equal(3,c.ModelState.Count); Assert.Equal("a@example.vn",c.ModelState["Email"]!.RawValue);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-105"),Trait("Dataset","DL01")]
public void UT_105_DL01(){
var c=Account(); c.ModelState.SetModelValue("Email","a@example.vn","a@example.vn"); c.ModelState.SetModelValue("MatKhau","abc","abc"); Call(typeof(TaiKhoanController),"ClearPassword",c); Assert.Null(c.ModelState["MatKhau"]!.RawValue);Assert.Null(c.ModelState["MatKhau"]!.AttemptedValue); Assert.Equal(2,c.ModelState.Count); Assert.Equal("a@example.vn",c.ModelState["Email"]!.RawValue);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-106"),Trait("Dataset","DL01")]
public void UT_106_DL01(){
var c=Account(); c.ModelState.SetModelValue("Email","a@example.vn","a@example.vn"); c.ModelState.SetModelValue("XacNhanMatKhau","abc","abc"); Call(typeof(TaiKhoanController),"ClearPassword",c); Assert.Null(c.ModelState["XacNhanMatKhau"]!.RawValue);Assert.Null(c.ModelState["XacNhanMatKhau"]!.AttemptedValue); Assert.Equal(2,c.ModelState.Count); Assert.Equal("a@example.vn",c.ModelState["Email"]!.RawValue);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-107"),Trait("Dataset","DL01")]
public void UT_107_DL01(){
var c=Account(); c.ModelState.SetModelValue("Email","a@example.vn","a@example.vn");  Call(typeof(TaiKhoanController),"ClearPassword",c);  Assert.Equal(1,c.ModelState.Count); Assert.Equal("a@example.vn",c.ModelState["Email"]!.RawValue);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-108"),Trait("Dataset","DL01")]
public void UT_108_DL01(){
var h=new DefaultHttpContext(); h.Request.Path="/NhuCau/Index";  Assert.Equal(true,(bool)Call(typeof(KiemTraCookieTaiKhoan),"IsActive",null,h.Request)!);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-109"),Trait("Dataset","DL01")]
public void UT_109_DL01(){
var h=new DefaultHttpContext(); h.Request.Path="/cap-nhat";  Assert.Equal(false,(bool)Call(typeof(KiemTraCookieTaiKhoan),"IsActive",null,h.Request)!);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-110"),Trait("Dataset","DL01")]
public void UT_110_DL01(){
var h=new DefaultHttpContext(); h.Request.Path="/cap-nhat/negotiate";  Assert.Equal(false,(bool)Call(typeof(KiemTraCookieTaiKhoan),"IsActive",null,h.Request)!);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-111"),Trait("Dataset","DL01")]
public void UT_111_DL01(){
var h=new DefaultHttpContext(); h.Request.Path="/ThongBao/ChuaDoc";  Assert.Equal(false,(bool)Call(typeof(KiemTraCookieTaiKhoan),"IsActive",null,h.Request)!);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-112"),Trait("Dataset","DL01")]
public void UT_112_DL01(){
var h=new DefaultHttpContext(); h.Request.Path="/TinNhan/DULIEU";  Assert.Equal(false,(bool)Call(typeof(KiemTraCookieTaiKhoan),"IsActive",null,h.Request)!);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-113"),Trait("Dataset","DL01")]
public void UT_113_DL01(){
var h=new DefaultHttpContext(); h.Request.Path="/NhuCau/Index"; h.Request.Headers["X-Edu-Background"]="1"; Assert.Equal(false,(bool)Call(typeof(KiemTraCookieTaiKhoan),"IsActive",null,h.Request)!);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-114"),Trait("Dataset","DL01")]
public void UT_114_DL01(){
var h=new DefaultHttpContext(); h.Request.Path="/cap-nhat-khac";  Assert.Equal(true,(bool)Call(typeof(KiemTraCookieTaiKhoan),"IsActive",null,h.Request)!);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-115"),Trait("Dataset","DL01")]
public void UT_115_DL01(){
var h=new DefaultHttpContext(); h.Request.Path="/NhuCau"; h.Request.Headers["X-Edu-Background"]=""; Assert.Equal(false,(bool)Call(typeof(KiemTraCookieTaiKhoan),"IsActive",null,h.Request)!);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-116"),Trait("Dataset","DL01")]
public void UT_116_DL01(){
var h=new DefaultHttpContext(); h.Request.Path="/TaiKhoan/HoSo";  Assert.Equal(false,(bool)Call(typeof(KiemTraCookieTaiKhoan),"IsApi",null,h.Request)!);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-117"),Trait("Dataset","DL01")]
public void UT_117_DL01(){
var h=new DefaultHttpContext(); h.Request.Path="/TaiKhoan/HoSo"; h.Request.Headers["X-Requested-With"]="XMLHttpRequest"; Assert.Equal(true,(bool)Call(typeof(KiemTraCookieTaiKhoan),"IsApi",null,h.Request)!);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-118"),Trait("Dataset","DL01")]
public void UT_118_DL01(){
var h=new DefaultHttpContext(); h.Request.Path="/cap-nhat/negotiate";  Assert.Equal(true,(bool)Call(typeof(KiemTraCookieTaiKhoan),"IsApi",null,h.Request)!);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-119"),Trait("Dataset","DL01")]
public void UT_119_DL01(){
var h=new DefaultHttpContext(); h.Request.Path="/TaiKhoan/HoSo"; h.Request.Headers["X-Requested-With"]="xmlhttprequest"; Assert.Equal(false,(bool)Call(typeof(KiemTraCookieTaiKhoan),"IsApi",null,h.Request)!);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-120"),Trait("Dataset","DL01")]
public void UT_120_DL01(){
var h=new DefaultHttpContext(); h.Request.Path="/cap-nhat-khac";  Assert.Equal(false,(bool)Call(typeof(KiemTraCookieTaiKhoan),"IsApi",null,h.Request)!);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-121"),Trait("Dataset","DL01")]
public void UT_121_DL01(){
CurrentStaticBoundary.Run(121,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-122"),Trait("Dataset","DL01")]
public void UT_122_DL01(){
CurrentStaticBoundary.Run(122,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-123"),Trait("Dataset","DL01")]
public void UT_123_DL01(){
CurrentStaticBoundary.Run(123,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-124"),Trait("Dataset","DL01")]
public void UT_124_DL01(){
CurrentStaticBoundary.Run(124,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-125"),Trait("Dataset","DL01")]
public void UT_125_DL01(){
CurrentStaticBoundary.Run(125,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-126"),Trait("Dataset","DL01")]
public void UT_126_DL01(){
CurrentStaticBoundary.Run(126,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-127"),Trait("Dataset","DL01")]
public void UT_127_DL01(){
CurrentStaticBoundary.Run(127,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-128"),Trait("Dataset","DL01")]
public void UT_128_DL01(){
CurrentStaticBoundary.Run(128,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-129"),Trait("Dataset","DL01")]
public void UT_129_DL01(){
CurrentStaticBoundary.Run(129,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-130"),Trait("Dataset","DL01")]
public void UT_130_DL01(){
CurrentStaticBoundary.Run(130,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-131"),Trait("Dataset","DL01")]
public void UT_131_DL01(){
CurrentStaticBoundary.Run(131,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-132"),Trait("Dataset","DL01")]
public void UT_132_DL01(){
CurrentStaticBoundary.Run(132,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-133"),Trait("Dataset","DL01")]
public void UT_133_DL01(){
var r=(IdentityResult)Call(typeof(TaiKhoanService),"InvalidLink",null)!; Assert.False(r.Succeeded); Assert.Equal("Liên kết không hợp lệ hoặc đã hết hạn. Hãy yêu cầu liên kết mới.",Assert.Single(r.Errors).Description);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-134"),Trait("Dataset","DL01")]
public void UT_134_DL01(){
Assert.Equal("",HoSoGiaSuService.ChuanHoa(null));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-135"),Trait("Dataset","DL01")]
public void UT_135_DL01(){
Assert.Equal("",HoSoGiaSuService.ChuanHoa(""));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-136"),Trait("Dataset","DL01")]
public void UT_136_DL01(){
Assert.Equal("",HoSoGiaSuService.ChuanHoa("  \t\n  "));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-137"),Trait("Dataset","DL01")]
public void UT_137_DL01(){
Assert.Equal("Nguyễn Văn An",HoSoGiaSuService.ChuanHoa("  Nguyễn\tVăn\nAn  "));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-137"),Trait("Dataset","DL02")]
public void UT_137_DL02(){
Assert.Equal("Nguyễn Văn An",HoSoGiaSuService.ChuanHoa("Nguyễn Văn An"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-138"),Trait("Dataset","DL01")]
public void UT_138_DL01(){
Assert.Equal(new[]{"Lớp 8","Lớp 9","Lớp 8"},HoSoLuaChon.LopDay(" Lớp 8, ,Lớp 9,, Lớp 8 "));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-139"),Trait("Dataset","DL01")]
public void UT_139_DL01(){
Assert.Equal(Array.Empty<string>(),HoSoLuaChon.LopDay(null));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-140"),Trait("Dataset","DL01")]
public void UT_140_DL01(){
Assert.Equal(Array.Empty<string>(),HoSoLuaChon.LopDay(""));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-141"),Trait("Dataset","DL01")]
public void UT_141_DL01(){
Assert.Equal(Array.Empty<string>(),HoSoLuaChon.LopDay(", , ,"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-142"),Trait("Dataset","DL01")]
public void UT_142_DL01(){
var c=Account(); var view=Assert.IsType<ViewResult>(c.DoiMatKhau()); Assert.Null(view.ViewName); Assert.IsType<DoiMatKhauInput>(view.Model);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-143"),Trait("Dataset","DL01")]
public void UT_143_DL01(){
var sut=new BangCapService(null!,null!,null!);var e=Assert.ThrowsAsync<LoiNghiepVu>(()=>sut.TaiLenAsync([])).GetAwaiter().GetResult();Assert.Equal(400,e.Status);Assert.Equal("Thêm bằng cấp bằng nút Lưu hồ sơ để kiểm tra trạng thái và phiên bản.",e.Message);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-144"),Trait("Dataset","DL01")]
public void UT_144_DL01(){
var sut=new BangCapService(null!,null!,null!);var e=Assert.ThrowsAsync<LoiNghiepVu>(()=>sut.XoaAsync(Guid.NewGuid())).GetAwaiter().GetResult();Assert.Equal(400,e.Status);Assert.Equal("Chọn xóa bằng cấp trong hồ sơ rồi bấm Lưu hồ sơ.",e.Message);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-145"),Trait("Dataset","DL01")]
public void UT_145_DL01(){
var dict=(Dictionary<string,string[]>)typeof(DiaGioiHanhChinh).GetField("DanhMuc",BindingFlags.Static|BindingFlags.NonPublic)!.GetValue(null)!;var saved=dict.ToArray();try{dict.Clear();dict["Tỉnh A"]=["Xã A1","Xã A2"];Assert.Equal(new[]{"Xã A1","Xã A2"},DiaGioiHanhChinh.PhuongXa("Tỉnh A"));}finally{dict.Clear();foreach(var p in saved)dict.Add(p.Key,p.Value);}
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-146"),Trait("Dataset","DL01")]
public void UT_146_DL01(){
var dict=(Dictionary<string,string[]>)typeof(DiaGioiHanhChinh).GetField("DanhMuc",BindingFlags.Static|BindingFlags.NonPublic)!.GetValue(null)!;var saved=dict.ToArray();try{dict.Clear();dict["Tỉnh A"]=["Xã A1","Xã A2"];Assert.Equal(Array.Empty<string>(),DiaGioiHanhChinh.PhuongXa(null));}finally{dict.Clear();foreach(var p in saved)dict.Add(p.Key,p.Value);}
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-147"),Trait("Dataset","DL01")]
public void UT_147_DL01(){
var dict=(Dictionary<string,string[]>)typeof(DiaGioiHanhChinh).GetField("DanhMuc",BindingFlags.Static|BindingFlags.NonPublic)!.GetValue(null)!;var saved=dict.ToArray();try{dict.Clear();dict["Tỉnh A"]=["Xã A1","Xã A2"];Assert.Equal(Array.Empty<string>(),DiaGioiHanhChinh.PhuongXa("Tỉnh B"));}finally{dict.Clear();foreach(var p in saved)dict.Add(p.Key,p.Value);}
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-148"),Trait("Dataset","DL01")]
public void UT_148_DL01(){
var dict=(Dictionary<string,string[]>)typeof(DiaGioiHanhChinh).GetField("DanhMuc",BindingFlags.Static|BindingFlags.NonPublic)!.GetValue(null)!;var saved=dict.ToArray();try{dict.Clear();dict["Tỉnh A"]=["Xã A1","Xã A2"];Assert.Equal(Array.Empty<string>(),DiaGioiHanhChinh.PhuongXa(""));}finally{dict.Clear();foreach(var p in saved)dict.Add(p.Key,p.Value);}
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-149"),Trait("Dataset","DL01")]
public void UT_149_DL01(){
Assert.Equal(1,MonHocCatalog.ThuTuHienThi("TIẾNG VIỆT"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-150"),Trait("Dataset","DL01")]
public void UT_150_DL01(){
Assert.Equal(0,MonHocCatalog.ThuTuHienThi("Toán Học"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-151"),Trait("Dataset","DL01")]
public void UT_151_DL01(){
Assert.Equal(8,MonHocCatalog.ThuTuHienThi("GDCD"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-152"),Trait("Dataset","DL01")]
public void UT_152_DL01(){
Assert.Equal(99,MonHocCatalog.ThuTuHienThi("Cờ vua"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-153"),Trait("Dataset","DL01")]
public void UT_153_DL01(){
Assert.Equal(0,MonHocCatalog.ThuTuHienThi("TOÁN HỌC"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","07"),Trait("TestCaseId","UT-154"),Trait("Dataset","DL01")]
public void UT_154_DL01(){
Assert.Equal(99,MonHocCatalog.ThuTuHienThi(" Toán Học "));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-155"),Trait("Dataset","DL01")]
public void UT_155_DL01(){
var vm=new TaoNhuCauBenHocViewModel{MaNhuCau=Guid.NewGuid(),MaThaoTac=Guid.NewGuid(),MonHocIds=[2,1],MonHocId=9,ChonMonKhac=true,TenMonKhac="Tin học",KhoiLop="Lớp 8",HocLuc="Khá",MucTieu="Ôn Toán",SoBuoiTuan=2,HinhThucHoc="TrucTiep",KhuVuc="Hà Nội",PhuongXa="Phường Ba Đình",DiaChiChiTiet="Số 1",HocPhiDuKien=100000,DonViHocPhi="VND/Gio",TenNguoiHoc="An",TuoiNguoiHoc=14,BanNhap=true,LichRanh=[new(){ThuTrongTuan=1,GioBatDau=new(18,0),GioKetThuc=new(19,0)}]};var result=vm.ToInput();Assert.Equal(vm.MaNhuCau,result.Id);Assert.Equal(vm.MaThaoTac,result.MaThaoTac);foreach(var name in new[]{"MonHocId","ChonMonKhac","TenMonKhac","KhoiLop","HocLuc","MucTieu","SoBuoiTuan","HinhThucHoc","KhuVuc","PhuongXa","DiaChiChiTiet","HocPhiDuKien","DonViHocPhi","TenNguoiHoc","TuoiNguoiHoc","BanNhap"})Assert.Equal(vm.GetType().GetProperty(name)!.GetValue(vm),result.GetType().GetProperty(name)!.GetValue(result));Assert.NotSame(vm.MonHocIds,result.MonHocIds);Assert.NotSame(vm.LichRanh,result.LichRanh);Assert.Equal(vm.LichRanh,result.LichRanh);Assert.Equal(new[]{2,1},result.MonHocIds);Assert.Equal(null,result.RowVersion);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-156"),Trait("Dataset","DL01")]
public void UT_156_DL01(){
var vm=new TaoNhuCauBenHocViewModel{MaNhuCau=Guid.NewGuid(),MaThaoTac=Guid.NewGuid(),MonHocIds=[2,1],MonHocId=9,ChonMonKhac=true,TenMonKhac="Tin học",KhoiLop="Lớp 8",HocLuc="Khá",MucTieu="Ôn Toán",SoBuoiTuan=2,HinhThucHoc="TrucTiep",KhuVuc="Hà Nội",PhuongXa="Phường Ba Đình",DiaChiChiTiet="Số 1",HocPhiDuKien=100000,DonViHocPhi="VND/Gio",TenNguoiHoc="An",TuoiNguoiHoc=14,BanNhap=true,LichRanh=[new(){ThuTrongTuan=1,GioBatDau=new(18,0),GioKetThuc=new(19,0)}]};vm.RowVersion=[1,2,3,4,5,6,7,8];vm.MonHocIds=[];vm.MonHocId=7;var result=vm.ToInput();Assert.Equal(vm.MaNhuCau,result.Id);Assert.Equal(vm.MaThaoTac,result.MaThaoTac);foreach(var name in new[]{"MonHocId","ChonMonKhac","TenMonKhac","KhoiLop","HocLuc","MucTieu","SoBuoiTuan","HinhThucHoc","KhuVuc","PhuongXa","DiaChiChiTiet","HocPhiDuKien","DonViHocPhi","TenNguoiHoc","TuoiNguoiHoc","BanNhap"})Assert.Equal(vm.GetType().GetProperty(name)!.GetValue(vm),result.GetType().GetProperty(name)!.GetValue(result));Assert.NotSame(vm.MonHocIds,result.MonHocIds);Assert.NotSame(vm.LichRanh,result.LichRanh);Assert.Equal(vm.LichRanh,result.LichRanh);Assert.Equal(new[]{7},result.MonHocIds);Assert.Equal("AQIDBAUGBwg=",result.RowVersion);Assert.Equal(new byte[]{1,2,3,4,5,6,7,8},vm.RowVersion);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-157"),Trait("Dataset","DL01")]
public void UT_157_DL01(){
var vm=new TaoNhuCauBenHocViewModel{MaNhuCau=Guid.NewGuid(),MaThaoTac=Guid.NewGuid(),MonHocIds=[2,1],MonHocId=9,ChonMonKhac=true,TenMonKhac="Tin học",KhoiLop="Lớp 8",HocLuc="Khá",MucTieu="Ôn Toán",SoBuoiTuan=2,HinhThucHoc="TrucTiep",KhuVuc="Hà Nội",PhuongXa="Phường Ba Đình",DiaChiChiTiet="Số 1",HocPhiDuKien=100000,DonViHocPhi="VND/Gio",TenNguoiHoc="An",TuoiNguoiHoc=14,BanNhap=true,LichRanh=[new(){ThuTrongTuan=1,GioBatDau=new(18,0),GioKetThuc=new(19,0)}]};vm.MonHocIds=[];vm.MonHocId=0;vm.LichRanh=[];var result=vm.ToInput();Assert.Equal(vm.MaNhuCau,result.Id);Assert.Equal(vm.MaThaoTac,result.MaThaoTac);foreach(var name in new[]{"MonHocId","ChonMonKhac","TenMonKhac","KhoiLop","HocLuc","MucTieu","SoBuoiTuan","HinhThucHoc","KhuVuc","PhuongXa","DiaChiChiTiet","HocPhiDuKien","DonViHocPhi","TenNguoiHoc","TuoiNguoiHoc","BanNhap"})Assert.Equal(vm.GetType().GetProperty(name)!.GetValue(vm),result.GetType().GetProperty(name)!.GetValue(result));Assert.NotSame(vm.MonHocIds,result.MonHocIds);Assert.NotSame(vm.LichRanh,result.LichRanh);Assert.Equal(vm.LichRanh,result.LichRanh);Assert.Equal(Array.Empty<int>(),result.MonHocIds);Assert.Equal(null,result.RowVersion);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-158"),Trait("Dataset","DL01")]
public void UT_158_DL01(){
var c=new BaseProbe();c.ModelState.AddModelError("MucTieu","Thiếu mục tiêu");c.ModelState.AddModelError("SoBuoiTuan","Số buổi sai");var e=Assert.Throws<LoiNghiepVu>(c.Validate);Assert.Equal(400,e.Status);Assert.Equal("Thiếu mục tiêu · Số buổi sai",e.Message);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-159"),Trait("Dataset","DL01")]
public void UT_159_DL01(){
var c=new BaseProbe();c.ModelState.AddModelError("x",new Exception("secret"),new EmptyModelMetadataProvider().GetMetadataForType(typeof(string)));var e=Assert.Throws<LoiNghiepVu>(c.Validate);Assert.Equal(400,e.Status);Assert.Equal("Dữ liệu nhập chưa đúng định dạng.",e.Message);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-160"),Trait("Dataset","DL01")]
public void UT_160_DL01(){
var c=new BaseProbe();c.Validate();Assert.Empty(c.ModelState);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-161"),Trait("Dataset","DL01")]
public void UT_161_DL01(){
var c=new BaseProbe();c.TempData=new TempDataDictionary(new DefaultHttpContext(),new TempProvider());c.TempData["Success"]="Cũ";c.TempData["Other"]=42;c.Save("Đã lưu nhu cầu.");Assert.Equal(42,c.TempData["Other"]);Assert.Equal("Đã lưu nhu cầu.",c.TempData["Success"]);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-162"),Trait("Dataset","DL01")]
public void UT_162_DL01(){
var c=new BaseProbe();c.TempData=new TempDataDictionary(new DefaultHttpContext(),new TempProvider());c.Save();Assert.Equal("Đã lưu thay đổi.",c.TempData["Success"]);Assert.Single(c.TempData);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-163"),Trait("Dataset","DL01")]
public void UT_163_DL01(){
var sut=new PhienCapNhat();var field=typeof(PhienCapNhat).GetField("connections",BindingFlags.Instance|BindingFlags.NonPublic)!;var dict=(System.Collections.Concurrent.ConcurrentDictionary<string,PhienCapNhat.KetNoi>)field.GetValue(sut)!;var c1=new PhienCapNhat.KetNoi("C1","S",Guid.Empty,Guid.Empty,()=>{});var c2=c1 with{Id="C2"};Assert.Equal("hoithoai:000000000000000000000000000000ab:C1",c1.NhomHoiThoai(Guid.Parse("00000000-0000-0000-0000-0000000000ab")));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-164"),Trait("Dataset","DL01")]
public void UT_164_DL01(){
var sut=new PhienCapNhat();var field=typeof(PhienCapNhat).GetField("connections",BindingFlags.Instance|BindingFlags.NonPublic)!;var dict=(System.Collections.Concurrent.ConcurrentDictionary<string,PhienCapNhat.KetNoi>)field.GetValue(sut)!;var c1=new PhienCapNhat.KetNoi("C1","S",Guid.Empty,Guid.Empty,()=>{});var c2=c1 with{Id="C2"};dict["C1"]=c1;Assert.Same(c1,sut.Lay("C1"));Assert.Single(dict);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-165"),Trait("Dataset","DL01")]
public void UT_165_DL01(){
var sut=new PhienCapNhat();var field=typeof(PhienCapNhat).GetField("connections",BindingFlags.Instance|BindingFlags.NonPublic)!;var dict=(System.Collections.Concurrent.ConcurrentDictionary<string,PhienCapNhat.KetNoi>)field.GetValue(sut)!;var c1=new PhienCapNhat.KetNoi("C1","S",Guid.Empty,Guid.Empty,()=>{});var c2=c1 with{Id="C2"};Assert.Null(sut.Lay("C2"));Assert.Empty(dict);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-166"),Trait("Dataset","DL01")]
public void UT_166_DL01(){
var sut=new PhienCapNhat();var field=typeof(PhienCapNhat).GetField("connections",BindingFlags.Instance|BindingFlags.NonPublic)!;var dict=(System.Collections.Concurrent.ConcurrentDictionary<string,PhienCapNhat.KetNoi>)field.GetValue(sut)!;var c1=new PhienCapNhat.KetNoi("C1","S",Guid.Empty,Guid.Empty,()=>{});var c2=c1 with{Id="C2"};Assert.Empty(sut.DanhSach());
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-167"),Trait("Dataset","DL01")]
public void UT_167_DL01(){
var sut=new PhienCapNhat();var field=typeof(PhienCapNhat).GetField("connections",BindingFlags.Instance|BindingFlags.NonPublic)!;var dict=(System.Collections.Concurrent.ConcurrentDictionary<string,PhienCapNhat.KetNoi>)field.GetValue(sut)!;var c1=new PhienCapNhat.KetNoi("C1","S",Guid.Empty,Guid.Empty,()=>{});var c2=c1 with{Id="C2"};dict["C1"]=c1;dict["C2"]=c2;var result=sut.DanhSach();Assert.Equal(2,result.Length);Assert.Contains(result,x=>ReferenceEquals(x,c1));Assert.Contains(result,x=>ReferenceEquals(x,c2));dict["C3"]=c1 with{Id="C3"};Assert.Equal(2,result.Length);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-168"),Trait("Dataset","DL01")]
public void UT_168_DL01(){
var model=new RequiredModel{Ten=""}; var e=Assert.Throws<LoiNghiepVu>(()=>Rules.ValidateInput(model)); Assert.Equal(400,e.Status); Assert.Equal("Nhập tên.",e.Message); Assert.Equal("Ten",e.Truong);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-169"),Trait("Dataset","DL01")]
public void UT_169_DL01(){
var model=new Validatable(new ValidationResult("E1",["Ten"]),new ValidationResult("E2",["Khac"])); var e=Assert.Throws<LoiNghiepVu>(()=>Rules.ValidateInput(model)); Assert.Equal(400,e.Status); Assert.Equal("E1",e.Message); Assert.Equal("Ten",e.Truong);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-170"),Trait("Dataset","DL01")]
public void UT_170_DL01(){
var model=new Validatable(new ValidationResult("E0")); var e=Assert.Throws<LoiNghiepVu>(()=>Rules.ValidateInput(model)); Assert.Equal(400,e.Status); Assert.Equal("E0",e.Message); Assert.Equal(null,e.Truong);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-171"),Trait("Dataset","DL01")]
public void UT_171_DL01(){
var model=new Validatable(new ValidationResult(null,["Ten"])); var e=Assert.Throws<LoiNghiepVu>(()=>Rules.ValidateInput(model)); Assert.Equal(400,e.Status); Assert.Equal("Dữ liệu nhập chưa hợp lệ.",e.Message); Assert.Equal("Ten",e.Truong);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-172"),Trait("Dataset","DL01")]
public void UT_172_DL01(){
var m=new RequiredModel{Ten="Hợp lệ"}; Rules.ValidateInput(m); Assert.Equal("Hợp lệ",m.Ten);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-173"),Trait("Dataset","DL01")]
public void UT_173_DL01(){
Exception failure=new InvalidOperationException("fixture"); Assert.Equal(500,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-174"),Trait("Dataset","DL01")]
public void UT_174_DL01(){
Exception failure=SqlFailure(1205); Assert.Equal(409,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-174"),Trait("Dataset","DL02")]
public void UT_174_DL02(){
Exception failure=SqlFailure(2601); Assert.Equal(409,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-174"),Trait("Dataset","DL03")]
public void UT_174_DL03(){
Exception failure=SqlFailure(2627); Assert.Equal(409,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-174"),Trait("Dataset","DL04")]
public void UT_174_DL04(){
Exception failure=SqlFailure(51033); Assert.Equal(409,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-174"),Trait("Dataset","DL05")]
public void UT_174_DL05(){
Exception failure=SqlFailure(51034); Assert.Equal(409,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-174"),Trait("Dataset","DL06")]
public void UT_174_DL06(){
Exception failure=SqlFailure(51037); Assert.Equal(409,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-174"),Trait("Dataset","DL07")]
public void UT_174_DL07(){
Exception failure=SqlFailure(51038); Assert.Equal(409,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-174"),Trait("Dataset","DL08")]
public void UT_174_DL08(){
Exception failure=SqlFailure(51039); Assert.Equal(409,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-174"),Trait("Dataset","DL09")]
public void UT_174_DL09(){
Exception failure=SqlFailure(51138); Assert.Equal(409,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-174"),Trait("Dataset","DL10")]
public void UT_174_DL10(){
Exception failure=SqlFailure(51141); Assert.Equal(409,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-174"),Trait("Dataset","DL11")]
public void UT_174_DL11(){
Exception failure=SqlFailure(51144); Assert.Equal(409,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-174"),Trait("Dataset","DL12")]
public void UT_174_DL12(){
Exception failure=SqlFailure(51145); Assert.Equal(409,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-174"),Trait("Dataset","DL13")]
public void UT_174_DL13(){
Exception failure=SqlFailure(51147); Assert.Equal(409,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-174"),Trait("Dataset","DL14")]
public void UT_174_DL14(){
Exception failure=SqlFailure(51149); Assert.Equal(409,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-174"),Trait("Dataset","DL15")]
public void UT_174_DL15(){
Exception failure=SqlFailure(51150); Assert.Equal(409,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-174"),Trait("Dataset","DL16")]
public void UT_174_DL16(){
Exception failure=SqlFailure(51152); Assert.Equal(409,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-174"),Trait("Dataset","DL17")]
public void UT_174_DL17(){
Exception failure=SqlFailure(51162); Assert.Equal(409,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-174"),Trait("Dataset","DL18")]
public void UT_174_DL18(){
Exception failure=SqlFailure(51163); Assert.Equal(409,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-175"),Trait("Dataset","DL01")]
public void UT_175_DL01(){
Exception failure=SqlFailure(51032); Assert.Equal(404,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-175"),Trait("Dataset","DL02")]
public void UT_175_DL02(){
Exception failure=SqlFailure(51035); Assert.Equal(404,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-176"),Trait("Dataset","DL01")]
public void UT_176_DL01(){
Exception failure=SqlFailure(51140); Assert.Equal(403,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-176"),Trait("Dataset","DL02")]
public void UT_176_DL02(){
Exception failure=SqlFailure(51146); Assert.Equal(403,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-176"),Trait("Dataset","DL03")]
public void UT_176_DL03(){
Exception failure=SqlFailure(51160); Assert.Equal(403,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-176"),Trait("Dataset","DL04")]
public void UT_176_DL04(){
Exception failure=SqlFailure(51161); Assert.Equal(403,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-177"),Trait("Dataset","DL01")]
public void UT_177_DL01(){
Exception failure=SqlFailure(51999); Assert.Equal(400,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-178"),Trait("Dataset","DL01")]
public void UT_178_DL01(){
Exception failure=new Exception("wrapper",SqlFailure(51032)); Assert.Equal(404,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-179"),Trait("Dataset","DL01")]
public void UT_179_DL01(){
Exception failure=new Exception("wrapper",new InvalidOperationException("inner")); Assert.Equal(500,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-180"),Trait("Dataset","DL01")]
public void UT_180_DL01(){
Exception failure=SqlFailure(50999); Assert.Equal(500,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-181"),Trait("Dataset","DL01")]
public void UT_181_DL01(){
Exception failure=SqlFailure(51000); Assert.Equal(400,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-181"),Trait("Dataset","DL02")]
public void UT_181_DL02(){
Exception failure=SqlFailure(51001); Assert.Equal(400,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-182"),Trait("Dataset","DL01")]
public void UT_182_DL01(){
Exception failure=new LoiNghiepVu("fixture",418); Assert.Equal(418,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-183"),Trait("Dataset","DL01")]
public void UT_183_DL01(){
Exception failure=new DbUpdateConcurrencyException("fixture"); Assert.Equal(409,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-184"),Trait("Dataset","DL01")]
public void UT_184_DL01(){
Exception failure=new Exception("wrapper",new LoiNghiepVu("inner",403)); Assert.Equal(500,Rules.ErrorStatus(failure));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-185"),Trait("Dataset","DL01")]
public void UT_185_DL01(){
var e=Assert.Throws<LoiNghiepVu>(()=>Rules.Check(false,"Lỗi fixture",409,"Ten")); Assert.Equal(409,e.Status); Assert.Equal("Lỗi fixture",e.Message); Assert.Equal("Ten",e.Truong);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-186"),Trait("Dataset","DL01")]
public void UT_186_DL01(){
var e=Assert.Throws<LoiNghiepVu>(()=>Rules.Check(false,"Lỗi mặc định")); Assert.Equal(400,e.Status); Assert.Equal("Lỗi mặc định",e.Message); Assert.Equal(null,e.Truong);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-187"),Trait("Dataset","DL01")]
public void UT_187_DL01(){
Rules.Check(true,"Lỗi fixture",409,"Ten");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-188"),Trait("Dataset","DL01")]
public void UT_188_DL01(){
var value=new DateTime(2026,11,24,18,30,0,DateTimeKind.Unspecified); var result=Rules.Local(value); Assert.Equal(DateTimeOffset.Parse("2026-11-24T18:30:00+07:00"),result); Assert.Equal(TimeSpan.FromHours(7),result.Offset); Assert.Equal(18,result.Hour);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-189"),Trait("Dataset","DL01")]
public void UT_189_DL01(){
var value=new DateTime(2026,11,24,18,30,0,DateTimeKind.Utc); var result=Rules.Local(value); Assert.Equal(DateTimeOffset.Parse("2026-11-24T18:30:00+07:00"),result); Assert.Equal(TimeSpan.FromHours(7),result.Offset); Assert.Equal(18,result.Hour);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-190"),Trait("Dataset","DL01")]
public void UT_190_DL01(){
var value=new DateTime(2026,11,24,18,30,0,DateTimeKind.Local); var result=Rules.Local(value); Assert.Equal(DateTimeOffset.Parse("2026-11-24T18:30:00+07:00"),result); Assert.Equal(TimeSpan.FromHours(7),result.Offset); Assert.Equal(18,result.Hour);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-191"),Trait("Dataset","DL01")]
public void UT_191_DL01(){
Assert.Equal("24/11/2026 18:30",Rules.Date(DateTimeOffset.Parse("2026-11-24T18:30:00+07:00")));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-192"),Trait("Dataset","DL01")]
public void UT_192_DL01(){
Assert.Equal("01/01/2027 03:00",Rules.Date(DateTimeOffset.Parse("2026-12-31T20:00:00+00:00")));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-193"),Trait("Dataset","DL01")]
public void UT_193_DL01(){
Assert.Equal("Chưa xác định",Rules.Fee(null,"VND/Gio"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-194"),Trait("Dataset","DL01")]
public void UT_194_DL01(){
Assert.Equal("100.000 đ / giờ",Rules.Fee(100000m,"VND/Gio"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-195"),Trait("Dataset","DL01")]
public void UT_195_DL01(){
Assert.Equal("100.000 đ / buổi",Rules.Fee(100000m,"VND/Buoi"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-196"),Trait("Dataset","DL01")]
public void UT_196_DL01(){
Assert.Equal("0 đ / giờ",Rules.Fee(0m,"VND/Gio"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-197"),Trait("Dataset","DL01")]
public void UT_197_DL01(){
Assert.Equal("100.000 đ / giờ",Rules.Fee(100000m,null));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-198"),Trait("Dataset","DL01")]
public void UT_198_DL01(){
Assert.Equal("1.235 đ / giờ",Rules.Fee(1234.56m,"VND/Gio"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-199"),Trait("Dataset","DL01")]
public void UT_199_DL01(){
Assert.Equal("UNIT_UNKNOWN", Rules.Label("UNIT_UNKNOWN"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-199"),Trait("Dataset","DL02")]
public void UT_199_DL02(){
Assert.Equal("", Rules.Label(""));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-200"),Trait("Dataset","DL01")]
public void UT_200_DL01(){
Assert.Equal("Hồ sơ", Rules.Label("HoSo"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-201"),Trait("Dataset","DL01")]
public void UT_201_DL01(){
Assert.Equal("Dừng", Rules.Label("Dung"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-202"),Trait("Dataset","DL01")]
public void UT_202_DL01(){
Assert.Equal("Đã xử lý vi phạm", Rules.Label("XuLy"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-203"),Trait("Dataset","DL01")]
public void UT_203_DL01(){
Assert.Equal("Duyệt hồ sơ", Rules.Label("Duyet"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-204"),Trait("Dataset","DL01")]
public void UT_204_DL01(){
Assert.Equal("Đã học", Rules.Label("DaHoc"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-205"),Trait("Dataset","DL01")]
public void UT_205_DL01(){
Assert.Equal("Chưa đủ căn cứ xử lý", Rules.Label("BacBo"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-206"),Trait("Dataset","DL01")]
public void UT_206_DL01(){
Assert.Equal("Trực tuyến và trực tiếp", Rules.Label("CaHai"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-207"),Trait("Dataset","DL01")]
public void UT_207_DL01(){
Assert.Equal("Đã rút", Rules.Label("DaRut"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-208"),Trait("Dataset","DL01")]
public void UT_208_DL01(){
Assert.Equal("Đã hủy", Rules.Label("DaHuy"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-209"),Trait("Dataset","DL01")]
public void UT_209_DL01(){
Assert.Equal("Gia sư", Rules.Label("GiaSu"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-210"),Trait("Dataset","DL01")]
public void UT_210_DL01(){
Assert.Equal("Báo cáo vi phạm", Rules.Label("BaoCao"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-211"),Trait("Dataset","DL01")]
public void UT_211_DL01(){
Assert.Equal("Nhu cầu", Rules.Label("NhuCau"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-212"),Trait("Dataset","DL01")]
public void UT_212_DL01(){
Assert.Equal("Liên hệ và kết nối", Rules.Label("KetNoi"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-213"),Trait("Dataset","DL01")]
public void UT_213_DL01(){
Assert.Equal("Đã nghỉ", Rules.Label("DaNghi"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-214"),Trait("Dataset","DL01")]
public void UT_214_DL01(){
Assert.Equal("Dự kiến", Rules.Label("DuKien"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-215"),Trait("Dataset","DL01")]
public void UT_215_DL01(){
Assert.Equal("Bị khóa", Rules.Label("BiKhoa"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-216"),Trait("Dataset","DL01")]
public void UT_216_DL01(){
Assert.Equal("Đã xử lý", Rules.Label("DaXuLy"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-217"),Trait("Dataset","DL01")]
public void UT_217_DL01(){
Assert.Equal("Lớp học", Rules.Label("LopHoc"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-218"),Trait("Dataset","DL01")]
public void UT_218_DL01(){
Assert.Equal("Bổ sung thông tin", Rules.Label("BoSung"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-219"),Trait("Dataset","DL01")]
public void UT_219_DL01(){
Assert.Equal("Đang mở", Rules.Label("DangMo"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-220"),Trait("Dataset","DL01")]
public void UT_220_DL01(){
Assert.Equal("Đã đóng", Rules.Label("DaDong"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-221"),Trait("Dataset","DL01")]
public void UT_221_DL01(){
Assert.Equal("Từ chối", Rules.Label("TuChoi"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-222"),Trait("Dataset","DL01")]
public void UT_222_DL01(){
Assert.Equal("Hết hạn", Rules.Label("HetHan"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-223"),Trait("Dataset","DL01")]
public void UT_223_DL01(){
Assert.Equal("Phản hồi quá trình học", Rules.Label("PhanHoi"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-224"),Trait("Dataset","DL01")]
public void UT_224_DL01(){
Assert.Equal("Quản trị", Rules.Label("QuanTri"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-225"),Trait("Dataset","DL01")]
public void UT_225_DL01(){
Assert.Equal("Tạm dừng", Rules.Label("TamDung"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-226"),Trait("Dataset","DL01")]
public void UT_226_DL01(){
Assert.Equal("Tiếp tục", Rules.Label("TiepTuc"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-227"),Trait("Dataset","DL01")]
public void UT_227_DL01(){
Assert.Equal("Chờ xử lý", Rules.Label("ChoXuLy"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-228"),Trait("Dataset","DL01")]
public void UT_228_DL01(){
Assert.Equal("Tin nhắn", Rules.Label("TinNhan"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-229"),Trait("Dataset","DL01")]
public void UT_229_DL01(){
Assert.Equal("Đã duyệt", Rules.Label("DaDuyet"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-230"),Trait("Dataset","DL01")]
public void UT_230_DL01(){
Assert.Equal("Bản nháp", Rules.Label("BanNhap"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-231"),Trait("Dataset","DL01")]
public void UT_231_DL01(){
Assert.Equal("Học sinh", Rules.Label("HocSinh"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-232"),Trait("Dataset","DL01")]
public void UT_232_DL01(){
Assert.Equal("Tiếp nhận báo cáo", Rules.Label("NhanXuLy"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-233"),Trait("Dataset","DL01")]
public void UT_233_DL01(){
Assert.Equal("Nhắc việc", Rules.Label("NhacViec"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-234"),Trait("Dataset","DL01")]
public void UT_234_DL01(){
Assert.Equal("Hoạt động", Rules.Label("HoatDong"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-235"),Trait("Dataset","DL01")]
public void UT_235_DL01(){
Assert.Equal("Đang xử lý", Rules.Label("DangXuLy"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-236"),Trait("Dataset","DL01")]
public void UT_236_DL01(){
Assert.Equal("Tài khoản", Rules.Label("TaiKhoan"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-237"),Trait("Dataset","DL01")]
public void UT_237_DL01(){
Assert.Equal("Chờ duyệt", Rules.Label("ChoDuyet"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-238"),Trait("Dataset","DL01")]
public void UT_238_DL01(){
Assert.Equal("Ẩn nhu cầu", Rules.Label("AnNhuCau"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-239"),Trait("Dataset","DL01")]
public void UT_239_DL01(){
Assert.Equal("Đã chấp nhận", Rules.Label("ChapNhan"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-240"),Trait("Dataset","DL01")]
public void UT_240_DL01(){
Assert.Equal("Chờ gia sư", Rules.Label("ChoGiaSu"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-241"),Trait("Dataset","DL01")]
public void UT_241_DL01(){
Assert.Equal("Trực tiếp", Rules.Label("TrucTiep"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-242"),Trait("Dataset","DL01")]
public void UT_242_DL01(){
Assert.Equal("Phụ huynh", Rules.Label("PhuHuynh"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-243"),Trait("Dataset","DL01")]
public void UT_243_DL01(){
Assert.Equal("Thay đổi quyền quản trị", Rules.Label("PhanQuyen"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-244"),Trait("Dataset","DL01")]
public void UT_244_DL01(){
Assert.Equal("Thành công", Rules.Label("ThanhCong"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-245"),Trait("Dataset","DL01")]
public void UT_245_DL01(){
Assert.Equal("Kiểm duyệt", Rules.Label("KiemDuyet"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-246"),Trait("Dataset","DL01")]
public void UT_246_DL01(){
Assert.Equal("Lời mời vào lớp", Rules.Label("LoiMoiLop"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-247"),Trait("Dataset","DL01")]
public void UT_247_DL01(){
Assert.Equal("Thành viên", Rules.Label("ThanhVien"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-248"),Trait("Dataset","DL01")]
public void UT_248_DL01(){
Assert.Equal("Điều chỉnh", Rules.Label("DieuChinh"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-249"),Trait("Dataset","DL01")]
public void UT_249_DL01(){
Assert.Equal("Hồ sơ gia sư", Rules.Label("HoSoGiaSu"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-250"),Trait("Dataset","DL01")]
public void UT_250_DL01(){
Assert.Equal("Nhu cầu học", Rules.Label("NhuCauHoc"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-251"),Trait("Dataset","DL01")]
public void UT_251_DL01(){
Assert.Equal("Ẩn nội dung", Rules.Label("AnNoiDung"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-252"),Trait("Dataset","DL01")]
public void UT_252_DL01(){
Assert.Equal("Duyệt hồ sơ", Rules.Label("DuyetHoSo"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-253"),Trait("Dataset","DL01")]
public void UT_253_DL01(){
Assert.Equal("Xem nhật ký", Rules.Label("XemNhatKy"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-254"),Trait("Dataset","DL01")]
public void UT_254_DL01(){
Assert.Equal("Chờ bên học", Rules.Label("ChoBenHoc"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-255"),Trait("Dataset","DL01")]
public void UT_255_DL01(){
Assert.Equal("Đã kết thúc", Rules.Label("DaKetThuc"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-256"),Trait("Dataset","DL01")]
public void UT_256_DL01(){
Assert.Equal("Trực tuyến", Rules.Label("TrucTuyen"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-257"),Trait("Dataset","DL01")]
public void UT_257_DL01(){
Assert.Equal("Đã lên lịch", Rules.Label("DaLenLich"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-258"),Trait("Dataset","DL01")]
public void UT_258_DL01(){
Assert.Equal("Gỡ biện pháp", Rules.Label("GoBienPhap"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-259"),Trait("Dataset","DL01")]
public void UT_259_DL01(){
Assert.Equal("Đang diễn ra", Rules.Label("DangDienRa"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-260"),Trait("Dataset","DL01")]
public void UT_260_DL01(){
Assert.Equal("Tạm dừng lớp", Rules.Label("TamDungLop"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-261"),Trait("Dataset","DL01")]
public void UT_261_DL01(){
Assert.Equal("Xử lý báo cáo", Rules.Label("XuLyBaoCao"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-262"),Trait("Dataset","DL01")]
public void UT_262_DL01(){
Assert.Equal("Chờ phản hồi", Rules.Label("ChoPhanHoi"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-263"),Trait("Dataset","DL01")]
public void UT_263_DL01(){
Assert.Equal("Đã tạo lớp", Rules.Label("DaChapNhan"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-264"),Trait("Dataset","DL01")]
public void UT_264_DL01(){
Assert.Equal("Cấp quyền quản trị", Rules.Label("QuanLyQuyen"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-265"),Trait("Dataset","DL01")]
public void UT_265_DL01(){
Assert.Equal("Cần chỉnh sửa", Rules.Label("CanChinhSua"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-266"),Trait("Dataset","DL01")]
public void UT_266_DL01(){
Assert.Equal("Đã điều chỉnh", Rules.Label("DaDieuChinh"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-267"),Trait("Dataset","DL01")]
public void UT_267_DL01(){
Assert.Equal("Khóa tài khoản", Rules.Label("KhoaTaiKhoan"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-268"),Trait("Dataset","DL01")]
public void UT_268_DL01(){
Assert.Equal("Đề nghị xem lại", Rules.Label("DeNghiXemLai"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-269"),Trait("Dataset","DL01")]
public void UT_269_DL01(){
Assert.Equal("Đang học", Rules.Label("DangHoatDong"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-270"),Trait("Dataset","DL01")]
public void UT_270_DL01(){
Assert.Equal("Cập nhật môn học", Rules.Label("CapNhatMonHoc"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-271"),Trait("Dataset","DL01")]
public void UT_271_DL01(){
Assert.Equal("Đánh giá người dùng", Rules.Label("DanhGiaDoiTac"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-272"),Trait("Dataset","DL01")]
public void UT_272_DL01(){
Assert.Equal("Quản lý danh mục", Rules.Label("QuanLyDanhMuc"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-273"),Trait("Dataset","DL01")]
public void UT_273_DL01(){
Assert.Equal("Không thành công", Rules.Label("KhongThanhCong"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-274"),Trait("Dataset","DL01")]
public void UT_274_DL01(){
Assert.Equal("Tiếp nhận báo cáo", Rules.Label("NhanXuLyBaoCao"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-275"),Trait("Dataset","DL01")]
public void UT_275_DL01(){
Assert.Equal("Quản lý tài khoản", Rules.Label("QuanLyTaiKhoan"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-276"),Trait("Dataset","DL01")]
public void UT_276_DL01(){
Assert.Equal("Liên kết tài khoản", Rules.Label("LienKetTaiKhoan"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-277"),Trait("Dataset","DL01")]
public void UT_277_DL01(){
Assert.Equal("Thay đổi quyền quản trị", Rules.Label("PhanQuyenQuanTri"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-278"),Trait("Dataset","DL01")]
public void UT_278_DL01(){
Assert.Equal("Yêu cầu chỉnh sửa nhu cầu", Rules.Label("YeuCauChinhSuaNhuCau"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-279"),Trait("Dataset","DL01")]
public void UT_279_DL01(){
Assert.Equal("", Rules.Label(null));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-280"),Trait("Dataset","DL01")]
public void UT_280_DL01(){
CurrentStaticBoundary.Run(280,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-281"),Trait("Dataset","DL01")]
public void UT_281_DL01(){
CurrentStaticBoundary.Run(281,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-282"),Trait("Dataset","DL01")]
public void UT_282_DL01(){
CurrentRelationalBoundary.Run(282);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-283"),Trait("Dataset","DL01")]
public void UT_283_DL01(){
CurrentRelationalBoundary.Run(283);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-284"),Trait("Dataset","DL01")]
public void UT_284_DL01(){
CurrentRelationalBoundary.Run(284);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-285"),Trait("Dataset","DL01")]
public void UT_285_DL01(){
CurrentRelationalBoundary.Run(285);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-286"),Trait("Dataset","DL01")]
public void UT_286_DL01(){
var e=new LoiNghiepVu("Sai",409);var sut=(KetQua<string>)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(KetQua<string>));typeof(KetQua<string>).GetField("duLieu",BindingFlags.Instance|BindingFlags.NonPublic)!.SetValue(sut,"A");sut.BaoLoiNeuThatBai();Assert.Equal("A",Field(sut,"duLieu"));Assert.Null(Field(sut,"loi"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-287"),Trait("Dataset","DL01")]
public void UT_287_DL01(){
var e=new LoiNghiepVu("Sai",409);var sut=(KetQua<string>)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(KetQua<string>));typeof(KetQua<string>).GetField("duLieu",BindingFlags.Instance|BindingFlags.NonPublic)!.SetValue(sut,"A");typeof(KetQua<string>).GetField("loi",BindingFlags.Instance|BindingFlags.NonPublic)!.SetValue(sut,e);Assert.Same(e,Assert.Throws<LoiNghiepVu>(sut.BaoLoiNeuThatBai));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-288"),Trait("Dataset","DL01")]
public void UT_288_DL01(){
var accessor=new HttpContextAccessor{HttpContext=new DefaultHttpContext{User=new ClaimsPrincipal()}};var sut=new NguoiDungHienTaiService(null!,accessor);var e=Assert.ThrowsAsync<LoiNghiepVu>(()=>sut.HienTaiAsync()).GetAwaiter().GetResult();Assert.Equal(401,e.Status);Assert.Equal("Vui lòng đăng nhập để tiếp tục.",e.Message);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-289"),Trait("Dataset","DL01")]
public void UT_289_DL01(){
var accessor=new HttpContextAccessor{HttpContext=new DefaultHttpContext{User=new ClaimsPrincipal(new ClaimsIdentity(new[]{new Claim(ClaimTypes.NameIdentifier,"invalid-guid")},"Test"))}};var sut=new NguoiDungHienTaiService(null!,accessor);var e=Assert.ThrowsAsync<LoiNghiepVu>(()=>sut.HienTaiAsync()).GetAwaiter().GetResult();Assert.Equal(401,e.Status);Assert.Equal("Vui lòng đăng nhập để tiếp tục.",e.Message);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-290"),Trait("Dataset","DL01")]
public void UT_290_DL01(){
CurrentEfBoundary.Run(290,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-291"),Trait("Dataset","DL01")]
public void UT_291_DL01(){
var accessor=new HttpContextAccessor{HttpContext=null};var sut=new NguoiDungHienTaiService(null!,accessor);var e=Assert.ThrowsAsync<LoiNghiepVu>(()=>sut.HienTaiAsync()).GetAwaiter().GetResult();Assert.Equal(401,e.Status);Assert.Equal("Vui lòng đăng nhập để tiếp tục.",e.Message);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-292"),Trait("Dataset","DL01")]
public void UT_292_DL01(){
var accessor=new HttpContextAccessor{HttpContext=new DefaultHttpContext{User=new ClaimsPrincipal()}};var sut=new NguoiDungHienTaiService(null!,accessor);var e=Assert.ThrowsAsync<LoiNghiepVu>(()=>sut.HienTaiAsync()).GetAwaiter().GetResult();Assert.Equal(401,e.Status);Assert.Equal("Vui lòng đăng nhập để tiếp tục.",e.Message);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-293"),Trait("Dataset","DL01")]
public void UT_293_DL01(){
var accessor=new HttpContextAccessor{HttpContext=new DefaultHttpContext{User=new ClaimsPrincipal(new ClaimsIdentity([],"Test"))}};var sut=new NguoiDungHienTaiService(null!,accessor);var e=Assert.ThrowsAsync<LoiNghiepVu>(()=>sut.HienTaiAsync()).GetAwaiter().GetResult();Assert.Equal(401,e.Status);Assert.Equal("Vui lòng đăng nhập để tiếp tục.",e.Message);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-294"),Trait("Dataset","DL01")]
public void UT_294_DL01(){
CurrentEfBoundary.Run(294,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-295"),Trait("Dataset","DL01")]
public void UT_295_DL01(){
CurrentEfBoundary.Run(295,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-296"),Trait("Dataset","DL01")]
public void UT_296_DL01(){
CurrentEfBoundary.Run(296,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-297"),Trait("Dataset","DL01")]
public void UT_297_DL01(){
var sut=new NguoiDungHienTaiService(null!,null!);var e=Assert.ThrowsAsync<LoiNghiepVu>(()=>sut.ChonAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"))).GetAwaiter().GetResult();Assert.Equal(403,e.Status);Assert.Equal("Vui lòng đăng nhập bằng tài khoản của bạn.",e.Message);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-298"),Trait("Dataset","DL01")]
public void UT_298_DL01(){
CurrentAuditBoundary.Run(298);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-299"),Trait("Dataset","DL01")]
public void UT_299_DL01(){
CurrentAuditBoundary.Run(299);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-300"),Trait("Dataset","DL01")]
public void UT_300_DL01(){
var h=new DefaultHttpContext();var key=typeof(NhatKyAnToanService).GetField("EventsKey",BindingFlags.NonPublic|BindingFlags.Static)!.GetValue(null)!;var existing=new List<NhatKyAnToan>{new()};var result=(List<NhatKyAnToan>)Call(typeof(NhatKyAnToanService),"Events",null,h)!;Assert.Same(result,h.Items[key]);Assert.Empty(result);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-301"),Trait("Dataset","DL01")]
public void UT_301_DL01(){
var h=new DefaultHttpContext();var key=typeof(NhatKyAnToanService).GetField("EventsKey",BindingFlags.NonPublic|BindingFlags.Static)!.GetValue(null)!;var existing=new List<NhatKyAnToan>{new()};h.Items[key]=existing;var result=(List<NhatKyAnToan>)Call(typeof(NhatKyAnToanService),"Events",null,h)!;Assert.Same(result,h.Items[key]);Assert.Same(existing,result);Assert.Same(existing[0],Assert.Single(result));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-302"),Trait("Dataset","DL01")]
public void UT_302_DL01(){
var h=new DefaultHttpContext();var key=typeof(NhatKyAnToanService).GetField("EventsKey",BindingFlags.NonPublic|BindingFlags.Static)!.GetValue(null)!;var existing=new List<NhatKyAnToan>{new()};h.Items[key]="Sai kiểu";var result=(List<NhatKyAnToan>)Call(typeof(NhatKyAnToanService),"Events",null,h)!;Assert.Same(result,h.Items[key]);Assert.Empty(result);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-303"),Trait("Dataset","DL01")]
public void UT_303_DL01(){
var value="AB\r\n\u0000CD!XYZ"; Assert.Equal("ABCD!",Call(typeof(NhatKyAnToanService),"Clean",null,value,5)); Assert.Equal("AB\r\n\u0000CD!XYZ",value);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-303"),Trait("Dataset","DL02")]
public void UT_303_DL02(){
var value="ABC"; Assert.Equal("ABC",Call(typeof(NhatKyAnToanService),"Clean",null,value,5)); Assert.Equal("ABC",value);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-304"),Trait("Dataset","DL01")]
public void UT_304_DL01(){
var value="ABCDE"; Assert.Equal("",Call(typeof(NhatKyAnToanService),"Clean",null,value,0)); Assert.Equal("ABCDE",value);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-305"),Trait("Dataset","DL01")]
public void UT_305_DL01(){
var value="ABCDE"; Assert.Equal("",Call(typeof(NhatKyAnToanService),"Clean",null,value,-1)); Assert.Equal("ABCDE",value);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-306"),Trait("Dataset","DL01")]
public void UT_306_DL01(){
var value="ABCDE"; Assert.Equal("A",Call(typeof(NhatKyAnToanService),"Clean",null,value,1)); Assert.Equal("ABCDE",value);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-307"),Trait("Dataset","DL01")]
public void UT_307_DL01(){
var value="ABCDE"; Assert.Equal("ABCD",Call(typeof(NhatKyAnToanService),"Clean",null,value,4)); Assert.Equal("ABCDE",value);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-308"),Trait("Dataset","DL01")]
public void UT_308_DL01(){
var value="ABCDE"; Assert.Equal("ABCDE",Call(typeof(NhatKyAnToanService),"Clean",null,value,5)); Assert.Equal("ABCDE",value);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-309"),Trait("Dataset","DL01")]
public void UT_309_DL01(){
var value="ABCDE"; Assert.Equal("ABCDE",Call(typeof(NhatKyAnToanService),"Clean",null,value,6)); Assert.Equal("ABCDE",value);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-310"),Trait("Dataset","DL01")]
public void UT_310_DL01(){
var value=""; Assert.Equal("",Call(typeof(NhatKyAnToanService),"Clean",null,value,5)); Assert.Equal("",value);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-311"),Trait("Dataset","DL01")]
public void UT_311_DL01(){
var value="\r\n\t\u0000"; Assert.Equal("",Call(typeof(NhatKyAnToanService),"Clean",null,value,5)); Assert.Equal("\r\n\t\u0000",value);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-312"),Trait("Dataset","DL01")]
public void UT_312_DL01(){
var value="Học\t sinh\n"; Assert.Equal("Học sinh",Call(typeof(NhatKyAnToanService),"Clean",null,value,20)); Assert.Equal("Học\t sinh\n",value);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-313"),Trait("Dataset","DL01")]
public void UT_313_DL01(){
Assert.False((bool)Call(typeof(TepService),"Docx",null,new byte[]{0,1,2,3})!);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-313"),Trait("Dataset","DL02")]
public void UT_313_DL02(){
Assert.False((bool)Call(typeof(TepService),"Docx",null,Array.Empty<byte>())!);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-314"),Trait("Dataset","DL01")]
public void UT_314_DL01(){
var bytes=Zip(2,100,true,true,null,false); var copy=bytes.ToArray(); Assert.Equal(true,(bool)Call(typeof(TepService),"Docx",null,bytes)!); Assert.Equal(copy,bytes);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-315"),Trait("Dataset","DL01")]
public void UT_315_DL01(){
var bytes=Zip(2,1000,false,true,null,false); var copy=bytes.ToArray(); Assert.Equal(false,(bool)Call(typeof(TepService),"Docx",null,bytes)!); Assert.Equal(copy,bytes);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-316"),Trait("Dataset","DL01")]
public void UT_316_DL01(){
var bytes=Zip(2,1000,true,false,null,false); var copy=bytes.ToArray(); Assert.Equal(false,(bool)Call(typeof(TepService),"Docx",null,bytes)!); Assert.Equal(copy,bytes);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-317"),Trait("Dataset","DL01")]
public void UT_317_DL01(){
var bytes=Zip(1999,1000,true,true,null,false); var copy=bytes.ToArray(); Assert.Equal(true,(bool)Call(typeof(TepService),"Docx",null,bytes)!); Assert.Equal(copy,bytes);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-318"),Trait("Dataset","DL01")]
public void UT_318_DL01(){
var bytes=Zip(2000,1000,true,true,null,false); var copy=bytes.ToArray(); Assert.Equal(false,(bool)Call(typeof(TepService),"Docx",null,bytes)!); Assert.Equal(copy,bytes);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-319"),Trait("Dataset","DL01")]
public void UT_319_DL01(){
var bytes=Zip(2001,1000,true,true,null,false); var copy=bytes.ToArray(); Assert.Equal(false,(bool)Call(typeof(TepService),"Docx",null,bytes)!); Assert.Equal(copy,bytes);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-320"),Trait("Dataset","DL01")]
public void UT_320_DL01(){
var bytes=Zip(2,49999999,true,true,null,false); var copy=bytes.ToArray(); Assert.Equal(true,(bool)Call(typeof(TepService),"Docx",null,bytes)!); Assert.Equal(copy,bytes);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-321"),Trait("Dataset","DL01")]
public void UT_321_DL01(){
var bytes=Zip(2,50000000,true,true,null,false); var copy=bytes.ToArray(); Assert.Equal(false,(bool)Call(typeof(TepService),"Docx",null,bytes)!); Assert.Equal(copy,bytes);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-322"),Trait("Dataset","DL01")]
public void UT_322_DL01(){
var bytes=Zip(2,50000001,true,true,null,false); var copy=bytes.ToArray(); Assert.Equal(false,(bool)Call(typeof(TepService),"Docx",null,bytes)!); Assert.Equal(copy,bytes);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-323"),Trait("Dataset","DL01")]
public void UT_323_DL01(){
var bytes=Zip(2,1000,true,true,"word/vbaProject.bin",false); var copy=bytes.ToArray(); Assert.Equal(false,(bool)Call(typeof(TepService),"Docx",null,bytes)!); Assert.Equal(copy,bytes);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-324"),Trait("Dataset","DL01")]
public void UT_324_DL01(){
var bytes=Zip(2,1000,true,true,"word/VBAPROJECT.bin",false); var copy=bytes.ToArray(); Assert.Equal(false,(bool)Call(typeof(TepService),"Docx",null,bytes)!); Assert.Equal(copy,bytes);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-325"),Trait("Dataset","DL01")]
public void UT_325_DL01(){
var bytes=Zip(2,1000,true,true,"word/../extra.xml",false); var copy=bytes.ToArray(); Assert.Equal(false,(bool)Call(typeof(TepService),"Docx",null,bytes)!); Assert.Equal(copy,bytes);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-326"),Trait("Dataset","DL01")]
public void UT_326_DL01(){
var bytes=Zip(2,1000,true,true,null,true); var copy=bytes.ToArray(); Assert.Equal(false,(bool)Call(typeof(TepService),"Docx",null,bytes)!); Assert.Equal(copy,bytes);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-327"),Trait("Dataset","DL01")]
public void UT_327_DL01(){
var bytes=Zip(2,1000,true,true,"word/a..b.xml",false); var copy=bytes.ToArray(); Assert.Equal(false,(bool)Call(typeof(TepService),"Docx",null,bytes)!); Assert.Equal(copy,bytes);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-328"),Trait("Dataset","DL01")]
public void UT_328_DL01(){
var bytes=Zip(2,1000,true,true,"word/vbaProject.txt",false); var copy=bytes.ToArray(); Assert.Equal(false,(bool)Call(typeof(TepService),"Docx",null,bytes)!); Assert.Equal(copy,bytes);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-329"),Trait("Dataset","DL01")]
public void UT_329_DL01(){
TepService.DonDep(Array.Empty<string>());
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-330"),Trait("Dataset","DL01")]
public void UT_330_DL01(){
CurrentStaticBoundary.Run(330,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-331"),Trait("Dataset","DL01")]
public void UT_331_DL01(){
CurrentStaticBoundary.Run(331,"DL01");
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-332"),Trait("Dataset","DL01")]
public void UT_332_DL01(){
var h=new DefaultHttpContext();h.Request.Headers["X-Requested-With"]="XMLHttpRequest";Assert.Equal(true,UiResponseFilter.IsAjax(h.Request));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-333"),Trait("Dataset","DL01")]
public void UT_333_DL01(){
var h=new DefaultHttpContext();Assert.Equal(false,UiResponseFilter.IsAjax(h.Request));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-334"),Trait("Dataset","DL01")]
public void UT_334_DL01(){
var h=new DefaultHttpContext();h.Request.Headers["X-Requested-With"]="xmlhttprequest";Assert.Equal(false,UiResponseFilter.IsAjax(h.Request));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-335"),Trait("Dataset","DL01")]
public void UT_335_DL01(){
var sut=new VietnameseValidationMetadataProvider();var c=new DisplayMetadataProviderContext(ModelMetadataIdentity.ForProperty(typeof(string),"HoTen",typeof(RequiredModel)),Attributes([]));Func<string> old=()=>"Cũ";sut.CreateDisplayMetadata(c);Assert.Equal("Họ tên",c.DisplayMetadata.DisplayName!());
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-336"),Trait("Dataset","DL01")]
public void UT_336_DL01(){
var sut=new VietnameseValidationMetadataProvider();var c=new DisplayMetadataProviderContext(ModelMetadataIdentity.ForProperty(typeof(string),"KhongCoNhan",typeof(RequiredModel)),Attributes([]));Func<string> old=()=>"Cũ";c.DisplayMetadata.DisplayName=old;sut.CreateDisplayMetadata(c);Assert.Same(old,c.DisplayMetadata.DisplayName);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-337"),Trait("Dataset","DL01")]
public void UT_337_DL01(){
var sut=new VietnameseValidationMetadataProvider();var c=new DisplayMetadataProviderContext(ModelMetadataIdentity.ForType(typeof(RequiredModel)),Attributes([]));Func<string> old=()=>"Cũ";c.DisplayMetadata.DisplayName=old;sut.CreateDisplayMetadata(c);Assert.Same(old,c.DisplayMetadata.DisplayName);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-338"),Trait("Dataset","DL01")]
public void UT_338_DL01(){
object[] attrs=[new RequiredAttribute()];var c=new ValidationMetadataProviderContext(ModelMetadataIdentity.ForType(typeof(RequiredModel)),Attributes(attrs));new VietnameseValidationMetadataProvider().CreateValidationMetadata(c);var actual=attrs.OfType<ValidationAttribute>().ToArray();Assert.Equal(1,actual.Length);Assert.Equal("{0} không được để trống.",actual[0].ErrorMessage);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-339"),Trait("Dataset","DL01")]
public void UT_339_DL01(){
object[] attrs=[new RangeAttribute(1,9)];var c=new ValidationMetadataProviderContext(ModelMetadataIdentity.ForType(typeof(RequiredModel)),Attributes(attrs));new VietnameseValidationMetadataProvider().CreateValidationMetadata(c);var actual=attrs.OfType<ValidationAttribute>().ToArray();Assert.Equal(1,actual.Length);Assert.Equal("{0} phải từ {1} đến {2}.",actual[0].ErrorMessage);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-340"),Trait("Dataset","DL01")]
public void UT_340_DL01(){
object[] attrs=[new StringLengthAttribute(20){MinimumLength=2}];var c=new ValidationMetadataProviderContext(ModelMetadataIdentity.ForType(typeof(RequiredModel)),Attributes(attrs));new VietnameseValidationMetadataProvider().CreateValidationMetadata(c);var actual=attrs.OfType<ValidationAttribute>().ToArray();Assert.Equal(1,actual.Length);Assert.Equal("{0} phải từ {2} đến {1} ký tự.",actual[0].ErrorMessage);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-341"),Trait("Dataset","DL01")]
public void UT_341_DL01(){
object[] attrs=[new StringLengthAttribute(20)];var c=new ValidationMetadataProviderContext(ModelMetadataIdentity.ForType(typeof(RequiredModel)),Attributes(attrs));new VietnameseValidationMetadataProvider().CreateValidationMetadata(c);var actual=attrs.OfType<ValidationAttribute>().ToArray();Assert.Equal(1,actual.Length);Assert.Equal("{0} không được vượt quá {1} ký tự.",actual[0].ErrorMessage);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-342"),Trait("Dataset","DL01")]
public void UT_342_DL01(){
object[] attrs=[new CompareAttribute("Other")];var c=new ValidationMetadataProviderContext(ModelMetadataIdentity.ForType(typeof(RequiredModel)),Attributes(attrs));new VietnameseValidationMetadataProvider().CreateValidationMetadata(c);var actual=attrs.OfType<ValidationAttribute>().ToArray();Assert.Equal(1,actual.Length);Assert.Equal("{0} chưa trùng khớp.",actual[0].ErrorMessage);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-343"),Trait("Dataset","DL01")]
public void UT_343_DL01(){
object[] attrs=[new RegularExpressionAttribute(".*")];var c=new ValidationMetadataProviderContext(ModelMetadataIdentity.ForType(typeof(RequiredModel)),Attributes(attrs));new VietnameseValidationMetadataProvider().CreateValidationMetadata(c);var actual=attrs.OfType<ValidationAttribute>().ToArray();Assert.Equal(1,actual.Length);Assert.Equal("{0} chưa đúng định dạng.",actual[0].ErrorMessage);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-344"),Trait("Dataset","DL01")]
public void UT_344_DL01(){
object[] attrs=[new EmailAddressAttribute{ErrorMessage=null}];var c=new ValidationMetadataProviderContext(ModelMetadataIdentity.ForType(typeof(RequiredModel)),Attributes(attrs));new VietnameseValidationMetadataProvider().CreateValidationMetadata(c);var actual=attrs.OfType<ValidationAttribute>().ToArray();Assert.Equal(1,actual.Length);Assert.Equal("{0} chưa phải địa chỉ email hợp lệ.",actual[0].ErrorMessage);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-345"),Trait("Dataset","DL01")]
public void UT_345_DL01(){
object[] attrs=[new DataTypeAttribute(DataType.Date)];var c=new ValidationMetadataProviderContext(ModelMetadataIdentity.ForType(typeof(RequiredModel)),Attributes(attrs));new VietnameseValidationMetadataProvider().CreateValidationMetadata(c);var actual=attrs.OfType<ValidationAttribute>().ToArray();Assert.Equal(1,actual.Length);Assert.Equal("{0} chưa hợp lệ.",actual[0].ErrorMessage);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-346"),Trait("Dataset","DL01")]
public void UT_346_DL01(){
object[] attrs=[new RequiredAttribute(),new RangeAttribute(1,9),new RequiredAttribute{ErrorMessage="Riêng"},new StringLengthAttribute(20){MinimumLength=2}];var c=new ValidationMetadataProviderContext(ModelMetadataIdentity.ForType(typeof(RequiredModel)),Attributes(attrs));new VietnameseValidationMetadataProvider().CreateValidationMetadata(c);var actual=attrs.OfType<ValidationAttribute>().ToArray();Assert.Equal(4,actual.Length);Assert.Equal("{0} không được để trống.",actual[0].ErrorMessage);Assert.Equal("{0} phải từ {1} đến {2}.",actual[1].ErrorMessage);Assert.Equal("Riêng",actual[2].ErrorMessage);Assert.Equal("{0} phải từ {2} đến {1} ký tự.",actual[3].ErrorMessage);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-347"),Trait("Dataset","DL01")]
public void UT_347_DL01(){
object[] attrs=[];var c=new ValidationMetadataProviderContext(ModelMetadataIdentity.ForType(typeof(RequiredModel)),Attributes(attrs));new VietnameseValidationMetadataProvider().CreateValidationMetadata(c);var actual=attrs.OfType<ValidationAttribute>().ToArray();Assert.Equal(0,actual.Length);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-348"),Trait("Dataset","DL01")]
public void UT_348_DL01(){
object[] attrs=[new DisplayAttribute{Name="Tên"},new RequiredAttribute()];var c=new ValidationMetadataProviderContext(ModelMetadataIdentity.ForType(typeof(RequiredModel)),Attributes(attrs));new VietnameseValidationMetadataProvider().CreateValidationMetadata(c);var actual=attrs.OfType<ValidationAttribute>().ToArray();Assert.Equal(1,actual.Length);Assert.Equal("{0} không được để trống.",actual[0].ErrorMessage);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-349"),Trait("Dataset","DL01")]
public void UT_349_DL01(){
object[] attrs=[new RequiredAttribute{ErrorMessage="Riêng"},new RangeAttribute(1,9){ErrorMessageResourceType=typeof(TestMessages)},new RequiredAttribute()];var c=new ValidationMetadataProviderContext(ModelMetadataIdentity.ForType(typeof(RequiredModel)),Attributes(attrs));new VietnameseValidationMetadataProvider().CreateValidationMetadata(c);var actual=attrs.OfType<ValidationAttribute>().ToArray();Assert.Equal(3,actual.Length);Assert.Equal("Riêng",actual[0].ErrorMessage);Assert.Equal(null,actual[1].ErrorMessage);Assert.Equal("{0} không được để trống.",actual[2].ErrorMessage);Assert.Equal(typeof(TestMessages),actual[1].ErrorMessageResourceType);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-350"),Trait("Dataset","DL01")]
public void UT_350_DL01(){
object[] attrs=[new RequiredAttribute{ErrorMessage=""}];var c=new ValidationMetadataProviderContext(ModelMetadataIdentity.ForType(typeof(RequiredModel)),Attributes(attrs));new VietnameseValidationMetadataProvider().CreateValidationMetadata(c);var actual=attrs.OfType<ValidationAttribute>().ToArray();Assert.Equal(1,actual.Length);Assert.Equal("",actual[0].ErrorMessage);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-351"),Trait("Dataset","DL01")]
public void UT_351_DL01(){
var builder=new ModelBuilder();var type=typeof(EduConnect.Mvc.Data.AppDbContext).Assembly.GetType("EduConnect.Mvc.Data.DanhGiaQuanTriMapping")!;type.GetMethod("Map",BindingFlags.Static|BindingFlags.NonPublic)!.MakeGenericMethod(typeof(MapFixture)).Invoke(null,[builder,true]);var entity=builder.Model.FindEntityType(typeof(MapFixture))!;Assert.Equal("MapFixture",entity.GetTableName());Assert.Equal("Id",Assert.Single(entity.FindPrimaryKey()!.Properties).Name);Assert.Equal(Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.Never,entity.FindProperty("Id")!.ValueGenerated);var row=entity.FindProperty("RowVersion")!;Assert.Equal(typeof(byte[]),row.ClrType);Assert.True(row.IsShadowProperty());Assert.True(row.IsConcurrencyToken);Assert.Equal(Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAddOrUpdate,row.ValueGenerated);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-352"),Trait("Dataset","DL01")]
public void UT_352_DL01(){
var builder=new ModelBuilder();var type=typeof(EduConnect.Mvc.Data.AppDbContext).Assembly.GetType("EduConnect.Mvc.Data.DanhGiaQuanTriMapping")!;type.GetMethod("Map",BindingFlags.Static|BindingFlags.NonPublic)!.MakeGenericMethod(typeof(MapFixture)).Invoke(null,[builder,false]);var entity=builder.Model.FindEntityType(typeof(MapFixture))!;Assert.Equal("MapFixture",entity.GetTableName());Assert.Equal("Id",Assert.Single(entity.FindPrimaryKey()!.Properties).Name);Assert.Equal(Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.Never,entity.FindProperty("Id")!.ValueGenerated);Assert.Null(entity.FindProperty("RowVersion"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-353"),Trait("Dataset","DL01")]
public void UT_353_DL01(){
var builder=new ModelBuilder();var type=typeof(EduConnect.Mvc.Data.AppDbContext).Assembly.GetType("EduConnect.Mvc.Data.DanhGiaQuanTriMapping")!;type.GetMethod("Map",BindingFlags.Static|BindingFlags.NonPublic)!.MakeGenericMethod(typeof(MapFixture)).Invoke(null,[builder,Type.Missing]);var entity=builder.Model.FindEntityType(typeof(MapFixture))!;Assert.Equal("MapFixture",entity.GetTableName());Assert.Equal("Id",Assert.Single(entity.FindPrimaryKey()!.Properties).Name);Assert.Equal(Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.Never,entity.FindProperty("Id")!.ValueGenerated);var row=entity.FindProperty("RowVersion")!;Assert.Equal(typeof(byte[]),row.ClrType);Assert.True(row.IsShadowProperty());Assert.True(row.IsConcurrencyToken);Assert.Equal(Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAddOrUpdate,row.ValueGenerated);
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-354"),Trait("Dataset","DL01")]
public void UT_354_DL01(){
var builder=new ModelBuilder();var type=typeof(EduConnect.Mvc.Data.AppDbContext).Assembly.GetType("EduConnect.Mvc.Data.NhatKyAnToanMapping")!;Call(type,"Configure",null,builder);var entity=builder.Model.FindEntityType(typeof(NhatKyAnToan))!;Assert.Equal("NhatKyAnToan",entity.GetTableName());Assert.Equal("Id",Assert.Single(entity.FindPrimaryKey()!.Properties).Name);Assert.Equal(Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.Never,entity.FindProperty("Id")!.ValueGenerated);foreach(var (name,max) in new(string,int)[]{("Nhom",32),("SuKien",160),("KetQua",16),("DinhDanhBam",64),("DiaChiIp",45),("DuongDan",512),("PhuongThuc",16),("MaYeuCau",128),("LyDo",100),("DoiTuongId",64)})Assert.Equal(max,entity.FindProperty(name)!.GetMaxLength());foreach(var name in new[]{"Nhom","KetQua","DinhDanhBam","DiaChiIp","PhuongThuc","MaYeuCau","DoiTuongId"})Assert.False(entity.FindProperty(name)!.IsUnicode(),name);foreach(var fk in entity.GetForeignKeys())Assert.Equal(DeleteBehavior.NoAction,fk.DeleteBehavior);foreach(var names in new[]{new[]{"ThoiDiem","Id"},new[]{"TaiKhoanId","ThoiDiem"},new[]{"Nhom","ThoiDiem"}})Assert.Contains(entity.GetIndexes(),i=>i.Properties.Select(p=>p.Name).SequenceEqual(names));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-355"),Trait("Dataset","DL01")]
public void UT_355_DL01(){
var error=new LoiNghiepVu("Không có quyền",403);var type=typeof(KetQua<string>);var ctor=type.GetConstructor(BindingFlags.Instance|BindingFlags.NonPublic,null,[typeof(string)],null)!;var sut=ctor.Invoke(["Dữ liệu A"]);Assert.Equal((object?)"Dữ liệu A",Field(sut,"duLieu"));Assert.Null(Field(sut,"loi"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-356"),Trait("Dataset","DL01")]
public void UT_356_DL01(){
var error=new LoiNghiepVu("Không có quyền",403);var type=typeof(KetQua<int>);var ctor=type.GetConstructor(BindingFlags.Instance|BindingFlags.NonPublic,null,[typeof(int)],null)!;var sut=ctor.Invoke([42]);Assert.Equal((object?)42,Field(sut,"duLieu"));Assert.Null(Field(sut,"loi"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-357"),Trait("Dataset","DL01")]
public void UT_357_DL01(){
var error=new LoiNghiepVu("Không có quyền",403);var type=typeof(KetQua<string>);var ctor=type.GetConstructor(BindingFlags.Instance|BindingFlags.NonPublic,null,[typeof(string)],null)!;var sut=ctor.Invoke([null]);Assert.Equal((object?)null,Field(sut,"duLieu"));Assert.Null(Field(sut,"loi"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-358"),Trait("Dataset","DL01")]
public void UT_358_DL01(){
var error=new LoiNghiepVu("Không có quyền",403);var type=typeof(KetQua<string>);var ctor=type.GetConstructor(BindingFlags.Instance|BindingFlags.NonPublic,null,[typeof(LoiNghiepVu)],null)!;var sut=ctor.Invoke([error]);Assert.Same(error,Field(sut,"loi"));Assert.Null(Field(sut,"duLieu"));
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","08"),Trait("TestCaseId","UT-359"),Trait("Dataset","DL01")]
public void UT_359_DL01(){
var context=new System.Runtime.Loader.AssemblyLoadContext("initializer-fixture",true);try{var assembly=context.LoadFromAssemblyPath(typeof(VietnameseValidationMetadataProvider).Assembly.Location);var type=assembly.GetType(typeof(VietnameseValidationMetadataProvider).FullName!)!;System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);var labels=(Dictionary<string,string>)type.GetField("Labels",BindingFlags.Static|BindingFlags.NonPublic)!.GetValue(null)!;Assert.Equal("Tên đợt",labels["Ten"]);Assert.Equal("Từ ngày",labels["TuNgay"]);Assert.Equal("Đến ngày",labels["DenNgay"]);Assert.Equal("Hạn phản hồi",labels["HanGui"]);Assert.Equal("Mức hài lòng",labels["MucDoHaiLong"]);Assert.Equal("Điểm sao",labels["SoSao"]);Assert.Equal("Lý do",labels["LyDo"]);Assert.Equal("Kết luận",labels["KetLuan"]);Assert.Equal("Kết quả",labels["KetQua"]);Assert.Equal("Hướng xử lý",labels["QuyetDinh"]);Assert.Equal("Ghi chú",labels["GhiChu"]);Assert.Equal("Tên môn",labels["TenMon"]);Assert.Equal("Số điện thoại",labels["SoDienThoai"]);Assert.Equal("Mật khẩu hiện tại",labels["MatKhauHienTai"]);Assert.Equal("Mật khẩu",labels["MatKhau"]);Assert.Equal("Xác nhận mật khẩu",labels["XacNhanMatKhau"]);}finally{context.Unload();}
}
[Fact,Trait("Suite","CurrentDesign"),Trait("Group","10"),Trait("TestCaseId","UT-360"),Trait("Dataset","DL01")]
public void UT_360_DL01(){
CurrentRelationalBoundary.Run(360);
}
private static ModelAttributes Attributes(object[] attrs)=>(ModelAttributes)typeof(ModelAttributes).GetConstructors(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance).Single(c=>c.GetParameters().Length==1).Invoke([attrs]);
private static object? Field(object o,string n)=>o.GetType().GetField(n,BindingFlags.NonPublic|BindingFlags.Instance)!.GetValue(o);
public sealed class TestMessages{public static string Error=>"Resource";}
public sealed class MapFixture{public Guid Id{get;set;}}
internal static object? Call(Type type,string method,object? target,params object?[] args) {
 var m=type.GetMethods(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static|BindingFlags.Instance).Single(x=>x.Name==method&&x.GetParameters().Length==args.Length);
 try{return m.Invoke(target,args);}catch(TargetInvocationException ex)when(ex.InnerException!=null){System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex.InnerException).Throw();throw;}
}
private sealed class RequiredModel{[Required(ErrorMessage="Nhập tên.")]public string? Ten{get;set;}}
private sealed class Validatable(params ValidationResult[] errors):IValidatableObject{public IEnumerable<ValidationResult> Validate(ValidationContext context)=>errors;}
private sealed class BaseProbe:BaseController{public void Validate()=>ValidateInput(); public void Save(string text="Đã lưu thay đổi.")=>Success(text);}
private sealed class TempProvider:ITempDataProvider{public IDictionary<string,object> LoadTempData(HttpContext c)=>new Dictionary<string,object>();public void SaveTempData(HttpContext c,IDictionary<string,object> d){}}
private static TaiKhoanController Account()=>new(null!){ControllerContext=new ControllerContext{HttpContext=new DefaultHttpContext()}};
private sealed class UrlFixture(ActionContext c,bool local):IUrlHelper{public ActionContext ActionContext=>c;public bool IsLocalUrl(string? u)=>local;public string? Action(UrlActionContext c)=>null;public string? Content(string? s)=>s;public string? Link(string? r,object? v)=>null;public string? RouteUrl(UrlRouteContext c)=>null;}
private static SuKienLichViewModel Event(int id,int day,double start,double end)=>new(){Cot=day,PhutDau=start,PhutCuoi=end,Lich=new(){Id=Guid.Parse($"00000000-0000-0000-0000-{id:000000000000}")}};
private static byte[] Zip(int count,int length,bool types,bool doc,string? extra,bool wrongCase){
 using var memory=new MemoryStream();
 using(var zip=new ZipArchive(memory,ZipArchiveMode.Create,true)){
  var names=new List<string>();if(types)names.Add(wrongCase?"[content_types].xml":"[Content_Types].xml");if(doc)names.Add("word/document.xml");if(extra!=null)names.Add(extra);
  while(names.Count<count)names.Add($"extra{names.Count}.xml");
  for(int i=0;i<names.Count;i++){using var stream=zip.CreateEntry(names[i],CompressionLevel.Fastest).Open();var size=i==0?length:0;var buffer=new byte[8192];while(size>0){var chunk=Math.Min(size,buffer.Length);stream.Write(buffer,0,chunk);size-=chunk;}}
 }return memory.ToArray();
}
private static SqlException SqlFailure(int number){
 var ctor=typeof(SqlError).GetConstructors(BindingFlags.NonPublic|BindingFlags.Instance).OrderByDescending(c=>c.GetParameters().Length).First();
 var args=ctor.GetParameters().Select(p=>p.ParameterType==typeof(int)?(object)((p.Name??"").Contains("Number",StringComparison.OrdinalIgnoreCase)?number:1):p.ParameterType==typeof(byte)?(byte)16:p.ParameterType==typeof(uint)?0u:p.ParameterType==typeof(string)?"SQL fixture":null).ToArray();
 var e=(SqlError)ctor.Invoke(args);var collection=(SqlErrorCollection)Activator.CreateInstance(typeof(SqlErrorCollection),true)!;
 typeof(SqlErrorCollection).GetMethod("Add",BindingFlags.NonPublic|BindingFlags.Instance)!.Invoke(collection,[e]);
 var m=typeof(SqlException).GetMethods(BindingFlags.Static|BindingFlags.NonPublic).Single(m=>m.Name=="CreateException"&&m.GetParameters().Length==2&&m.GetParameters()[0].ParameterType==typeof(SqlErrorCollection));
 var result=(SqlException)m.Invoke(null,[collection,"16.0"])!;Assert.Equal(number,result.Number);return result;
}
}