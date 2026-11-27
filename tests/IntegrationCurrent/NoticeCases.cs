using Microsoft.EntityFrameworkCore;
using EduConnect.Mvc.Models;
using Xunit;

internal static partial class Cases
{
    internal static async Task NoticeCases(Fixture f, string key)
    {
        switch (key)
        {
            case "notice_store":
            {
                await f.SeedClass();
                var eventId = Guid.NewGuid();
                await using var scope = f.Scope(f.H);
                await using var tx = await scope.Db.Database.BeginTransactionAsync();
                scope.Notices.Tao(f.H, "LichHoc", f.Class, f.Tag, eventId);
                await scope.Db.SaveChangesAsync();
                Assert.Single(await scope.Db.Set<ThongBao>().Where(x => x.MaSuKien == eventId).ToListAsync());
                Assert.Single(await scope.Db.Set<SuKienChoXuLy>().Where(x => x.Id == eventId).ToListAsync());
                await tx.CommitAsync();
                await using var observer = f.Db();
                var notice = await observer.Set<ThongBao>().SingleAsync(x => x.MaSuKien == eventId);
                var ev = await observer.Set<SuKienChoXuLy>().SingleAsync(x => x.Id == eventId);
                Assert.Equal(f.H, notice.NguoiNhanId); Assert.Equal("LichHoc", notice.LoaiThongBao); Assert.Equal(f.Class, notice.DoiTuongId);
                Assert.Equal("ChoXuLy", ev.TrangThai); Assert.Contains(f.H.ToString(), ev.DuLieuToiThieu, StringComparison.OrdinalIgnoreCase);
                break;
            }
            case "notice_http":
            {
                var ids = new List<Guid>();
                await using (var scope = f.Scope(f.H))
                {
                    await using var tx = await scope.Db.Database.BeginTransactionAsync();
                    for (int i = 0; i < 3; i++)
                    {
                        scope.Notices.Tao(f.H, "LichHoc", Guid.NewGuid(), f.Tag + i);
                    }
                    for (int i = 0; i < 2; i++) scope.Notices.Tao(f.X, "BaiTap", Guid.NewGuid(), f.Tag + "X" + i);
                    await scope.Db.SaveChangesAsync();
                    ids.AddRange(await scope.Db.Set<ThongBao>().Where(x => x.NguoiNhanId == f.H && x.NoiDungTomTat.StartsWith(f.Tag)).OrderBy(x => x.NoiDungTomTat).Select(x => x.Id).ToListAsync());
                    Assert.Equal(3, ids.Count);
                    await tx.CommitAsync();
                }
                using (var h = await f.Client(f.H))
                {
                    using var count3 = await h.GetAsync("/Home/ChuaDoc"); Assert.Equal(System.Net.HttpStatusCode.OK, count3.StatusCode); Assert.Contains("3", await count3.Content.ReadAsStringAsync());
                    using var read = await f.Post(h, "/Home/Doc", new() { ["id"] = ids[0].ToString() }, "/Home/ThongBao", ajax: false); Assert.Equal(System.Net.HttpStatusCode.Redirect, read.StatusCode); Assert.Contains("/HocTap/LopHoc/ChiTiet/", read.Headers.Location!.OriginalString);
                    await using var db = f.Db(); Assert.NotNull((await db.Set<ThongBao>().SingleAsync(x => x.Id == ids[0])).DaDocLuc);
                    using var count2 = await h.GetAsync("/Home/ChuaDoc"); Assert.Contains("2", await count2.Content.ReadAsStringAsync());
                    using var all = await f.Post(h, "/Home/DocTatCa", new(), "/Home/ThongBao", ajax: false); Assert.Equal(System.Net.HttpStatusCode.Redirect, all.StatusCode);
                    using var count0 = await h.GetAsync("/Home/ChuaDoc"); Assert.Contains("0", await count0.Content.ReadAsStringAsync());
                }
                using (var x = await f.Client(f.X))
                {
                    using var foreign = await f.Post(x, "/Home/Doc", new() { ["id"] = ids[0].ToString() }, "/Home/ThongBao", ajax: false);
                    Assert.Equal(System.Net.HttpStatusCode.NotFound, foreign.StatusCode);
                    await using var db = f.Db(); Assert.Equal(2, await db.Set<ThongBao>().CountAsync(n => n.NguoiNhanId == f.X && n.DaDocLuc == null));
                }
                break;
            }
            default: throw new InvalidOperationException(key);
        }
    }
}
