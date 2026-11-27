using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Models;
using EduConnect.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

internal static partial class Cases
{
    internal static async Task SearchCases(Fixture f, string key)
    {
        switch (key)
        {
            case "search_store":
            {
                await using (var db = f.Db())
                {
                    await db.Set<HoSoGiaSuMonDay>().Where(x => x.HoSoGiaSuId == f.Profile && x.MonHocId == f.M1).ExecuteUpdateAsync(s => s.SetProperty(x => x.HocPhiDeXuat, 100000m));
                }
                await using var scope = f.Scope(f.X);
                var result = await scope.Search.TimKiemAsync(new BoLocTimKiemViewModel
                {
                    LoaiDoiTuong = "GiaSu", MonHocId = f.M1, HocPhiToiDa = 100000, TuKhoa = f.Tag,
                    DonViHocPhi = "VND/Gio", Trang = 1, SapXep = "MoiNhat"
                }, null, default);
                Assert.True(result.ThanhCong, result.ThongBao);
                Assert.Equal(1, result.DuLieu.Total);
                var only = Assert.Single(result.DuLieu.Items);
                Assert.Equal(f.Profile, only.Id);
                Assert.Contains(only.MonTheoHoSo, m => m.MonHocId == f.M1 && m.HocPhi == 100000 && m.DonVi == "VND/Gio");
                break;
            }
            case "search_suggest":
            {
                await f.SeedNeed();
                await using (var scope = f.Scope(f.H))
                {
                    var (items, hasNeed) = await scope.Search.GoiYAsync();
                    Assert.True(hasNeed);
                    var candidate = Assert.Single(items);
                    Assert.Equal(f.Profile, candidate.Id);
                    Assert.Contains(candidate.LyDo, x => x.Contains("Cùng môn", StringComparison.Ordinal));
                    Assert.Contains(candidate.LyDo, x => x.Contains("Cùng hình thức", StringComparison.Ordinal));
                }
                await f.SeedNeed(f.X);
                await using (var scope = f.Scope(f.G))
                {
                    var (items, hasNeed) = await scope.Search.GoiYAsync();
                    Assert.True(hasNeed);
                    Assert.Contains(items, x => x.Id == f.Need);
                }
                var h2 = await f.User("HocSinh");
                await using (var scope = f.Scope(h2))
                {
                    var (items, hasNeed) = await scope.Search.GoiYAsync();
                    Assert.False(hasNeed);
                    Assert.Empty(items);
                }
                await using var db = f.Db();
                Assert.Empty(await db.Set<YeuCauLienHe>().Where(x => x.BenHocId == f.H || x.BenHocId == f.X).ToListAsync());
                Assert.Empty(await db.Set<YeuCauKetNoiLopHoc>().Where(x => x.BenHocId == f.H || x.BenHocId == f.X).ToListAsync());
                break;
            }
            case "search_filter":
            {
                var province = "Hà Nội";
                var ward = DiaGioiHanhChinh.PhuongXa(province).First();
                await using (var db = f.Db())
                {
                    var profile = await db.Set<HoSoGiaSu>().SingleAsync(x => x.Id == f.Profile);
                    profile.DayTrucTiep = true; profile.KhuVuc = province; profile.PhuongXa = ward;
                    await db.SaveChangesAsync();
                    await db.Set<HoSoGiaSuMonDay>().Where(x => x.HoSoGiaSuId == f.Profile && x.MonHocId == f.M1).ExecuteUpdateAsync(s => s.SetProperty(x => x.HocPhiDeXuat, 100000m).SetProperty(x => x.DonViHocPhi, "VND/Gio"));
                    await db.Set<HoSoGiaSuMonDay>().Where(x => x.HoSoGiaSuId == f.Profile && x.MonHocId == f.M2).ExecuteUpdateAsync(s => s.SetProperty(x => x.HocPhiDeXuat, 0m).SetProperty(x => x.DonViHocPhi, "VND/Buoi"));
                    await db.SaveChangesAsync();
                }
                await using var scope = f.Scope(f.X);
                foreach (var (subject, price, unit, mode) in new[] { (f.M1, 100000m, "VND/Gio", (string?)null), (f.M2, 0m, "VND/Buoi", (string?)null), (f.M1, 100000m, "VND/Gio", "TrucTiep") })
                {
                    var result = await scope.Search.TimKiemAsync(new BoLocTimKiemViewModel { LoaiDoiTuong = "GiaSu", MonHocId = subject, HocPhiToiDa = price, DonViHocPhi = unit, HinhThucHoc = mode, KhuVuc = mode == "TrucTiep" ? province : null, PhuongXa = mode == "TrucTiep" ? ward : null, TuKhoa = f.Tag }, null, default);
                    Assert.True(result.ThanhCong, result.ThongBao);
                    Assert.Contains(result.DuLieu.Items, x => x.Id == f.Profile);
                    Assert.All(result.DuLieu.Items, x => Assert.Contains(x.MonTheoHoSo, m => m.MonHocId == subject && m.HocPhi <= price && m.DonVi == unit));
                    using var client = await f.Client();
                    var query = new Dictionary<string, string> { ["LoaiDoiTuong"] = "GiaSu", ["MonHocId"] = subject.ToString(), ["HocPhiToiDa"] = price.ToString(System.Globalization.CultureInfo.InvariantCulture), ["DonViHocPhi"] = unit, ["TuKhoa"] = f.Tag };
                    if (mode != null) { query["HinhThucHoc"] = mode; query["KhuVuc"] = province; query["PhuongXa"] = ward; }
                    using var response = await client.GetAsync("/TimKiem/TimKiem/Tim?" + string.Join("&", query.Select(x => Uri.EscapeDataString(x.Key) + "=" + Uri.EscapeDataString(x.Value))));
                    Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
                    Assert.Contains(f.Profile.ToString(), await response.Content.ReadAsStringAsync(), StringComparison.OrdinalIgnoreCase);
                }
                break;
            }
            case "search_visibility":
            {
                await f.SeedNeed();
                var unreviewed = await f.User("GiaSu");
                Guid hiddenProfile;
                await using (var db = f.Db())
                {
                    hiddenProfile = Guid.NewGuid();
                    db.Add(new HoSoGiaSu { Id = hiddenProfile, GiaSuId = unreviewed, TrinhDo = "Đại học", ChuyenMon = f.Tag, KinhNghiem = "3 năm", MoTa = f.Tag, TrangThaiDuyet = "ChoDuyet", DangHienThi = false });
                    db.Add(new BienPhapQuanTri { Id = Guid.NewGuid(), LoaiDoiTuong = "NhuCauHoc", DoiTuongId = f.Need, Loai = "AnNoiDung", LyDo = f.Tag, NguoiApDungId = f.A, ApDungLuc = DateTimeOffset.UtcNow });
                    var prof = await db.Set<HoSoGiaSu>().SingleAsync(x => x.Id == f.Profile); prof.DangHienThi = false;
                    await db.SaveChangesAsync();
                    await db.Users.Where(x => x.Id == unreviewed).ExecuteUpdateAsync(s => s.SetProperty(x => x.TrangThai, "BiKhoa"));
                }
                await using var scope = f.Scope(f.X);
                var result = await scope.Search.TimKiemAsync(new BoLocTimKiemViewModel { LoaiDoiTuong = "GiaSu" }, null, default);
                Assert.True(result.ThanhCong, result.ThongBao);
                Assert.DoesNotContain(result.DuLieu.Items, x => x.Id == hiddenProfile || x.Id == f.Profile);
                using (var client = await f.Client())
                {
                    using var list = await client.GetAsync("/TimKiem/TimKiem/GiaSu?TuKhoa=" + Uri.EscapeDataString(f.Tag));
                    Assert.Equal(System.Net.HttpStatusCode.OK, list.StatusCode);
                    var body = await list.Content.ReadAsStringAsync();
                    Assert.DoesNotContain(hiddenProfile.ToString(), body, StringComparison.OrdinalIgnoreCase);
                    Assert.DoesNotContain(f.Profile.ToString(), body, StringComparison.OrdinalIgnoreCase);
                    foreach (var id in new[] { hiddenProfile, f.Profile })
                    {
                        using var direct = await client.GetAsync("/TimKiem/TimKiem/ChiTiet/" + id);
                        Assert.Equal(System.Net.HttpStatusCode.NotFound, direct.StatusCode);
                        Assert.DoesNotContain(f.Tag, await direct.Content.ReadAsStringAsync());
                    }
                }
                using (var client = await f.Client())
                {
                    using var hiddenNeed = await client.GetAsync("/TimKiem/TimKiem/ChiTiet/" + f.Need);
                    Assert.Equal(System.Net.HttpStatusCode.NotFound, hiddenNeed.StatusCode);
                    Assert.DoesNotContain(f.Tag, await hiddenNeed.Content.ReadAsStringAsync());
                }
                break;
            }
            case "search_pages":
            {
                var ids = new List<Guid> { f.Profile };
                await using (var db = f.Db())
                {
                    var baseProfile = await db.Set<HoSoGiaSu>().SingleAsync(x => x.Id == f.Profile);
                    for (int i = 1; i < 20; i++)
                    {
                        var actor = await f.User("GiaSu"); var id = Guid.NewGuid(); ids.Add(id);
                        db.Add(new HoSoGiaSu { Id = id, GiaSuId = actor, TrinhDo = "Đại học", ChuyenMon = f.Tag, KinhNghiem = "3 năm", MoTa = f.Tag, TrangThaiDuyet = "DaDuyet", DangHienThi = true, DayTrucTuyen = true, DuyetLuc = baseProfile.DuyetLuc });
                        db.Add(new HoSoGiaSuMonDay { HoSoGiaSuId = id, MonHocId = f.M1, PhamViDay = "Lớp 8", HocPhiDeXuat = 100000 + (i % 2), DonViHocPhi = "VND/Gio" });
                    }
                    await db.SaveChangesAsync();
                }
                await using var scope = f.Scope(f.X);
                var pages = new List<Guid>();
                for (int page = 1; page <= 3; page++)
                {
                    var result = await scope.Search.TimKiemAsync(new BoLocTimKiemViewModel { LoaiDoiTuong = "GiaSu", Trang = page, TuKhoa = f.Tag }, null, default);
                    Assert.True(result.ThanhCong, result.ThongBao); Assert.Equal(20, result.DuLieu.Total); Assert.InRange(result.DuLieu.Items.Count, 1, 9);
                    pages.AddRange(result.DuLieu.Items.Select(x => x.Id));
                }
                Assert.Equal(20, pages.Count); Assert.Equal(20, pages.Distinct().Count()); Assert.Equal(ids.OrderBy(x => x), pages.OrderBy(x => x));
                var repeat = await scope.Search.TimKiemAsync(new BoLocTimKiemViewModel { LoaiDoiTuong = "GiaSu", Trang = 1, TuKhoa = f.Tag }, null, default);
                var again = await scope.Search.TimKiemAsync(new BoLocTimKiemViewModel { LoaiDoiTuong = "GiaSu", Trang = 1, TuKhoa = f.Tag }, null, default);
                Assert.Equal(repeat.DuLieu.Items.Select(x => x.Id), again.DuLieu.Items.Select(x => x.Id));
                break;
            }
            default: throw new InvalidOperationException(key);
        }
    }
}


