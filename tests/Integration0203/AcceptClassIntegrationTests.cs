using System.Net;
using System.Text.Json;
using EduConnect.Mvc.Areas.KetNoi.ViewModels;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

public sealed partial class Integration0203Tests
{
    private async Task Run05(int n)
    {
        It0203Case? c=null;Exception? error=null;var trace=new It04SqlTrace();
        try
        {
            c=await It0203Case.Create($"IT-{n:000}");c.Factory=new It0203Factory(trace);
            await using(var db=c.Db()){await db.Database.OpenConnectionAsync();await using var cmd=db.Database.GetDbConnection().CreateCommand();cmd.CommandText="SELECT DB_NAME()+'|'+@@SERVERNAME";c.Proof["serverDatabase"]=await cmd.ExecuteScalarAsync();}
            var need=await c.CreateNeed();var main=await SeedProposal05(c,c.HS,need);var proposal=await ReadRequest(c,main);var oldVersion=proposal.RowVersion.ToArray();
            c.Proof["requestId"]=main;c.Proof["needId"]=need;c.Proof["initialVersion"]=Convert.ToBase64String(oldVersion);
            if(n==58)
            {
                var revised=ProposalInput(c,proposal.LienHeId,need);revised.Id=main;revised.RowVersion=Convert.ToBase64String(oldVersion);revised.TenLop="Đề xuất mới v2";
                await using(var s=c.Scope(c.GS))await s.Contacts.GuiDeXuatAsync(revised);
                c.Proof["currentVersion"]=Convert.ToBase64String((await ReadRequest(c,main)).RowVersion);
            }
            if(n==59)
            {
                await using var db=c.Db();await db.Database.ExecuteSqlInterpolatedAsync($"DECLARE @expired datetimeoffset(0)=DATEADD(hour,-1,SYSDATETIMEOFFSET()); UPDATE dbo.YeuCauKetNoiLopHoc SET HetHanLuc=@expired,GuiLuc=DATEADD(hour,-72,@expired),DeXuatLuc=DATEADD(hour,-72,@expired) WHERE Id={main}");oldVersion=(await ReadRequest(c,main)).RowVersion;
                c.Proof["expiredRequest"]=await ReadRequest(c,main);c.Proof["workerStopped"]=true;
            }
            if(n==60)
            {
                var need2=await c.CreateNeed(actor:c.HS2);var otherRequest=await SeedProposal05(c,c.HS2,need2);var rv=(await ReadRequest(c,otherRequest)).RowVersion;Guid existing;
                await using(var s=c.Scope(c.HS2))existing=await s.Contacts.ChapNhanDeXuatAsync(otherRequest,Convert.ToBase64String(rv));
                c.Proof["conflictClassId"]=existing;c.Proof["conflictRequestId"]=otherRequest;
            }
            var before=await Snapshot05(c);c.Proof["before"]=before;var actor=n==57?c.PH:n==63?c.GS:c.HS;using var client=await c.Client(actor);
            var path=n==62?"/KetNoi/KetNoi/TuChoi":n>=63?"/KetNoi/KetNoi/Rut":"/KetNoi/KetNoi/ChapNhan";
            var form=new Dictionary<string,string>{["MaYeuCau"]=main.ToString(),["RowVersion"]=Convert.ToBase64String(oldVersion)};if(n==62)form["lyDo"]="Chưa phù hợp lịch học";
            form["__RequestVerificationToken"]=await It0203Case.Token(client,"/KetNoi/KetNoi/LienHe");
            c.Proof["requestPath"]=path;c.Proof["input"]=form.Where(x=>x.Key!="__RequestVerificationToken").ToDictionary(x=>x.Key,x=>x.Value);
            var responses=new List<object>();var classIds=new List<Guid>();
            if(n==61)
            {
                using var second=await c.Client(c.HS);var f2=new Dictionary<string,string>(form){["__RequestVerificationToken"]=await It0203Case.Token(second,"/KetNoi/KetNoi/LienHe")};
                var gate=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                async Task<HttpResponseMessage> Send(HttpClient cl,Dictionary<string,string> data){await gate.Task;return await Send04(cl,path,data);}
                var t1=Send(client,form);var t2=Send(second,f2);gate.SetResult();var rr=await Task.WhenAll(t1,t2);
                foreach(var response in rr){await c.RecordHttp(response);responses.Add(c.Proof["http"]!);await Ok(response);classIds.Add(await RedirectClass05(response));response.Dispose();}
                c.Proof["parallelResponses"]=responses;c.Proof["responseClassIds"]=classIds;Check(classIds.Distinct().Count()==1,"Hai client phải trỏ cùng một lớp.");
                await using var db=c.Db();var index=await db.Database.SqlQueryRaw<string>("SELECT name AS Value FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.LopHoc') AND name='UQ_YeuCau_LopHoc' AND is_unique=1").SingleOrDefaultAsync();c.Proof["uniqueClassIndex"]=index;Check(index!=null,"Thiếu unique index UQ_YeuCau_LopHoc.");
            }
            else
            {
                using var response=await Send04(client,path,form);await c.RecordHttp(response);responses.Add(c.Proof["http"]!);var status=(int)response.StatusCode;
                if(n is 55 or 56 or 62 or 63)await Ok(response);
                if(n is 55 or 56)classIds.Add(await RedirectClass05(response));
                if(n is 57 or 64)Check(status==403,"Sai người thao tác phải HTTP403.");
                if(n is 58 or 59 or 60)Check(status==409,"Phiên bản cũ/hết hạn/trùng lịch phải HTTP409.");
                if(n==56)
                {
                    var first=await Snapshot05(c);c.Proof["afterFirst"]=first;form["__RequestVerificationToken"]=await It0203Case.Token(client,"/KetNoi/KetNoi/LienHe");
                    using var second=await Send04(client,path,form);await c.RecordHttp(second);responses.Add(c.Proof["http"]!);await Ok(second);classIds.Add(await RedirectClass05(second));var twice=await Snapshot05(c);
                    Check(classIds.Distinct().Count()==1,"Chấp nhận lặp phải trả cùng mã lớp.");Check(SameJson(first)==SameJson(twice),"Lặp chấp nhận không được nhân lịch/thành viên/notice hoặc đổi dữ liệu đã tạo.");c.Proof["repeatResponses"]=responses;c.Proof["responseClassIds"]=classIds;
                }
            }
            var after=await Snapshot05(c);c.Proof["after"]=after;var target=after.Business.Requests.Single(x=>x.Id==main);var targetClasses=after.Business.Classes.Where(x=>x.YeuCauKetNoiId==main).ToList();
            if(n is 55 or 56 or 61)
            {
                Check(target.TrangThai=="DaChapNhan"&&targetClasses.Count==1,"Yêu cầu phải DaChapNhan và đúng một lớp.");
                if(targetClasses.Count==1)
                {
                    var cl=targetClasses[0];Check(classIds.All(x=>x==cl.Id),"URL phải trỏ đúng mã lớp được SQL tạo.");
                    Check(cl.TrangThai=="DangHoatDong"&&cl.GiaSuId==c.GS&&cl.BenHocId==c.HS&&cl.MonHocId==proposal.MonHocId&&cl.TenLop==proposal.TenLopDuKien&&cl.MucTieu==proposal.MucTieu&&cl.HinhThucHoc==proposal.HinhThucHoc&&cl.SoBuoiTuan==2&&cl.ThoiLuongPhut==60&&cl.HocPhi==150000&&cl.DonViHocPhi=="VND/Gio","Lớp phải sao chép đủ điều kiện đề xuất.");
                    var members=after.Members.Where(x=>x.LopHocId==cl.Id).ToList();Check(members.Count==2&&members.Select(x=>x.TaiKhoanId).Order().SequenceEqual(new[]{c.GS,c.HS}.Order())&&members.All(x=>x.TrangThaiDuyet=="DaDuyet"&&x.MucQuyen=="ChinhSua"&&x.DuyetLuc!=null),"Phải đủ hai thành viên chính đã duyệt/quyền ChinhSua.");
                    Check(after.Business.Rooms.Count(x=>x.LopHocId==cl.Id&&x.LoaiHoiThoai=="TheoLop")==1,"Chỉ một hội thoại TheoLop.");
                    var calendar=after.Calendar.Where(x=>x.LopHocId==cl.Id).ToList();Check(calendar.Count==2,"Phải đủ hai lịch đầu tiên.");
                    foreach(var slot in after.Business.Slots.Where(x=>x.YeuCauKetNoiId==main))
                    {
                        var date=proposal.BatDauHocLuc!.Value.ToOffset(TimeSpan.FromHours(7)).Date;var first=date.AddDays((slot.ThuTrongTuan-(int)date.DayOfWeek+7)%7);var start=new DateTimeOffset(first.Add(slot.GioBatDau.ToTimeSpan()),TimeSpan.FromHours(7));var end=new DateTimeOffset(first.Add(slot.GioKetThuc.ToTimeSpan()),TimeSpan.FromHours(7));
                        Check(calendar.Count(x=>x.BatDauLuc==start&&x.KetThucLuc==end&&x.TrangThai=="DaLenLich")==1,"Sai ngày/giờ đầu tiên hoặc trạng thái lịch từ khung đề xuất.");
                    }
                    var lids=calendar.Select(x=>x.Id).ToArray();var lessons=after.Lessons.Where(x=>lids.Contains(x.LichHocId)).ToList();Check(lessons.Count==2&&lessons.Select(x=>x.LichHocId).Distinct().Count()==2&&lessons.All(x=>x.TrangThai=="DuKien"),"Trigger phải tạo đúng một BuoiHoc DuKien cho mỗi lịch.");
                    Check(after.Business.Notices.Count-before.Business.Notices.Count==1&&after.Business.Events.Count-before.Business.Events.Count==1,"Chấp nhận chỉ thêm một notice/outbox, kể cả lặp/đồng thời.");
                }
            }
            if(n is 57 or 58 or 59 or 60 or 64)
            {
                Check(targetClasses.Count==0,"Ca bị từ chối không được còn lớp tạo dở.");Check(SameJson(before)==SameJson(after),"Toàn bộ nghiệp vụ trước/sau phải giữ nguyên khi thất bại, không có thành viên/hội thoại/lịch/buổi/notice mồ côi.");
                if(n is 58 or 59)Check(trace.Errors.Contains(51152),"Phải quan sát lỗi SQL51152 thật cho phiên bản/hạn.");
                if(n==60)Check(trace.Errors.Contains(51145),"Phải quan sát lỗi SQL51145 trùng lịch thật, không mô phỏng rollback.");
            }
            if(n is 62 or 63)
            {
                Check(target.TrangThai==(n==62?"TuChoi":"DaRut")&&targetClasses.Count==0,"Từ chối/rút phải đổi đúng trạng thái, không tạo lớp.");
                var recipient=n==62?c.GS:c.HS;var notices=after.Business.Notices.Where(x=>!before.Business.Notices.Any(b=>b.Id==x.Id)).ToList();Check(notices.Count==1&&notices[0].NguoiNhanId==recipient&&notices[0].DoiTuongId==main,"Thông báo phải đúng đối tác và mã yêu cầu.");
            }
        }
        catch(Exception ex){error=ex;}
        finally
        {
            if(c!=null)
            {
                c.Proof["sqlCommands"]=trace.Commands.ToArray();c.Proof["sqlErrorNumbers"]=trace.Errors.ToArray();
                try{await c.DisposeAsync();}catch(Exception ex){c.Proof["cleanupError"]=ex.ToString();error??=ex;}
                c.Proof["assertionDifferences"]=failures;c.Proof["exception"]=error?.ToString();var folder=Environment.GetEnvironmentVariable("IT05_EVIDENCE");if(folder!=null){Directory.CreateDirectory(folder);await File.WriteAllTextAsync(Path.Combine(folder,$"IT-{n:000}.json"),JsonSerializer.Serialize(c.Proof,new JsonSerializerOptions{WriteIndented=true}));}
                output.WriteLine("OBSERVATION_JSON:"+JsonSerializer.Serialize(c.Proof));
            }
        }
        Assert.True(error==null,"Không hoàn tất ca: "+error);Assert.True(failures.Count==0,string.Join("\n",failures));
    }
    static async Task<Guid> SeedProposal05(It0203Case c,Guid learner,Guid need)
    {
        await using(var s=c.Scope(learner))await s.Contacts.GuiLienHeAsync(new(){DoiTuongId=c.GS,LoiNhan="Liên hệ cho lớp fixture"});
        EduConnect.Mvc.Models.YeuCauLienHe contact;await using(var db=c.Db())contact=await db.Set<YeuCauLienHe>().AsNoTracking().SingleAsync(x=>x.GiaSuId==c.GS&&x.BenHocId==learner);
        await using(var s=c.Scope(c.GS))await s.Contacts.PhanHoiLienHeAsync(contact.Id,"ChapNhan",Convert.ToBase64String(contact.RowVersion));
        await using var tutor=c.Scope(c.GS);return await tutor.Contacts.GuiDeXuatAsync(ProposalInput(c,contact.Id,need));
    }
    static async Task<Guid> RedirectClass05(HttpResponseMessage response)
    {
        var body=await response.Content.ReadAsStringAsync();using var json=JsonDocument.Parse(body);if(!json.RootElement.TryGetProperty("redirectUrl",out var value))return Guid.Empty;var url=value.GetString()??"";const string prefix="/HocTap/LopHoc/ChiTiet/";return url.StartsWith(prefix)&&Guid.TryParse(url[prefix.Length..],out var id)?id:Guid.Empty;
    }
    static async Task<It05Snapshot> Snapshot05(It0203Case c)
    {
        var b=await ContactSnapshot(c);var ids=b.Classes.Select(x=>x.Id).ToArray();await using var db=c.Db();var calendar=await db.Set<LichHoc>().AsNoTracking().Where(x=>ids.Contains(x.LopHocId)).OrderBy(x=>x.Id).ToListAsync();var lids=calendar.Select(x=>x.Id).ToArray();
        return new(b,await db.Set<ThanhVienLopHoc>().AsNoTracking().Where(x=>ids.Contains(x.LopHocId)).OrderBy(x=>x.Id).ToListAsync(),calendar,await db.Set<BuoiHoc>().AsNoTracking().Where(x=>lids.Contains(x.LichHocId)).OrderBy(x=>x.Id).ToListAsync());
    }
}
internal sealed record It05Snapshot(It04Snapshot Business,List<ThanhVienLopHoc> Members,List<LichHoc> Calendar,List<BuoiHoc> Lessons);
