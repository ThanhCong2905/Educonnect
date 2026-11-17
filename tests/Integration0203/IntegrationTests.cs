using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using EduConnect.Mvc.Models;
using EduConnect.Mvc.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

public sealed partial class Integration0203Tests(ITestOutputHelper output)
{
    private readonly List<string> failures=[];
    private void Check(bool condition,string message){if(!condition)failures.Add(message);}
    private async Task Run(int n)
    {
        var id=$"IT-{n:000}";It0203Case? c=null;Exception? error=null;
        try
        {
            c=await It0203Case.Create(id);
            await using(var db=c.Db())
            {
                await db.Database.OpenConnectionAsync();var cmd=db.Database.GetDbConnection().CreateCommand();cmd.CommandText="SELECT DB_NAME()+'|'+@@SERVERNAME";c.Proof["serverDatabase"]=await cmd.ExecuteScalarAsync();
            }
            c.Proof["before"]=await Snapshot(c);
            if(n<30)await Need(c,n);else await ProfileSearch(c,n);
        }
        catch(Exception ex){error=ex;}
        finally
        {
            if(c!=null)
            {
                try{c.Proof["after"]=await Snapshot(c);}catch(Exception ex){c.Proof["snapshotError"]=ex.ToString();error??=ex;}
                try{await c.DisposeAsync();}catch(Exception ex){c.Proof["cleanupError"]=ex.ToString();error??=ex;}
                c.Proof["assertionDifferences"]=failures;c.Proof["exception"]=error?.ToString();
                output.WriteLine("OBSERVATION_JSON:"+JsonSerializer.Serialize(c.Proof));
            }
        }
        Assert.True(error==null,"Không hoàn tất ca: "+error);
        Assert.True(failures.Count==0,string.Join("\n",failures));
    }
    private static async Task<object> Snapshot(It0203Case c)
    {
        await using var db=c.Db();
        var needs=await db.Set<NhuCauHoc>().AsNoTracking().Where(x=>c.Users.Contains(x.ChuSoHuuId)).ToListAsync();var ids=needs.Select(x=>x.Id).ToList();
        var profiles=await db.Set<HoSoGiaSu>().AsNoTracking().Where(x=>c.Users.Contains(x.GiaSuId)).ToListAsync();var pids=profiles.Select(x=>x.Id).ToList();
        return new{needs,links=await db.Set<NhuCauMonHoc>().AsNoTracking().Where(x=>ids.Contains(x.NhuCauId)).Select(x=>new{x.NhuCauId,x.MonHocId}).ToListAsync(),slots=await db.Set<KhungGioRanh>().AsNoTracking().Where(x=>ids.Contains(x.NhuCauId)).ToListAsync(),posts=await db.Set<LuotDangNhuCau>().AsNoTracking().Where(x=>c.Users.Contains(x.TaiKhoanId)).ToListAsync(),profiles,subjects=await db.Set<HoSoGiaSuMonDay>().AsNoTracking().Where(x=>pids.Contains(x.HoSoGiaSuId)).ToListAsync()};
    }
    private static Dictionary<string,string> Form(It0203Case c)=>new(){["MaThaoTac"]=Guid.NewGuid().ToString(),["MonHocId"]=c.Math.ToString(),["MonHocIds[0]"]=c.Math.ToString(),["KhoiLop"]="Lớp 8",["HocLuc"]="Trung bình",["MucTieu"]=c.Tag,["SoBuoiTuan"]="2",["HinhThucHoc"]="TrucTuyen",["HocPhiDuKien"]="150000",["DonViHocPhi"]="VND/Gio",["BanNhap"]="false",["LichRanh[0].ThuTrongTuan"]="1",["LichRanh[0].GioBatDau"]="19:00",["LichRanh[0].GioKetThuc"]="20:00",["LichRanh[1].ThuTrongTuan"]="3",["LichRanh[1].GioBatDau"]="19:00",["LichRanh[1].GioKetThuc"]="20:00"};
    private static async Task<HttpResponseMessage> Post(It0203Case c,HttpClient client,string path,Dictionary<string,string> form,string tokenPath="/NhuCau/NhuCau/Tao",bool token=true)
    {
        if(token)form["__RequestVerificationToken"]=await It0203Case.Token(client,tokenPath);
        else form.Remove("__RequestVerificationToken");
        using var request=new HttpRequestMessage(HttpMethod.Post,path){Content=new FormUrlEncodedContent(form)};request.Headers.Add("X-Requested-With","XMLHttpRequest");
        c.Proof["requestPath"]=path;c.Proof["input"]=form.Where(x=>x.Key!="__RequestVerificationToken").ToDictionary(x=>x.Key,x=>x.Value);
        var response=await client.SendAsync(request);await c.RecordHttp(response);return response;
    }
    private async Task Ok(HttpResponseMessage r)
    {
        Check((int)r.StatusCode==200,$"HTTP kỳ vọng 200, thực tế {(int)r.StatusCode}.");
        var body=await r.Content.ReadAsStringAsync();
        try{using var j=JsonDocument.Parse(body);Check(j.RootElement.TryGetProperty("ok",out var ok)&&ok.ValueKind==JsonValueKind.True,"Kỳ vọng JSON ok=true.");Check(j.RootElement.TryGetProperty("redirectUrl",out var url)&&!string.IsNullOrWhiteSpace(url.GetString()),"Thiếu redirectUrl thành công.");}
        catch(JsonException){Check(false,"Phản hồi không phải JSON thành công.");}
    }
    private async Task Need(It0203Case c,int n)
    {
        Guid? actor=n==18?c.PH:n==19?c.GS:n==22?c.HS2:n==28?null:c.HS;
        using var client=await c.Client(actor);var form=Form(c);Guid? target=null;NhuCauHoc? before=null;
        if(n is 16 or 18 or 20)form["MonHocIds[1]"]=c.Literature.ToString();
        if(n==17){form["BanNhap"]="true";foreach(var k in form.Keys.Where(x=>x.StartsWith("LichRanh[")).ToList())form.Remove(k);}
        if(n==18){form["TenNguoiHoc"]="An";form["TuoiNguoiHoc"]="12";}
        if(n==20){await using var db=c.Db();await db.Set<MonHoc>().Where(x=>x.Id==c.Literature).ExecuteUpdateAsync(s=>s.SetProperty(x=>x.DangSuDung,false));}
        if(n is 21 or 22 or 23 or 25 or 26 or 27)
        {
            target=await c.CreateNeed();before=await c.ReadNeed(target.Value);form["Id"]=target.ToString()!;form["RowVersion"]=Convert.ToBase64String(before.RowVersion);
            c.Proof["targetBefore"]=before;c.Proof["targetBeforeSlots"]=await c.ReadSlots(target.Value);
            if(n==21){form["MonHocIds[0]"]=c.Literature.ToString();form["MonHocId"]=c.Literature.ToString();form["LichRanh[0].ThuTrongTuan"]="2";form["LichRanh[1].ThuTrongTuan"]="4";}
            if(n==23)
            {
                var a=new Dictionary<string,string>(form){["MucTieu"]=c.Tag+" A"};using var saved=await Post(c,client,"/NhuCau/NhuCau/Luu",a);Assert.Equal(HttpStatusCode.OK,saved.StatusCode);
                var current=await c.ReadNeed(target.Value);Assert.NotEqual(Convert.ToBase64String(before.RowVersion),Convert.ToBase64String(current.RowVersion));c.Proof["savedA"]=current;c.Proof["savedASlots"]=await c.ReadSlots(target.Value);form["MucTieu"]=c.Tag+" B";
            }
            if(n==25)
            {
                await using(var db=c.Db()){var post=await db.Set<LuotDangNhuCau>().AsNoTracking().SingleAsync(x=>x.NhuCauId==target);form["MaThaoTac"]=post.MaThaoTac.ToString();}
                var second=c.Input();second.MucTieu=c.Tag+" N2";second.MonHocIds=[c.Literature];var other=await c.CreateNeed(second);var row=await c.ReadNeed(other);form["Id"]=other.ToString();form["RowVersion"]=Convert.ToBase64String(row.RowVersion);form["MucTieu"]=c.Tag+" ghi nhầm";c.Proof["secondBefore"]=row;
            }
            if(n==27){using var close=await Post(c,client,"/NhuCau/NhuCau/TrangThai",new(){["id"]=target.ToString()!,["state"]="DaDong",["rowVersion"]=Convert.ToBase64String(before.RowVersion)});Assert.Equal(HttpStatusCode.OK,close.StatusCode);await using(var db=c.Db()){await db.Set<NhuCauHoc>().Where(x=>x.Id==target).ExecuteUpdateAsync(s=>s.SetProperty(x=>x.XacNhanConNhuCauLuc,DateTimeOffset.UtcNow.AddDays(-1)));}before=await c.ReadNeed(target.Value);c.Proof["closedBeforeReopen"]=before;}
        }
        var initialCount=await c.NeedCount();int initialPosts;await using(var db=c.Db())initialPosts=await db.Set<LuotDangNhuCau>().CountAsync(x=>c.Users.Contains(x.TaiKhoanId));
        if(n is 26 or 27)
        {
            var state=n==26?"DaDong":"DangMo";using var response=await Post(c,client,"/NhuCau/NhuCau/TrangThai",new(){["id"]=target.ToString()!,["state"]=state,["rowVersion"]=Convert.ToBase64String(before!.RowVersion)});await Ok(response);
            var row=await c.ReadNeed(target!.Value);Check(row.TrangThai==state,$"Trạng thái kỳ vọng {state}, thực tế {row.TrangThai}.");Check((await c.ReadSlots(row.Id)).Count==2,"Lịch nhu cầu bị mất khi đổi trạng thái.");
            await using var db=c.Db();if(n==27){Check(await db.Set<LuotDangNhuCau>().AnyAsync(x=>x.NhuCauId==row.Id&&x.TaiKhoanId==c.HS&&x.HanhDong=="MoLai"),"Thiếu lượt MoLai đúng chủ.");Check(row.XacNhanConNhuCauLuc>before!.XacNhanConNhuCauLuc&&row.XacNhanConNhuCauLuc>DateTimeOffset.UtcNow.AddMinutes(-2),"Mở lại chưa cập nhật mốc xác nhận mới.");}
            else {await using var s=c.Scope(c.GS);var error=await Assert.ThrowsAsync<LoiNghiepVu>(()=>s.Search.ChiTietAsync(row.Id));Check(error.Status==404,"Nhu cầu đã đóng phải bị loại khỏi truy vấn công khai với 404.");}
            return;
        }
        c.Proof["beforeOperation"]=await Snapshot(c);
        using var r=await Post(c,client,"/NhuCau/NhuCau/Luu",form,tokenPath:n==19?"/NhuCau/HoSoGiaSu/Sua":n==28?"/TaiKhoan/DangNhap":"/NhuCau/NhuCau/Tao",token:n!=29);
        var status=(int)r.StatusCode;
        if(n==24)
        {
            var first=await Snapshot(c);c.Proof["afterFirst"]=first;c.Proof["firstHttp"]=c.Proof["http"];using var retry=await Post(c,client,"/NhuCau/NhuCau/Luu",form);await Ok(retry);
            Check(JsonSerializer.Serialize(first)==JsonSerializer.Serialize(await Snapshot(c)),"Retry phải giữ nguyên cùng ID, dữ liệu và lượt đăng sau lần đầu.");
            Check(await c.NeedCount()==initialCount+1,"Retry tạo thêm nhu cầu hoặc không tạo đủ một nhu cầu.");await using var db=c.Db();var operation=Guid.Parse(form["MaThaoTac"]);Check(await db.Set<LuotDangNhuCau>().CountAsync(x=>x.TaiKhoanId==c.HS&&x.MaThaoTac==operation)==1,"Cùng mã thao tác phải có đúng một lượt đăng.");return;
        }
        if(n is 19 or 22 or 23 or 25 or 28 or 29 or 20)
        {
            var expected=n is 19 or 22?403:n is 23 or 25?409:n==28?401:n==29?400:422;
            Check(n==20?status>=400&&status<500:status==expected,$"HTTP kỳ vọng {(n==20?"4xx":expected)}, thực tế {status}.");
            if(n==20)Check((await r.Content.ReadAsStringAsync()).Contains("ok"),"Từ chối phải có body lý do.");
            Check(await c.NeedCount()==initialCount,"Thao tác bị từ chối đã đổi số nhu cầu.");await using var db=c.Db();Check(await db.Set<LuotDangNhuCau>().CountAsync(x=>c.Users.Contains(x.TaiKhoanId))==initialPosts,"Thao tác bị từ chối đã đổi lượt đăng.");
            Check(JsonSerializer.Serialize(c.Proof["beforeOperation"])==JsonSerializer.Serialize(await Snapshot(c)),"Yêu cầu bị từ chối đã đổi dữ liệu nghiệp vụ (nhu cầu/môn/lịch/lượt đăng/hồ sơ).");
            if(target.HasValue)
            {
                var after=await c.ReadNeed(target.Value);var expectedRow=n==23?(NhuCauHoc)c.Proof["savedA"]!:before!;Check(JsonSerializer.Serialize(after)==JsonSerializer.Serialize(expectedRow),"Nhu cầu mục tiêu bị sửa dù yêu cầu phải từ chối.");
                Check(JsonSerializer.Serialize(await c.ReadSlots(target.Value))==JsonSerializer.Serialize(c.Proof[n==23?"savedASlots":"targetBeforeSlots"]),"Lịch mục tiêu thay đổi ngoài kỳ vọng.");
            }
            if(n==25){var row=(NhuCauHoc)c.Proof["secondBefore"]!;Check(JsonSerializer.Serialize(await c.ReadNeed(row.Id))==JsonSerializer.Serialize(row),"N2 bị sửa bằng mã thao tác N1.");}
            return;
        }
        await Ok(r);await using(var db=c.Db())
        {
            var rows=await db.Set<NhuCauHoc>().AsNoTracking().Where(x=>c.Users.Contains(x.ChuSoHuuId)).ToListAsync();var row=target.HasValue?rows.SingleOrDefault(x=>x.Id==target):rows.SingleOrDefault();
            Check(row!=null,"Không có nhu cầu được lưu.");if(row==null)return;c.Proof["target"]=row.Id;
            Check(row.ChuSoHuuId==actor,"Sai chủ nhu cầu.");Check(row.TrangThai==(n==17?"BanNhap":"DangMo"),"Sai trạng thái nhu cầu.");
            var links=await db.Set<NhuCauMonHoc>().Where(x=>x.NhuCauId==row.Id).Select(x=>x.MonHocId).ToListAsync();var expectedSubjects=n is 16 or 18?new[]{c.Math,c.Literature}:new[]{n==21?c.Literature:c.Math};Check(links.Order().SequenceEqual(expectedSubjects.Order()),"Danh sách môn lưu thiếu/thừa/sai.");
            var slots=await c.ReadSlots(row.Id);Check(slots.Count==(n==17?0:2),"Số lịch không đúng.");
            if(n==18)Check(row.TenNguoiHoc=="An"&&row.TuoiNguoiHoc==12,"Tên/tuổi người học không đúng.");
            if(n==21){Check(!row.RowVersion.SequenceEqual(before!.RowVersion),"RowVersion không đổi sau update.");Check(slots.Select(x=>(int)x.ThuTrongTuan).SequenceEqual(new[]{2,4}),"Lịch cũ chưa được thay hoàn toàn.");}
            if(n==17)Check(!await db.Set<LuotDangNhuCau>().AnyAsync(x=>x.NhuCauId==row.Id),"Bản nháp lại tạo lượt đăng công khai.");
            if(n==16){var op=Guid.Parse(form["MaThaoTac"]);Check(await db.Set<LuotDangNhuCau>().CountAsync(x=>x.NhuCauId==row.Id&&x.TaiKhoanId==actor&&x.MaThaoTac==op)==1,"Thiếu/trùng lượt đăng đúng mã thao tác.");Check(rows.Count==1,"Hai môn phải dùng chung một nhu cầu.");}
        }
    }
    private async Task ProfileSearch(It0203Case c,int n)
    {
        if(n==36)
        {
            c.Proof["beforeOperation"]=await Snapshot(c);await using var db=c.Db();await db.Database.OpenConnectionAsync();using var cmd=db.Database.GetDbConnection().CreateCommand();cmd.CommandText="EXEC dbo.usp_TimKiemGiaSu @ThuTrongTuan=1";try{await cmd.ExecuteNonQueryAsync();Check(false,"Procedure phải trả SQL 51202.");}catch(SqlException ex){c.Proof["sqlError"]=new{ex.Number,ex.Message};Check(ex.Number==51202,$"SQL kỳ vọng 51202, thực tế {ex.Number}.");}Check(JsonSerializer.Serialize(c.Proof["beforeOperation"])==JsonSerializer.Serialize(await Snapshot(c)),"Procedure tìm kiếm đã đổi dữ liệu nghiệp vụ.");return;
        }
        if(n is 30 or 31)
        {
            await using(var db=c.Db()){await db.Set<HoSoGiaSu>().Where(x=>x.Id==c.P1).ExecuteUpdateAsync(s=>s.SetProperty(x=>x.DangHienThi,false).SetProperty(x=>x.TrangThaiDuyet,n==31?"ChoDuyet":"BanNhap"));}
            if(n==30)
            {
                using var tutor=await c.Client(c.GS);await using var db=c.Db();var profile=await db.Set<HoSoGiaSu>().AsNoTracking().SingleAsync(x=>x.Id==c.P1);
                var form=new Dictionary<string,string>{["RowVersion"]=Convert.ToBase64String(profile.RowVersion),["HoTen"]=c.Tag+" Gia sư",["TrinhDo"]="Đại học",["ChuyenMon"]="Toán, Văn",["KinhNghiem"]="3 năm",["MoTa"]=c.Tag,["KhuVuc"]="Hà Nội",["DayTrucTuyen"]="true",["DayTrucTiep"]="false"};
                for(int i=0;i<2;i++){form[$"MonDay[{i}].MonHocId"]=(i==0?c.Math:c.Literature).ToString();form[$"MonDay[{i}].PhamViDay"]="Lớp 8";form[$"MonDay[{i}].HocPhiDeXuat"]=(150000+i*50000).ToString();form[$"MonDay[{i}].DonViHocPhi"]="VND/Gio";}
                using var r=await Post(c,tutor,"/NhuCau/HoSoGiaSu/Luu",form,"/NhuCau/HoSoGiaSu/Sua");await Ok(r);await using var fresh=c.Db();var h=await fresh.Set<HoSoGiaSu>().AsNoTracking().SingleAsync(x=>x.Id==c.P1);var rows=await fresh.Set<HoSoGiaSuMonDay>().Where(x=>x.HoSoGiaSuId==c.P1).OrderBy(x=>x.MonHocId).ToListAsync();Check(h.TrangThaiDuyet=="BanNhap"&&!h.DangHienThi,"Lưu hồ sơ không được tự duyệt/công khai.");Check(rows.Count==2&&rows.Any(x=>x.MonHocId==c.Math&&x.HocPhiDeXuat==150000)&&rows.Any(x=>x.MonHocId==c.Literature&&x.HocPhiDeXuat==200000),"Môn/phí chưa lưu đúng.");return;
            }
            var admin=await c.User("QuanTri","Admin");await using(var db=c.Db()){db.Add(new IdentityUserClaim<Guid>{UserId=admin,ClaimType="EduConnect.AdminPermission",ClaimValue="DuyetHoSo"});await db.SaveChangesAsync();}
            using(var adminClient=await c.Client(admin)){await using var db=c.Db();var h=await db.Set<HoSoGiaSu>().AsNoTracking().SingleAsync(x=>x.Id==c.P1);using var approve=await Post(c,adminClient,"/QuanTri/HoSo/Duyet",new(){["id"]=c.P1.ToString(),["rowVersion"]=Convert.ToBase64String(h.RowVersion),["approve"]="true"},"/QuanTri/HoSo/ChiTiet/"+c.P1);await Ok(approve);c.Proof["approvalHttp"]=c.Proof["http"];}
            using(var tutor=await c.Client(c.GS)){await using var db=c.Db();var h=await db.Set<HoSoGiaSu>().AsNoTracking().SingleAsync(x=>x.Id==c.P1);using var show=await Post(c,tutor,"/NhuCau/HoSoGiaSu/TrangThai",new(){["operation"]="Hien",["rowVersion"]=Convert.ToBase64String(h.RowVersion)},"/NhuCau/HoSoGiaSu/Sua");await Ok(show);}
        }
        if(n==32)
        {
            var g2=await c.User("GiaSu","Pending");c.Profiles["H2"]=await c.Profile(g2,c.Math,150000,"VND/Gio",true,false,"Hà Nội",false,"ChoDuyet");
            var g3=await c.User("GiaSu","Hidden");c.Profiles["H3"]=await c.Profile(g3,c.Math,150000,"VND/Gio",true,false,"Hà Nội",false);
        }
        if(n==33)
        {
            foreach(var p in new[]{("PerSession",150000m,"VND/Buoi"),("Expensive",250000m,"VND/Gio")}){var g=await c.User("GiaSu",p.Item1);c.Profiles[p.Item1]=await c.Profile(g,c.Math,p.Item2,p.Item3,true,false,"Hà Nội");}
        }
        if(n is 34 or 38 or 39 or 40)
        {
            var count=n==34?22:3;for(var i=0;i<count;i++){var g=await c.User("GiaSu","Extra"+i);c.Profiles["E"+i]=await c.Profile(g,c.Math,150000,"VND/Gio",true,false,"Hà Nội");}
        }
        if(n==41){await using var db=c.Db();await db.Set<HoSoGiaSu>().Where(x=>x.Id==c.P1).ExecuteUpdateAsync(s=>s.SetProperty(x=>x.DangHienThi,false));}
        c.Proof["profiles"]=c.Profiles;
        c.Proof["beforeOperation"]=await Snapshot(c);
        using var client=await c.Client(c.HS);
        var path="/TimKiem/TimKiem/Tim?TuKhoa="+c.Tag+"&MonHocId="+c.Math;
        if(n==33)path+="&HocPhiToiDa=200000&DonViHocPhi=VND%2FGio";
        if(n==34)path+="&Trang=1&KichThuocTrang=20";
        if(n==35)path+="&Trang=abc";
        if(n is >=37 and <=40)
        {
            var ids=n==37?new[]{c.P1}:n==38?new[]{c.P1,c.Profiles["E0"],c.Profiles["E1"]}:n==39?new[]{c.P1,c.Profiles["E0"],c.Profiles["E1"],c.Profiles["E2"]}:new[]{c.P1,c.P1,c.Profiles["E0"],c.Profiles["E1"]};
            path="/TimKiem/TimKiem/SoSanh?"+string.Join("&",ids.Select(x=>"ids="+x));
        }
        if(n==41)path="/TimKiem/TimKiem/ChiTiet/"+c.P1;
        using var req=new HttpRequestMessage(HttpMethod.Get,path);req.Headers.Add("X-Requested-With","XMLHttpRequest");if(n is >=37 and <=40)req.Headers.Add("X-Edu-Modal","true");
        using var response=await client.SendAsync(req);await c.RecordHttp(response);c.Proof["requestPath"]=path;var body=await response.Content.ReadAsStringAsync();var status=(int)response.StatusCode;
        Check(JsonSerializer.Serialize(c.Proof["beforeOperation"])==JsonSerializer.Serialize(await Snapshot(c)),"GET tìm kiếm/so sánh/chi tiết đã đổi dữ liệu nghiệp vụ.");
        Check(status==(n is 35 or 39?400:n==41?404:200),$"HTTP kỳ vọng {(n is 35 or 39?400:n==41?404:200)}, thực tế {status}.");
        if(n is 35 or 39 or 41)return;
        if(n is >=37 and <=40){Check(response.Content.Headers.ContentType?.MediaType=="text/html","So sánh phải trả HTML.");Check(!body.Contains("<html",StringComparison.OrdinalIgnoreCase)&&!body.Contains("<head",StringComparison.OrdinalIgnoreCase),"So sánh modal trả layout đầy đủ.");Check(body.Contains(c.P1.ToString()),"Thiếu đối tượng A trong bảng so sánh.");if(n is 38 or 40){Check(body.Contains(c.Profiles["E0"].ToString())&&body.Contains(c.Profiles["E1"].ToString()),"Thiếu B/C trong bảng so sánh.");var header=Regex.Match(body,"<thead>(.*?)</thead>",RegexOptions.Singleline).Value;var columns=Regex.Matches(header,"<th(?:\\s|>)").Count;c.Proof["compareColumns"]=columns;Check(columns==4,"Bảng phải có 1 cột tiêu chí và đúng 3 cột đối tượng phân biệt.");}return;}
        var cards=Regex.Matches(body,"<article\\s+class=\"card candidate\"").Count;c.Proof["cards"]=cards;
        if(n is 31 or 32 or 33)Check(cards==1,$"Kỳ vọng đúng 1 ứng viên, thực tế {cards}.");
        if(n==32)foreach(var hidden in new[]{"H2","H3"})Check(!body.Contains(c.Profiles[hidden].ToString()),"Lộ hồ sơ "+hidden);
        if(n==33)foreach(var bad in new[]{"PerSession","Expensive"})Check(!body.Contains(c.Profiles[bad].ToString()),"Lọc phí trả hồ sơ sai đơn vị/vượt phí "+bad);
        if(n==31){await using var db=c.Db();var h=await db.Set<HoSoGiaSu>().AsNoTracking().SingleAsync(x=>x.Id==c.P1);Check(h.TrangThaiDuyet=="DaDuyet"&&h.DangHienThi,"Hồ sơ chưa DaDuyet/hiển thị sau duyệt và bật.");Check(!await db.Set<NhuCauHoc>().AnyAsync(x=>x.ChuSoHuuId==c.GS),"Fixture không được dùng NhanLop cũ.");}
        if(n==34)
        {
            Check(cards==20,$"Trang 1 phải có 20 ứng viên, thực tế {cards}.");c.Proof["page1Http"]=c.Proof["http"];
            using var second=await client.GetAsync(path.Replace("Trang=1","Trang=2"));await c.RecordHttp(second);var b2=await second.Content.ReadAsStringAsync();var count2=Regex.Matches(b2,"<article\\s+class=\"card candidate\"").Count;c.Proof["page2Cards"]=count2;Check((int)second.StatusCode==200,"Trang 2 không trả 200.");Check(count2==3,$"Trang 2 phải có 3 ứng viên, thực tế {count2}.");
            var ids=c.Profiles.Values.ToList();var one=ids.Where(id=>body.Contains(id.ToString())).ToList();var two=ids.Where(id=>b2.Contains(id.ToString())).ToList();Check(one.Count==20&&two.Count==3&&!one.Intersect(two).Any()&&one.Union(two).Count()==23,"Phân trang trùng/thiếu ID hoặc tổng không bằng 23.");Check(WebUtility.HtmlDecode(body).Contains("23"),"HTML không có tổng 23.");c.Proof["page1Ids"]=one;c.Proof["page2Ids"]=two;
        }
    }
}
