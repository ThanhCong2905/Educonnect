using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Infrastructure;
public sealed class EfThongBaoData(AppDbContext db):IThongBaoData
{
    public Task<LopHoc> ReadClassAsync(Guid id)=>db.Set<LopHoc>().SingleAsync(x=>x.Id==id);
    public Task<List<Guid>> ReadEligibleMemberIdsAsync(Guid lop)=>db.Set<ThanhVienLopHoc>()
        .Where(x=>x.LopHocId==lop&&x.TrangThaiDuyet=="DaDuyet"&&x.RoiLopLuc==null
            &&db.Set<LienKetPhuHuynhHocSinh>().Any(l=>l.Id==x.LienKetId&&l.TrangThai=="DaLienKet"&&l.KetThucLuc==null))
        .Select(x=>x.TaiKhoanId).ToListAsync();
    public void AddEvent(SuKienChoXuLy ev)=>db.Add(ev);
    public void AddNotice(ThongBao notice)=>db.Add(notice);
}
