using System.Text.Json;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xunit;
internal static partial class Cases
{
 public static async Task SqlContracts(Fixture f,string key)
 {
  switch(key){
  case "db_role":{
   await using var db=f.Db();var role=await db.Roles.SingleAsync(x=>x.Name=="PhuHuynh");
   var e=await Assert.ThrowsAsync<SqlException>(()=>f.Sql("INSERT dbo.AspNetUserRoles(UserId,RoleId) VALUES(@u,@r)",new(){{"@u",f.H},{"@r",role.Id}},true));Assert.Contains(e.Number,new[]{2601,2627});Assert.Equal(1,await db.UserRoles.CountAsync(x=>x.UserId==f.H));Assert.False(await db.Set<PhuHuynh>().AnyAsync(x=>x.TaiKhoanId==f.H));break;}
  case "db_need":{
   await f.SeedNeed();
   Guid needWithoutLink;
   await using(var seed=f.Db()){
    var n=new NhuCauHoc{Id=Guid.NewGuid(),ChuSoHuuId=f.H,LoaiNhuCau="TimGiaSu",MonHocId=f.M1,KhoiLop="Lớp 8",HocLuc="Trung bình",MucTieu=f.Tag,SoBuoiTuan=1,HinhThucHoc="TrucTuyen",HocPhiDuKien=150000,DonViHocPhi="VND/Gio",TrangThai="DangMo",TaoLuc=DateTimeOffset.UtcNow};
    seed.Add(n);await seed.SaveChangesAsync();needWithoutLink=n.Id;
   }
   await using(var pre=f.Db()) f.Proof["IT002_Precondition"] = new {
    DL01_NeedExists=await pre.Set<NhuCauHoc>().AnyAsync(x=>x.Id==f.Need),
    DL01_SubjectExists=await pre.Set<MonHoc>().AnyAsync(x=>x.Id==f.M1),
    DL01_OriginalLinkCount=await pre.Set<NhuCauMonHoc>().CountAsync(x=>x.NhuCauId==f.Need&&x.MonHocId==f.M1),
    DL02_NeedExists=await pre.Set<NhuCauHoc>().AnyAsync(x=>x.Id==needWithoutLink),
    DL02_InvalidSubjectExists=await pre.Set<MonHoc>().AnyAsync(x=>x.Id==int.MaxValue),
    DL02_InitialLinkCount=await pre.Set<NhuCauMonHoc>().CountAsync(x=>x.NhuCauId==needWithoutLink),
    DL01_NeedId=f.Need,DL02_NeedId=needWithoutLink,SubjectId=f.M1
   };
   foreach(var dl in new[]{("DL01",f.Need,f.M1,new[]{2601,2627}),("DL02",needWithoutLink,int.MaxValue,new[]{547})}){
    var e=await Assert.ThrowsAsync<SqlException>(()=>f.Sql("INSERT dbo.NhuCauMonHoc(NhuCauId,MonHocId) VALUES(@n,@m)",new(){{"@n",dl.Item2},{"@m",dl.Item3}},true));
    f.Proof["IT002_"+dl.Item1]=new { NeedId=dl.Item2,SubjectId=dl.Item3,ExpectedNumbers=dl.Item4,ActualNumber=e.Number,Message=e.Message,Errors=e.Errors.Cast<SqlError>().Select(x=>new{x.Number,x.State,x.Class,x.Procedure,x.LineNumber,x.Message}).ToArray() };
    Assert.Contains(e.Number,dl.Item4);
   }
   await using var db=f.Db();
   Assert.Single(await db.Set<NhuCauMonHoc>().Where(x=>x.NhuCauId==f.Need).ToListAsync());
   Assert.Empty(await db.Set<NhuCauMonHoc>().Where(x=>x.NhuCauId==needWithoutLink).ToListAsync());
   break;}
  case "db_contact":{
   await f.SeedContact(false);await f.SeedNeed();await ProposalSqlReject(f,f.M1);await f.Sql("UPDATE dbo.YeuCauLienHe SET TrangThai='ChapNhan',PhanHoiLuc=SYSDATETIMEOFFSET(),NguoiPhanHoiId=GiaSuId WHERE Id=@id",new(){{"@id",f.Contact}});
   await ProposalSqlReject(f,int.MaxValue);await ProposalSqlReject(f,f.M2);await using var db=f.Db();Assert.False(await db.Set<YeuCauKetNoiLopHoc>().AnyAsync(x=>x.LienHeId==f.Contact));break;}
  case "db_accept":{
   await f.SeedProposal();await f.Sql("UPDATE dbo.YeuCauKetNoiLopHoc SET SoBuoiTuan=2 WHERE Id=@r; INSERT dbo.KhungGioDeXuat(YeuCauKetNoiId,ThuTrongTuan,GioBatDau,GioKetThuc) VALUES(@r,3,'19:00','20:00')",new(){{"@r",f.Request}});var args=await AcceptArgs(f);var a=await f.Sql(AcceptSql,args,true);var b=await f.Sql(AcceptSql,args,true);var ar=a.Single(x=>x.ContainsKey("Id"));var br=b.Single(x=>x.ContainsKey("Id"));Assert.Equal(ar["Id"],br["Id"]);f.Class=(Guid)ar["Id"]!;await ClassGraph(f,2);break;}
  case "db_accept_fail":{
   for(int dl=0;dl<3;dl++) {await using var d=new Fixture(f.Id+"-DL"+dl);await d.Init();await d.SeedProposal();
    if(dl==1)await d.Sql("DELETE dbo.KhungGioDeXuat WHERE YeuCauKetNoiId=@r",new(){{"@r",d.Request}});
    if(dl==2){await d.SeedClass();await d.Sql("UPDATE dbo.YeuCauKetNoiLopHoc SET TrangThai='DaChapNhan' WHERE Id=@r",new(){{"@r",d.Request}});await d.SeedProposal();await using var seed=d.Db();var proposal=await seed.Set<YeuCauKetNoiLopHoc>().SingleAsync(x=>x.Id==d.Request);var date=proposal.BatDauHocLuc!.Value.Date;while(date.DayOfWeek!=DayOfWeek.Monday)date=date.AddDays(1);var start=new DateTimeOffset(date.AddHours(19),proposal.BatDauHocLuc.Value.Offset);seed.Add(new LichHoc{Id=Guid.NewGuid(),LopHocId=d.Class,TieuDe="Collision",BatDauLuc=start,KetThucLuc=start.AddHours(1),TrangThai="DaLenLich"});await seed.SaveChangesAsync();}
    var args=await AcceptArgs(d);if(dl==0)args["@h"]=d.X;
    var error=await Assert.ThrowsAsync<SqlException>(()=>d.Sql(AcceptSql,args,true));Assert.InRange(error.Number,51130,51164);
    await using var db=d.Db();Assert.False(await db.Set<LopHoc>().AnyAsync(x=>x.YeuCauKetNoiId==d.Request));Assert.Equal("ChoBenHoc",(await db.Set<YeuCauKetNoiLopHoc>().SingleAsync(x=>x.Id==d.Request)).TrangThai);
   }break;}
  case "db_schedule":{
   await f.SeedClass();var start=DateTimeOffset.UtcNow.AddDays(4);var slots=new[]{new{TieuDe="Lịch A",BatDauLuc=start,KetThucLuc=start.AddHours(1)},new{TieuDe="Lịch B",BatDauLuc=start.AddDays(1),KetThucLuc=start.AddDays(1).AddHours(1)}};var args=new Dictionary<string,object?>{{"@c",f.Class},{"@g",f.G},{"@j",JsonSerializer.Serialize(slots)}};await f.Sql("EXEC dbo.usp_ThemLichHocHangLoat @LopHocId=@c,@GiaSuId=@g,@DanhSachBuoiJson=@j",args,true);await using var db=f.Db();var ids=await db.Set<LichHoc>().Where(x=>x.LopHocId==f.Class).Select(x=>x.Id).ToListAsync();Assert.Equal(2,ids.Count);Assert.Equal(2,await db.Set<BuoiHoc>().CountAsync(x=>ids.Contains(x.LichHocId)));break;}
  case "db_lesson":{
   await f.SeedLesson();var args=new Dictionary<string,object?>{{"@s",f.Schedule},{"@g",f.H},{"@v",await f.Version<BuoiHoc>(f.Lesson)}};
   const string sql="EXEC dbo.usp_GhiNhanBuoiHoc @LichHocId=@s,@GiaSuId=@g,@KetQuaBuoiHoc='DaDienRa',@NhanXet=N'Hoàn thành',@TrangThaiGiaSu='CoMat',@TrangThaiBenHoc='CoMat',@NoiDung=N'Đại số',@RowVersionCu=@v";
   var e=await Assert.ThrowsAsync<SqlException>(()=>f.Sql(sql,args,true));Assert.Equal(51146,e.Number);args["@g"]=f.G;await f.Sql(sql,args,true);await using var db=f.Db();var b=await db.Set<BuoiHoc>().SingleAsync(x=>x.Id==f.Lesson);Assert.Equal("DaHoc",b.TrangThai);Assert.Equal(f.G,b.NguoiGhiNhanId);Assert.Equal("Đại số",b.NoiDungGiangDay);break;}
  case "db_submit":{
   await f.SeedAssignment();var op=Guid.NewGuid();var a=await SubmitSql(f,op);var b=await SubmitSql(f,op);Assert.Equal(a[0]["Id"],b[0]["Id"]);await SubmitSql(f,Guid.NewGuid());await using var db=f.Db();var rows=await db.Set<BaiTapNop>().Where(x=>x.BaiTapGiaoId==f.Assignment).OrderBy(x=>x.LanNop).ToListAsync();Assert.Equal(new[]{1,2},rows.Select(x=>x.LanNop));break;}
  case "db_grade":{
   await f.SeedAssignment();var a=await SubmitSql(f,Guid.NewGuid());var b=await SubmitSql(f,Guid.NewGuid());var old=(Guid)a[0]["Id"]!;var latest=(Guid)b[0]["Id"]!;await Assert.ThrowsAsync<SqlException>(()=>GradeSql(f,old,f.G,8));await Assert.ThrowsAsync<SqlException>(()=>GradeSql(f,latest,f.H,8));foreach(var score in new[]{10.01m})await Assert.ThrowsAsync<SqlException>(()=>GradeSql(f,latest,f.G,score));await GradeSql(f,latest,f.G,8.25m);await using var db=f.Db();var k=await db.Set<KetQuaChamBai>().SingleAsync(x=>x.BaiTapNopId==latest);Assert.Equal(8.25m,k.Diem);Assert.False(await db.Set<KetQuaChamBai>().AnyAsync(x=>x.BaiTapNopId==old));break;}
  case "db_auth":{
   var expectedMinutes=new[]{0,1,5,15};
   for(int dl=0;dl<4;dl++){var account=f.Tag+dl+"@example.invalid";var ip=f.Tag+dl;
    await f.Sql("INSERT dbo.TrangThaiThuDangNhap(LoaiKhoa,GiaTriKhoa,SoLanSai,LanSaiCuoiLuc) VALUES('TaiKhoan',@a,@n,SYSDATETIMEOFFSET()),('DiaChiIp',@ip,@n,SYSDATETIMEOFFSET())",new(){{"@a",account},{"@ip",ip},{"@n",4+dl}});
    await f.Sql("EXEC dbo.usp_GhiNhanDangNhapSai @TaiKhoanChuanHoa=@a,@DiaChiIp=@ip",new(){{"@a",account},{"@ip",ip}},true);
    await using var db=f.Db();var rows=await db.Set<TrangThaiThuDangNhap>().Where(x=>x.GiaTriKhoa==account||x.GiaTriKhoa==ip).ToListAsync();Assert.Equal(2,rows.Count);
    foreach(var r in rows){Assert.Equal(5+dl,r.SoLanSai);var delay=(r.ChoDenLuc-r.LanSaiCuoiLuc)?.TotalSeconds??0;f.Proof["DL"+dl]=new{expectedSeconds=expectedMinutes[dl]*60,actualSeconds=delay};Assert.Equal(expectedMinutes[dl]*60,delay);}
   }break;}
  case "db_session":{
   await using var db=f.Db();var u=await db.Users.SingleAsync(x=>x.Id==f.H);var rows=await f.Sql("EXEC dbo.usp_DangNhapThanhCong @TaiKhoanId=@u,@TaiKhoanChuanHoa=@a,@DiaChiIp=@ip",new(){{"@u",f.H},{"@a",u.NormalizedEmail},{"@ip",f.Tag}},true);var id=(Guid)rows[0]["PhienDangNhapId"]!;
   async Task<bool> Valid(Guid owner){var r=await f.Sql("EXEC dbo.usp_KiemTraPhien @PhienId=@p,@TaiKhoanId=@u,@LaHoatDongChuDong=0",new(){{"@p",id},{"@u",owner}},true);return (bool)r[0]["HopLe"]!;}
   Assert.True(await Valid(f.H));Assert.False(await Valid(f.X));await f.Sql("UPDATE dbo.PhienDangNhap SET TaoLuc=DATEADD(hour,-2,SYSDATETIMEOFFSET()),HoatDongCuoiLuc=DATEADD(minute,-31,SYSDATETIMEOFFSET()) WHERE Id=@p",new(){{"@p",id}});Assert.False(await Valid(f.H));await f.Sql("UPDATE dbo.PhienDangNhap SET HoatDongCuoiLuc=SYSDATETIMEOFFSET(),ThuHoiLuc=SYSDATETIMEOFFSET() WHERE Id=@p",new(){{"@p",id}});Assert.False(await Valid(f.H));break;}
  case "db_audit":{
   var auditId=Guid.NewGuid();await f.Sql("INSERT dbo.NhatKyAnToan(Id,ThoiDiem,Nhom,SuKien,KetQua,DuongDan,PhuongThuc,LyDo,DiaChiIp,MaYeuCau) VALUES(@id,SYSDATETIMEOFFSET(),'Authorization','IT.db_audit','TuChoi',N'/integration/audit','POST',N'fixture',N'127.0.0.1',@request)",new(){{"@id",auditId},{"@request",Guid.NewGuid()}},true);
   var selected=await f.Sql("SELECT Id,Nhom,SuKien,KetQua,DuongDan FROM dbo.NhatKyAnToan WHERE Id=@id",new(){{"@id",auditId},{"@request",Guid.NewGuid()}},true);Assert.Single(selected);Assert.Equal(auditId,selected[0]["Id"]);Assert.Equal("Authorization",selected[0]["Nhom"]);Assert.Equal("TuChoi",selected[0]["KetQua"]);Assert.Equal("/integration/audit",selected[0]["DuongDan"]);
   foreach(var table in new[]{"NhatKyAnToan","NhatKyQuanTri"})foreach(var verb in new[]{"UPDATE","DELETE"}){var sql=verb=="UPDATE"?$"UPDATE dbo.{table} SET Id=Id WHERE 1=0":$"DELETE dbo.{table} WHERE 1=0";var e=await Assert.ThrowsAsync<SqlException>(()=>f.Sql(sql,app:true));Assert.Equal(229,e.Number);}break;}
  case "db_orm":{
   await f.SeedClass();await using var d1=f.Db(true);await using var d2=f.Db(true);var a=await d1.Set<LopHoc>().SingleAsync(x=>x.Id==f.Class);var b=await d2.Set<LopHoc>().SingleAsync(x=>x.Id==f.Class);Assert.Equal(8,a.RowVersion.Length);a.TrangThai="DaKetThuc";a.KetThucLuc=DateTimeOffset.UtcNow.AddSeconds(-5);a.CachKetThuc="ThoaThuan";await d1.SaveChangesAsync();b.TenLop="Không được ghi đè";await Assert.ThrowsAsync<DbUpdateConcurrencyException>(()=>d2.SaveChangesAsync());await using var fresh=f.Db();var r=await fresh.Set<LopHoc>().SingleAsync(x=>x.Id==f.Class);Assert.Equal("DaKetThuc",r.TrangThai);Assert.NotNull(r.KetThucLuc);Assert.Equal("ThoaThuan",r.CachKetThuc);break;}
  case "db_schema_cols":{
   var rows=await f.Sql("SELECT t.name AS TableName,c.name AS ColumnName,c.is_nullable AS IsNullable FROM sys.columns c JOIN sys.tables t ON t.object_id=c.object_id WHERE t.name IN ('LopHoc','YeuCauHuyKetNoi') AND c.name IN ('KetThucLuc','CachKetThuc','NguoiPhanHoiId')");
   Assert.Contains(rows,x=>Equals(x["TableName"],"LopHoc")&&Equals(x["ColumnName"],"KetThucLuc"));
   Assert.Contains(rows,x=>Equals(x["TableName"],"LopHoc")&&Equals(x["ColumnName"],"CachKetThuc"));
   Assert.Contains(rows,x=>Equals(x["TableName"],"YeuCauHuyKetNoi")&&Equals(x["ColumnName"],"NguoiPhanHoiId"));
   await f.SeedClass();await using var db=f.Db();var row=await db.Set<LopHoc>().SingleAsync(x=>x.Id==f.Class);Assert.Equal("DangHoatDong",row.TrangThai);Assert.Null(row.KetThucLuc);break;}
  case "db_class_unique":{
   await f.SeedClass();await using(var db=f.Db()){var src=await db.Set<LopHoc>().AsNoTracking().SingleAsync(x=>x.Id==f.Class);var duplicate=new LopHoc{Id=Guid.NewGuid(),YeuCauKetNoiId=src.YeuCauKetNoiId,GiaSuId=src.GiaSuId,BenHocId=src.BenHocId,TenLop=src.TenLop+" duplicate",MonHocId=src.MonHocId,MucTieu=src.MucTieu,SoBuoiTuan=src.SoBuoiTuan,ThoiLuongPhut=src.ThoiLuongPhut,HinhThucHoc=src.HinhThucHoc,HocPhi=src.HocPhi,TrangThai=src.TrangThai,DonViHocPhi=src.DonViHocPhi,TenNguoiHoc=src.TenNguoiHoc,TuoiNguoiHoc=src.TuoiNguoiHoc};db.Add(duplicate);var error=await Assert.ThrowsAsync<DbUpdateException>(()=>db.SaveChangesAsync());Assert.Contains("2601",error.ToString());}
   var idx=await f.Sql("SELECT i.name AS IndexName,i.is_unique AS IsUnique FROM sys.indexes i WHERE i.object_id=OBJECT_ID('dbo.LopHoc') AND i.name='UQ_YeuCau_LopHoc'");Assert.True(Convert.ToBoolean(Assert.Single(idx)["IsUnique"]));await using var verify=f.Db();Assert.Single(await verify.Set<LopHoc>().Where(x=>x.YeuCauKetNoiId==f.Request).ToListAsync());break;}
  case "db_class_paused":{
   await f.SeedClass();var update=await f.Sql("UPDATE dbo.LopHoc SET TrangThai='TamDung' WHERE Id=@id",new(){{"@id",f.Class}},true);Assert.Empty(update);await using var db=f.Db();var row=await db.Set<LopHoc>().SingleAsync(x=>x.Id==f.Class);Assert.Equal("TamDung",row.TrangThai);var check=await f.Sql("SELECT definition FROM sys.check_constraints WHERE name='CK_LopHoc_5'");Assert.Contains("TamDung",Convert.ToString(Assert.Single(check)["definition"]),StringComparison.OrdinalIgnoreCase);break;}
  default:throw new InvalidOperationException(key);
  }
 }
 internal const string AcceptSql="EXEC dbo.usp_ChapNhanDeXuatTaoLop @YeuCauId=@r,@BenHocId=@h,@RowVersionCu=@v,@NguoiThucHienId=@h";
 internal static async Task<Dictionary<string,object?>> AcceptArgs(Fixture f)=>new(){{"@r",f.Request},{"@h",f.H},{"@v",await f.Version<YeuCauKetNoiLopHoc>(f.Request)}};
 internal static async Task ClassGraph(Fixture f,int expectedLessons=1){await using var db=f.Db();Assert.Equal(1,await db.Set<LopHoc>().CountAsync(x=>x.YeuCauKetNoiId==f.Request));var ids=await db.Set<LichHoc>().Where(x=>x.LopHocId==f.Class).Select(x=>x.Id).ToListAsync();Assert.Equal(expectedLessons,ids.Count);Assert.Equal(expectedLessons,await db.Set<BuoiHoc>().CountAsync(x=>ids.Contains(x.LichHocId)));Assert.Equal(2,await db.Set<ThanhVienLopHoc>().CountAsync(x=>x.LopHocId==f.Class));Assert.Single(await db.Set<CuocHoiThoai>().Where(x=>x.LopHocId==f.Class).ToListAsync());Assert.Equal("DaChapNhan",(await db.Set<YeuCauKetNoiLopHoc>().SingleAsync(x=>x.Id==f.Request)).TrangThai);}
 internal static async Task ProposalSqlReject(Fixture f,int subject){var sql="EXEC dbo.usp_GuiDeXuatLop @LienHeId=@c,@GiaSuId=@g,@TenLop=N'Test',@MonHocId=@m,@MucTieu=N'Toán',@SoBuoiTuan=1,@ThoiLuongPhut=60,@BatDauHocLuc=@start,@HinhThucHoc='TrucTuyen',@HocPhi=150000,@DonViHocPhi='VND/Gio',@KhungGioJson=@j,@NhuCauId=@n,@NguoiThucHienId=@g,@LoiNhan=N'Đề xuất'";var e=await Assert.ThrowsAsync<SqlException>(()=>f.Sql(sql,new(){{"@c",f.Contact},{"@g",f.G},{"@m",subject},{"@start",DateTimeOffset.UtcNow.AddDays(7)},{"@j","[{\"ThuTrongTuan\":1,\"GioBatDau\":\"19:00\",\"GioKetThuc\":\"20:00\"}]"},{"@n",f.Need}},true));Assert.InRange(e.Number,51130,51164);}
 internal static Task<List<Dictionary<string,object?>>> SubmitSql(Fixture f,Guid op)=>f.Sql("EXEC dbo.usp_NopBaiTap @BaiTapGiaoId=@a,@NguoiNopId=@u,@MaThaoTac=@m,@NoiDung=N'Bài làm'",new(){{"@a",f.Assignment},{"@u",f.H},{"@m",op}},true);
 internal static Task<List<Dictionary<string,object?>>> GradeSql(Fixture f,Guid submission,Guid actor,decimal score)=>f.Sql("EXEC dbo.usp_ChamBaiTap @BaiTapNopId=@n,@NguoiChamId=@u,@Diem=@d,@NhanXet=N'Nhận xét'",new(){{"@n",submission},{"@u",actor},{"@d",score}},true);
}
