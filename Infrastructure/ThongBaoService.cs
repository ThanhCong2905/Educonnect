using System.Text.Json;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Infrastructure;

public sealed class ThongBaoService(AppDbContext db, NguoiDungHienTaiService current)
{
    public async Task TaoChoLopAsync(Guid lopHocId, string type, Guid target, string text, Guid? loaiTru = null)
    {
        var lop = await db.Set<LopHoc>().SingleAsync(x => x.Id == lopHocId);
        var recipients = await db.Set<ThanhVienLopHoc>()
            .Where(x => x.LopHocId == lopHocId && x.TrangThaiDuyet == "DaDuyet" && x.RoiLopLuc == null
                && db.Set<LienKetPhuHuynhHocSinh>().Any(l => l.Id == x.LienKetId && l.TrangThai == "DaLienKet" && l.KetThucLuc == null))
            .Select(x => x.TaiKhoanId).ToListAsync();
        recipients.Add(lop.GiaSuId);
        recipients.Add(lop.BenHocId);
        foreach (var id in recipients.Distinct().Where(id => id != loaiTru))
            Tao(id, type, target, text);
    }

    // Sự kiện và thông báo được lưu trong cùng giao dịch nghiệp vụ.
    public void Tao(Guid recipient, string type, Guid target, string text)
    {
        var ev = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        db.Add(new SuKienChoXuLy { Id = ev, LoaiSuKien = type, DuLieuToiThieu = JsonSerializer.Serialize(new { recipient, target }), TaoLuc = now, TrangThai = "ChoXuLy" });
        db.Add(new ThongBao { Id = Guid.NewGuid(), NguoiNhanId = recipient, LoaiThongBao = type, DoiTuongId = target, NoiDungTomTat = text, TaoLuc = now, MaSuKien = ev });
    }
    public async Task<List<ThongBao>> LayAsync()
    {
        var me = await current.HienTaiAsync();
        return await db.Set<ThongBao>().AsNoTracking().Where(x => x.NguoiNhanId == me.Id).OrderByDescending(x => x.TaoLuc).Take(200).ToListAsync();
    }
    public async Task<int> ChuaDocAsync()
    {
        var me = await current.HienTaiAsync();
        return await db.Set<ThongBao>().CountAsync(x => x.NguoiNhanId == me.Id && x.DaDocLuc == null);
    }
    public async Task<Page<ThongBao>> PhanTrangAsync(int trang=1,string? state=null,string? q=null,string? loai=null)
    {
        Rules.Check(trang is >=1 and <=100000&&(q?.Length??0)<=100,"Trang hoặc từ khóa không hợp lệ.");
        Rules.Check(state==null||state is "ChuaDoc" or "DaDoc","Trạng thái đọc không hợp lệ.");
        Rules.Check(loai==null||Loai.Contains(loai),"Loại thông báo không hợp lệ.");
        var me=await current.HienTaiAsync();
        var query=db.Set<ThongBao>().AsNoTracking().Where(x=>x.NguoiNhanId==me.Id&&(q==null||x.NoiDungTomTat.Contains(q))
            &&(loai==null||x.LoaiThongBao==loai)&&(state==null||(state=="ChuaDoc"?x.DaDocLuc==null:x.DaDocLuc!=null)));
        return new Page<ThongBao> { Trang=trang,KichThuocTrang=20,Total=await query.CountAsync(),
            Items=await query.OrderByDescending(x=>x.TaoLuc).ThenBy(x=>x.Id).Skip((trang-1)*20).Take(20).ToListAsync() };
    }

    public static readonly string[] Loai=["KetNoi","LichHoc","BaiTap","ThanhVien","NhacViec","QuanTri","HoSo","LoiMoiLop","LienKetTaiKhoan","NhuCau","PhanHoi","DanhGiaDoiTac","BaoCao","KiemDuyet","TaiKhoan"];

    public async Task DocTatCaAsync()
    {
        var me=await current.HienTaiAsync();
        await using var tx=await db.Database.BeginTransactionAsync();
        var now=await GiaoDichNghiepVu.GioMayChuAsync(db);
        var changed=await db.Set<ThongBao>().Where(x=>x.NguoiNhanId==me.Id&&x.DaDocLuc==null).ExecuteUpdateAsync(s=>s.SetProperty(x=>x.DaDocLuc,now));
        if(changed>0)
            db.Add(new SuKienChoXuLy { Id=Guid.NewGuid(),LoaiSuKien="ThongBaoDaDoc",DuLieuToiThieu=JsonSerializer.Serialize(new { recipient=me.Id,target=Guid.Empty }),TaoLuc=now,TrangThai="ChoXuLy" });
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }
    public async Task<ThongBao> DocAsync(Guid id)
    {
        var me = await current.HienTaiAsync();
        var n = await db.Set<ThongBao>().SingleOrDefaultAsync(x => x.Id == id && x.NguoiNhanId == me.Id) ?? throw new LoiNghiepVu("Không tìm thấy thông báo.", 404);
        if (n.DaDocLuc == null)
        {
            n.DaDocLuc = DateTimeOffset.UtcNow;
            db.Add(new SuKienChoXuLy { Id = Guid.NewGuid(), LoaiSuKien = "ThongBaoDaDoc", DuLieuToiThieu = JsonSerializer.Serialize(new { recipient = me.Id, target = n.Id }), TaoLuc = DateTimeOffset.UtcNow, TrangThai = "ChoXuLy" });
        }
        await db.SaveChangesAsync();
        return n;
    }
}
