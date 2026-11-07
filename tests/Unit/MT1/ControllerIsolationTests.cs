using System.Reflection;
using EduConnect.Mvc.Areas.NhuCau.Controllers;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Areas.TimKiem.Controllers;
using EduConnect.Mvc.Areas.TimKiem.Services;
using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Areas.KetNoi.Services;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace EduConnect.UnitTests.MT1;

// Strict fake: every unexpected call fails, including any unconfigured persistence.
public class StrictProxy : DispatchProxy
{
    public Dictionary<string, Func<object?[], object?>> Handlers = [];
    public List<(string Method, object?[] Args)> Calls = [];
    protected override object? Invoke(MethodInfo? method, object?[]? args)
    {
        Calls.Add((method!.Name, args ?? []));
        return Handlers.TryGetValue(method.Name, out var handler)
            ? handler(args ?? []) : throw new InvalidOperationException("Unexpected call: " + method.Name);
    }
    public static T Create<T>() where T : class => DispatchProxy.Create<T, StrictProxy>();
}

public class ControllerIsolationTests
{
    readonly INhuCauService needs = StrictProxy.Create<INhuCauService>();
    readonly IHoSoGiaSuService profiles = StrictProxy.Create<IHoSoGiaSuService>();
    readonly ITimKiemService search = StrictProxy.Create<ITimKiemService>();
    readonly IKetNoiService contacts = StrictProxy.Create<IKetNoiService>();
    readonly IBangCapService certificates = StrictProxy.Create<IBangCapService>();
    readonly INguoiDungHienTaiService current = StrictProxy.Create<INguoiDungHienTaiService>();
    readonly Guid id = Guid.Parse("00000300-0000-0000-0000-000000000001");
    readonly NguoiThamGia me = new() { Id = Guid.Parse("00000100-0000-0000-0000-000000000001"), LoaiNguoi = "HocSinh" };
    readonly List<MonHoc> subjects = [new() { Id = 101, TenMon = "Toán Học" }];
    readonly List<TepDinhKem> files = [new() { Id = Guid.NewGuid(), TenGoc = "bangcap.pdf" }];
    static StrictProxy Fake(object value) => (StrictProxy)value;
    static void On(object fake, string name, Func<object?[], object?> handler) => Fake(fake).Handlers[name] = handler;
    static void Once(object fake, string method, params object?[] args)
    {
        var call = Assert.Single(Fake(fake).Calls, c => c.Method == method);
        Assert.Equal(args, call.Args);
    }
    static void Never(object fake, string method) => Assert.DoesNotContain(Fake(fake).Calls, c => c.Method == method);
    T Setup<T>(T c) where T : Controller
    {
        c.ControllerContext = new() { HttpContext = new DefaultHttpContext() };
        c.TempData = new TempDataDictionary(c.HttpContext, new MemoryTempData());
        return c;
    }
    sealed class MemoryTempData : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
        public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
    }
    public ControllerIsolationTests()
    {
        On(current, "HienTaiAsync", _ => Task.FromResult(me));
        On(needs, "MonDuocChonAsync", _ => Task.FromResult(subjects.ToList()));
        On(needs, "MonTenAsync", _ => Task.FromResult(subjects.ToList()));
        On(needs, "MonAsync", _ => Task.FromResult(subjects.ToList()));
        On(certificates, "CuaToiAsync", _ => Task.FromResult(files));
    }
    NhuCauController NeedController() => Setup(new NhuCauController(needs, current));
    HoSoGiaSuController ProfileController() => Setup(new HoSoGiaSuController(profiles, needs, certificates, search, contacts, current));
    TimKiemController SearchController() => Setup(new TimKiemController(search, needs, contacts, current));
    NhuCauInput Model() => new() { Id = id, MonHocIds = [101], HinhThucHoc = "TrucTuyen" };
    static void Success(Controller c, IActionResult result, string action)
    {
        Assert.Equal(action, Assert.IsType<RedirectToActionResult>(result).ActionName);
        Assert.False(string.IsNullOrWhiteSpace(Assert.IsType<string>(c.TempData["Success"])));
    }

    [Fact, Trait("TestCaseId", "MT1-UT-050")]
    public async Task MT1_UT_050()
    {
        var c = NeedController(); var m = Model(); c.ModelState.AddModelError("HinhThucHoc", "Sai");
        var v = Assert.IsType<ViewResult>(await c.Luu(m));
        Assert.Equal("Sua", v.ViewName); Assert.Same(m, v.Model); Assert.False(c.ModelState.IsValid); Never(needs, "LuuAsync");
    }
    [Fact, Trait("TestCaseId", "MT1-UT-051")]
    public async Task MT1_UT_051()
    {
        var c = NeedController(); var m = Model(); On(needs, "LuuAsync", _ => Task.FromResult(id));
        Success(c, await c.Luu(m), "Index"); Once(needs, "LuuAsync", m);
    }
    [Fact, Trait("TestCaseId", "MT1-UT-052")]
    public async Task MT1_UT_052()
    {
        var c = NeedController(); var m = Model(); On(needs, "LuuAsync", _ => Task.FromException<Guid>(new LoiNghiepVu("Môn học không hợp lệ.")));
        var v = Assert.IsType<ViewResult>(await c.Luu(m));
        Assert.Equal("Sua", v.ViewName); Assert.Same(m, v.Model); Assert.Equal("Môn học không hợp lệ.", Assert.Single(c.ModelState[""]!.Errors).ErrorMessage); Once(needs, "LuuAsync", m);
    }
    [Fact, Trait("TestCaseId", "MT1-UT-053")]
    public async Task MT1_UT_053()
    {
        var c = NeedController(); On(needs, "TrangThaiAsync", _ => Task.CompletedTask);
        Success(c, await c.TrangThai(id, "DaDong", "AQIDBAUGBwg="), "Index"); Once(needs, "TrangThaiAsync", id, "DaDong", "AQIDBAUGBwg=");
    }
    [Fact, Trait("TestCaseId", "MT1-UT-054")]
    public async Task MT1_UT_054()
    {
        var c = ProfileController(); var m = new HoSoInput(); c.ModelState.AddModelError("HoTen", "Thiếu");
        var v = Assert.IsType<ViewResult>(await c.Luu(m)); Assert.Equal("Sua", v.ViewName); Assert.Same(m, v.Model);
        Assert.Same(files, c.ViewData["Certificates"]); Assert.Single(Assert.IsType<List<MonHoc>>(c.ViewData["Subjects"])); Never(profiles, "CapNhatAsync");
    }
    [Fact, Trait("TestCaseId", "MT1-UT-055")]
    public async Task MT1_UT_055()
    {
        var c = ProfileController(); var m = new HoSoInput(); On(profiles, "CapNhatAsync", _ => Task.CompletedTask);
        Success(c, await c.Luu(m), "Sua"); Once(profiles, "CapNhatAsync", m);
    }
    [Fact, Trait("TestCaseId", "MT1-UT-056")]
    public async Task MT1_UT_056()
    {
        var c = ProfileController(); var m = new HoSoInput(); On(profiles, "CapNhatAsync", _ => Task.FromException(new LoiNghiepVu("Dữ liệu hồ sơ không hợp lệ.")));
        var v = Assert.IsType<ViewResult>(await c.Luu(m)); Assert.Equal("Sua", v.ViewName); Assert.Same(m, v.Model);
        Assert.Equal("Dữ liệu hồ sơ không hợp lệ.", Assert.Single(c.ModelState[""]!.Errors).ErrorMessage); Assert.False(c.TempData.ContainsKey("Success"));
    }
    [Fact, Trait("TestCaseId", "MT1-UT-057")]
    public async Task MT1_UT_057()
    {
        var c = SearchController(); var f = new BoLocTimKiemViewModel { Trang = 2 }; var items = new List<UngVienViewModel> { new() { Id = Guid.NewGuid() }, new() { Id = Guid.NewGuid() } };
        On(search, "TimKiemAsync", _ => KetQua<Page<UngVienViewModel>>.ThucHienAsync(() => Task.FromResult(new Page<UngVienViewModel> { Items = items, Total = 12, Trang = 2 })));
        var v = Assert.IsType<ViewResult>(await c.Index(f)); var m = Assert.IsType<SearchPage>(v.Model);
        Assert.Equal("Index", v.ViewName); Assert.Same(f, m.Filter); Assert.Same(items, m.Items); Assert.Equal(12, m.Total); Once(search, "TimKiemAsync", f, me.Id, c.HttpContext.RequestAborted);
    }
    [Fact, Trait("TestCaseId", "MT1-UT-058")]
    public async Task MT1_UT_058()
    {
        var c = SearchController(); Guid[] ids = [Guid.NewGuid(), Guid.NewGuid()];
        var items = ids.Select(x => new NhuCauCongKhaiViewModel { Id = x }).ToList();
        On(search, "SoSanhAsync", _ => KetQua<SoSanhViewModel>.ThucHienAsync(() => Task.FromResult(new SoSanhViewModel { MaDoiTuong = ids, NhuCau = items })));
        var v = Assert.IsType<ViewResult>(await c.SoSanh(ids)); Assert.Equal(ids, Assert.IsAssignableFrom<IEnumerable<UngVienViewModel>>(v.Model).Select(x => x.Id));
        Once(search, "SoSanhAsync", ids, me.Id, c.HttpContext.RequestAborted);
    }
    [Fact, Trait("TestCaseId", "MT1-UT-059")]
    public async Task MT1_UT_059()
    {
        var c = NeedController(); var m = Model(); m.ChonMonKhac = true;
        On(needs, "LayDeSuaAsync", _ => KetQua<NhuCauInput>.ThucHienAsync(() => Task.FromResult(m)));
        var v = Assert.IsType<ViewResult>(await c.Sua(id)); Assert.Same(m, v.Model); Assert.False(m.ChonMonKhac); Assert.Single(Assert.IsType<List<MonHoc>>(c.ViewData["Subjects"])); Once(needs, "LayDeSuaAsync", id);
    }
    [Fact, Trait("TestCaseId", "MT1-UT-060")]
    public async Task MT1_UT_060()
    {
        var c = ProfileController(); var m = new HoSoInput(); On(profiles, "LayDeSuaAsync", _ => Task.FromResult(m));
        Assert.Same(m, Assert.IsType<ViewResult>(await c.Sua()).Model); Assert.Same(files, c.ViewData["Certificates"]);
        Assert.Equal(101, Assert.Single(Assert.IsType<List<MonHoc>>(c.ViewData["Subjects"])).Id); Once(profiles, "LayDeSuaAsync");
    }
    [Fact, Trait("TestCaseId", "MT1-UT-061")]
    public async Task MT1_UT_061()
    {
        var c = NeedController(); var m = Model(); Assert.NotEmpty(m.LichRanh);
        c.Request.ContentType = "application/x-www-form-urlencoded"; c.Request.Form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues> { ["MucTieu"] = "Ôn tập" });
        On(needs, "LuuAsync", a => { Assert.Same(m, a[0]); Assert.Empty(((NhuCauInput)a[0]!).LichRanh); return Task.FromResult(id); });
        Success(c, await c.Luu(m), "Index"); Once(needs, "LuuAsync", m);
    }
    [Fact, Trait("TestCaseId", "MT1-UT-062")]
    public async Task MT1_UT_062()
    {
        var c = NeedController(); var rows = new List<NhuCauHoc> { new() { Id = id, ChuSoHuuId = me.Id } }; var linked = new HashSet<Guid> { id };
        On(needs, "LayAsync", _ => Task.FromResult(rows)); On(needs, "DaKetNoiAsync", _ => Task.FromResult(linked));
        Assert.Same(rows, Assert.IsType<ViewResult>(await c.Index()).Model); Assert.Same(linked, c.ViewData["Linked"]);
        Assert.Equal(101, Assert.Single(Assert.IsType<List<MonHoc>>(c.ViewData["Subjects"])).Id); Once(needs, "LayAsync");
    }
    [Fact, Trait("TestCaseId", "MT1-UT-063")]
    public Task MT1_UT_063() => ContactDetail("ChapNhan", true);
    [Fact, Trait("TestCaseId", "MT1-UT-064")]
    public Task MT1_UT_064() => ContactDetail("ChoPhanHoi", false);
    async Task ContactDetail(string state, bool accepted)
    {
        var c = SearchController(); var detail = new UngVienViewModel { Id = id, ChuSoHuuId = Guid.NewGuid(), LoaiNguoi = "HocSinh" };
        var contact = new YeuCauLienHe { Id = Guid.NewGuid(), TrangThai = state }; var chat = Guid.NewGuid(); var related = new List<UngVienViewModel> { detail };
        On(search, "ChiTietAsync", _ => Task.FromResult(detail)); On(contacts, "LienHeVoiAsync", _ => Task.FromResult<YeuCauLienHe?>(contact));
        On(search, "LienQuanAsync", _ => Task.FromResult(related)); if (accepted) On(contacts, "HoiThoaiAsync", _ => Task.FromResult<Guid?>(chat));
        Assert.Same(detail, Assert.IsType<ViewResult>(await c.ChiTiet(id)).Model); Assert.Same(contact, c.ViewData["Contact"]); Assert.Same(related, c.ViewData["Related"]);
        Once(search, "ChiTietAsync", id, default(CancellationToken)); Once(contacts, "LienHeVoiAsync", detail.ChuSoHuuId); Once(search, "LienQuanAsync", detail.ChuSoHuuId);
        if (accepted) { Assert.Equal(chat, c.ViewData["ContactChat"]); Once(contacts, "HoiThoaiAsync", contact.Id); }
        else { Assert.Null(c.ViewData["ContactChat"]); Never(contacts, "HoiThoaiAsync"); }
    }
}
