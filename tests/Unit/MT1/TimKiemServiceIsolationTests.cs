using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Areas.TimKiem.Services;
using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;

namespace EduConnect.UnitTests.MT1;

public class TimKiemServiceIsolationTests
{
    [Fact, Trait("TestCaseId", "MT1-UT-047")]
    public Task MT1_UT_047() => CompareGuard([]);
    [Fact, Trait("TestCaseId", "MT1-UT-048")]
    public Task MT1_UT_048() => CompareGuard([Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()]);
    static async Task CompareGuard(Guid[] ids)
    {
        var current = StrictProxy.Create<INguoiDungHienTaiService>();
        var user = new NguoiThamGia { Id = Guid.NewGuid(), LoaiNguoi = "HocSinh" };
        ((StrictProxy)current).Handlers["HienTaiAsync"] = _ => Task.FromResult(user);
        ((StrictProxy)current).Handlers["KiemTraNguoiThucHienAsync"] = _ => Task.FromResult(user);
        // No DbContext: any query reaching persistence fails instead of contacting SQL.
        var service = new TimKiemService(null!, current);
        var ex = await Assert.ThrowsAsync<LoiNghiepVu>(() => service.SoSanhAsync(ids));
        Assert.Equal(400, ex.Status); Assert.Equal("Chọn từ 1 đến 3 đối tượng.", ex.Message);
    }
    static NhuCauHoc Need() => new() { MonHocId = 101, KhoiLop = "Lớp 8", HinhThucHoc = "TrucTuyen" };
    static UngVienViewModel Profile(int number, int subject = 101, bool online = true) => new()
    {
        Id = Guid.Parse($"00000200-0000-0000-0000-{number:D12}"), ChuSoHuuId = Guid.NewGuid(),
        MonHocId = subject, TenMon = subject == 101 ? "Toán Học" : "Ngữ Văn", KhoiLop = "Lớp 8",
        DayTrucTuyen = online, DayTrucTiep = !online, HinhThucHoc = online ? "TrucTuyen" : "TrucTiep",
        TaoLuc = DateTimeOffset.Parse("2026-10-28T12:00:00+07:00").AddMinutes(-number),
        MonTheoHoSo = [new() { MonHocId = subject, TenMon = subject == 101 ? "Toán Học" : "Ngữ Văn", PhamViDay = "Lớp 8" }],
        Slots = [new SlotInput { ThuTrongTuan = 1, GioBatDau = new(19, 0), GioKetThuc = new(20, 0) }]
    };
    [Fact, Trait("TestCaseId", "MT1-UT-065")]
    public void MT1_UT_065_BenHoc_LyDoTheoMonHinhThuc_KhongDungLichGiaSu()
    {
        // SRS (2), P02 and FR-06/07: criteria-backed reasons, no tutor availability,
        // no percentage score. The specification does not require ranking or top 3.
        var p1 = Profile(1); var p2 = Profile(2, online: false); var p3 = Profile(3, 102);
        p1.Slots = []; // Tutors are not required to supply availability.
        p2.Slots = [new SlotInput { ThuTrongTuan = 6, GioBatDau = new(6, 0), GioKetThuc = new(7, 0) }];
        var rows = GoiYRules.ChoBenHoc([Need()], [p3, p2, p1]);
        Assert.Contains(p1, rows); Assert.Contains(p2, rows); Assert.DoesNotContain(p3, rows);
        Assert.Contains("Cùng môn Toán Học", p1.LyDo); Assert.Contains("Cùng hình thức học", p1.LyDo);
        Assert.Contains("Cùng môn Toán Học", p2.LyDo); Assert.DoesNotContain("Cùng hình thức học", p2.LyDo);
        Assert.All(rows.SelectMany(x => x.LyDo), reason =>
        {
            Assert.DoesNotContain("%", reason);
            Assert.DoesNotContain("lịch", reason, StringComparison.OrdinalIgnoreCase);
        });
    }
    [Fact, Trait("TestCaseId", "MT1-UT-066")]
    public void MT1_UT_066_GiaSu_GoiYNhuCauTheoMonDaDangKy()
    {
        // SRS (2), P02/FR-06: tutor searches learning needs by registered subject.
        // Candidate query is already public; public-state SQL filtering belongs to IT.
        var n1 = new UngVienViewModel { Id = Guid.NewGuid(), MonHocId = 101 };
        var n2 = new UngVienViewModel { Id = Guid.NewGuid(), MonHocId = 102 };
        var n3 = new UngVienViewModel { Id = Guid.NewGuid(), MonHocId = 102 };
        var candidates = new[] { n3, n2, n1 }.AsQueryable();
        var links = new[] { new NhuCauMonHoc { NhuCauId = n2.Id, MonHocId = 101 } }.AsQueryable();
        var rows = GoiYRules.ChoGiaSu(candidates, new[] { 101 }, links).ToList();
        Assert.Contains(n1, rows); Assert.Contains(n2, rows); Assert.DoesNotContain(n3, rows);
        Assert.Equal(2, rows.Count);
        Assert.Empty(GoiYRules.ChoGiaSu(candidates, new[] { 103 }, links));
        Assert.Empty(GoiYRules.ChoGiaSu(candidates, Array.Empty<int>(), links));
    }
    [Fact, Trait("TestCaseId", "MT1-UT-067")]
    public void MT1_UT_067_LyDoPhanAnhDuLieu()
    {
        var a = Profile(1); var b = Profile(2, online: false); var c = Profile(3, 102);
        var rows = GoiYRules.ChoBenHoc([Need()], [c, b, a]);
        Assert.Equal(new[] { "Cùng môn Toán Học", "Cùng hình thức học" }, a.LyDo);
        Assert.Equal(new[] { "Cùng môn Toán Học" }, b.LyDo); Assert.Empty(c.LyDo);
        Assert.DoesNotContain(c, rows); Assert.All(rows.SelectMany(x => x.LyDo), reason => Assert.DoesNotContain("%", reason));
        // Pure in-memory inputs: no contact, connection or class persistence exists here.
    }
}
