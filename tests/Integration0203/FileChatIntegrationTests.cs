using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using Xunit;
public sealed partial class Integration0203Tests
{
 static Dictionary<string,string> ChatForm(Guid room,Guid op,string text="Nội dung A")=>new(){["MaHoiThoai"]=room.ToString(),["MaThaoTac"]=op.ToString(),["NoiDung"]=text};
 static async Task<JsonElement> WsRead(WebSocket socket,CancellationToken token)
 {var bytes=new byte[65536];using var ms=new MemoryStream();WebSocketReceiveResult r;do{r=await socket.ReceiveAsync(new ArraySegment<byte>(bytes),token);if(r.MessageType==WebSocketMessageType.Close)throw new InvalidOperationException("Socket đóng");ms.Write(bytes,0,r.Count);}while(!r.EndOfMessage);var s=Encoding.UTF8.GetString(ms.ToArray()).Split('\u001e',StringSplitOptions.RemoveEmptyEntries);return JsonDocument.Parse(s[0]).RootElement.Clone();}
 static Task WsSend(WebSocket socket,object payload,CancellationToken token)=>socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload)+"\u001e")),WebSocketMessageType.Text,true,token);
 async Task Realtime106(It0203Case c,It89Box box,Guid room,Guid op)
 {
  using var timeout=new CancellationTokenSource(TimeSpan.FromSeconds(20));var ct=timeout.Token;
  using var g=await c.Client(c.GS);using var h=await c.Client(c.HS);
  using var negotiate=await g.PostAsync("/cap-nhat/negotiate?negotiateVersion=1",new StringContent(""));negotiate.EnsureSuccessStatusCode();var neg=JsonDocument.Parse(await negotiate.Content.ReadAsStringAsync()).RootElement;
  var wc=c.Factory!.Server.CreateWebSocketClient();wc.ConfigureRequest=req=>req.Headers["Cookie"]=c.AuthenticationCookies[c.GS];using var socket=await wc.ConnectAsync(new Uri("wss://localhost/cap-nhat?id="+Uri.EscapeDataString(neg.GetProperty("connectionToken").GetString()!)),ct);
  await WsSend(socket,new{protocol="json",version=1},ct);var handshake=await WsRead(socket,ct);Check(!handshake.TryGetProperty("error",out _),"SignalR handshake lỗi.");
  await WsSend(socket,new{type=1,invocationId="join",target="ThamGiaHoiThoai",arguments=new[]{room}},ct);JsonElement joined;do{joined=await WsRead(socket,ct);}while(!joined.TryGetProperty("invocationId",out _));Check(!joined.TryGetProperty("error",out _),"Không tham gia được group hội thoại.");
  var sp=c.Factory.Services;var worker=new XuLySuKienWorker(sp.GetRequiredService<IServiceScopeFactory>(),sp.GetRequiredService<IHubContext<CapNhatThoiGianThucHub>>(),sp.GetRequiredService<PhienCapNhat>(),sp.GetRequiredService<ILogger<XuLySuKienWorker>>());await worker.XuLySuKienDenHanAsync(100,ct);
  async Task<JsonElement> ReceiveChat(){while(true){var ev=await WsRead(socket,ct);if(ev.TryGetProperty("target",out var t)&&t.GetString()=="CapNhat"){var a=ev.GetProperty("arguments")[0];if(a.TryGetProperty("loai",out var l)&&l.GetString()=="TinNhan")return a.Clone();}}}
  var receive=ReceiveChat();box.Commit.Armed=1;var posting=PostWithToken(h,"/HocTap/TinNhan/Gui",ChatForm(room,op),"/HocTap/LopHoc");
  try{await box.Commit.Reached.Task.WaitAsync(TimeSpan.FromSeconds(8));await worker.XuLySuKienDenHanAsync(100,ct);Check(!receive.IsCompleted,"Có sự kiện TinNhan trước commit.");c.Proof["earlyEventBeforeCommit"]=receive.IsCompleted;box.Commit.Release.TrySetResult();using var response=await posting;await c.RecordHttp(response);await Ok(response);for(int i=0;i<40&&!receive.IsCompleted;i++){await worker.XuLySuKienDenHanAsync(100,ct);await Task.WhenAny(receive,Task.Delay(100,ct));}var actual=await receive.WaitAsync(TimeSpan.FromSeconds(3));c.Proof["signalREvent"]=actual;await using var db=c.Db();var message=await db.Set<TinNhan>().AsNoTracking().SingleAsync(x=>x.MaThaoTac==op&&x.NguoiGuiId==c.HS);Check(actual.GetProperty("doiTuongId").GetGuid()==message.Id,"Event trỏ sai Id tin nhắn đã commit.");Check(actual.GetProperty("hoiThoaiId").GetGuid()==room,"Sai group hội thoại.");}
  finally{box.Commit.Release.TrySetResult();try{await posting;}catch{}timeout.Cancel();socket.Abort();try{await receive;}catch{}}
 }
 private async Task Run09(int n)
 {
  It0203Case? c=null;It89Box? box=null;Exception? error=null;var trace=new It04SqlTrace();
  try
  {
   c=await It0203Case.Create($"IT-{n:000}");box=new(c,trace);await Identify67(c);if(n==118)await SeedLink(c);var cl=await SeedClass(c,n==118?c.PH:null);if(n==118)await SeedInvite(c,cl,true);var initial=await Snapshot89(c,box);var room=initial.Base.Class.Business.Rooms.Single(x=>x.LoaiHoiThoai=="TheoLop").Id;var op=Guid.NewGuid();c.Proof["classId"]=cl;c.Proof["roomId"]=room;c.Proof["operationId"]=op;
   using var client=await c.Client(c.HS);
   if(n is 107 or 108 or 109 or 111 or 117)
   {using var seed=n==117?await Multipart(client,"/HocTap/TinNhan/Gui",ChatForm(room,op),("private.pdf",PdfBytes())):await PostWithToken(client,"/HocTap/TinNhan/Gui",ChatForm(room,op),"/HocTap/LopHoc");await Ok(seed);}
   if(n==110)
   {await using var db=c.Db();var now=DateTimeOffset.UtcNow.AddSeconds(1);now=now.AddTicks(-(now.Ticks%TimeSpan.TicksPerSecond));for(int i=1;i<=120;i++)db.Add(new TinNhan{Id=Guid.Parse($"00000000-0000-0000-0000-{i:000000000000}"),CuocHoiThoaiId=room,NguoiGuiId=c.HS,MaThaoTac=Guid.NewGuid(),NoiDung="Tie "+i,GuiLuc=now});await db.SaveChangesAsync();}
   if(n==111){await using var db=c.Db();await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.LopHoc SET TrangThai='DaKetThuc',KetThucLuc=SYSDATETIMEOFFSET(),CachKetThuc='HaiBenDongY' WHERE Id={cl}");}
   var before=await Snapshot89(c,box);c.Proof["before"]=before;
   if(n==106)await Realtime106(c,box,room,op);
   else if(n==110)
   {
    var seen=new List<Guid>();Guid? cursor=null;var pages=new List<object>();for(int i=0;i<4;i++){using var response=await client.GetAsync($"/HocTap/TinNhan/DuLieu?id={room}"+(cursor.HasValue?$"&before={cursor}":""));var raw=await response.Content.ReadAsStringAsync();Check(response.IsSuccessStatusCode,"Tải trang chat lỗi.");var j=JsonDocument.Parse(raw).RootElement;var ids=j.GetProperty("items").EnumerateArray().Select(x=>x.GetProperty("id").GetGuid()).ToList();pages.Add(new{cursor,ids,hasMore=j.GetProperty("hasMore").GetBoolean()});seen.AddRange(ids);if(ids.Count==0||!j.GetProperty("hasMore").GetBoolean())break;cursor=ids[0];}
    c.Proof["beforePages"]=pages;Check(seen.Count==120&&seen.Distinct().Count()==120,"before phải đọc đủ120 tin cùng GuiLuc, không mất/trùng.");var pivot=seen[0];using var afterResponse=await client.GetAsync($"/HocTap/TinNhan/DuLieu?id={room}&after={pivot}");var afterJson=JsonDocument.Parse(await afterResponse.Content.ReadAsStringAsync()).RootElement;var afterIds=afterJson.GetProperty("items").EnumerateArray().Select(x=>x.GetProperty("id").GetGuid()).ToList();c.Proof["afterPage"]=afterJson.Clone();Check(afterIds.Count==119&&afterIds.Distinct().Count()==119&&!afterIds.Contains(pivot),"after phải bù đủ119 tie khác pivot, không lặp trong response.");c.Proof["afterOverlapContract"]="API trả lại các tie cùng timestamp để client hợp nhất theo Id; test kiểm đủ tie và không lặp trong một response, không kiểm DOM frontend.";
   }
   else if(n==118)
   {
    box.Scanner.Pause=true;var pending=Multipart(client,"/HocTap/TinNhan/Gui",ChatForm(room,op),("race.pdf",PdfBytes()));await box.Scanner.Reached.Task.WaitAsync(TimeSpan.FromSeconds(8));bool revoked=false;int sqlNumber=0;
    try{await using var db=c.Db();await db.Database.OpenConnectionAsync();await using var cmd=db.Database.GetDbConnection().CreateCommand();cmd.CommandText="SET LOCK_TIMEOUT 1000; UPDATE dbo.ThanhVienLopHoc SET MucQuyen='Xem' WHERE LopHocId=@class AND TaiKhoanId=@user;";cmd.Parameters.Add(new SqlParameter("@class",cl));cmd.Parameters.Add(new SqlParameter("@user",c.HS));await cmd.ExecuteNonQueryAsync();revoked=true;}catch(SqlException ex){sqlNumber=ex.Number;}finally{box.Scanner.Release.TrySetResult();}
    using var result=await pending;await c.RecordHttp(result);c.Proof["revocationCommittedBeforeScanRelease"]=revoked;c.Proof["revocationSqlNumber"]=sqlNumber;
    if(revoked){Check((int)result.StatusCode==403,"Thu quyền đã commit lúc đang quét phải403.");Check((await Snapshot89(c,box)).Base.Messages.Count==before.Base.Messages.Count,"Có tin được lưu sau thu quyền trước khi quét xong.");}
    else{Check(false,$"Không đạt thứ tự yêu cầu: thu quyền chưa commit trước nhả scanner (SQL{sqlNumber}); không kết luận bỏ qua quyền. Serializable giữ khóa, request gửi hoàn tất trước thu quyền.");await using(var db=c.Db())await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.ThanhVienLopHoc SET MucQuyen='Xem' WHERE LopHocId={cl} AND TaiKhoanId={c.HS}");using var next=await PostWithToken(client,"/HocTap/TinNhan/Gui",ChatForm(room,Guid.NewGuid()),"/HocTap/LopHoc");c.Proof["afterRevocationHttp"]=(int)next.StatusCode;Check((int)next.StatusCode==403,"Sau thu quyền commit, request tiếp phải403.");}
   }
   else
   {
    HttpResponseMessage response;
    if(n==109){using var outsider=await c.Client(c.HS2);response=await outsider.GetAsync($"/HocTap/TinNhan/DuLieu?id={room}");}
    else if(n==117){using var outsider=await c.Client(c.HS2);response=await outsider.GetAsync($"/HocTap/TinNhan/Tai?id={before.Files.Single().Id}");}
    else if(n is >=112 and <=116)
    {box.Scanner.Reject=n==116;var files=n==114?Enumerable.Range(1,6).Select(i=>($"file{i}.pdf",PdfBytes())).ToArray():new[]{("upload.pdf",n==115?Encoding.UTF8.GetBytes("NOT A PDF"):PdfBytes(n==112?10000000:n==113?10000001:512))};response=await Multipart(client,"/HocTap/TinNhan/Gui",ChatForm(room,op),files);}
    else
    {if(n==111){using var history=await client.GetAsync($"/HocTap/TinNhan/DuLieu?id={room}");var raw=await history.Content.ReadAsStringAsync();Check(history.IsSuccessStatusCode&&JsonDocument.Parse(raw).RootElement.GetProperty("items").EnumerateArray().Any(x=>x.GetProperty("text").GetString()=="Nội dung A"),"Lớp kết thúc vẫn phải đọc được lịch sử.");c.Proof["endedClassHistory"]=raw;}response=await PostWithToken(client,"/HocTap/TinNhan/Gui",ChatForm(room,op,n==108?"Nội dung B":"Nội dung A"),"/HocTap/LopHoc");}
    using(response){await c.RecordHttp(response);var status=(int)response.StatusCode;if(n is 107 or 112)await Ok(response);else if(n==108)Check(status==409,"Trùng mã khác nội dung phải409.");else if(n is 109 or 117)Check(status is 403 or 404,$"Ngoài hội thoại phải403/404, thực tế{status}.");else if(n==111)Check(status==403,$"Lớp kết thúc không gửi được: kỳ vọng403 thực tế{status}.");else Check(status>=400&&status<500,$"Upload bị từ chối phải4xx, thực tế{status}.");}
   }
   var after=await Snapshot89(c,box);c.Proof["after"]=after;
   if(n is 107 or 108 or 109 or 110 or 111 or 113 or 114 or 115 or 117)Check(SameJson(before)==SameJson(after),"Không được đổi tin/tệp/metadata/outbox/dữ liệu nghiệp vụ.");
   if(n==106){Check(after.Base.Messages.Count==before.Base.Messages.Count+1,"Phải một tin sau commit.");Check(after.ChatEvents.Count==before.ChatEvents.Count+1,"Phải một outbox TinNhan.");}
   if(n==112){var file=after.Files.Single();Check(file.KichThuoc==10000000&&file.TrangThaiQuet=="Sach"&&file.TinNhanId==after.Base.Messages.Single().Id,"Metadata/link/kích thước/trạng thái tệp sai.");using var download=await client.GetAsync($"/HocTap/TinNhan/Tai?id={file.Id}");var bytes=await download.Content.ReadAsByteArrayAsync();var hash=Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(bytes));c.Proof["download"]=new{status=(int)download.StatusCode,length=bytes.Length,sha256=hash};Check(download.IsSuccessStatusCode&&bytes.Length==10000000&&hash==file.Checksum,"Download đúng bytes/checksum tệp sạch.");}
   if(n==115)Check(box.Scanner.Calls==0,"Sai chữ ký PDF phải bị loại trước scanner.");
   if(n==116){Check(after.Files.All(x=>x.TrangThaiQuet!="Sach")&&after.Physical.Length==0&&after.Base.Messages.Count==before.Base.Messages.Count,"Rủi ro không được lưu sạch/phục vụ.");Check(after.Files.Any(x=>x.TrangThaiQuet is "CachLy" or "TuChoi"),"Kịch bản yêu cầu metadata CachLy/TuChoi, nhưng scanner ném lỗi trước khi tạo metadata; từ chối an toàn song thiếu bản ghi cách ly/từ chối.");}
  }
  catch(Exception ex){error=ex;}
  finally{await Finish89(c,box,error,trace,n);}
  Assert.True(error==null,"Không hoàn tất ca: "+error);Assert.True(failures.Count==0,string.Join("\n",failures));
 }
}
