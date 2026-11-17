using System.Collections.Concurrent;
using System.Data.Common;
using System.Net;
using System.Text.Json;
using EduConnect.Mvc.Areas.KetNoi.ViewModels;
using EduConnect.Mvc.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;

public sealed partial class Integration0203Tests
{
    private async Task Run04(int n)
    {
        It0203Case? c=null;Exception? error=null;var trace=new It04SqlTrace();
        try
        {
            c=await It0203Case.Create($"IT-{n:000}");c.Factory=new It0203Factory(trace);
            await using(var db=c.Db())
            {
                await db.Database.OpenConnectionAsync();await using var cmd=db.Database.GetDbConnection().CreateCommand();cmd.CommandText="SELECT DB_NAME()+'|'+@@SERVERNAME";c.Proof["serverDatabase"]=await cmd.ExecuteScalarAsync();
            }
            Guid? contact=null,need=null;string? stale=null;
            if(n is >=46 and <=52)
            {
                await using(var s=c.Scope())await s.Contacts.GuiLienHeAsync(new(){DoiTuongId=c.GS,LoiNhan="Liên hệ fixture"});
                contact=(await c.Contact()).Id;
                if(n>=48){await using(var s=c.Scope(c.GS))await s.Contacts.PhanHoiLienHeAsync(contact.Value,"ChapNhan",Convert.ToBase64String((await c.Contact()).RowVersion));}
            }
            if(n is >=48 and <=52){var input=c.Input();input.MonHocIds=[c.Math,c.Literature];need=await c.CreateNeed(input);}
            var form=ContactForm(c);string path="/KetNoi/KetNoi/GuiLienHe";Guid? actor=n==46||n>=49&&n<=52?c.GS:n==47?c.HS2:n==53?null:c.HS;
            if(n==43)form["LoiNhan"]="";if(n==45)form["DoiTuongId"]=c.HS.ToString();
            if(n is 46 or 47){path="/KetNoi/KetNoi/PhanHoiLienHe";form=new(){["id"]=contact!.Value.ToString(),["action"]="ChapNhan",["rowVersion"]=Convert.ToBase64String((await c.Contact()).RowVersion)};}
            if(n==48){path="/KetNoi/KetNoi/GuiYeuCau";form=new(){["LienHeId"]=contact!.Value.ToString(),["NhuCauId"]=need!.Value.ToString(),["LoiNhan"]=" Muốn kết nối lớp "};}
            if(n is >=49 and <=52)
            {
                path="/KetNoi/KetNoi/DeXuat";form=ProposalForm(c,contact!.Value,need!.Value);
                if(n==50)
                {
                    var third=await c.Subject();await using var db=c.Db();db.Add(new HoSoGiaSuMonDay{HoSoGiaSuId=c.P1,MonHocId=third,PhamViDay="Lớp 8",HocPhiDeXuat=150000,DonViHocPhi="VND/Gio"});await db.SaveChangesAsync();form["MonHocId"]=third.ToString();c.Proof["subjectOutsideNeed"]=third;c.Proof["needSubjects"]=new[]{c.Math,c.Literature};
                }
                if(n==51)
                {
                    Guid request;await using(var s=c.Scope(c.GS))request=await s.Contacts.GuiDeXuatAsync(ProposalInput(c,contact.Value,need.Value));
                    var first=await ReadRequest(c,request);stale=Convert.ToBase64String(first.RowVersion);
                    var revised=ProposalInput(c,contact.Value,need.Value);revised.Id=request;revised.RowVersion=stale;revised.TenLop="Phiên bản mới fixture";revised.LichRanh=[new(){ThuTrongTuan=1,GioBatDau=new(18,0),GioKetThuc=new(19,0)},new(){ThuTrongTuan=3,GioBatDau=new(18,0),GioKetThuc=new(19,0)}];
                    await using(var s=c.Scope(c.GS))await s.Contacts.GuiDeXuatAsync(revised);
                    form["Id"]=request.ToString();form["RowVersion"]=stale;form["TenLop"]="KHÔNG ĐƯỢC GHI ĐÈ";c.Proof["requestId"]=request;c.Proof["staleVersion"]=stale;c.Proof["currentVersion"]=Convert.ToBase64String((await ReadRequest(c,request)).RowVersion);
                }
                if(n==52)foreach(var k in form.Keys.Where(x=>x.StartsWith("LichRanh[")).ToArray())form.Remove(k);
            }
            c.Proof["contactId"]=contact;c.Proof["needId"]=need;
            var before=await ContactSnapshot(c);c.Proof["before"]=before;
            using var client=await c.Client(actor);
            if(n==44)
            {
                using var second=await c.Client(c.HS);var t1=await It0203Case.Token(client,"/KetNoi/KetNoi/LienHe");var t2=await It0203Case.Token(second,"/KetNoi/KetNoi/LienHe");
                var barrier=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                async Task<HttpResponseMessage> Send(HttpClient cl,string token){await barrier.Task;var data=ContactForm(c);data["__RequestVerificationToken"]=token;return await Send04(cl,path,data);}
                var one=Send(client,t1);var two=Send(second,t2);barrier.SetResult();var rs=await Task.WhenAll(one,two);var evidence=new List<object>();
                foreach(var r in rs){await c.RecordHttp(r);evidence.Add(c.Proof["http"]!);}c.Proof["parallelResponses"]=evidence;
                var statuses=rs.Select(x=>(int)x.StatusCode).ToArray();c.Proof["statuses"]=statuses;Check(statuses.Count(x=>x==200)==1&&statuses.Count(x=>x is 400 or 409)==1,"Hai request đồng thời phải một thành công và một bị từ chối/xung đột.");foreach(var r in rs)r.Dispose();
                await using var db=c.Db();var index=await db.Database.SqlQueryRaw<string>("SELECT name AS Value FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.YeuCauLienHe') AND name='UQ_DangDung_YeuCauLienHe' AND is_unique=1").SingleOrDefaultAsync();c.Proof["uniqueIndex"]=index;Check(index!=null,"Thiếu unique index liên hệ hoạt động.");
            }
            else
            {
                if(n!=54)form["__RequestVerificationToken"]=await It0203Case.Token(client,n==53?"/TaiKhoan/DangNhap":"/KetNoi/KetNoi/LienHe");
                c.Proof["requestPath"]=path;c.Proof["input"]=form.Where(x=>x.Key!="__RequestVerificationToken").ToDictionary(x=>x.Key,x=>x.Value);
                using var response=await Send04(client,path,form);await c.RecordHttp(response);var status=(int)response.StatusCode;var body=await response.Content.ReadAsStringAsync();
                if(n is 42 or 46 or 48 or 49)await Ok(response);
                if(n==43){Check(status==422,"Thiếu lời nhắn phải HTTP 422.");Check(body.Contains("LoiNhan"),"JSON errors phải xác định LoiNhan.");}
                if(n==45)Check(status==400,"Tự gửi liên hệ phải HTTP 400.");
                if(n==47)Check(status==403,"Sai người phản hồi phải HTTP 403.");
                if(n is 50 or 51 or 52){Check(status>=400&&status<500,"Đề xuất không hợp lệ phải HTTP 4xx.");Check(!JsonOk(body),"Không được JSON ok=true.");}
                if(n==53){Check(status==401,"AJAX chưa đăng nhập phải 401.");Check(response.Headers.Location==null&&!body.Contains("<html",StringComparison.OrdinalIgnoreCase),"Không chuyển HTML đăng nhập.");}
                if(n==54)Check(status==400,"Thiếu antiforgery token phải HTTP 400.");
            }
            var after=await ContactSnapshot(c);c.Proof["after"]=after;c.Proof["sqlCommands"]=trace.Commands.ToArray();c.Proof["sqlErrorNumbers"]=trace.Errors.ToArray();
            Check(after.Classes.Count==0,"Các ca liên hệ/đề xuất không được tạo lớp học.");
            if(n is 42 or 44)
            {
                Check(after.Contacts.Count==1&&after.Contacts[0].TrangThai=="ChoPhanHoi","Phải đúng một liên hệ chờ phản hồi.");
                if(after.Contacts.Count==1){var row=after.Contacts[0];Check(row.NguoiGuiId==c.HS&&row.GiaSuId==c.GS&&row.BenHocId==c.HS&&row.LoiNhan=="Xin học","Lời nhắn phải trim và đúng hai bên/người gửi.");var notice=after.Notices.Where(x=>x.NguoiNhanId==c.GS&&x.DoiTuongId==row.Id).ToArray();Check(notice.Length==1,"Phải một thông báo đúng gia sư/mã liên hệ.");if(notice.Length==1)Check(after.Events.Any(x=>x.Id==notice[0].MaSuKien&&x.DuLieuToiThieu.Contains(c.GS.ToString())&&x.DuLieuToiThieu.Contains(row.Id.ToString())),"Notice phải gắn outbox đúng người và liên hệ.");}
                Check(after.Requests.Count==0&&after.Rooms.Count==0&&after.Slots.Count==0,"Gửi liên hệ không được tạo yêu cầu lớp/hội thoại/khung đề xuất.");
            }
            if(n==46){Check(after.Contacts.Single().TrangThai=="ChapNhan"&&after.Contacts.Single().NguoiPhanHoiId==c.GS,"Liên hệ phải ChapNhan, đúng người phản hồi.");Check(after.Rooms.Count==1&&after.Rooms[0].LoaiHoiThoai=="TruocKetNoi"&&after.Rooms[0].LienHeId==contact,"Chấp nhận chỉ tạo một hội thoại trước kết nối.");Check(after.Requests.Count==0,"Chấp nhận liên hệ chưa tạo yêu cầu lớp.");}
            if(n==48){Check(after.Requests.Count==1,"Phải đúng một yêu cầu kết nối.");if(after.Requests.Count==1){var r=after.Requests[0];Check(r.TrangThai=="ChoGiaSu"&&r.LienHeId==contact&&r.NguoiGuiId==c.HS&&r.NhuCauId==need,"Yêu cầu phải ChoGiaSu và đúng người/liên hệ/nhu cầu.");Check(r.HetHanLuc-r.GuiLuc==TimeSpan.FromHours(72),"Hạn yêu cầu phải GuiLuc+72h.");}}
            if(n==49){Check(after.Requests.Count==1&&after.Slots.Count==2,"Phải lưu một đề xuất và hai khung.");if(after.Requests.Count==1){var r=after.Requests[0];Check(r.TrangThai=="ChoBenHoc"&&r.NguoiDeXuatId==c.GS&&r.DeXuatLuc!=null&&r.HetHanLuc-r.DeXuatLuc==TimeSpan.FromHours(72),"Sai trạng thái/người đề xuất/thời hạn 72h.");Check(r.HocPhi==150000&&r.DonViHocPhi=="VND/Gio"&&r.MonHocId==c.Math,"Sai môn/học phí/đơn vị.");Check(after.Slots.All(x=>x.YeuCauKetNoiId==r.Id&&(x.GioKetThuc-x.GioBatDau).TotalMinutes==60)&&after.Slots.Select(x=>x.ThuTrongTuan).Order().SequenceEqual(new byte[]{1,3}),"Khung phải gắn đúng đề xuất, thứ 1/3 và 60 phút.");}}
            if(n is 43 or 45 or 47 or 50 or 51 or 52 or 53 or 54)Check(SameBusiness(before,after),"Dữ liệu nghiệp vụ trước/sau khác nhau dù request phải bị từ chối.");
            if(n==51)Check(trace.Errors.Contains(51139),"Chưa quan sát SQL51139 thật trong nhánh phiên bản cũ.");
        }
        catch(Exception ex){error=ex;}
        finally
        {
            if(c!=null)
            {
                c.Proof["sqlCommands"]=trace.Commands.ToArray();c.Proof["sqlErrorNumbers"]=trace.Errors.ToArray();
                try{await c.DisposeAsync();}catch(Exception ex){c.Proof["cleanupError"]=ex.ToString();error??=ex;}
                c.Proof["assertionDifferences"]=failures;c.Proof["exception"]=error?.ToString();
                var folder=Environment.GetEnvironmentVariable("IT04_EVIDENCE");if(folder!=null){Directory.CreateDirectory(folder);await File.WriteAllTextAsync(Path.Combine(folder,$"IT-{n:000}.json"),JsonSerializer.Serialize(c.Proof,new JsonSerializerOptions{WriteIndented=true}));}
                output.WriteLine("OBSERVATION_JSON:"+JsonSerializer.Serialize(c.Proof));
            }
        }
        Assert.True(error==null,"Không hoàn tất ca: "+error);Assert.True(failures.Count==0,string.Join("\n",failures));
    }
    static Dictionary<string,string> ContactForm(It0203Case c)=>new(){["DoiTuongId"]=c.GS.ToString(),["LoiNhan"]=" Xin học "};
    static DeXuatLopViewModel ProposalInput(It0203Case c,Guid contact,Guid need)=>new(){LienHeId=contact,NhuCauId=need,TenLop="Toán tích hợp "+c.Id,MonHocId=c.Math,MucTieu="Ôn Toán lớp 8",SoBuoiTuan=2,ThoiLuongPhut=60,BatDauHocLuc=DateTime.Today.AddDays(3).AddHours(19),HinhThucHoc="TrucTuyen",HocPhi=150000,DonViHocPhi="VND/Gio",LichRanh=[new(){ThuTrongTuan=1,GioBatDau=new(19,0),GioKetThuc=new(20,0)},new(){ThuTrongTuan=3,GioBatDau=new(19,0),GioKetThuc=new(20,0)}]};
    static Dictionary<string,string> ProposalForm(It0203Case c,Guid contact,Guid need)
    {var m=ProposalInput(c,contact,need);return new(){["LienHeId"]=contact.ToString(),["NhuCauId"]=need.ToString(),["TenLop"]=m.TenLop,["MonHocId"]=m.MonHocId.ToString(),["MucTieu"]=m.MucTieu,["SoBuoiTuan"]="2",["ThoiLuongPhut"]="60",["BatDauHocLuc"]=m.BatDauHocLuc.ToString("yyyy-MM-ddTHH:mm"),["HinhThucHoc"]="TrucTuyen",["HocPhi"]="150000",["DonViHocPhi"]="VND/Gio",["LichRanh[0].ThuTrongTuan"]="1",["LichRanh[0].GioBatDau"]="19:00",["LichRanh[0].GioKetThuc"]="20:00",["LichRanh[1].ThuTrongTuan"]="3",["LichRanh[1].GioBatDau"]="19:00",["LichRanh[1].GioKetThuc"]="20:00"};}
    static async Task<HttpResponseMessage> Send04(HttpClient client,string path,Dictionary<string,string> form){using var request=new HttpRequestMessage(HttpMethod.Post,path){Content=new FormUrlEncodedContent(form)};request.Headers.Add("X-Requested-With","XMLHttpRequest");return await client.SendAsync(request);}
    static bool JsonOk(string body){try{return JsonDocument.Parse(body).RootElement.TryGetProperty("ok",out var ok)&&ok.ValueKind==JsonValueKind.True;}catch(JsonException){return false;}}
    static async Task<YeuCauKetNoiLopHoc> ReadRequest(It0203Case c,Guid id){await using var db=c.Db();return await db.Set<YeuCauKetNoiLopHoc>().AsNoTracking().SingleAsync(x=>x.Id==id);}
    static string SameJson(object value)=>JsonSerializer.Serialize(value);
    static bool SameBusiness(It04Snapshot a,It04Snapshot b)=>SameJson(a)==SameJson(b);
    static async Task<It04Snapshot> ContactSnapshot(It0203Case c)
    {
        await using var db=c.Db();var requests=await db.Set<YeuCauKetNoiLopHoc>().AsNoTracking().Where(x=>c.Users.Contains(x.GiaSuId)||c.Users.Contains(x.BenHocId)).OrderBy(x=>x.Id).ToListAsync();var ids=requests.Select(x=>x.Id).ToList();
        var contacts=await db.Set<YeuCauLienHe>().AsNoTracking().Where(x=>c.Users.Contains(x.GiaSuId)||c.Users.Contains(x.BenHocId)).OrderBy(x=>x.Id).ToListAsync();
        var notices=await db.Set<ThongBao>().AsNoTracking().Where(x=>c.Users.Contains(x.NguoiNhanId)).OrderBy(x=>x.Id).ToListAsync();var events=notices.Select(x=>x.MaSuKien).ToList();
        return new(contacts,requests,await db.Set<KhungGioDeXuat>().AsNoTracking().Where(x=>ids.Contains(x.YeuCauKetNoiId)).OrderBy(x=>x.Id).ToListAsync(),await db.Set<CuocHoiThoai>().AsNoTracking().Where(x=>c.Users.Contains(x.GiaSuId??Guid.Empty)||c.Users.Contains(x.BenHocId??Guid.Empty)).OrderBy(x=>x.Id).ToListAsync(),await db.Set<LopHoc>().AsNoTracking().Where(x=>c.Users.Contains(x.GiaSuId)||c.Users.Contains(x.BenHocId)).OrderBy(x=>x.Id).ToListAsync(),notices,await db.Set<SuKienChoXuLy>().AsNoTracking().Where(x=>events.Contains(x.Id)).OrderBy(x=>x.Id).ToListAsync());
    }
}
internal sealed record It04Snapshot(List<YeuCauLienHe> Contacts,List<YeuCauKetNoiLopHoc> Requests,List<KhungGioDeXuat> Slots,List<CuocHoiThoai> Rooms,List<LopHoc> Classes,List<ThongBao> Notices,List<SuKienChoXuLy> Events);
internal sealed class It04SqlTrace:DbCommandInterceptor
{
    public ConcurrentQueue<string> Commands=new();public ConcurrentQueue<int> Errors=new();
    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command,CommandEventData data,InterceptionResult<DbDataReader> result,CancellationToken ct=default){if(command.CommandText.Contains("EXEC",StringComparison.OrdinalIgnoreCase))Commands.Enqueue(command.CommandText);return ValueTask.FromResult(result);}
    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command,CommandEventData data,InterceptionResult<int> result,CancellationToken ct=default){Commands.Enqueue(command.CommandText);return ValueTask.FromResult(result);}
    public override Task CommandFailedAsync(DbCommand command,CommandErrorEventData data,CancellationToken ct=default){if(data.Exception is SqlException e)Errors.Enqueue(e.Number);return Task.CompletedTask;}
}
