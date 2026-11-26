using System.Data;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Areas.NhuCau.Services;

public sealed class HoSoGiaSuService(AppDbContext db, NguoiDungHienTaiService current, MonHocCatalog catalog, TepService files, ThongBaoService notices)
{
    public Task<KetQua<HoSoGiaSuCongKhaiViewModel>> LayHoSoAsync(Guid maHoSo, Guid? maNguoiXem, CancellationToken huy)
        => KetQua<HoSoGiaSuCongKhaiViewModel>.ThucHienAsync(async () =>
        {
            await current.NguoiXemAsync(maNguoiXem, huy);
            var query = new HoSoCongKhaiQuery(db);
            var profile = await query.CongKhai().SingleOrDefaultAsync(x => x.Id == maHoSo, huy)
                ?? throw new LoiNghiepVu("Hồ sơ đang ẩn, chưa được duyệt hoặc không còn công khai.", 404);
            return (await query.LayDanhSachAsync([profile], huy)).Single();
        });

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
        var subjects = await db.Set<HoSoGiaSuMonDay>().Where(x => x.HoSoGiaSuId == h.Id).OrderBy(x => x.MonHocId)
            .Select(x => new MonDayInput { MonHocId = x.MonHocId, PhamViDay = x.PhamViDay, HocPhiDeXuat = x.HocPhiDeXuat, DonViHocPhi = x.DonViHocPhi, MinhChungId = x.MinhChungId, PhanKhoaHoc = x.PhanKhoaHoc }).ToListAsync();
        foreach (var subject in subjects)
        {
            subject.LopDay = HoSoLuaChon.LopDay(subject.PhamViDay).Select(PhamViMonHoc.DocKhoi).Where(grade => grade > 0).ToList();
            subject.PhanTheoLop = subject.LopDay.SelectMany(grade => Enumerable.Range(0, 3)
                .Where(part => (subject.PhanKhoaHoc & PhamViMonHoc.BitPhan(grade, part)) != 0)
                .Select(part => grade * 10 + part + 1)).ToList();
        }
        return new()
        {
            HoTen=u.HoTen, RowVersion=Convert.ToBase64String(h.RowVersion), TrinhDo=h.TrinhDo,
            MinhChungTrinhDoId=h.MinhChungTrinhDoId, MinhChungKinhNghiemId=h.MinhChungKinhNghiemId,
            ChuyenMon=h.ChuyenMon, KinhNghiem=h.KinhNghiem, MoTa=h.MoTa, KhuVuc=h.KhuVuc,
            PhuongXa=h.PhuongXa, DiaChiChiTiet=h.DiaChiChiTiet,
            DangHienThi=h.DangHienThi, TrangThaiDuyet=h.TrangThaiDuyet, LyDoTuChoi=h.LyDoTuChoi,
            DayTrucTuyen=h.DayTrucTuyen, DayTrucTiep=h.DayTrucTiep,
            MonDay=subjects
        };
    }
    public async Task CapNhatAsync(HoSoInput m)
    {
        var u=await GiaSuAsync();
        Rules.ValidateInput(m);
        Rules.Check(HoSoLuaChon.TrinhDo.Contains(m.TrinhDo), "Chọn trình độ trong danh sách.", field: nameof(m.TrinhDo));
        Rules.Check(HoSoLuaChon.KinhNghiem.Contains(m.KinhNghiem), "Chọn kinh nghiệm trong danh sách.", field: nameof(m.KinhNghiem));
        Rules.Check(HoSoLuaChon.KhuVuc.Contains(m.KhuVuc), "Chọn tỉnh hoặc thành phố trong danh sách.", field: nameof(m.KhuVuc));
        Rules.Check(DiaGioiHanhChinh.HopLe(m.KhuVuc, m.PhuongXa), "Chọn phường / xã thuộc tỉnh / thành phố đã chọn.", field: nameof(m.PhuongXa));
        Rules.Check(m.MoTa.Trim().Length is >= 100 and <= 4000, "Giới thiệu cần từ 100 đến 4.000 ký tự.", field: nameof(m.MoTa));
        Rules.Check(m.TepTrinhDo?.Length > 0 || m.MinhChungTrinhDoId.HasValue && !m.XoaTepIds.Contains(m.MinhChungTrinhDoId.Value),
            "Tải minh chứng cho trình độ đã chọn trước khi lưu hồ sơ.", field: nameof(m.TepTrinhDo));
        Rules.Check(m.KinhNghiem == "Chưa có" || m.TepKinhNghiem?.Length > 0 || m.MinhChungKinhNghiemId.HasValue && !m.XoaTepIds.Contains(m.MinhChungKinhNghiemId.Value),
            "Tải minh chứng cho kinh nghiệm đã chọn trước khi lưu hồ sơ.", field: nameof(m.TepKinhNghiem));
        var uploads=await files.KiemTraAsync(m.Files);
        var degreeUpload = await KiemTraMinhChungAsync(m.TepTrinhDo, nameof(m.TepTrinhDo));
        var experienceUpload = await KiemTraMinhChungAsync(m.TepKinhNghiem, nameof(m.TepKinhNghiem));
        var written=new List<string>(); var retired=new List<string>();
        await using var tx=await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        await HoSoLock.KhoaAsync(db,u.Id);
        try
        {
            var h=await db.Set<HoSoGiaSu>().SingleOrDefaultAsync(x=>x.GiaSuId==u.Id);
            if(h!=null){KiemTraSua(h);Rules.Concurrency(db,h,m.RowVersion);}
            Rules.Check(m.DayTrucTuyen||m.DayTrucTiep,"Chọn ít nhất một hình thức dạy.");
            Rules.Check(m.MonDay.Count is >0 and <=10,"Chọn từ 1 đến 10 môn dạy.");
            for (var index = 0; index < m.MonDay.Count; index++)
            {
                var row = m.MonDay[index];
                Rules.Check(row.LopDay.Count > 0 && row.LopDay.All(grade => grade is >= 1 and <= 12), "Chọn ít nhất một lớp từ lớp 1 đến lớp 12.", field: $"MonDay[{index}].LopDay");
                row.PhamViDay = string.Join(", ", row.LopDay.Distinct().Order().Select(grade => $"Lớp {grade}"));
                try { Rules.ValidateInput(row); }
                catch (LoiNghiepVu error) { throw new LoiNghiepVu(error.Message, error.Status) { Truong = $"MonDay[{index}].{error.Truong}" }; }
                if(row.MonHocId==0)row.MonHocId=await catalog.LayHoacThemAsync(row.TenMonKhac);
                try { Rules.FeeCheck(row.HocPhiDeXuat,row.DonViHocPhi); }
                catch (LoiNghiepVu error) { throw new LoiNghiepVu(error.Message, error.Status) { Truong = $"MonDay[{index}].{(string.IsNullOrEmpty(row.DonViHocPhi) ? "DonViHocPhi" : "HocPhiDeXuat")}" }; }
                Rules.Check(row.HocPhiDeXuat == null || row.HocPhiDeXuat >= 0 && decimal.Truncate(row.HocPhiDeXuat.Value) == row.HocPhiDeXuat,
                    "Học phí phải là số VND nguyên không âm; nhập 0 nếu dạy miễn phí.", field: $"MonDay[{index}].HocPhiDeXuat");
                Rules.Check(HoSoLuaChon.PhamViHopLe(row.PhamViDay), "Chọn ít nhất một lớp từ lớp 1 đến lớp 12.", field: $"MonDay[{index}].LopDay");
                Rules.Check(!m.MonDay.Take(index).Any(previous => previous.MonHocId == row.MonHocId), "Môn này đã được chọn ở dòng trước.", field: $"MonDay[{index}].MonHocId");
            }
            Rules.Check(m.MonDay.Select(x=>x.MonHocId).Distinct().Count()==m.MonDay.Count,"Không thêm trùng cùng một môn.");
            var ids=m.MonDay.Select(x=>x.MonHocId).ToList();
            Rules.Check(await db.Set<MonHoc>().CountAsync(x=>ids.Contains(x.Id)&&x.DangSuDung)==ids.Count,"Môn học không hợp lệ hoặc đã ngừng sử dụng.");
            var subjects = await db.Set<MonHoc>().Where(x => ids.Contains(x.Id)).ToDictionaryAsync(x => x.Id);
            for (var index = 0; index < m.MonDay.Count; index++)
            {
                var row = m.MonDay[index];
                var name = subjects[row.MonHocId].TenMon;
                var (from, to) = PhamViMonHoc.Khoi(name);
                Rules.Check(row.LopDay.All(grade => grade >= from && grade <= to),
                    $"{name} chỉ áp dụng cho lớp {from}–{to}.", field: $"MonDay[{index}].LopDay");
                row.PhanKhoaHoc = 0;
                if (!PhamViMonHoc.LaKhoaHocTuNhien(name)) continue;
                var parts = row.PhanTheoLop.Distinct().ToList();
                Rules.Check(parts.All(p => row.LopDay.Contains(p / 10) && p % 10 is >= 1 and <= 3)
                    && row.LopDay.All(grade => parts.Any(p => p / 10 == grade)),
                    "Chọn ít nhất một phần kiến thức cho mỗi lớp Khoa học tự nhiên đã chọn.", field: $"MonDay[{index}].LopDay");
                row.PhanKhoaHoc = parts.Aggregate(0L, (mask, part) => mask | PhamViMonHoc.BitPhan(part / 10, part % 10 - 1));
            }
            if(h==null){h=new(){Id=Guid.NewGuid(),GiaSuId=u.Id,TrangThaiDuyet="BanNhap"};db.Add(h);}
            var existing=await db.Set<TepDinhKem>().Where(x=>x.HoSoGiaSuId==h.Id).ToListAsync();
            var removed=m.XoaTepIds.Distinct().ToList();
            Rules.Check(existing.Count(x=>removed.Contains(x.Id))==removed.Count,"Không có quyền xóa tệp này.",403);
            Rules.Check(existing.Count-removed.Count+uploads.Count+degreeUpload.Count+experienceUpload.Count<=5,"Hồ sơ có tối đa 5 tệp minh chứng. Bạn có thể chọn lại một tệp cho nhiều mục.",field:"Files");
            var retained = existing.Where(x => !removed.Contains(x.Id)).Select(x => x.Id).ToHashSet();
            if (degreeUpload.Count == 0)
                KiemTraTepDaLuu(m.MinhChungTrinhDoId, retained, nameof(m.MinhChungTrinhDoId));
            if (experienceUpload.Count == 0)
                KiemTraTepDaLuu(m.MinhChungKinhNghiemId, retained, nameof(m.MinhChungKinhNghiemId));
            h.TrinhDo=m.TrinhDo.Trim();h.KinhNghiem=m.KinhNghiem.Trim();
            h.MoTa=m.MoTa.Trim();h.KhuVuc=ChuanHoa(m.KhuVuc);h.DayTrucTuyen=m.DayTrucTuyen;h.DayTrucTiep=m.DayTrucTiep;
            h.PhuongXa=string.IsNullOrWhiteSpace(m.PhuongXa)?null:m.PhuongXa.Trim();
            h.DiaChiChiTiet=string.IsNullOrWhiteSpace(m.DiaChiChiTiet)?null:m.DiaChiChiTiet.Trim();
            h.TrangThaiDuyet="BanNhap";h.DangHienThi=false;h.NguoiDuyetId=null;h.DuyetLuc=null;
            h.LyDoTuChoi = null;
            db.Entry(h).Property(x=>x.TrinhDo).IsModified=true;
            var person=await db.Users.SingleAsync(x=>x.Id==u.Id);person.HoTen=ChuanHoa(m.HoTen);
            await db.SaveChangesAsync();
            h.MinhChungTrinhDoId = await LuuMinhChungAsync(degreeUpload, m.MinhChungTrinhDoId, h.Id, written);
            h.MinhChungKinhNghiemId = await LuuMinhChungAsync(experienceUpload, m.MinhChungKinhNghiemId, h.Id, written);
            var old=await db.Set<HoSoGiaSuMonDay>().Where(x=>x.HoSoGiaSuId==h.Id).ToListAsync();
            db.RemoveRange(old.Where(x=>!ids.Contains(x.MonHocId)));
            foreach(var row in m.MonDay)
            {
                var entity=old.SingleOrDefault(x=>x.MonHocId==row.MonHocId);
                if(entity==null){entity=new(){HoSoGiaSuId=h.Id,MonHocId=row.MonHocId};db.Add(entity);}
                entity.PhamViDay=ChuanHoa(row.PhamViDay);entity.HocPhiDeXuat=row.HocPhiDeXuat;entity.DonViHocPhi=row.DonViHocPhi;
                entity.MatNaKhoi = row.LopDay.Distinct().Aggregate(0L, (mask, grade) => mask | PhamViMonHoc.BitKhoi(grade));
                entity.PhanKhoaHoc = row.PhanKhoaHoc;
                if (entity.MinhChungId is Guid oldProof && removed.Contains(oldProof))
                    entity.MinhChungId = null;
            }
            var deleting=existing.Where(x=>removed.Contains(x.Id)).ToList();retired=deleting.Select(x=>x.TenLuuTru).ToList();db.RemoveRange(deleting);
            written.AddRange(await files.LuuAsync(uploads,hoSo:h.Id));
            await db.SaveChangesAsync();await tx.CommitAsync();
        }
        catch{TepService.DonDep(written);throw;}
        files.DonDepDaXoa(retired);
    }
    private async Task<List<(string Name, string Mime, byte[] Bytes)>> KiemTraMinhChungAsync(IFormFile? file, string field)
    {
        if (file == null) return [];
        try { return await files.KiemTraAsync([file]); }
        catch (LoiNghiepVu error) { throw new LoiNghiepVu(error.Message, error.Status) { Truong = field }; }
    }

    private static void KiemTraTepDaLuu(Guid? id, HashSet<Guid> retained, string field)
        => Rules.Check(!id.HasValue || retained.Contains(id.Value), "Chọn tệp còn được giữ trong hồ sơ của bạn.", field: field);

    private async Task<Guid?> LuuMinhChungAsync(List<(string Name, string Mime, byte[] Bytes)> upload, Guid? existing, Guid profile, List<string> written)
    {
        if (upload.Count == 0) return existing;
        var paths = await files.LuuAsync(upload, hoSo: profile);
        written.AddRange(paths);
        var key = Path.GetFileName(paths.Single());
        return db.ChangeTracker.Entries<TepDinhKem>().Single(entry => entry.Entity.TenLuuTru == key).Entity.Id;
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
                Rules.Check(h.TrangThaiDuyet == "DaDuyet", "Hồ sơ phải được quản trị duyệt trước khi bật hiển thị.");
                await DuDieuKienAsync(h);
                h.DangHienThi = true;
                break;
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
        Rules.Check(HoSoLuaChon.TrinhDo.Contains(h.TrinhDo), "Chọn lại trình độ trong danh sách trước khi gửi duyệt.", field: "TrinhDo");
        Rules.Check(HoSoLuaChon.KinhNghiem.Contains(h.KinhNghiem), "Chọn lại kinh nghiệm trong danh sách trước khi gửi duyệt.", field: "KinhNghiem");
        Rules.Check(h.MoTa.Trim().Length is >=100 and <=4000, "Giới thiệu cần từ 100 đến 4.000 ký tự.", field: "MoTa");
        Rules.Check(!string.IsNullOrWhiteSpace(h.KhuVuc), "Chọn khu vực nhận dạy.", field: "KhuVuc");
        var proofs = await db.Set<TepDinhKem>().Where(file => file.HoSoGiaSuId == h.Id && (file.TrangThaiQuet == "Sach" || files.ChoPhepTepChuaQuet && file.TrangThaiQuet == "ChoQuet")).Select(file=>file.Id).ToListAsync();
        Rules.Check(h.MinhChungTrinhDoId.HasValue && proofs.Contains(h.MinhChungTrinhDoId.Value), "Chọn minh chứng trình độ đã lưu trong hồ sơ.", field: "MinhChungTrinhDoId");
        Rules.Check(h.KinhNghiem == "Chưa có" || h.MinhChungKinhNghiemId.HasValue && proofs.Contains(h.MinhChungKinhNghiemId.Value), "Chọn minh chứng cho kinh nghiệm đã khai.", field: "MinhChungKinhNghiemId");
        Rules.Check(h.DayTrucTuyen||h.DayTrucTiep,"Chọn ít nhất một hình thức dạy.");
        var rows=await (from d in db.Set<HoSoGiaSuMonDay>() join m in db.Set<MonHoc>() on d.MonHocId equals m.Id where d.HoSoGiaSuId==h.Id select new{d,m.DangSuDung,m.TenMon}).ToListAsync();
        Rules.Check(rows.Count>0&&rows.All(x=>x.DangSuDung&&x.d.HocPhiDeXuat>=0&&HoSoLuaChon.PhamViHopLe(x.d.PhamViDay)),"Mỗi môn cần lớp dạy, học phí VND nguyên không âm và đơn vị hợp lệ.", field:"MonDay");
        foreach (var row in rows)
        {
            var (from, to) = PhamViMonHoc.Khoi(row.TenMon);
            var grades = HoSoLuaChon.LopDay(row.d.PhamViDay).Select(PhamViMonHoc.DocKhoi);
            Rules.Check(grades.All(grade => grade >= from && grade <= to),
                $"Cập nhật lại phạm vi lớp của môn {row.TenMon} trước khi gửi duyệt.", field: "MonDay");
            Rules.Check(!PhamViMonHoc.LaKhoaHocTuNhien(row.TenMon) || row.d.PhanKhoaHoc != 0,
                "Chọn các phần Khoa học tự nhiên bạn dạy trước khi gửi duyệt.", field: "MonDay");
            Rules.FeeCheck(row.d.HocPhiDeXuat, row.d.DonViHocPhi);
        }
    }
    private async Task QuanTriAsync()
    {
        var actor=await current.HienTaiAsync();
        Rules.Check(actor.LoaiNguoi=="QuanTri"&&await db.UserClaims.AnyAsync(x=>x.UserId==actor.Id&&x.ClaimType=="EduConnect.AdminPermission"&&x.ClaimValue=="DuyetHoSo"),"Bạn chưa được cấp quyền duyệt hồ sơ.",403);
    }
    public async Task<List<HoSoGiaSu>> ChoDuyetAsync(){await QuanTriAsync();return await db.Set<HoSoGiaSu>().AsNoTracking().Where(x=>x.TrangThaiDuyet=="ChoDuyet").OrderBy(x=>x.GuiDuyetLuc).ToListAsync();}
    public async Task<Page<HoSoGiaSu>> LayDanhSachDuyetAsync(int trang=1,string state="ChoDuyet",string? q=null)
    {
        await QuanTriAsync();
        Rules.Check(trang is >=1 and <=100000&&(q?.Length??0)<=100,"Trang hoặc từ khóa không hợp lệ.");
        Rules.Check(state is "ChoDuyet" or "CanChinhSua" or "DaDuyet" or "TuChoi","Trạng thái hồ sơ không hợp lệ.");
        var query=db.Set<HoSoGiaSu>().AsNoTracking().Where(x=>x.TrangThaiDuyet==state&&(q==null||db.Users.Any(u=>u.Id==x.GiaSuId&&u.HoTen.Contains(q))));
        return new Page<HoSoGiaSu> { Trang=trang,KichThuocTrang=20,Total=await query.CountAsync(),Items=await query.OrderBy(x=>x.GuiDuyetLuc).ThenBy(x=>x.Id).Skip((trang-1)*20).Take(20).ToListAsync() };
    }
    public async Task<HoSoInput> XemDuyetAsync(Guid id)
    {
        await QuanTriAsync();var h=await db.Set<HoSoGiaSu>().AsNoTracking().SingleOrDefaultAsync(x=>x.Id==id)??throw new LoiNghiepVu("Không có hồ sơ.",404);
        return new()
        {
            HoTen = await db.Users.Where(x => x.Id == h.GiaSuId).Select(x => x.HoTen).SingleAsync(),
            TrinhDo = h.TrinhDo, ChuyenMon = h.ChuyenMon, KinhNghiem = h.KinhNghiem,
            MoTa = h.MoTa, KhuVuc = h.KhuVuc, DayTrucTuyen = h.DayTrucTuyen, DayTrucTiep = h.DayTrucTiep,
            PhuongXa = h.PhuongXa, DiaChiChiTiet = h.DiaChiChiTiet,
            TrangThaiDuyet = h.TrangThaiDuyet, LyDoTuChoi = h.LyDoTuChoi, RowVersion = Convert.ToBase64String(h.RowVersion),
            MinhChungTrinhDoId = h.MinhChungTrinhDoId, MinhChungKinhNghiemId = h.MinhChungKinhNghiemId,
            MonDay = await db.Set<HoSoGiaSuMonDay>().Where(x => x.HoSoGiaSuId == id)
                .Select(x => new MonDayInput { MonHocId = x.MonHocId, PhamViDay = x.PhamViDay, HocPhiDeXuat = x.HocPhiDeXuat, DonViHocPhi = x.DonViHocPhi, MinhChungId = x.MinhChungId, PhanKhoaHoc = x.PhanKhoaHoc }).ToListAsync()
        };
    }
    public async Task DuyetAsync(Guid id,string version,bool approve,string? reason,bool tuChoi=false)
    {
        await QuanTriAsync();var admin=await current.HienTaiAsync();
        var tutor=await db.Set<HoSoGiaSu>().Where(x=>x.Id==id).Select(x=>(Guid?)x.GiaSuId).SingleOrDefaultAsync()??throw new LoiNghiepVu("Không có hồ sơ.",404);
        await using var tx=await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        await HoSoLock.KhoaAsync(db,tutor);
        await GiaoDichNghiepVu.KiemTraQuyenQuanTriAsync(db,admin.Id,"DuyetHoSo");
        var h=await db.Set<HoSoGiaSu>().SingleAsync(x=>x.Id==id);Rules.Concurrency(db,h,version);
        Rules.Check(h.TrangThaiDuyet=="ChoDuyet"&&!h.DangHienThi,"Hồ sơ không còn chờ duyệt. Hãy tải lại.",409);
        Rules.Check(approve||(!string.IsNullOrWhiteSpace(reason)&&reason.Length<=2000),"Nhập lý do yêu cầu chỉnh sửa, tối đa 2000 ký tự.");
        if(approve)
        {
            await GiaoDichNghiepVu.KiemTraTaiKhoanAsync(db,tutor);
            await DuDieuKienAsync(h);
        }
        Rules.Check(!approve||!tuChoi,"Quyết định duyệt không hợp lệ.");
        h.TrangThaiDuyet=approve?"DaDuyet":tuChoi?"TuChoi":"CanChinhSua";h.LyDoTuChoi=approve?null:reason!.Trim();h.DuyetLuc=DateTimeOffset.UtcNow;h.NguoiDuyetId=admin.Id;h.DangHienThi=false;
        db.Add(new NhatKyQuanTri { NguoiThucHienId=admin.Id,HanhDong="DuyetHoSo",LoaiDoiTuong="HoSoGiaSu",DoiTuongId=h.Id.ToString(),KetQua=h.TrangThaiDuyet,ThoiDiem=DateTimeOffset.UtcNow,MaLienKetYeuCau=Guid.NewGuid().ToString() });
        notices.Tao(tutor,"HoSo",h.Id,approve?"Hồ sơ đã được duyệt. Bạn có thể bật hiển thị.":tuChoi?"Hồ sơ chưa được chấp nhận. Xem lý do trong Hồ sơ của tôi.":"Hồ sơ cần chỉnh sửa. Xem lý do trong Hồ sơ của tôi.");
        await db.SaveChangesAsync();await tx.CommitAsync();
    }
}
