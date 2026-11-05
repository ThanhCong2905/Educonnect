using System.Diagnostics;
using System.Text.Json;
using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Areas.TimKiem.Services;
using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Areas.KetNoi.Services;
using EduConnect.Mvc.Areas.KetNoi.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xunit;
using EduConnect.Mvc.Areas.NhuCau.Controllers;
using EduConnect.Mvc.Areas.TimKiem.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

public class IntegrationTests
{
    public static IEnumerable<object[]> Cases => Enumerable.Range(1,59).Concat([60,76,77,78,79,80,81,83,84]).Concat(Enumerable.Range(147,21)).Select(n=>new object[]{n});
    [Theory][MemberData(nameof(Cases))]
    public async Task UC1_database_and_service(int n)
    {
        var cs=Environment.GetEnvironmentVariable("EDU_TEST_CONNECTION")??throw new Exception("EDU_TEST_CONNECTION required");
        Assert.StartsWith("EduConnect_Verification_",new SqlConnectionStringBuilder(cs).InitialCatalog);
        var watch=Stopwatch.StartNew();var id=n>=147?$"MT1-UT-{n-100:000}":$"MT1-IT-{n:000}";var status="Pass";var actual="";
        await using var db=new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(cs).Options);
        var http=new DefaultHttpContext{Session=new MemorySession()};
        var current=new NguoiDungMauService(db,new HttpContextAccessor{HttpContext=http});
        var needs=new NhuCauService(db,current,new MonHocCatalog(db));var search=new TimKiemService(db,current);
        var learner=Guid.NewGuid();var parent=Guid.NewGuid();var tutor=Guid.NewGuid();var profile=Guid.NewGuid();var tag="MT1_"+Guid.NewGuid().ToString("N");
        var proof=new Dictionary<string,object?>{{"caseId",id},{"database",new SqlConnectionStringBuilder(cs).InitialCatalog},{"tag",tag}};
        try
        {
            db.AddRange(new NguoiThamGia{Id=learner,HoTen=tag+" học sinh",LoaiNguoi="HocSinh"},new NguoiThamGia{Id=parent,HoTen=tag+" phụ huynh",LoaiNguoi="PhuHuynh"},new NguoiThamGia{Id=tutor,HoTen=tag+" gia sư",LoaiNguoi="GiaSu"});
            await db.SaveChangesAsync();db.Add(new GiaSu{TaiKhoanId=tutor});await db.SaveChangesAsync();
            db.Add(new HoSoGiaSu{Id=profile,GiaSuId=tutor,TrinhDo="Đại học",ChuyenMon=tag+" Toán",KinhNghiem="3 năm",MoTa="Ôn tập tiếng Việt",KhuVuc="Hà Nội",DangHienThi=true,TrangThaiDuyet="DaDuyet",DayTrucTuyen=true,DayTrucTiep=true});await db.SaveChangesAsync();
            db.Add(new HoSoGiaSuMonDay{HoSoGiaSuId=profile,MonHocId=1,PhamViDay="Lớp 10–12",HocPhiDeXuat=150000,DonViHocPhi="VND/Gio"});await db.SaveChangesAsync();
            await current.ChonAsync(learner);
            TaoNhuCauBenHocViewModel Input()=>new(){MonHocIds=[1],MucTieu=tag,KhoiLop="Lớp 10",SoBuoiTuan=2,HocPhiDuKien=150000,DonViHocPhi="VND/Gio",LichRanh=[new(){ThuTrongTuan=1,GioBatDau=new(18,0),GioKetThuc=new(20,0)}]};
            async Task<Guid> Create(TaoNhuCauBenHocViewModel? m=null,Guid? actor=null){var r=await needs.TaoNhuCauHocAsync(m??Input(),actor??learner,default);Assert.True(r.ThanhCong,$"Tạo thất bại: status={r.MaTrangThai}");return r.DuLieu;}
            async Task<int> Count()=>await db.Set<NhuCauHoc>().AsNoTracking().CountAsync(x=>x.ChuSoHuuId==learner||x.ChuSoHuuId==parent);
            async Task Reject(TaoNhuCauBenHocViewModel m,int expected=400,Guid? actor=null){var before=await Count();var r=await needs.TaoNhuCauHocAsync(m,actor??learner,default);Assert.False(r.ThanhCong);Assert.Equal(expected,r.MaTrangThai);db.ChangeTracker.Clear();Assert.Equal(before,await Count());proof["status"]=r.MaTrangThai;proof["unchangedRows"]=before;}
            async Task Exec(string sql,params SqlParameter[] p){await db.Database.OpenConnectionAsync();await using var command=db.Database.GetDbConnection().CreateCommand();command.CommandText=sql;command.Parameters.AddRange(p);await command.ExecuteNonQueryAsync();}
            async Task SqlBlocked(string sql,params SqlParameter[] p){var blocked=false;try{await Exec(sql,p);}catch(SqlException ex){proof["sqlNumber"]=ex.Number;blocked=ex.Number is 547 or 2601 or 2627 or >=50000;}Assert.True(blocked,"SQL đã chấp nhận dữ liệu mà Expected yêu cầu từ chối.");}
            var profileSet=new Dictionary<string,Guid>{{"P1",profile}};
            async Task ProfileFixture()
            {
                await db.Set<HoSoGiaSu>().Where(x=>x.Id==profile).ExecuteUpdateAsync(s=>s.SetProperty(x=>x.KhuVuc,"Thủ Đức"));
                foreach(var label in new[]{"P2","P3","P4","P5","P6","P7"})
                {
                    var u=Guid.NewGuid();var h=Guid.NewGuid();profileSet[label]=h;
                    db.Add(new NguoiThamGia{Id=u,HoTen=tag+" "+label,LoaiNguoi="GiaSu"});await db.SaveChangesAsync();db.Add(new GiaSu{TaiKhoanId=u});await db.SaveChangesAsync();
                    db.Add(new HoSoGiaSu{Id=h,GiaSuId=u,TrinhDo="Đại học",ChuyenMon=label=="P3"?"Văn":"Toán",KinhNghiem="3 năm",MoTa=tag,KhuVuc=label is "P2" or "P3"?"Hà Nội":"Thủ Đức",DayTrucTuyen=label!="P2",DayTrucTiep=label=="P2",DangHienThi=label!="P4"&&label!="P5",TrangThaiDuyet=label=="P5"?"ChoDuyet":"DaDuyet"});await db.SaveChangesAsync();
                    var monId=label=="P3"?2:1;
                    if(label=="P6"){var mon=new MonHoc{TenMon=tag+" ngừng dùng",DangSuDung=false};db.Add(mon);await db.SaveChangesAsync();monId=mon.Id;}
                    db.Add(new HoSoGiaSuMonDay{HoSoGiaSuId=h,MonHocId=monId,PhamViDay="Lớp 10–12",HocPhiDeXuat=label=="P2"?200000:label=="P7"?300000:120000,DonViHocPhi=label=="P2"?"VND/Buoi":"VND/Gio"});await db.SaveChangesAsync();
                    if(label=="P7"){db.Add(new HoSoGiaSuMonDay{HoSoGiaSuId=h,MonHocId=2,PhamViDay="Lớp 10–12",HocPhiDeXuat=100000,DonViHocPhi="VND/Gio"});await db.SaveChangesAsync();}
                }
                db.ChangeTracker.Clear();proof["profiles"]=profileSet;
            }
            string InsertNeed(string owner,string extra="")=>$"INSERT dbo.NhuCauHoc(Id,ChuSoHuuId,LoaiNhuCau,MonHocId,HinhThucHoc,TrangThai,TaoLuc{extra}) VALUES(NEWID(),'{owner}','TimGiaSu',1,'TrucTuyen','BanNhap',SYSDATETIMEOFFSET()";
            if(n<=13)
            {
                switch(n)
                {
                    case 1:
                        await db.Database.OpenConnectionAsync();await using(var cmd=db.Database.GetDbConnection().CreateCommand()){cmd.CommandText="SELECT name,is_nullable,TYPE_NAME(system_type_id) FROM sys.columns WHERE object_id=OBJECT_ID('dbo.HoSoGiaSu')";await using var reader=await cmd.ExecuteReaderAsync();var cols=new Dictionary<string,(bool nullable,string type)>();while(await reader.ReadAsync())cols[reader.GetString(0)]=(reader.GetBoolean(1),reader.GetString(2));foreach(var name in new[]{"TrinhDo","ChuyenMon","KinhNghiem","MoTa","KhuVuc","TrangThaiDuyet"})Assert.False(cols[name].nullable);Assert.Contains(cols["RowVersion"].type,new[]{"timestamp","rowversion"});proof["columns"]=cols.Keys;}break;
                    case 2:await SqlBlocked($"INSERT dbo.HoSoGiaSu(Id,GiaSuId,TrinhDo,ChuyenMon,KinhNghiem,MoTa,KhuVuc,TrangThaiDuyet) VALUES(NEWID(),'{tutor}',N'A',N'A',N'A',N'A',N'A','BanNhap')");break;
                    case 3:var mon=new MonHoc{TenMon=tag,DangSuDung=true};db.Add(mon);await db.SaveChangesAsync();var mon2=new MonHoc{TenMon=tag,DangSuDung=true};db.Add(mon2);await db.SaveChangesAsync();Assert.Equal(2,await db.Set<MonHoc>().CountAsync(x=>x.TenMon==tag));mon.TenMon=tag+" sửa";await db.SaveChangesAsync();db.RemoveRange(mon,mon2);await db.SaveChangesAsync();Assert.False(await db.Set<MonHoc>().AnyAsync(x=>x.Id==mon.Id||x.Id==mon2.Id));break;
                    case 4:await SqlBlocked($"INSERT dbo.HoSoGiaSuMonDay(HoSoGiaSuId,MonHocId,PhamViDay) VALUES('{profile}',1,N'Lớp 10')");await SqlBlocked($"INSERT dbo.HoSoGiaSuMonDay(HoSoGiaSuId,MonHocId,PhamViDay) VALUES('{Guid.NewGuid()}',1,N'Lớp 10')");break;
                    case 5:await SqlBlocked(InsertNeed(Guid.NewGuid().ToString())+")");proof["identityBaseline"]="NguoiThamGia, không phải AspNetUsers";break;
                    case 6:foreach(var extra in new[]{",SoBuoiTuan",",HocPhiDuKien",",HinhThucHoc"}){if(extra==",HinhThucHoc")await SqlBlocked(InsertNeed(learner.ToString()).Replace("'TrucTuyen'","'Khac'")+")");else await SqlBlocked(InsertNeed(learner.ToString(),extra)+(extra.Contains("SoBuoi")?",8)":",-1)"));}break;
                    case 7:var need7=await Create();foreach(var data in new[]{"7,'18:00','20:00'","1,'19:00','19:00'","1,'20:00','19:00'"})await SqlBlocked($"INSERT dbo.KhungGioRanh(Id,NhuCauId,ThuTrongTuan,GioBatDau,GioKetThuc) VALUES(NEWID(),'{need7}',{data})");break;
                    case 8:await SqlBlocked($"INSERT dbo.KhungGioRanh(Id,NhuCauId,ThuTrongTuan,GioBatDau,GioKetThuc) VALUES(NEWID(),'{Guid.NewGuid()}',1,'18:00','20:00')");break;
                    case 9:var need9=await Create();db.ChangeTracker.Clear();var old=await db.Set<NhuCauHoc>().SingleAsync(x=>x.Id==need9);var v=old.RowVersion.ToArray();old.MucTieu+=" sửa";await db.SaveChangesAsync();Assert.Equal(8,v.Length);Assert.Equal(8,old.RowVersion.Length);Assert.False(v.SequenceEqual(old.RowVersion));proof["v1"]=Convert.ToBase64String(v);proof["v2"]=Convert.ToBase64String(old.RowVersion);break;
                    case 10:var need10=await Create();await SqlBlocked($"DELETE dbo.NhuCauHoc WHERE Id='{need10}'");Assert.True(await db.Set<KhungGioRanh>().AnyAsync(x=>x.NhuCauId==need10));break;
                    case 11:var need11=await Create();await SqlBlocked($"INSERT dbo.KhungGioRanh(Id,NhuCauId,ThuTrongTuan,GioBatDau,GioKetThuc) VALUES(NEWID(),'{need11}',1,'19:00','21:00')");break;
                    case 12:await Exec(InsertNeed(learner.ToString())+")");Assert.Equal(1,await Count());var m12=Input();m12.MucTieu=null;await Reject(m12);break;
                    case 13:var m13=Input();m13.MucTieu="Tiếng Việt: Nguyễn, Đặng, học phí, phụ huynh";var need13=await Create(m13);db.ChangeTracker.Clear();Assert.Equal(m13.MucTieu,(await db.Set<NhuCauHoc>().SingleAsync(x=>x.Id==need13)).MucTieu);break;
                }
            }
            else if(n<=42)
            {
                var m=Input();
                switch(n)
                {
                    case 14:m.LichRanh.Add(new(){ThuTrongTuan=3,GioBatDau=new(19,0),GioKetThuc=new(20,0)});var created=await Create(m);Assert.Equal("DangMo",(await db.Set<NhuCauHoc>().SingleAsync(x=>x.Id==created)).TrangThai);Assert.Equal(2,await db.Set<KhungGioRanh>().CountAsync(x=>x.NhuCauId==created));Assert.Null((await db.Set<NhuCauHoc>().SingleAsync(x=>x.Id==created)).TenNguoiHoc);break;
                    case 15:await current.ChonAsync(parent);m.TenNguoiHoc="Nguyễn Minh";m.TuoiNguoiHoc=15;var p15=await Create(m,parent);Assert.Equal("Nguyễn Minh",(await db.Set<NhuCauHoc>().SingleAsync(x=>x.Id==p15)).TenNguoiHoc);break;
                    case 16:await current.ChonAsync(tutor);await Reject(m,403,tutor);break;
                    case 17:m.HinhThucHoc="TrucTiep";m.KhuVuc="Hà Nội";m.DiaChiChiTiet="Số 10 ngõ 20";var n17=await Create(m);Assert.Equal(m.KhuVuc,(await db.Set<NhuCauHoc>().SingleAsync(x=>x.Id==n17)).KhuVuc);break;
                    case 18:m.HinhThucHoc="TrucTiep";m.KhuVuc=null;await Reject(m);break;
                    case 19:m.KhuVuc="Hà Nội";m.DiaChiChiTiet="Không công khai";var n19=await Create(m);var row19=await db.Set<NhuCauHoc>().SingleAsync(x=>x.Id==n19);Assert.Null(row19.KhuVuc);Assert.Null(row19.DiaChiChiTiet);break;
                    case 20:m.MucTieu=" ";await Reject(m);break;
                    case 21:m.SoBuoiTuan=null;await Reject(m);break;
                    case 22:m.BanNhap=true;m.MucTieu=null;m.LichRanh=[];var n22=await Create(m);Assert.Equal("BanNhap",(await db.Set<NhuCauHoc>().SingleAsync(x=>x.Id==n22)).TrangThai);break;
                    case 23:m.BanNhap=true;m.HocPhiDuKien=-1;await Reject(m);break;
                    case 24:await current.ChonAsync(parent);m.TuoiNguoiHoc=15;m.TenNguoiHoc=null;await Reject(m,400,parent);break;
                    case 25:await current.ChonAsync(parent);m.TenNguoiHoc="Minh";m.TuoiNguoiHoc=null;m.KhoiLop="Lớp 10";await Create(m,parent);break;
                    case 26:m.LichRanh.Add(new(){ThuTrongTuan=3,GioBatDau=new(19,0),GioKetThuc=new(20,0)});m.MonHocIds=[1,2];await Create(m);Assert.Equal(2,await Count());Assert.Equal(4,await db.Set<KhungGioRanh>().CountAsync(s=>db.Set<NhuCauHoc>().Any(n=>n.Id==s.NhuCauId&&n.ChuSoHuuId==learner)));break;
                    case 27:m.MonHocIds=[1,2,1];await Create(m);Assert.Equal(2,await Count());break;
                    case 28:m.MonHocIds=[];m.MonHocId=0;await Reject(m);break;
                    case 29:case 30:var ids=new List<int>();for(int i=0;i<(n==29?10:11);i++){var sub=new MonHoc{TenMon=tag+i,DangSuDung=true};db.Add(sub);await db.SaveChangesAsync();ids.Add(sub.Id);}m.MonHocIds=ids;if(n==29){m.LichRanh.Add(new(){ThuTrongTuan=3,GioBatDau=new(19,0),GioKetThuc=new(20,0)});await Create(m);Assert.Equal(10,await Count());Assert.Equal(20,await db.Set<KhungGioRanh>().CountAsync(s=>db.Set<NhuCauHoc>().Any(x=>x.Id==s.NhuCauId&&x.ChuSoHuuId==learner)));}else await Reject(m);break;
                    case 31:case 32:case 33:case 34:case 35:case 36:case 37:case 38:case 39:case 40:case 41:
                        var nid=await Create();var edit=(await needs.LayDeSuaAsync(nid)).DuLieu;var vm=TaoNhuCauBenHocViewModel.FromInput(edit);var original=vm.RowVersion!.ToArray();
                        if(n==31){vm.MonHocIds=[1,2];var r31=await needs.CapNhatAsync(nid,vm,original,learner,default);Assert.False(r31.ThanhCong);Assert.Equal(400,r31.MaTrangThai);}
                        if(n==32){vm.MucTieu=tag+" sửa";vm.LichRanh=[new(){ThuTrongTuan=2,GioBatDau=new(19,0),GioKetThuc=new(21,0)}];Assert.True((await needs.CapNhatAsync(nid,vm,original,learner,default)).ThanhCong);db.ChangeTracker.Clear();var saved=(await needs.LayDeSuaAsync(nid)).DuLieu;Assert.Equal(vm.MucTieu,saved.MucTieu);Assert.Equal(2,saved.LichRanh.Single().ThuTrongTuan);}
                        if(n==33){await current.ChonAsync(parent);Assert.Equal(404,(await needs.LayDeSuaAsync(nid)).MaTrangThai);}
                        if(n==34){vm.TenNguoiHoc="Minh";vm.TuoiNguoiHoc=15;await current.ChonAsync(parent);Assert.Equal(403,(await needs.CapNhatAsync(nid,vm,original,parent,default)).MaTrangThai);}
                        if(n is 35 or 36){if(n==35)vm.MucTieu+=" lượt 1";else vm.LichRanh[0].ThuTrongTuan=2;Assert.True((await needs.CapNhatAsync(nid,vm,original,learner,default)).ThanhCong);db.ChangeTracker.Clear();vm.MucTieu="Không được ghi đè";var conflict=await needs.CapNhatAsync(nid,vm,original,learner,default);Assert.Equal(409,conflict.MaTrangThai);db.ChangeTracker.Clear();Assert.NotEqual(vm.MucTieu,(await needs.LayDeSuaAsync(nid)).DuLieu.MucTieu);proof["conflictStatus"]=409;}
                        if(n==37){await Exec("CREATE TRIGGER dbo.MT1_RejectSlot ON dbo.KhungGioRanh AFTER INSERT AS BEGIN THROW 51999,'MT1 injected slot failure',1; END");try{var failing=Input();failing.MucTieu=tag+"_must_rollback";await Assert.ThrowsAnyAsync<Exception>(()=>needs.TaoNhuCauHocAsync(failing,learner,default));db.ChangeTracker.Clear();Assert.False(await db.Set<NhuCauHoc>().AnyAsync(x=>x.MucTieu==failing.MucTieu));Assert.Equal(1,await Count());vm.MucTieu="Must rollback";await Assert.ThrowsAnyAsync<Exception>(()=>needs.CapNhatAsync(nid,vm,original,learner,default));}finally{await Exec("DROP TRIGGER dbo.MT1_RejectSlot");}db.ChangeTracker.Clear();var saved=(await needs.LayDeSuaAsync(nid)).DuLieu;Assert.Equal(edit.MucTieu,saved.MucTieu);Assert.Equal(edit.LichRanh[0].ThuTrongTuan,saved.LichRanh[0].ThuTrongTuan);proof["faultInjection"]="SQL trigger throws after slot insert; removed in finally";}
                        if(n==38){m.MonHocIds=[1,int.MaxValue];await Reject(m);Assert.Equal(1,await Count());}
                        if(n is 39 or 40){await needs.TrangThaiAsync(nid,"DaDong",edit.RowVersion!);db.ChangeTracker.Clear();Assert.Equal("DaDong",(await needs.LayDeSuaAsync(nid)).DuLieu.BanNhap?"BanNhap":(await db.Set<NhuCauHoc>().AsNoTracking().SingleAsync(x=>x.Id==nid)).TrangThai);if(n==40){var rv=Convert.ToBase64String((await db.Set<NhuCauHoc>().AsNoTracking().SingleAsync(x=>x.Id==nid)).RowVersion);await needs.TrangThaiAsync(nid,"DangMo",rv);db.ChangeTracker.Clear();Assert.Equal("DangMo",(await db.Set<NhuCauHoc>().SingleAsync(x=>x.Id==nid)).TrangThai);}}
                        if(n==41){m.BanNhap=true;var draft=await Create(m);db.ChangeTracker.Clear();var rv=Convert.ToBase64String((await db.Set<NhuCauHoc>().SingleAsync(x=>x.Id==draft)).RowVersion);var e=await Assert.ThrowsAsync<LoiNghiepVu>(()=>needs.TrangThaiAsync(draft,"DangMo",rv));Assert.Equal(400,e.Status);}
                        break;
                    case 42:m.LichRanh.Add(new(){ThuTrongTuan=1,GioBatDau=new(19,0),GioKetThuc=new(21,0)});await Reject(m);break;
                }
            }
            else if(n<=60)
            {
                var nid=await Create();
                if(n is 43 or 46 or 47 or 48 or 49 or 51)await ProfileFixture();
                if(n==43){var r=(await search.TimKiemAsync(new(){TuKhoa=tag},learner,default)).DuLieu;Assert.Equal(4,r.Total);Assert.Equal(profileSet.Where(p=>new[]{"P1","P2","P3","P7"}.Contains(p.Key)).Select(p=>p.Value).Order(),r.Items.Select(x=>x.Id).Order());}
                if(n is 44 or 45 or 53){for(var i=0;i<11;i++)await Create();if(n!=53){var closed=await Create();var version=(await needs.LayDeSuaAsync(closed)).DuLieu.RowVersion!;await needs.TrangThaiAsync(closed,"DaDong",version);var draft=Input();draft.BanNhap=true;await Create(draft);var other=Input();other.MonHocIds=[2];await Create(other);}await current.ChonAsync(tutor);var r=(await search.TimKiemAsync(new(){TuKhoa=tag,MonHocId=1,Trang=n==44?1:n==45?2:3},tutor,default)).DuLieu;Assert.Equal(12,r.Total);Assert.Equal(n==44?9:n==45?3:0,r.Items.Count);if(n==45){var first=(await search.TimKiemAsync(new(){TuKhoa=tag,MonHocId=1},tutor,default)).DuLieu;Assert.Empty(first.Items.Select(x=>x.Id).Intersect(r.Items.Select(x=>x.Id)));}if(n==53)Assert.Equal(0,(await search.TimKiemAsync(new(){TuKhoa="absent_"+tag},tutor,default)).DuLieu.Total);proof["total"]=r.Total;proof["items"]=r.Items.Count;}
                if(n==46){var r=(await search.TimKiemAsync(new(){TuKhoa=tag,MonHocIds=[1],HocPhiToiDa=200000,DonViHocPhi="VND/Gio"},learner,default)).DuLieu;Assert.Single(r.Items);Assert.Equal(profile,r.Items[0].Id);}
                if(n==47){var r=(await search.TimKiemAsync(new(){TuKhoa=tag,MonHocIds=[1,2]},learner,default)).DuLieu;Assert.Equal(4,r.Total);Assert.All(r.Items,x=>Assert.All(x.MonTheoHoSo,m=>Assert.Contains(m.MonHocId,new[]{1,2})));}
                if(n==48){var r=(await search.TimKiemAsync(new(){TuKhoa=tag,HinhThucHoc="TrucTuyen",KhuVuc=" Thủ   Đức "},learner,default)).DuLieu;Assert.Equal(2,r.Total);Assert.All(r.Items,x=>{Assert.True(x.DayTrucTuyen);Assert.Equal("Thủ Đức",x.KhuVuc);});}
                if(n==49){foreach(var h in profileSet.Values)await db.Set<HoSoGiaSu>().Where(x=>x.Id==h).ExecuteUpdateAsync(s=>s.SetProperty(x=>x.TrinhDo,tag));var lower=(await search.TimKiemAsync(new(){TuKhoa="toán",TrinhDo=tag},learner,default)).DuLieu;var upper=(await search.TimKiemAsync(new(){TuKhoa="TOÁN",TrinhDo=tag},learner,default)).DuLieu;Assert.Equal(3,lower.Total);Assert.Equal(lower.Items.Select(x=>x.Id).Order(),upper.Items.Select(x=>x.Id).Order());var plain=(await search.TimKiemAsync(new(){TuKhoa="toan",TrinhDo=tag},learner,default)).DuLieu;proof["accentedCount"]=lower.Total;proof["unaccentedCount"]=plain.Total;}
                if(n==50){await current.ChonAsync(tutor);foreach(var hours in new[]{(18,20,true),(19,20,true),(17,20,false),(19,21,false)}){var r=(await search.TimKiemAsync(new(){TuKhoa=tag,ThuTrongTuan=1,GioBatDau=new(hours.Item1,0),GioKetThuc=new(hours.Item2,0)},tutor,default)).DuLieu;Assert.Equal(hours.Item3?1:0,r.Total);}}
                if(n==51){var r=(await search.TimKiemAsync(new(){TuKhoa=tag,MonHocIds=[1],SapXep="HocPhiTang",DonViHocPhi="VND/Gio"},learner,default)).DuLieu;Assert.Equal(2,r.Total);Assert.Equal(new[]{profile,profileSet["P7"]},r.Items.Select(x=>x.Id));Assert.All(r.Items,x=>Assert.All(x.MonTheoHoSo,m=>Assert.Equal("VND/Gio",m.DonVi)));}
                if(n==52)foreach(var page in new[]{0,-1,100001})Assert.Equal(400,(await search.TimKiemAsync(new(){Trang=page},learner,default)).MaTrangThai);
                if(n==54){var json=JsonSerializer.Serialize(await search.ChiTietAsync(nid));Assert.DoesNotContain("TenNguoiHoc",json);Assert.DoesNotContain("TuoiNguoiHoc",json);Assert.DoesNotContain("DiaChiChiTiet",json);var dto=await search.ChiTietAsync(profile);Assert.Single(dto.MonTheoHoSo);Assert.Equal(150000,dto.MonTheoHoSo[0].HocPhi);proof["publicDto"]=json;}
                if(n is 55 or 58){if(n==55)await db.Set<HoSoGiaSu>().Where(x=>x.Id==profile).ExecuteUpdateAsync(s=>s.SetProperty(x=>x.DangHienThi,false));if(n==55){var ex=await Assert.ThrowsAsync<LoiNghiepVu>(()=>search.ChiTietAsync(profile));Assert.Equal(404,ex.Status);await needs.TrangThaiAsync(nid,"DaDong",Convert.ToBase64String((await db.Set<NhuCauHoc>().AsNoTracking().SingleAsync(x=>x.Id==nid)).RowVersion));ex=await Assert.ThrowsAsync<LoiNghiepVu>(()=>search.ChiTietAsync(nid));Assert.Equal(404,ex.Status);}else{await needs.TrangThaiAsync(nid,"DaDong",(await needs.LayDeSuaAsync(nid)).DuLieu.RowVersion!);Assert.Equal(404,(await search.SoSanhAsync(new List<Guid>{nid,profile},learner,default)).MaTrangThai);}}
                if(n is 56 or 57){var nid2=await Create();var ids=n==56?new List<Guid>{nid,profile,nid2}:new List<Guid>{nid,profile,nid};var result=(await search.SoSanhAsync(ids,learner,default)).DuLieu;var json=JsonSerializer.Serialize(result);Assert.Contains(nid.ToString(),json);Assert.Contains(profile.ToString(),json);proof["comparison"]=json;Assert.Equal(n==56?3:2,result.Items.Count);}
                if(n==59){foreach(var bound in new decimal?[]{149999,150000,null}){var r=(await search.TimKiemAsync(new(){TuKhoa=tag,HocPhiToiDa=bound},learner,default)).DuLieu;Assert.Equal(bound==149999?0:1,r.Total);}Assert.Equal(400,(await search.TimKiemAsync(new(){HocPhiToiDa=-1},learner,default)).MaTrangThai);proof["contract"]="HocPhiToiDa theo sơ đồ lớp Final; không có cận dưới.";}
                if(n==60){var before=await db.Set<YeuCauLienHe>().CountAsync();var requestCount=await db.Set<YeuCauKetNoiLopHoc>().CountAsync();var classCount=await db.Set<LopHoc>().CountAsync();var suggestions=await search.GoiYAsync();Assert.NotEmpty(suggestions.Items);Assert.All(suggestions.Items,x=>Assert.NotEmpty(x.LyDo));Assert.Null(typeof(UngVienViewModel).GetProperty("DiemPhuHop"));Assert.Equal(before,await db.Set<YeuCauLienHe>().CountAsync());Assert.Equal(requestCount,await db.Set<YeuCauKetNoiLopHoc>().CountAsync());Assert.Equal(classCount,await db.Set<LopHoc>().CountAsync());Assert.All(suggestions.Items,x=>Assert.Contains("Cùng môn "+x.MonTheoHoSo.First(m=>m.MonHocId==1).TenMon,x.LyDo));proof["reasons"]=suggestions.Items.Select(x=>x.LyDo).ToList();}
            }
            else if(n>=147)
            {
                proof["level"]="Component/Integration; Service và SQL thật, không dùng mock";
                var nc=new NhuCauController(needs,current){ControllerContext=new(){HttpContext=http},TempData=new TempDataDictionary(http,new MemoryTempData())};
                var notices=new ThongBaoService(db,current);var contacts=new KetNoiService(db,current,notices);
                var tc=new TimKiemController(search,needs,contacts,current){ControllerContext=new(){HttpContext=http}};
                var env=new LocalEnv();var rights=new QuyenLopHocService(db,current);var files=new TepService(db,current,rights,env);
                var ps=new HoSoGiaSuService(db,current,new MonHocCatalog(db),files,notices);
                var pc=new HoSoGiaSuController(ps,needs,new BangCapService(db,current,env),search,contacts,current){ControllerContext=new(){HttpContext=http},TempData=new TempDataDictionary(http,new MemoryTempData())};
                var k=n-100;
                if(k is 47 or 48){var ids=k==47?new List<Guid>():Enumerable.Range(0,4).Select(_=>Guid.NewGuid()).ToList();Assert.Equal(400,(await search.SoSanhAsync(ids,learner,default)).MaTrangThai);}
                if(k==49){tc.ModelState.AddModelError("TuKhoa","Quá dài");var ex=await Assert.ThrowsAsync<LoiNghiepVu>(()=>tc.Tim(new()));Assert.Contains("Quá dài",ex.Message);}
                if(k is 50 or 51 or 52){var input=Input();if(k==50)nc.ModelState.AddModelError("MucTieu","Không hợp lệ");if(k==52)input.MonHocIds=[];var r=await nc.TaoNhuCauHoc(input);if(k==51){Assert.IsType<RedirectToActionResult>(r);Assert.NotNull(nc.TempData["Success"]);Assert.Equal(1,await Count());}else{Assert.Equal("Tao",Assert.IsType<ViewResult>(r).ViewName);Assert.False(nc.ModelState.IsValid);Assert.Equal(0,await Count());}}
                if(k==53){var id53=await Create();var row=(await needs.LayDeSuaAsync(id53)).DuLieu;Assert.IsType<RedirectToActionResult>(await nc.TrangThai(id53,"DaDong",row.RowVersion!));Assert.NotNull(nc.TempData["Success"]);}
                if(k is 54 or 55 or 56 or 60){await current.ChonAsync(tutor);await db.Set<HoSoGiaSu>().Where(x=>x.Id==profile).ExecuteUpdateAsync(s=>s.SetProperty(x=>x.DangHienThi,false));db.ChangeTracker.Clear();var input=await ps.LayDeSuaAsync();if(k==60){var r=Assert.IsType<ViewResult>(await pc.Sua());Assert.IsType<HoSoInput>(r.Model);Assert.NotNull(pc.ViewBag.Subjects);Assert.NotNull(pc.ViewBag.Certificates);}else{if(k==54)pc.ModelState.AddModelError("HoTen","Bắt buộc");if(k==56)input.RowVersion="@@@";if(k==55)input.MoTa="MT1 hồ sơ đã sửa";var r=await pc.Luu(input);if(k==55){Assert.IsType<RedirectToActionResult>(r);Assert.NotNull(pc.TempData["Success"]);}else{Assert.Equal("Sua",Assert.IsType<ViewResult>(r).ViewName);Assert.False(pc.ModelState.IsValid);}}}
                if(k==57){var r=Assert.IsType<ViewResult>(await tc.Tim(new(){TuKhoa=tag}));Assert.Single(Assert.IsType<SearchPage>(r.Model).Items);}
                if(k==58){var nid58=await Create();var r=Assert.IsType<ViewResult>(await tc.SoSanh(new List<Guid>{nid58,profile}));Assert.Equal(2,Assert.IsType<List<UngVienViewModel>>(r.Model).Count);}
                if(k==59){var nid59=await Create();var r=Assert.IsType<ViewResult>(await nc.Sua(nid59));var m=Assert.IsType<NhuCauInput>(r.Model);Assert.Equal(nid59,m.Id);Assert.False(m.ChonMonKhac);Assert.NotNull(nc.ViewBag.Subjects);}
                if(k==61){http.Request.ContentType="application/x-www-form-urlencoded";http.Request.Form=new FormCollection(new Dictionary<string,Microsoft.Extensions.Primitives.StringValues>());var input=Input();var r=Assert.IsType<ViewResult>(await nc.TaoNhuCauHoc(input));Assert.Empty(input.LichRanh);Assert.False(nc.ModelState.IsValid);Assert.Equal(0,await Count());}
                if(k==62){await Create();var r=Assert.IsType<ViewResult>(await nc.Index());Assert.Single(Assert.IsType<List<NhuCauHoc>>(r.Model));Assert.NotNull(nc.ViewBag.Subjects);Assert.NotNull(nc.ViewBag.Linked);}
                if(k is 63 or 64){await contacts.GuiLienHeAsync(new(){DoiTuongId=tutor,LoiNhan="Xin trao đổi"});if(k==63){var c=await db.Set<YeuCauLienHe>().AsNoTracking().SingleAsync(x=>x.BenHocId==learner);await current.ChonAsync(tutor);await contacts.PhanHoiLienHeAsync(c.Id,"ChapNhan",Convert.ToBase64String(c.RowVersion));await current.ChonAsync(learner);}Assert.IsType<ViewResult>(await pc.ChiTiet(profile));Assert.Equal(k==63,pc.ViewBag.ContactChat!=null);Assert.NotNull(pc.ViewBag.Contact);}
                if(k is 65 or 66 or 67){await Create();if(k==66)await current.ChonAsync(tutor);var suggestions=await search.GoiYAsync();Assert.NotEmpty(suggestions.Items);Assert.True(suggestions.Items.Count<=3);Assert.All(suggestions.Items,x=>Assert.NotEmpty(x.LyDo));Assert.Null(typeof(UngVienViewModel).GetProperty("DiemPhuHop"));proof["reasons"]=suggestions.Items.Select(x=>x.LyDo);}
            }
            else
            {
                var contacts=new KetNoiService(db,current,new ThongBaoService(db,current));
                var target=n==77?learner:n==78?parent:tutor;
                if(n==79)await db.Set<HoSoGiaSu>().Where(x=>x.Id==profile).ExecuteUpdateAsync(s=>s.SetProperty(x=>x.DangHienThi,false));
                var msg=new GuiLienHeInput{DoiTuongId=target,LoiNhan="  Mong trao đổi việc học  "};
                if(n==81){await Exec("CREATE TRIGGER dbo.MT1_RejectNotice ON dbo.ThongBao AFTER INSERT AS BEGIN THROW 51998,'MT1 notice failure',1; END");try{await Assert.ThrowsAnyAsync<Exception>(()=>contacts.GuiLienHeAsync(msg));}finally{await Exec("DROP TRIGGER dbo.MT1_RejectNotice");}db.ChangeTracker.Clear();Assert.False(await db.Set<YeuCauLienHe>().AnyAsync(x=>x.BenHocId==learner));Assert.False(await db.Set<ThongBao>().AnyAsync(x=>x.NguoiNhanId==tutor));Assert.False(await db.Set<SuKienChoXuLy>().AnyAsync(x=>x.DuLieuToiThieu.Contains(tutor.ToString())));}
                else if(n is 77 or 78 or 79){var ex=await Assert.ThrowsAsync<LoiNghiepVu>(()=>contacts.GuiLienHeAsync(msg));Assert.Equal(n==79?409:400,ex.Status);Assert.False(await db.Set<YeuCauLienHe>().AnyAsync(x=>x.BenHocId==learner));}
                else{await contacts.GuiLienHeAsync(msg);var contact=await db.Set<YeuCauLienHe>().AsNoTracking().SingleAsync(x=>x.GiaSuId==tutor&&x.BenHocId==learner);Assert.Equal("ChoPhanHoi",contact.TrangThai);Assert.Equal("Mong trao đổi việc học",contact.LoiNhan);Assert.Equal(1,await db.Set<ThongBao>().CountAsync(x=>x.DoiTuongId==contact.Id));Assert.False(await db.Set<CuocHoiThoai>().AnyAsync(x=>x.LienHeId==contact.Id));if(n==80){await Assert.ThrowsAsync<LoiNghiepVu>(()=>contacts.GuiLienHeAsync(msg));Assert.Equal(1,await db.Set<YeuCauLienHe>().CountAsync(x=>x.GiaSuId==tutor&&x.BenHocId==learner));Assert.Equal(1,await db.Set<ThongBao>().CountAsync(x=>x.DoiTuongId==contact.Id));}if(n is 83 or 84){var rights=new QuyenLopHocService(db,current);var chat=new EduConnect.Mvc.Areas.HocTap.Services.TinNhanService(db,current,rights,new TepService(db,current,rights,new LocalEnv()));Assert.Null(await contacts.HoiThoaiAsync(contact.Id));await Assert.ThrowsAsync<LoiNghiepVu>(()=>contacts.GuiYeuCauAsync(new GuiYeuCauInput{LienHeId=contact.Id,LoiNhan="Chưa chấp nhận"}));await current.ChonAsync(tutor);await contacts.PhanHoiLienHeAsync(contact.Id,"ChapNhan",Convert.ToBase64String(contact.RowVersion));var room=await contacts.HoiThoaiAsync(contact.Id);Assert.NotNull(room);var message=new EduConnect.Mvc.Areas.HocTap.ViewModels.GuiTinNhanViewModel{MaHoiThoai=room!.Value,NoiDung="MT1 trao đổi"};await chat.GuiAsync(message);if(n==84)await chat.GuiAsync(message);Assert.Equal(1,await db.Set<TinNhan>().CountAsync(x=>x.CuocHoiThoaiId==room));await current.ChonAsync(learner);Assert.True(await chat.CoTheGuiAsync(room.Value));if(n==83){await chat.GuiAsync(new(){MaHoiThoai=room.Value,NoiDung="MT1 trả lời từ bên học"});Assert.Equal(2,await db.Set<TinNhan>().CountAsync(x=>x.CuocHoiThoaiId==room));}Assert.False(await db.Set<YeuCauKetNoiLopHoc>().AnyAsync(x=>x.LienHeId==contact.Id));proof["roomId"]=room;proof["idempotencyKey"]=message.MaThaoTac;}proof["contactId"]=contact.Id;}
            }
            actual="Các kiểm tra trong ca đều đạt. "+JsonSerializer.Serialize(proof);
        }
        catch(Exception ex){status="Fail";actual=ex.ToString();throw;}
        finally{var dir=Environment.GetEnvironmentVariable("MT1_EVIDENCE")!;File.AppendAllText(Path.Combine(dir,"integration-details.jsonl"),JsonSerializer.Serialize(new{caseId=id,status,actual,proof,elapsedMs=watch.ElapsedMilliseconds,utc=DateTimeOffset.UtcNow})+Environment.NewLine);}
    }
}
internal sealed class MemorySession:ISession
{
    readonly Dictionary<string,byte[]> data=[];public bool IsAvailable=>true;public string Id=>"MT1";public IEnumerable<string> Keys=>data.Keys;public void Clear()=>data.Clear();public Task CommitAsync(CancellationToken t=default)=>Task.CompletedTask;public Task LoadAsync(CancellationToken t=default)=>Task.CompletedTask;public void Remove(string k)=>data.Remove(k);public void Set(string k,byte[] v)=>data[k]=v;public bool TryGetValue(string k,out byte[] v)=>data.TryGetValue(k,out v!);
}
internal sealed class MemoryTempData:ITempDataProvider
{
    public IDictionary<string,object> LoadTempData(HttpContext c)=>new Dictionary<string,object>();
    public void SaveTempData(HttpContext c,IDictionary<string,object> v){}
}
internal sealed class LocalEnv:IWebHostEnvironment
{
    public string ApplicationName{get;set;}="MT1";public string EnvironmentName{get;set;}="Test";public string WebRootPath{get;set;}="";public IFileProvider WebRootFileProvider{get;set;}=new NullFileProvider();public string ContentRootPath{get;set;}=Path.Combine(Path.GetTempPath(),"EduConnect_MT1");public IFileProvider ContentRootFileProvider{get;set;}=new NullFileProvider();
}
