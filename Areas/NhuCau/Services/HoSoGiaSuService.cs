using System.Data;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Areas.NhuCau.Services;

public sealed class HoSoGiaSuService(AppDbContext db, NguoiDungMauService current, MonHocCatalog catalog, TepService files, ThongBaoService notices)
{
    public static string ChuanHoa(string? text) => string.Join(" ", (text ?? "").Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    public static void KiemTraSua(HoSoGiaSu h)
    {
        Rules.Check(!h.DangHienThi, "Hãy ẩn hồ sơ trước khi chỉnh sửa.", 409);
        Rules.Check(h.TrangThaiDuyet != "ChoDuyet", "Hồ sơ đang chờ duyệt. Hãy hủy gửi duyệt trước khi sửa.", 409);
    }
    private async Task<NguoiThamGia> GiaSuAsync()
    {
        var u = await current.HienTaiAsync();
        Rules.Check(u.LoaiNguoi == "GiaSu", "Chỉ gia sư được quản lý hồ sơ.", 403);
        return u;
    }
    public async Task<HoSoInput> LayDeSuaAsync()
    {
        var u = await GiaSuAsync();
        var h = await db.Set<HoSoGiaSu>().AsNoTracking().SingleOrDefaultAsync(x => x.GiaSuId == u.Id);
        if (h == null) return new() { HoTen = u.HoTen };
        return new()
        {
            HoTen=u.HoTen, RowVersion=Convert.ToBase64String(h.RowVersion), TrinhDo=h.TrinhDo,
            ChuyenMon=h.ChuyenMon, KinhNghiem=h.KinhNghiem, MoTa=h.MoTa, KhuVuc=h.KhuVuc,
            DangHienThi=h.DangHienThi, TrangThaiDuyet=h.TrangThaiDuyet, LyDoTuChoi=h.LyDoTuChoi,
            DayTrucTuyen=h.DayTrucTuyen, DayTrucTiep=h.DayTrucTiep,
            MonDay=await db.Set<HoSoGiaSuMonDay>().Where(x=>x.HoSoGiaSuId==h.Id).OrderBy(x=>x.MonHocId)
                .Select(x=>new MonDayInput{MonHocId=x.MonHocId,PhamViDay=x.PhamViDay,HocPhiDeXuat=x.HocPhiDeXuat,DonViHocPhi=x.DonViHocPhi}).ToListAsync()
        };
    }
    public async Task CapNhatAsync(HoSoInput m)
    {
        var u=await GiaSuAsync();
        Rules.Check(new[]{m.HoTen,m.TrinhDo,m.ChuyenMon,m.KinhNghiem,m.KhuVuc}.All(x=>!string.IsNullOrWhiteSpace(x)&&x.Length<=256),"Hoàn thiện thông tin hồ sơ, tối đa 256 ký tự mỗi trường.");
        Rules.Check(!string.IsNullOrWhiteSpace(m.MoTa)&&m.MoTa.Length<=5000,"Nhập giới thiệu tối đa 5000 ký tự.");
        var uploads=await files.KiemTraAsync(m.Files);
        var written=new List<string>(); var retired=new List<string>();
        await using var tx=await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        await HoSoLock.KhoaAsync(db,u.Id);
        try
        {
            var h=await db.Set<HoSoGiaSu>().SingleOrDefaultAsync(x=>x.GiaSuId==u.Id);
            if(h!=null){KiemTraSua(h);Rules.Concurrency(db,h,m.RowVersion);}
            Rules.Check(m.DayTrucTuyen||m.DayTrucTiep,"Chọn ít nhất một hình thức dạy.");
            Rules.Check(m.MonDay.Count is >0 and <=10,"Chọn từ 1 đến 10 môn dạy.");
            foreach(var row in m.MonDay)
            {
                if(row.MonHocId==0)row.MonHocId=await catalog.LayHoacThemAsync(row.TenMonKhac);
                Rules.FeeCheck(row.HocPhiDeXuat,row.DonViHocPhi);
                Rules.Check(!string.IsNullOrWhiteSpace(row.PhamViDay)&&row.PhamViDay.Length<=256,"Nhập phạm vi dạy cho từng môn.");
            }
            Rules.Check(m.MonDay.Select(x=>x.MonHocId).Distinct().Count()==m.MonDay.Count,"Không thêm trùng cùng một môn.");
            var ids=m.MonDay.Select(x=>x.MonHocId).ToList();
            Rules.Check(await db.Set<MonHoc>().CountAsync(x=>ids.Contains(x.Id)&&x.DangSuDung)==ids.Count,"Môn học không hợp lệ hoặc đã ngừng sử dụng.");
            if(h==null){h=new(){Id=Guid.NewGuid(),GiaSuId=u.Id,TrangThaiDuyet="BanNhap"};db.Add(h);}
            var existing=await db.Set<TepDinhKem>().Where(x=>x.HoSoGiaSuId==h.Id).ToListAsync();
            var removed=m.XoaTepIds.Distinct().ToList();
            Rules.Check(existing.Count(x=>removed.Contains(x.Id))==removed.Count,"Không có quyền xóa tệp này.",403);
            Rules.Check(existing.Count-removed.Count+uploads.Count<=5,"Hồ sơ có tối đa 5 tệp bằng cấp.");
            h.TrinhDo=m.TrinhDo.Trim();h.ChuyenMon=m.ChuyenMon.Trim();h.KinhNghiem=m.KinhNghiem.Trim();
            h.MoTa=m.MoTa.Trim();h.KhuVuc=ChuanHoa(m.KhuVuc);h.DayTrucTuyen=m.DayTrucTuyen;h.DayTrucTiep=m.DayTrucTiep;
            h.TrangThaiDuyet="BanNhap";h.DangHienThi=false;h.NguoiDuyetId=null;h.DuyetLuc=null;
            db.Entry(h).Property(x=>x.TrinhDo).IsModified=true;
            var person=await db.Set<NguoiThamGia>().SingleAsync(x=>x.Id==u.Id);person.HoTen=ChuanHoa(m.HoTen);
            await db.SaveChangesAsync();
            var old=await db.Set<HoSoGiaSuMonDay>().Where(x=>x.HoSoGiaSuId==h.Id).ToListAsync();
            db.RemoveRange(old.Where(x=>!ids.Contains(x.MonHocId)));
            foreach(var row in m.MonDay)
            {
                var entity=old.SingleOrDefault(x=>x.MonHocId==row.MonHocId);
                if(entity==null){entity=new(){HoSoGiaSuId=h.Id,MonHocId=row.MonHocId};db.Add(entity);}
                entity.PhamViDay=ChuanHoa(row.PhamViDay);entity.HocPhiDeXuat=row.HocPhiDeXuat;entity.DonViHocPhi=row.DonViHocPhi;
            }
            var deleting=existing.Where(x=>removed.Contains(x.Id)).ToList();retired=deleting.Select(x=>x.TenLuuTru).ToList();db.RemoveRange(deleting);
            written=await files.LuuAsync(uploads,hoSo:h.Id);
            await db.SaveChangesAsync();await tx.CommitAsync();
        }
        catch{TepService.DonDep(written);throw;}
        files.DonDepDaXoa(retired);
    }
    public async Task DoiTrangThaiAsync(string action,string version)
    {
        var u=await GiaSuAsync();await using var tx=await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        await HoSoLock.KhoaAsync(db,u.Id);
        var h=await db.Set<HoSoGiaSu>().SingleOrDefaultAsync(x=>x.GiaSuId==u.Id)??throw new LoiNghiepVu("Hãy lưu hồ sơ trước.");
        Rules.Concurrency(db,h,version);
        switch(action)
        {
            case "An":h.DangHienThi=false;break;
            case "Hien":
                Rules.Check(h.TrangThaiDuyet=="DaDuyet","Hồ sơ phải được duyệt trước khi bật hiển thị.");
                await DuDieuKienAsync(h);h.DangHienThi=true;break;
            case "GuiDuyet":
                KiemTraSua(h);await DuDieuKienAsync(h);h.TrangThaiDuyet="ChoDuyet";h.GuiDuyetLuc=DateTimeOffset.UtcNow;h.LyDoTuChoi=null;break;
            case "HuyDuyet":
                Rules.Check(h.TrangThaiDuyet=="ChoDuyet"&&!h.DangHienThi,"Hồ sơ không đang chờ duyệt.");h.TrangThaiDuyet="BanNhap";break;
            default:throw new LoiNghiepVu("Thao tác hồ sơ không hợp lệ.");
        }
        db.Entry(h).Property(x=>x.TrinhDo).IsModified=true;
        await db.SaveChangesAsync();await tx.CommitAsync();
    }
    public async Task DuDieuKienAsync(HoSoGiaSu h)
    {
        Rules.Check(!string.IsNullOrWhiteSpace(h.TrinhDo)&&!string.IsNullOrWhiteSpace(h.ChuyenMon)&&!string.IsNullOrWhiteSpace(h.KinhNghiem)&&!string.IsNullOrWhiteSpace(h.MoTa)&&!string.IsNullOrWhiteSpace(h.KhuVuc),"Hoàn thiện thông tin hồ sơ trước khi gửi duyệt hoặc hiển thị.");
        Rules.Check(h.DayTrucTuyen||h.DayTrucTiep,"Chọn ít nhất một hình thức dạy.");
        var rows=await (from d in db.Set<HoSoGiaSuMonDay>() join m in db.Set<MonHoc>() on d.MonHocId equals m.Id where d.HoSoGiaSuId==h.Id select new{d,m.DangSuDung}).ToListAsync();
        Rules.Check(rows.Count>0&&rows.All(x=>x.DangSuDung&&x.d.HocPhiDeXuat!=null&&!string.IsNullOrWhiteSpace(x.d.PhamViDay)),"Mỗi môn cần phạm vi, mức phí và đơn vị hợp lệ. Kiểm tra môn chưa có phí sau chuyển đổi.");
        foreach(var row in rows)Rules.FeeCheck(row.d.HocPhiDeXuat,row.d.DonViHocPhi);
    }
    private async Task QuanTriAsync()=>Rules.Check((await current.HienTaiAsync()).LoaiNguoi=="QuanTri","Chỉ quản trị được duyệt hồ sơ.",403);
    public async Task<List<HoSoGiaSu>> ChoDuyetAsync(){await QuanTriAsync();return await db.Set<HoSoGiaSu>().AsNoTracking().Where(x=>x.TrangThaiDuyet=="ChoDuyet").OrderBy(x=>x.GuiDuyetLuc).ToListAsync();}
    public async Task<HoSoInput> XemDuyetAsync(Guid id)
    {
        await QuanTriAsync();var h=await db.Set<HoSoGiaSu>().AsNoTracking().SingleOrDefaultAsync(x=>x.Id==id)??throw new LoiNghiepVu("Không có hồ sơ.",404);
        return new(){HoTen=await db.Set<NguoiThamGia>().Where(x=>x.Id==h.GiaSuId).Select(x=>x.HoTen).SingleAsync(),TrinhDo=h.TrinhDo,ChuyenMon=h.ChuyenMon,KinhNghiem=h.KinhNghiem,MoTa=h.MoTa,KhuVuc=h.KhuVuc,DayTrucTuyen=h.DayTrucTuyen,DayTrucTiep=h.DayTrucTiep,TrangThaiDuyet=h.TrangThaiDuyet,RowVersion=Convert.ToBase64String(h.RowVersion),MonDay=await db.Set<HoSoGiaSuMonDay>().Where(x=>x.HoSoGiaSuId==id).Select(x=>new MonDayInput{MonHocId=x.MonHocId,PhamViDay=x.PhamViDay,HocPhiDeXuat=x.HocPhiDeXuat,DonViHocPhi=x.DonViHocPhi}).ToListAsync()};
    }
    public async Task DuyetAsync(Guid id,string version,bool approve,string? reason)
    {
        await QuanTriAsync();var admin=await current.HienTaiAsync();
        var tutor=await db.Set<HoSoGiaSu>().Where(x=>x.Id==id).Select(x=>(Guid?)x.GiaSuId).SingleOrDefaultAsync()??throw new LoiNghiepVu("Không có hồ sơ.",404);
        await using var tx=await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        await HoSoLock.KhoaAsync(db,tutor);
        var h=await db.Set<HoSoGiaSu>().SingleAsync(x=>x.Id==id);Rules.Concurrency(db,h,version);
        Rules.Check(h.TrangThaiDuyet=="ChoDuyet"&&!h.DangHienThi,"Hồ sơ không còn chờ duyệt. Hãy tải lại.",409);
        Rules.Check(approve||(!string.IsNullOrWhiteSpace(reason)&&reason.Length<=2000),"Nhập lý do yêu cầu chỉnh sửa, tối đa 2000 ký tự.");
        if(approve)await DuDieuKienAsync(h);
        h.TrangThaiDuyet=approve?"DaDuyet":"CanChinhSua";h.LyDoTuChoi=approve?null:reason!.Trim();h.DuyetLuc=DateTimeOffset.UtcNow;h.NguoiDuyetId=admin.Id;h.DangHienThi=false;
        notices.Tao(tutor,"HoSo",h.Id,approve?"Hồ sơ đã được duyệt. Bạn có thể bật hiển thị.":"Hồ sơ cần chỉnh sửa. Xem lý do trong Hồ sơ của tôi.");
        await db.SaveChangesAsync();await tx.CommitAsync();
    }
}
