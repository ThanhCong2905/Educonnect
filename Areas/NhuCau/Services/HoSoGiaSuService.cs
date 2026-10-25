using System.Data;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Areas.NhuCau.Services;

public sealed class HoSoGiaSuService(AppDbContext db, NguoiDungMauService current, MonHocCatalog catalog, TepService files)
{
    public async Task<HoSoInput> LayDeSuaAsync()
    {
        var u = await current.HienTaiAsync();
        Rules.Check(u.LoaiNguoi == "GiaSu", "Chỉ gia sư được sửa hồ sơ.", 403);
        var h = await db.Set<HoSoGiaSu>().AsNoTracking().SingleOrDefaultAsync(x => x.GiaSuId == u.Id);
        if (h == null)
            return new()
            {
                HoTen = u.HoTen
            };
        return new()
        {
            HoTen = u.HoTen,
            RowVersion = Convert.ToBase64String(h.RowVersion),
            TrinhDo = h.TrinhDo,
            ChuyenMon = h.ChuyenMon,
            KinhNghiem = h.KinhNghiem,
            MoTa = h.MoTa,
            KhuVuc = h.KhuVuc,
            PhamViDay = await db.Set<HoSoGiaSuMonDay>().Where(x => x.HoSoGiaSuId == h.Id).Select(x => x.PhamViDay).FirstOrDefaultAsync() ?? "",
            MonHocIds = await db.Set<HoSoGiaSuMonDay>().Where(x => x.HoSoGiaSuId == h.Id).Select(x => x.MonHocId).ToListAsync()
        };
    }
    public async Task CapNhatAsync(HoSoInput m)
    {
        var u = await current.HienTaiAsync();
        Rules.Check(u.LoaiNguoi == "GiaSu", "Chỉ gia sư được sửa hồ sơ.", 403);
        var uploads = await files.KiemTraAsync(m.Files);
        var written = new List<string>();
        var retired = new List<string>();
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            if (m.ChonMonKhac)
                m.MonHocIds.Add(await catalog.LayHoacThemAsync(m.TenMonKhac));
            Rules.Check(m.MonHocIds.Count > 0, "Chọn ít nhất một môn.");
            Rules.Check(await db.Set<MonHoc>().CountAsync(x => m.MonHocIds.Contains(x.Id) && x.DangSuDung) == m.MonHocIds.Distinct().Count(), "Môn không hợp lệ.");
            var h = await db.Set<HoSoGiaSu>().SingleOrDefaultAsync(x => x.GiaSuId == u.Id);
            if (h == null)
            {
                h = new()
                {
                    Id = Guid.NewGuid(),
                    GiaSuId = u.Id,
                    TrangThaiDuyet = "ChoDuyet"
                };
                db.Add(h);
            }
            else
            {
                Rules.Concurrency(db, h, m.RowVersion);
                db.Entry(h).Property(x => x.TrinhDo).IsModified = true;
            }
            Rules.Check(!await db.Set<NhuCauHoc>().AnyAsync(x => x.ChuSoHuuId == u.Id && x.TrangThai == "DangMo" && !m.MonHocIds.Contains(x.MonHocId)), "Đóng nhu cầu đang mở trước khi bỏ môn khỏi hồ sơ.");
            var existing = await db.Set<TepDinhKem>().Where(x => x.HoSoGiaSuId == h.Id).ToListAsync();
            var removed = m.XoaTepIds.Distinct().ToList();
            Rules.Check(existing.Count(x => removed.Contains(x.Id)) == removed.Count, "Không có quyền xóa tệp này.", 403);
            Rules.Check(existing.Count - removed.Count + uploads.Count <= 5, "Hồ sơ có tối đa 5 tệp bằng cấp.");
            h.TrinhDo = m.TrinhDo;
            h.ChuyenMon = m.ChuyenMon;
            h.KinhNghiem = m.KinhNghiem;
            h.MoTa = m.MoTa;
            h.KhuVuc = m.KhuVuc;
            var person = await db.Set<NguoiThamGia>().SingleAsync(x => x.Id == u.Id);
            person.HoTen = m.HoTen;
            await db.SaveChangesAsync();
            db.RemoveRange(await db.Set<HoSoGiaSuMonDay>().Where(x => x.HoSoGiaSuId == h.Id && !m.MonHocIds.Contains(x.MonHocId)).ToListAsync());
            await db.SaveChangesAsync();
            foreach (var id in m.MonHocIds.Distinct())
            {
                var row = await db.Set<HoSoGiaSuMonDay>().SingleOrDefaultAsync(x => x.HoSoGiaSuId == h.Id && x.MonHocId == id);
                if (row == null)
                    db.Add(new HoSoGiaSuMonDay { HoSoGiaSuId = h.Id, MonHocId = id, PhamViDay = m.PhamViDay });
                else
                    row.PhamViDay = m.PhamViDay;
            }
            var deleting = existing.Where(x => removed.Contains(x.Id)).ToList();
            retired = deleting.Select(x => x.TenLuuTru).ToList();
            db.RemoveRange(deleting);
            written = await files.LuuAsync(uploads, hoSo: h.Id);
            await db.SaveChangesAsync();
            await tx.CommitAsync();
        }
        catch { TepService.DonDep(written); throw; }
        files.DonDepDaXoa(retired);
    }
}

