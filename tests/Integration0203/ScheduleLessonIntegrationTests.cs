using System.Net;
using System.Text.Json;
using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xunit;

public sealed partial class Integration0203Tests
{
    static Dictionary<string,string> RecordForm(Guid schedule,string rv,string text)=>new(){["LichHocId"]=schedule.ToString(),["RowVersion"]=rv,["KetQuaBuoiHoc"]="DaDienRa",["NoiDung"]=text,["NhanXet"]="Nhận xét đầy đủ",["TrangThaiGiaSu"]="CoMat",["TrangThaiBenHoc"]="CoMat",["CanHocBu"]="false"};
    static Dictionary<string,string> ChangeForm(Guid schedule,string rv,DateTime start)=>new(){["Id"]=schedule.ToString(),["RowVersion"]=rv,["TrangThai"]="DaDieuChinh",["LyDo"]="Đổi theo thỏa thuận hai bên",["Lich.TieuDe"]="Buổi theo lịch mới",["Lich.BatDauLuc"]=start.ToString("yyyy-MM-ddTHH:mm"),["Lich.KetThucLuc"]=start.AddHours(1).ToString("yyyy-MM-ddTHH:mm")};
    static Dictionary<string,string> BatchForm(Guid cl,params (DateTime Start,DateTime End)[] intervals)
    {
        var f=new Dictionary<string,string>{["LopHocId"]=cl.ToString()};for(int i=0;i<intervals.Length;i++){f[$"Buoi[{i}].TieuDe"]=$"Buổi mới {i+1}";f[$"Buoi[{i}].BatDauLuc"]=intervals[i].Start.ToString("yyyy-MM-ddTHH:mm");f[$"Buoi[{i}].KetThucLuc"]=intervals[i].End.ToString("yyyy-MM-ddTHH:mm");}return f;
    }
    static async Task<BuoiHoc> ReadLesson(It0203Case c,Guid schedule){await using var db=c.Db();return await db.Set<BuoiHoc>().AsNoTracking().SingleAsync(x=>x.LichHocId==schedule);}
    static async Task<LichHoc> ReadCalendar(It0203Case c,Guid schedule){await using var db=c.Db();return await db.Set<LichHoc>().AsNoTracking().SingleAsync(x=>x.Id==schedule);}
    static async Task<int> ProbeSql(It0203Case c,FormattableString sql)
    {await using var db=c.Db();try{await db.Database.ExecuteSqlInterpolatedAsync(sql);return 0;}catch(SqlException ex){return ex.Number;}}
    private async Task Run07(int n)
    {
        It0203Case? c=null;Exception? error=null;var trace=new It04SqlTrace();
        try
        {
            c=await It0203Case.Create($"IT-{n:000}");c.Factory=new It0203Factory(trace);await Identify67(c);var cl=await SeedClass(c);var first=await FirstLesson(c,cl);var schedule=first.Lich;var lesson=first.Buoi;
            if(n is 79 or 80){await SetLessonTime(c,schedule.Id,false);if(n==79){await using var s=c.Scope(c.GS);await CalendarService(s).BatDauBuoiAsync(schedule.Id,Convert.ToBase64String(lesson.RowVersion));}}
            if(n is 81 or 83 or 86)
            {
                await SetLessonTime(c,schedule.Id,true);
                if(n is 81 or 86){await using var s=c.Scope(c.GS);await CalendarService(s).GhiNhanAsync(new(){LichHocId=schedule.Id,RowVersion=Convert.ToBase64String(lesson.RowVersion),KetQuaBuoiHoc="DaDienRa",NoiDung="Bản ghi nền trước cập nhật",NhanXet="Nhận xét nền",TrangThaiGiaSu="CoMat",TrangThaiBenHoc="CoMat"});}
            }
            if(n==85){await using var s=c.Scope(c.GS);await CalendarService(s).ThongBaoNghiAsync(schedule.Id,"Nghỉ có báo trước",true,Convert.ToBase64String(lesson.RowVersion));}
            DateTime? conflictStart=null;
            if(n==77)
            {
                var need2=await c.CreateNeed(actor:c.HS2);var r=await SeedProposal05(c,c.HS2,need2);var proposed=await ReadRequest(c,r);var m=ProposalInput(c,proposed.LienHeId,need2);m.Id=r;m.RowVersion=Convert.ToBase64String(proposed.RowVersion);m.BatDauHocLuc=DateTime.Today.AddDays(18).AddHours(19);
                await using(var s=c.Scope(c.GS))await s.Contacts.GuiDeXuatAsync(m);var rv=(await ReadRequest(c,r)).RowVersion;Guid other;
                await using(var s=c.Scope(c.HS2))other=await s.Contacts.ChapNhanDeXuatAsync(r,Convert.ToBase64String(rv));
                var o=await FirstLesson(c,other);conflictStart=o.Lich.BatDauLuc.ToOffset(TimeSpan.FromHours(7)).DateTime;c.Proof["conflictClassId"]=other;c.Proof["conflictCalendarId"]=o.Lich.Id;
            }
            schedule=await ReadCalendar(c,schedule.Id);lesson=await ReadLesson(c,schedule.Id);var before=await Snapshot67(c);c.Proof["before"]=before;c.Proof["classId"]=cl;c.Proof["scheduleId"]=schedule.Id;c.Proof["lessonId"]=lesson.Id;
            var actor=n==83?await c.User("GiaSu","G2-outsider"):c.GS;c.Proof["actorId"]=actor;using var client=await c.Client(actor);var future=DateTime.Today.AddDays(20).AddHours(n==76?19:14);string path;Dictionary<string,string> form;
            if(n<=77)
            {
                path="/HocTap/LichHoc/Them";
                form=n==76?BatchForm(cl,(future,future.AddHours(1)),(future.AddMinutes(30),future.AddMinutes(90))):n==77?BatchForm(cl,(conflictStart!.Value,conflictStart.Value.AddHours(1)),(conflictStart.Value.AddHours(2),conflictStart.Value.AddHours(3))):BatchForm(cl,(future,future.AddHours(1)),(future.AddDays(1),future.AddDays(1).AddHours(1)));
            }
            else if(n is 78 or 79 or 85){path=n==85?"/HocTap/LichHoc/TaoBuoiBu":"/HocTap/LichHoc/Sua";form=ChangeForm(schedule.Id,Convert.ToBase64String(schedule.RowVersion),future);}
            else if(n==80){path="/HocTap/LichHoc/BatDauBuoi";form=new(){["id"]=schedule.Id.ToString(),["rowVersion"]=Convert.ToBase64String(lesson.RowVersion)};}
            else if(n==84){path="/HocTap/LichHoc/ThongBaoNghi";form=new(){["id"]=schedule.Id.ToString(),["reason"]=" Nghỉ vì việc gia đình ",["canHocBu"]="true",["rowVersion"]=Convert.ToBase64String(lesson.RowVersion)};}
            else{path="/HocTap/LichHoc/GhiNhan";form=RecordForm(schedule.Id,Convert.ToBase64String(lesson.RowVersion),"Nội dung A");}
            c.Proof["requestPath"]=path;c.Proof["input"]=new Dictionary<string,string>(form);
            var statuses=new List<int>();string? winner=null;
            if(n==86)
            {
                using var second=await c.Client(c.GS);form["__RequestVerificationToken"]=await It0203Case.Token(client,"/HocTap/LopHoc");var f2=new Dictionary<string,string>(form){["NoiDung"]="Nội dung B",["__RequestVerificationToken"]=await It0203Case.Token(second,"/HocTap/LopHoc")};var gate=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                async Task<HttpResponseMessage> Send(HttpClient clt,Dictionary<string,string> f){await gate.Task;return await Send04(clt,path,f);}
                var t1=Send(client,form);var t2=Send(second,f2);gate.SetResult();var rr=await Task.WhenAll(t1,t2);var http=new List<object>();
                for(int i=0;i<rr.Length;i++){statuses.Add((int)rr[i].StatusCode);await c.RecordHttp(rr[i]);http.Add(c.Proof["http"]!);if(rr[i].StatusCode==HttpStatusCode.OK)winner=i==0?"Nội dung A":"Nội dung B";rr[i].Dispose();}
                c.Proof["parallelResponses"]=http;c.Proof["statuses"]=statuses;c.Proof["winnerContent"]=winner;Check(statuses.Order().SequenceEqual(new[]{200,409}),"Hai cập nhật cùng RV phải một200 và một409.");Check(trace.Errors.Contains(51150),"Phải quan sát lỗiSQL51150 thật ở cập nhật dùng RV cũ.");
            }
            else
            {
                using var response=await PostWithToken(client,path,form,"/HocTap/LopHoc");await c.RecordHttp(response);int status=(int)response.StatusCode;statuses.Add(status);
                if(n is 75 or 78 or 80 or 81 or 84 or 85)await Ok(response);
                else if(n==83)Check(status==403,"Gia sư không phụ trách phảiHTTP403.");else Check(status>=400&&status<500,"Ca bị từ chối phải4xx, không500/200.");
            }
            if(n==79)
            {
                var guard=await ProbeSql(c,$"UPDATE dbo.LichHoc SET BatDauLuc=DATEADD(day,21,SYSDATETIMEOFFSET()),KetThucLuc=DATEADD(hour,1,DATEADD(day,21,SYSDATETIMEOFFSET())) WHERE Id={schedule.Id}");c.Proof["directSqlGuard"]=guard;Check(guard==51120,"SQL trực tiếp phải bị tr_LichHoc_GiuLichSu chặn bằng51120.");
            }
            if(n==82)
            {
                var guard=await ProbeSql(c,$"UPDATE dbo.BuoiHoc SET TrangThai='DaHoc',KetQuaBuoiHoc='DaDienRa',NoiDungGiangDay=N'Nội dung thử sai giờ',NhanXet=N'Nhận xét hợp lệ',TrangThaiThamDu='CoMat',TrangThaiThamDuGiaSu='CoMat',TrangThaiThamDuBenHoc='CoMat',NguoiGhiNhanId={c.GS},GhiNhanLuc=SYSDATETIMEOFFSET(),CapNhatLuc=SYSDATETIMEOFFSET(),PhienBan=PhienBan+1 WHERE Id={lesson.Id}");c.Proof["directSqlGuard"]=guard;Check(guard==51118,"SQL trực tiếp hoàn tất sớm phải bị51118.");
            }
            var after=await Snapshot67(c);c.Proof["after"]=after;var newLesson=after.Class.Lessons.Single(x=>x.Id==lesson.Id);var newSchedule=after.Class.Calendar.Single(x=>x.Id==schedule.Id);var hist=after.LessonHistory.Where(x=>x.BuoiHocId==lesson.Id).ToList();var oldHist=before.LessonHistory.Where(x=>x.BuoiHocId==lesson.Id).ToList();
            if(n is 76 or 77 or 79 or 82 or 83)Check(SameJson(before)==SameJson(after),"Ca bị từ chối phải giữ nguyên toàn bộ dữ liệu, không batch/lịch sử thành công giả.");
            if(n==76)Check(trace.Errors.Contains(51144),"Trùng nội bộ phải quan sátSQL51144 thật.");if(n==77)Check(trace.Errors.Contains(51145),"Trùng lịch lớp khác phảiSQL51145 thật.");
            if(n==75)
            {
                var created=after.Class.Calendar.Where(x=>!before.Class.Calendar.Any(y=>y.Id==x.Id)).ToList();var ids=created.Select(x=>x.Id).ToArray();var newLessons=after.Class.Lessons.Where(x=>ids.Contains(x.LichHocId)).ToList();
                Check(created.Count==2&&created.All(x=>x.LopHocId==cl&&x.TrangThai=="DaLenLich")&&newLessons.Count==2&&newLessons.Select(x=>x.LichHocId).Distinct().Count()==2&&newLessons.All(x=>x.TrangThai=="DuKien"),"Batch hợp lệ phải hai lịch/hai buổi đúng lớp, không thừa.");
            }
            if(n==78)
            {
                var history=after.ScheduleHistory.Where(x=>x.LichHocId==schedule.Id).ToList();Check(newSchedule.BatDauLuc==Rules.Local(future)&&newSchedule.KetThucLuc==Rules.Local(future.AddHours(1))&&newLesson.LichHocId==schedule.Id,"Đổi lịch phải đúng giờ và giữ liên kết buổi.");
                Check(history.Count==1&&history[0].BatDauCu==schedule.BatDauLuc&&history[0].KetThucCu==schedule.KetThucLuc&&history[0].BatDauMoi==newSchedule.BatDauLuc&&history[0].KetThucMoi==newSchedule.KetThucLuc&&history[0].LyDo=="Đổi theo thỏa thuận hai bên"&&history[0].NguoiSuaId==c.GS,"Thiếu/sai lịch sử điều chỉnh cũ/mới/lý do/người.");
            }
            if(n==80)Check(newLesson.TrangThai=="DangDienRa"&&newLesson.PhienBan==lesson.PhienBan+1&&hist.Count==oldHist.Count,"Bắt đầu phải tăng đúng phiên bản; trigger không lưu bản chưa ghi nhận.");
            if(n is 81 or 86)
            {
                Check(newLesson.TrangThai=="DaHoc"&&newLesson.KetQuaBuoiHoc=="DaDienRa"&&newLesson.NguoiGhiNhanId==c.GS&&newLesson.GhiNhanLuc!=null&&newLesson.GhiNhanLuc>=newSchedule.KetThucLuc&&newLesson.PhienBan==lesson.PhienBan+1,"Ghi nhận phải đúng trạng thái/người/giờ và tăng1phiên bản.");
                var added=hist.Where(x=>!oldHist.Any(y=>y.Id==x.Id)).ToList();Check(added.Count==1&&added[0].PhienBan==lesson.PhienBan&&added[0].NoiDungGiangDay==lesson.NoiDungGiangDay,"Phải đúng một bản lịch sử cũ.");Check(hist.Select(x=>x.PhienBan).Distinct().Count()==hist.Count,"Không trùng lịch sử theo phiên bản.");Check(newLesson.NoiDungGiangDay==(n==81?"Nội dung A":winner),"Không được mất cập nhật thắng trước.");
            }
            if(n==84)Check(newLesson.TrangThai=="DaNghi"&&newLesson.CanHocBu&&newLesson.LyDoNghi=="Nghỉ vì việc gia đình"&&newLesson.NguoiThongBaoNghiId==c.GS&&newLesson.ThongBaoNghiLuc!=null&&newSchedule.TrangThai=="DaHuy","Thông báo nghỉ thiếu trạng thái/flag/người/giờ/lý do.");
            if(n==85)
            {
                var added=after.Class.Calendar.Where(x=>!before.Class.Calendar.Any(y=>y.Id==x.Id)).ToList();Check(added.Count==1&&added[0].LaBuoiBu&&added[0].LichGocId==schedule.Id&&added[0].BatDauLuc==Rules.Local(future),"Buổi bù phải đúng gốc/giờ và một lịch mới.");Check(newLesson.TrangThai=="DaNghi"&&newLesson.CanHocBu&&newSchedule.TrangThai=="DaHuy","Phải giữ buổi/lịch gốc đã nghỉ.");Check(added.Count==1&&after.Class.Lessons.Count(x=>x.LichHocId==added[0].Id)==1,"Lịch bù phải một BuoiHoc mới.");
            }
        }
        catch(Exception ex){error=ex;}
        finally{await Finish67(c,error,trace,n);}
        Assert.True(error==null,"Không hoàn tất ca: "+error);Assert.True(failures.Count==0,string.Join("\n",failures));
    }
}
