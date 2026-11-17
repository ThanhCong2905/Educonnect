using System.Data;
using System.Globalization;
using System.Net;
using System.Text.Json;
using EduConnect.Mvc.Areas.HocTap.ViewModels;
using EduConnect.Mvc.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xunit;
public sealed partial class Integration0203Tests
{
    static Dictionary<string,string> SubmitForm(Guid assignment,Guid op,string text="Nội dung A")=>new(){["BaiTapGiaoId"]=assignment.ToString(),["MaThaoTac"]=op.ToString(),["NoiDung"]=text};
    static Dictionary<string,string> GradeForm(Guid assignment,Guid submission,string grade="8.25",string? rv=null)
    {var f=new Dictionary<string,string>{["BaiTapGiaoId"]=assignment.ToString(),["BaiTapNopId"]=submission.ToString(),["Diem"]=grade,["NhanXet"]="Nhận xét chấm đủ nội dung"};if(rv!=null)f["RowVersion"]=rv;return f;}
    private async Task Run08(int n)
    {
        It0203Case? c=null;It89Box? box=null;Exception? error=null;var trace=new It04SqlTrace();
        try
        {
            c=await It0203Case.Create($"IT-{n:000}");box=new(c,trace);await Identify67(c);bool extra=n is >=95 and <=97;Guid? link=null,member=null;
            if(extra)link=await SeedLink(c);var cl=await SeedClass(c,extra?c.PH:null);
            if(extra){member=await SeedInvite(c,cl,true);await using var s=c.Scope(c.GS);var m=await MemberOf(c,cl,c.HS);await ClassesService(s).DuyetAsync(m.Id,"DaDuyet",n==95?"Xem":"ChinhSua",Convert.ToBase64String(m.RowVersion),n==97);}
            if(n==97){var l=await LinkOf(c);await using var s=c.Scope(c.PH);await LinksService(s).HuyAsync(l.Id,Convert.ToBase64String(l.RowVersion));}
            Guid assignment=Guid.Empty,submission=Guid.Empty;var op=Guid.NewGuid();Guid secondAssignment=Guid.Empty,secondSubmission=Guid.Empty;string? stale=null;
            if(n is 87 or 88)
            {var first=await FirstLesson(c,cl);await SetLessonTime(c,first.Lich.Id,false);await using var s=c.Scope(c.GS);await CalendarService(s).BatDauBuoiAsync(first.Lich.Id,Convert.ToBase64String(first.Buoi.RowVersion));}
            else assignment=await SeedAssignment(c,cl,n==89);
            if(n is 93 or 94 or >=98 and <=102)submission=await SeedSubmission(c,assignment,op);
            if(n==91){await using var db=c.Db();await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.BaiTapGiao SET HanNopLuc=DATEADD(minute,-1,SYSDATETIMEOFFSET()) WHERE Id={assignment}");}
            if(n is 94 or 102)
            {
                await using(var s=c.Scope(c.GS))await TaskService(s).ChamAsync(new(){BaiTapGiaoId=assignment,BaiTapNopId=submission,Diem=8.25m,NhanXet="Kết quả nền"});
                if(n==102){var g=await ReadGrade(c,submission);stale=Convert.ToBase64String(g.RowVersion);await using var s=c.Scope(c.GS);await TaskService(s).ChamAsync(new(){BaiTapGiaoId=assignment,BaiTapNopId=submission,Diem=9m,NhanXet="Bản điểm mới",RowVersion=stale});c.Proof["staleVersion"]=stale;c.Proof["currentGrade"]=await ReadGrade(c,submission);}
            }
            if(n==101)await SeedSubmission(c,assignment,text:"Bài nộp lần2");
            if(n==99)
            {
                var first=await FirstLesson(c,cl);await using var s=c.Scope(c.GS);await TaskService(s).GiaoAsync(new(){LopHocId=cl,BuoiHocId=first.Buoi.Id,TieuDe="Bài tập dataset2",NoiDung="Bài độc lập",HanNopLuc=DateTime.Today.AddDays(10)});secondAssignment=await s.Db.Set<BaiTapGiao>().Where(x=>x.LopHocId==cl&&x.Id!=assignment).Select(x=>x.Id).SingleAsync();secondSubmission=await SeedSubmission(c,secondAssignment);
            }
            if(n==103){await using var db=c.Db();await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.LopHoc SET TrangThai='DaKetThuc',KetThucLuc=SYSDATETIMEOFFSET(),CachKetThuc='HaiBenDongY' WHERE Id={cl}");}
            var before=await Snapshot89(c,box);c.Proof["before"]=before;c.Proof["assignmentId"]=assignment;c.Proof["submissionId"]=submission;c.Proof["operationId"]=op;c.Proof["classId"]=cl;c.Proof["linkId"]=link;c.Proof["memberId"]=member;
            if(n==100)
            {
                c.Proof["mode"]="SQL direct with decimal(18,6), no MVC rounding";await using var db=c.Db();await db.Database.OpenConnectionAsync();await using var cmd=db.Database.GetDbConnection().CreateCommand();cmd.CommandText="EXEC dbo.usp_ChamBaiTap @BaiTapNopId,@NguoiChamId,@Diem,@NhanXet";cmd.Parameters.Add(new SqlParameter("@BaiTapNopId",submission));cmd.Parameters.Add(new SqlParameter("@NguoiChamId",c.GS));cmd.Parameters.Add(new SqlParameter("@Diem",SqlDbType.Decimal){Precision=18,Scale=6,Value=8.251m});cmd.Parameters.Add(new SqlParameter("@NhanXet","Nhận xét hợp lệ"));int code=0;try{await cmd.ExecuteNonQueryAsync();}catch(SqlException ex){code=ex.Number;}c.Proof["directSqlNumber"]=code;Check(code==51036,"SQL phải từ chối8.251 bằng51036, không làm tròn ngầm.");
            }
            else
            {
                Guid? actor=n is 87 or 88 or 89 or >=98 and <=102?c.GS:n==104?null:c.HS;c.Proof["actorId"]=actor;using var client=await c.Client(actor);string path;Dictionary<string,string> form;bool multipart=n is 87 or 90 or 92;
                if(n is 87 or 88){var first=await FirstLesson(c,cl);path="/HocTap/BaiTap/Giao";form=new(){["LopHocId"]=cl.ToString(),["BuoiHocId"]=first.Buoi.Id.ToString(),["TieuDe"]="Bài tập mới",["NoiDung"]="Nội dung giao bài",["HanNopLuc"]=DateTime.Today.AddDays(9).AddHours(21).ToString("yyyy-MM-ddTHH:mm"),["LuuNhap"]=(n==88).ToString()};}
                else if(n==89){path="/HocTap/BaiTap/CongBo";form=new(){["id"]=assignment.ToString(),["rowVersion"]=Convert.ToBase64String((await ReadAssignment(c,assignment)).RowVersion)};}
                else if(n>=98&&n<=102){path="/HocTap/BaiTap/Cham";form=GradeForm(assignment,submission,n==99?"10,01":n is 101 or 102?"8":"8.25",stale);}
                else{path="/HocTap/BaiTap/Nop";form=SubmitForm(assignment,n==94?Guid.NewGuid():op,n==93?"Nội dung B":"Nội dung A");}
                c.Proof["requestPath"]=path;c.Proof["input"]=new Dictionary<string,string>(form);
                HttpResponseMessage response;
                if(multipart)response=await Multipart(client,path,form,(n==87?"assignment.pdf":"answer.pdf",PdfBytes()));
                else if(n==105)response=await Send04(client,path,form);
                else response=await PostWithToken(client,path,form,n==104?"/TaiKhoan/DangNhap":"/HocTap/LopHoc");
                using(response)
                {
                    await c.RecordHttp(response);var primary=c.Proof["http"]!;var status=(int)response.StatusCode;
                    if(n is >=87 and <=92 or 98)await Ok(response);
                    else if(n is 93 or 94 or 101 or 102 or 103)Check(status==409,$"Ca xung đột/trạng thái phải409, thực tế{status}.");
                    else if(n is >=95 and <=97)Check(status==403,$"Không quyền nộp phải403, thực tế{status}.");
                    else if(n==99)Check(status>=400&&status<500,"Điểm ngoài miền phải lỗi4xx.");
                    else if(n==104)Check(status==401&&response.Headers.Location==null,"Anonymous AJAX phải401, không redirect.");else if(n==105)Check(status==400,"Thiếu token phải400.");
                    if(n==88)
                    {
                        await using(var db=c.Db())assignment=await db.Set<BaiTapGiao>().Where(x=>x.LopHocId==cl).Select(x=>x.Id).SingleAsync();using var h=await c.Client(c.HS);using var get=await h.GetAsync($"/HocTap/BaiTap/ChiTiet/{assignment}");await c.RecordHttp(get);c.Proof["draftRead"]=c.Proof["http"];Check((int)get.StatusCode>=400,"H không được đọc bài nháp.");using var post=await PostWithToken(h,"/HocTap/BaiTap/Nop",SubmitForm(assignment,Guid.NewGuid()),"/HocTap/LopHoc");await c.RecordHttp(post);c.Proof["draftSubmit"]=c.Proof["http"];Check((int)post.StatusCode>=400,"H không được nộp bài nháp.");
                    }
                    if(n==92)
                    {
                        var once=await Snapshot89(c,box);c.Proof["afterFirst"]=once;using var second=await Multipart(client,path,SubmitForm(assignment,op),("answer.pdf",PdfBytes()));await c.RecordHttp(second);c.Proof["repeatResponse"]=c.Proof["http"];await Ok(second);Check(SameJson(once)==SameJson(await Snapshot89(c,box)),"Retry không tăng bài nộp,LanNop,tệp hoặc notice.");
                    }
                    if(n==99)
                    {
                        var secondForm=GradeForm(secondAssignment,secondSubmission,"-0,01");using var second=await PostWithToken(client,path,secondForm,"/HocTap/LopHoc");await c.RecordHttp(second);c.Proof["secondDataset"]=new{assignmentId=secondAssignment,submissionId=secondSubmission,input=secondForm.Where(x=>x.Key!="__RequestVerificationToken"),http=c.Proof["http"]};Check((int)second.StatusCode>=400,"Dataset điểm âm phải lỗi.");
                    }
                    c.Proof["http"]=primary;
                }
            }
            var after=await Snapshot89(c,box);c.Proof["after"]=after;
            if(n is >=93 and <=97 or >=99 and <=105)Check(SameJson(before)==SameJson(after),"Ca bị từ chối phải giữ nguyên nghiệp vụ/tệp, không tăng lần nộp hoặc thông báo.");
            if(n==87)
            {var a=after.Base.Assignments.Single();var file=after.Files.Single();Check(a.TrangThai=="DaGiao"&&a.LopHocId==cl&&file.BaiTapGiaoId==a.Id&&file.NguoiTaiLenId==c.GS&&file.TrangThaiQuet=="Sach","Giao bài/tệp sai chủ,trạng thái hoặc liên kết.");var notices=after.Base.Class.Business.Notices.ExceptBy(before.Base.Class.Business.Notices.Select(x=>x.Id),x=>x.Id).ToList();Check(notices.Count==1&&notices[0].NguoiNhanId==c.HS&&notices[0].DoiTuongId==a.Id,"Thông báo giao bài phải đúng H/Id bài.");}
            if(n==88)Check(after.Base.Assignments.Single().TrangThai=="BanNhap"&&after.Base.Submissions.Count==0&&after.Base.Class.Business.Notices.Count==before.Base.Class.Business.Notices.Count,"Nháp chưa công bố/không notice mới.");
            if(n==89)Check(after.Base.Assignments.Count==1&&after.Base.Assignments.Single().TrangThai=="DaGiao"&&after.Base.Class.Business.Notices.Count-before.Base.Class.Business.Notices.Count==2,"Công bố phải giữ A1, thông báo mỗi người đủ quyền đúng một lần.");
            if(n is 90 or 91 or 92)
            {
                var a=after.Base.Assignments.Single();var sub=after.Base.Submissions.Single();Check(sub.BaiTapGiaoId==assignment&&sub.NguoiNopId==c.HS&&sub.MaThaoTac==op&&sub.LanNop==1,"Bài nộp sai ID/người/mã/lần.");
                if(n==91)Check(sub.NopLuc>a.HanNopLuc&&a.HanNopLuc==before.Base.Assignments.Single().HanNopLuc&&after.Base.Class.Business.Notices.Any(x=>x.NguoiNhanId==c.GS&&x.NoiDungTomTat=="Có bài nộp muộn."),"Nộp muộn phải giữ hạn và có notice chính xác.");
                else Check(after.Files.Count==1&&after.Files[0].BaiTapNopId==sub.Id&&after.Files[0].NguoiTaiLenId==c.HS,"Tệp bài nộp phải đúng chủ/Id.");
            }
            if(n==98){var grade=after.Grades.SingleOrDefault();Check(grade!=null&&grade.BaiTapNopId==submission&&grade.Diem==8.25m&&grade.NguoiChamId==c.GS,"Chấm lần đầu sai điểm/người/bài.");Check(after.Base.Class.Business.Notices.Count>before.Base.Class.Business.Notices.Count,"Thiếu notice kết quả.");}
            if(n is 93 or 94 or 101 or 102){var expected=n switch{93=>51033,94=>51034,101=>51037,_=>51039};Check(trace.Errors.Contains(expected),$"Phải thấy SQL{expected} thật.");}
        }
        catch(Exception ex){error=ex;}
        finally{await Finish89(c,box,error,trace,n);}
        Assert.True(error==null,"Không hoàn tất ca: "+error);Assert.True(failures.Count==0,string.Join("\n",failures));
    }
}
