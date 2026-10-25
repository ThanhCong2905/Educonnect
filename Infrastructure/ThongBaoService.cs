using System.Text.Json;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
namespace EduConnect.Mvc.Infrastructure;
public sealed class ThongBaoService(AppDbContext db,NguoiDungMauService current) {
 // Called within the business transaction; an event and its inbox records commit together.
 public void Tao(Guid recipient,string type,Guid target,string text) {
  var ev=Guid.NewGuid(); var now=DateTimeOffset.UtcNow;
  db.Add(new SuKienChoXuLy { Id=ev,LoaiSuKien=type,DuLieuToiThieu=JsonSerializer.Serialize(new {recipient,target}),TaoLuc=now,TrangThai="HoanTat",XuLyXongLuc=now });
  db.Add(new ThongBao {Id=Guid.NewGuid(),NguoiNhanId=recipient,LoaiThongBao=type,DoiTuongId=target,NoiDungTomTat=text,TaoLuc=now,MaSuKien=ev});
 }
 public async Task<List<ThongBao>> LayAsync() { var me=await current.HienTaiAsync();return await db.Set<ThongBao>().AsNoTracking().Where(x=>x.NguoiNhanId==me.Id).OrderByDescending(x=>x.TaoLuc).Take(200).ToListAsync(); }
 public async Task<int> ChuaDocAsync(){var me=await current.HienTaiAsync();return await db.Set<ThongBao>().CountAsync(x=>x.NguoiNhanId==me.Id&&x.DaDocLuc==null);}
 public async Task<ThongBao> DocAsync(Guid id) {var me=await current.HienTaiAsync();var n=await db.Set<ThongBao>().SingleOrDefaultAsync(x=>x.Id==id&&x.NguoiNhanId==me.Id)??throw new LoiNghiepVu("Không tìm thấy thông báo.",404); n.DaDocLuc??=DateTimeOffset.UtcNow;await db.SaveChangesAsync();return n;}
}
public sealed class BaoTriWorker(IServiceScopeFactory scopes,ILogger<BaoTriWorker> log): BackgroundService {
 protected override async Task ExecuteAsync(CancellationToken token) { using var timer=new PeriodicTimer(TimeSpan.FromSeconds(30)); while(!token.IsCancellationRequested) {try {using var scope=scopes.CreateScope();var db=scope.ServiceProvider.GetRequiredService<AppDbContext>();
  await db.Database.ExecuteSqlRawAsync("UPDATE dbo.YeuCauKetNoiLopHoc SET TrangThai='HetHan' WHERE TrangThai IN ('ChoGiaSu','ChoBenHoc') AND HetHanLuc<=SYSDATETIMEOFFSET()",token);
  await db.Database.ExecuteSqlRawAsync("BEGIN TRAN; UPDATE c SET TrangThai='DaHuy' FROM dbo.LopHoc c JOIN dbo.YeuCauHuyKetNoi y ON y.LopHocId=c.Id WHERE c.TrangThai='DangHoatDong' AND y.TrangThai='ChoPhanHoi' AND y.HetHanLuc<=SYSDATETIMEOFFSET(); UPDATE dbo.YeuCauHuyKetNoi SET TrangThai='TuDongChapNhan',PhanHoiLuc=SYSDATETIMEOFFSET() WHERE TrangThai='ChoPhanHoi' AND HetHanLuc<=SYSDATETIMEOFFSET(); COMMIT;",token);
 } catch(Exception ex) when(!token.IsCancellationRequested) {log.LogWarning(ex,"Bảo trì chưa hoàn tất; sẽ thử lại.");} try {if(!await timer.WaitForNextTickAsync(token))break;}catch(OperationCanceledException){break;} } }
}

