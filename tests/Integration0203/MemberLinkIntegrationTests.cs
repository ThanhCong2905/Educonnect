using System.Net;
using System.Text.Json;
using EduConnect.Mvc.Areas.HocTap.Services;
using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xunit;

public sealed partial class Integration0203Tests
{
    static LienKetTaiKhoanService LinksService(It0203Scope s)=>new(s.Db,s.Current,new ThongBaoService(s.Db,s.Current));
    static LopHocService ClassesService(It0203Scope s)=>new(s.Db,s.Current,new QuyenLopHocService(s.Db,s.Current),new ThongBaoService(s.Db,s.Current),s.Contacts);
    static LichHocService CalendarService(It0203Scope s)=>new(s.Db,s.Current,new QuyenLopHocService(s.Db,s.Current),new ThongBaoService(s.Db,s.Current));
    static async Task<string> EmailOf(It0203Case c,Guid user){await using var db=c.Db();return await db.Users.Where(x=>x.Id==user).Select(x=>x.Email!).SingleAsync();}
    static async Task<LienKetPhuHuynhHocSinh> LinkOf(It0203Case c){await using var db=c.Db();return await db.Set<LienKetPhuHuynhHocSinh>().AsNoTracking().SingleAsync(x=>x.PhuHuynhId==c.PH&&x.HocSinhId==c.HS);}
    static async Task<Guid> SeedLink(It0203Case c,bool agree=true)
    {
        await using(var s=c.Scope(c.PH))await LinksService(s).GuiDeNghiAsync(await EmailOf(c,c.HS));var link=await LinkOf(c);
        if(agree){await using var s=c.Scope(c.HS);await LinksService(s).PhanHoiAsync(link.Id,true,Convert.ToBase64String(link.RowVersion));}return link.Id;
    }
    static async Task<Guid> SeedClass(It0203Case c,Guid? learner=null)
    {
        var owner=learner??c.HS;var input=c.Input();if(owner==c.PH){input.TenNguoiHoc="Học sinh fixture";input.TuoiNguoiHoc=12;}
        var need=await c.CreateNeed(input,owner);var request=await SeedProposal05(c,owner,need);var rv=(await ReadRequest(c,request)).RowVersion;await using var s=c.Scope(owner);return await s.Contacts.ChapNhanDeXuatAsync(request,Convert.ToBase64String(rv));
    }
    static async Task<ThanhVienLopHoc> MemberOf(It0203Case c,Guid cl,Guid user){await using var db=c.Db();return await db.Set<ThanhVienLopHoc>().AsNoTracking().SingleAsync(x=>x.LopHocId==cl&&x.TaiKhoanId==user&&x.RoiLopLuc==null);}
    static async Task<Guid> SeedInvite(It0203Case c,Guid cl,bool approved=false)
    {
        await using(var s=c.Scope(c.PH))await ClassesService(s).DeNghiAsync(cl,await EmailOf(c,c.HS));var m=await MemberOf(c,cl,c.HS);
        await using(var s=c.Scope(c.HS))await ClassesService(s).PhanHoiLoiMoiAsync(m.Id,true,Convert.ToBase64String(m.RowVersion));m=await MemberOf(c,cl,c.HS);
        if(approved){await using var s=c.Scope(c.GS);await ClassesService(s).DuyetAsync(m.Id,"DaDuyet","ChinhSua",Convert.ToBase64String(m.RowVersion),true);}return m.Id;
    }
    static async Task<It67Snapshot> Snapshot67(It0203Case c)
    {
        var b=await Snapshot05(c);var classes=b.Business.Classes.Select(x=>x.Id).ToArray();var lids=b.Calendar.Select(x=>x.Id).ToArray();var bids=b.Lessons.Select(x=>x.Id).ToArray();await using var db=c.Db();
        return new(b,await db.Set<LienKetPhuHuynhHocSinh>().AsNoTracking().Where(x=>c.Users.Contains(x.PhuHuynhId)||c.Users.Contains(x.HocSinhId)).OrderBy(x=>x.Id).ToListAsync(),await db.Set<LichSuDieuChinhLich>().AsNoTracking().Where(x=>lids.Contains(x.LichHocId)).OrderBy(x=>x.Id).ToListAsync(),await db.Set<LichSuGhiNhanBuoiHoc>().AsNoTracking().Where(x=>bids.Contains(x.BuoiHocId)).OrderBy(x=>x.Id).ToListAsync(),await db.Set<TinNhan>().AsNoTracking().Where(x=>c.Users.Contains(x.NguoiGuiId)).OrderBy(x=>x.Id).ToListAsync(),await db.Set<BaiTapGiao>().AsNoTracking().Where(x=>classes.Contains(x.LopHocId)).OrderBy(x=>x.Id).ToListAsync(),await db.Set<BaiTapNop>().AsNoTracking().Where(x=>c.Users.Contains(x.NguoiNopId)).OrderBy(x=>x.Id).ToListAsync());
    }
    static async Task<(LichHoc Lich,BuoiHoc Buoi)> FirstLesson(It0203Case c,Guid cl)
    {
        await using var db=c.Db();var l=await db.Set<LichHoc>().AsNoTracking().Where(x=>x.LopHocId==cl).OrderBy(x=>x.BatDauLuc).FirstAsync();return(l,await db.Set<BuoiHoc>().AsNoTracking().SingleAsync(x=>x.LichHocId==l.Id));
    }
    static async Task SetLessonTime(It0203Case c,Guid id,bool past)
    {
        await using var db=c.Db();var start=past?-120:-5;var end=past?-60:55;await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.LichHoc SET BatDauLuc=DATEADD(minute,{start},SYSDATETIMEOFFSET()),KetThucLuc=DATEADD(minute,{end},SYSDATETIMEOFFSET()) WHERE Id={id}");
    }
    static async Task<HttpResponseMessage> PostWithToken(HttpClient client,string path,Dictionary<string,string> data,string tokenPath)
    {data["__RequestVerificationToken"]=await It0203Case.Token(client,tokenPath);return await Send04(client,path,data);}
    private async Task Run06(int n)
    {
        It0203Case? c=null;Exception? error=null;var trace=new It04SqlTrace();
        try
        {
            c=await It0203Case.Create($"IT-{n:000}");c.Factory=new It0203Factory(trace);await Identify67(c);
            Guid? link=null,cl=null,member=null;Guid? assignment=null;
            if(n!=65)link=await SeedLink(c,n is not 66 and not 67);
            if(n is >=68 and <=71 or 73)cl=await SeedClass(c,c.PH);
            if(n is 72 or 74)cl=await SeedClass(c);
            if(n is 68 or 70 or 71 or 73)member=await SeedInvite(c,cl!.Value,n is 68 or 73);
            if(n==70)
            {
                var lesson=await FirstLesson(c,cl!.Value);await SetLessonTime(c,lesson.Lich.Id,false);await using(var s=c.Scope(c.GS))await CalendarService(s).BatDauBuoiAsync(lesson.Lich.Id,Convert.ToBase64String(lesson.Buoi.RowVersion));
                await using(var s=c.Scope(c.GS))
                {
                    var rights=new QuyenLopHocService(s.Db,s.Current);var svc=new BaiTapService(s.Db,s.Current,rights,new ThongBaoService(s.Db,s.Current),new TepService(s.Db,s.Current,rights,new It0203Environment()));
                    await svc.GiaoAsync(new(){LopHocId=cl.Value,BuoiHocId=lesson.Buoi.Id,TieuDe="Bài tập kiểm quyền",NoiDung="Nội dung bài kiểm quyền",HanNopLuc=DateTime.Today.AddDays(10)});
                }
                await using var db=c.Db();assignment=await db.Set<BaiTapGiao>().Where(x=>x.LopHocId==cl).Select(x=>x.Id).SingleAsync();
            }
            var actor=n is 66 or 67?c.HS:n is 69 or 70 or 71 or 73?c.GS:n==72?c.HS2:c.PH;
            c.Proof["actorId"]=actor;using var client=await c.Client(actor);HttpClient? learner=null;
            if(n is 68 or 73){learner=await c.Client(c.HS);using var initial=await learner.GetAsync($"/HocTap/LopHoc/ChiTiet/{cl}");Check(initial.StatusCode==HttpStatusCode.OK,"Fixture phải cho H truy cập trước thu hồi quyền.");c.Proof["initialAccessStatus"]=(int)initial.StatusCode;}
            var before=await Snapshot67(c);c.Proof["before"]=before;c.Proof["classId"]=cl;c.Proof["linkId"]=link;c.Proof["memberId"]=member;c.Proof["assignmentId"]=assignment;
            string path;Dictionary<string,string> form=[];bool get=n is 72 or 74;
            if(n==65){path="/HocTap/LienKetTaiKhoan/GuiDeNghi";form["email"]=await EmailOf(c,c.HS);}
            else if(n is 66 or 67){var l=await LinkOf(c);path="/HocTap/LienKetTaiKhoan/PhanHoi";form=new(){["id"]=l.Id.ToString(),["dongY"]=(n==66).ToString(),["rowVersion"]=Convert.ToBase64String(l.RowVersion)};}
            else if(n==68){var l=await LinkOf(c);path="/HocTap/LienKetTaiKhoan/Huy";form=new(){["id"]=l.Id.ToString(),["rowVersion"]=Convert.ToBase64String(l.RowVersion)};}
            else if(n==69){path="/HocTap/LopHoc/MoiThanhVien";form=new(){["id"]=cl.ToString()!,["email"]=await EmailOf(c,c.HS)};}
            else if(n is 70 or 71 or 73){var m=await MemberOf(c,cl!.Value,c.HS);path="/HocTap/LopHoc/Duyet";form=new(){["id"]=cl.ToString()!,["member"]=m.Id.ToString(),["action"]="DaDuyet",["permission"]=n==71?"ChinhSua":"Xem",["choPhepNopBai"]=(n==71).ToString(),["rowVersion"]=Convert.ToBase64String(m.RowVersion)};c.Proof["parameterMapping"]="member là Id bản ghi ThanhVienLopHoc của H, không phải TaiKhoanId H.";}
            else path=$"/HocTap/LopHoc/ChiTiet/{cl}";
            c.Proof["requestPath"]=path;c.Proof["input"]=new Dictionary<string,string>(form);
            using var response=get?await client.GetAsync(path):await PostWithToken(client,path,form,"/HocTap/LopHoc");await c.RecordHttp(response);var primary=c.Proof["http"]!;
            if(get)Check((int)response.StatusCode is 403 or 404,"Không có quyền riêng ở lớp thì không được trả200 dữ liệu lớp.");else await Ok(response);
            if(n is 68 or 73)
            {
                var room=before.Class.Business.Rooms.Single(x=>x.LopHocId==cl&&x.LoaiHoiThoai=="TheoLop").Id;
                if(n==68){using var denied=await learner!.GetAsync($"/HocTap/LopHoc/ChiTiet/{cl}");await c.RecordHttp(denied);c.Proof["followUpAccess"]=c.Proof["http"];Check((int)denied.StatusCode is 403 or 404,"H phải mất quyền truy cập lớp sau hủy liên kết.");}
                var data=new Dictionary<string,string>{["MaHoiThoai"]=room.ToString(),["MaThaoTac"]=Guid.NewGuid().ToString(),["NoiDung"]="Không được ghi sau thu hồi"};
                using var deniedPost=await PostWithToken(learner!,"/HocTap/TinNhan/Gui",data,"/HocTap/LopHoc");await c.RecordHttp(deniedPost);c.Proof["followUpWrite"]=c.Proof["http"];Check(deniedPost.StatusCode==HttpStatusCode.Forbidden,"Cookie H giữ nguyên nhưng request ghi sau thu hồi phải403.");
                learner!.Dispose();
            }
            if(n==70)
            {
                using var h=await c.Client(c.HS);using var blocked=await PostWithToken(h,"/HocTap/BaiTap/Nop",new(){["BaiTapGiaoId"]=assignment.ToString()!,["MaThaoTac"]=Guid.NewGuid().ToString(),["NoiDung"]="Bài làm không được lưu"},"/HocTap/LopHoc");await c.RecordHttp(blocked);c.Proof["submissionAttempt"]=c.Proof["http"];Check(blocked.StatusCode==HttpStatusCode.Forbidden,"Thành viên Xem không được nộp bài.");
            }
            c.Proof["http"]=primary;var after=await Snapshot67(c);c.Proof["after"]=after;
            if(n<=68)
            {
                var l=after.Links.Single();Check(l.PhuHuynhId==c.PH&&l.HocSinhId==c.HS&&l.NguoiGuiId==c.PH,"Liên kết phải đúng cặp/người gửi.");
                if(n==65)Check(l.TrangThai=="ChoXacNhan"&&l.KetThucLuc==null&&after.Links.Count==1,"Phải một đề nghị đang chờ xác nhận.");
                if(n==66)Check(l.TrangThai=="DaLienKet"&&l.XacNhanLuc>=l.GuiLuc&&l.KetThucLuc==null,"Đồng ý phải DaLienKet và thời gian xác nhận hợp lệ.");
                if(n==67)Check(l.TrangThai=="TuChoi"&&l.KetThucLuc>=l.GuiLuc&&l.NguoiKetThucId==c.HS,"Từ chối phải đủ trạng thái/người/giờ kết thúc.");
                if(n==68){Check(l.TrangThai=="DaHuy"&&l.KetThucLuc!=null&&l.NguoiKetThucId==c.PH,"Hủy liên kết phải DaHuy đúng người.");var m=after.Class.Members.Single(x=>x.Id==member);Check(m.TrangThaiDuyet=="DaRoiLop"&&m.RoiLopLuc!=null&&after.Messages.Count==before.Messages.Count,"Quyền phụ thuộc phải thu hồi, không ghi tin.");}
            }
            if(n==69){var members=after.Class.Members.Where(x=>x.TaiKhoanId==c.HS&&x.LopHocId==cl).ToList();Check(members.Count==1&&members[0].TrangThaiDuyet=="ChoDuyet"&&members[0].DongYThamGiaLuc==null&&members[0].DuyetLuc==null,"Lời mời phải được tạo và chưa tự duyệt/xác nhận.");}
            if(n is 70 or 71 or 73)
            {
                var m=after.Class.Members.Single(x=>x.Id==member);Check(m.TrangThaiDuyet=="DaDuyet"&&m.MucQuyen==(n==71?"ChinhSua":"Xem")&&m.LienKetId==link&&m.ChoPhepNopBai==(n==71),"Quyền/flag nộp bài/liên kết không đúng.");
                if(n==70)Check(after.Submissions.Count==0,"Không lưu bài nộp khi chỉ có quyền Xem.");
                if(n==73)Check(after.Messages.Count==before.Messages.Count,"Không được ghi sau downgrade quyền.");
            }
            if(get)Check(SameJson(before)==SameJson(after),"GET bị từ chối không được tự tạo quyền hoặc sửa nghiệp vụ.");
        }
        catch(Exception ex){error=ex;}
        finally{await Finish67(c,error,trace,n);}
        Assert.True(error==null,"Không hoàn tất ca: "+error);Assert.True(failures.Count==0,string.Join("\n",failures));
    }
    static async Task Identify67(It0203Case c){await using var db=c.Db();await db.Database.OpenConnectionAsync();await using var cmd=db.Database.GetDbConnection().CreateCommand();cmd.CommandText="SELECT DB_NAME()+'|'+@@SERVERNAME";c.Proof["serverDatabase"]=await cmd.ExecuteScalarAsync();}
    async Task Finish67(It0203Case? c,Exception? error,It04SqlTrace trace,int n)
    {
        if(c==null){if(error!=null)failures.Add("Fixture không khởi tạo: "+error);return;}
        c.Proof["sqlCommands"]=trace.Commands.ToArray();c.Proof["sqlErrorNumbers"]=trace.Errors.ToArray();
        try{await c.DisposeAsync();}catch(Exception ex){c.Proof["cleanupError"]=ex.ToString();failures.Add("Cleanup lỗi: "+ex.Message);}
        c.Proof["assertionDifferences"]=failures;c.Proof["exception"]=error?.ToString();var folder=Environment.GetEnvironmentVariable("IT0607_EVIDENCE");if(folder!=null){Directory.CreateDirectory(folder);await File.WriteAllTextAsync(Path.Combine(folder,$"IT-{n:000}.json"),JsonSerializer.Serialize(c.Proof,new JsonSerializerOptions{WriteIndented=true}));}output.WriteLine("OBSERVATION_JSON:"+JsonSerializer.Serialize(c.Proof));
    }
}
internal sealed record It67Snapshot(It05Snapshot Class,List<LienKetPhuHuynhHocSinh> Links,List<LichSuDieuChinhLich> ScheduleHistory,List<LichSuGhiNhanBuoiHoc> LessonHistory,List<TinNhan> Messages,List<BaiTapGiao> Assignments,List<BaiTapNop> Submissions);
