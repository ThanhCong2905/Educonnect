using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Areas.TimKiem.ViewModels;
using EduConnect.Mvc.Areas.KetNoi.ViewModels;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
public class RulesTests
{
    public static IEnumerable<object[]> Cases => Enumerable.Range(1,46).Concat([68,69,70]).Select(n=>new object[]{$"MT1-UT-{n:000}"});
    [Theory][MemberData(nameof(Cases))]
    public void UC1_rules(string caseId)
    {
        var watch=Stopwatch.StartNew();string actual="";string status="Pass";
        try { actual=Run(int.Parse(caseId[^3..])); }
        catch(Exception ex){actual=ex.ToString();status="Fail";throw;}
        finally
        {
            var dir=Environment.GetEnvironmentVariable("MT1_EVIDENCE")??Path.Combine(AppContext.BaseDirectory,"evidence");Directory.CreateDirectory(dir);
            File.AppendAllText(Path.Combine(dir,"unit-details.jsonl"),JsonSerializer.Serialize(new{caseId,status,actual,elapsedMs=watch.ElapsedMilliseconds,utc=DateTimeOffset.UtcNow})+Environment.NewLine);
        }
    }
    static string Run(int n)
    {
        if(n<=13)
        {
            List<SlotInput> s=[];void Add(byte day,int start,int end)=>s.Add(new(){ThuTrongTuan=day,GioBatDau=new(start,0),GioKetThuc=new(end,0)});
            switch(n)
            {
                case 2:for(byte d=1;d<=6;d++)for(int h=1;h<=6;h++)Add(d,h,h+1);Assert.Equal(36,s.Count);break;
                case 3:Add(7,19,20);break;case 4:Add(1,19,19);break;case 5:Add(1,20,19);break;
                case 6:s=[new(){ThuTrongTuan=1,GioBatDau=new(18,0),GioKetThuc=new(19,30)},new(){ThuTrongTuan=1,GioBatDau=new(19,0),GioKetThuc=new(20,0)}];break;
                case 7:Add(1,18,19);Add(1,19,20);break;case 8:Add(1,18,19);Add(2,18,19);break;case 9:Add(0,0,1);break;
                case 10:for(byte d=0;d<=6;d++)for(int h=1;h<=5;h++)Add(d,h,h+1);Assert.Equal(35,s.Count);break;
                case 11:Add(6,18,19);break;case 12:Add(1,18,19);Add(1,18,19);break;case 13:Add(1,18,21);Add(1,19,20);break;
            }
            if(new[]{7,8,9,10,11}.Contains(n)){Rules.Slots(s);return $"Chấp nhận {s.Count} khung; không ném ngoại lệ.";}
            var ex=Assert.Throws<LoiNghiepVu>(()=>Rules.Slots(s));Assert.Equal(400,ex.Status);
            var message=n<=2?"Cần từ 1 đến 35 khung giờ.":n<=5?"Thứ hoặc giờ của khung giờ không hợp lệ.":"Các khung giờ cùng ngày không được chồng lấn.";
            Assert.Equal(message,ex.Message);return $"Status={ex.Status}; {ex.Message}";
        }
        if(n<=25)
        {
            (decimal? v,string? u) = n switch {14=>((decimal?)null,(string?)null),15=>((decimal?)150000,(string?)null),16=>((decimal?)null,"VND/Gio"),17=>(-1,"VND/Gio"),18=>(150000.123m,"VND/Buoi"),19=>(150000,"USD/Gio"),20=>(150000.50m,"VND/Buoi"),21=>(0,"VND/Gio"),22=>(9999999999999999.99m,"VND/Gio"),23=>(10000000000000000m,"VND/Gio"),24=>((decimal?)null,""),_=>(150000," ")};
            if(new[]{14,20,21,22,24}.Contains(n)){Rules.FeeCheck(v,u);return $"Chấp nhận học phí {v?.ToString()??"null"}, đơn vị {u??"null"}.";}
            var ex=Assert.Throws<LoiNghiepVu>(()=>Rules.FeeCheck(v,u));Assert.Equal(400,ex.Status);Assert.Equal(n is 15 or 16?"Nhập đồng thời học phí và đơn vị.":"Học phí không âm, tối đa hai chữ số thập phân; chọn đúng đơn vị.",ex.Message);return $"Status=400; {ex.Message}";
        }
        if(n<=30)
        {
            string? v=n switch {26=>"AQIDBAUGBwg=",27=>null,28=>"@@@",29=>"AQID",_=>""};
            if(n==26){Assert.Equal(new byte[]{1,2,3,4,5,6,7,8},Rules.Version(v));return "Giải mã đúng 8 byte 01..08.";}
            var ex=Assert.Throws<LoiNghiepVu>(()=>Rules.Version(v));Assert.Equal(400,ex.Status);Assert.Equal("Phiên bản không hợp lệ.",ex.Message);return "Từ chối phiên bản; Status=400.";
        }
        object model;string field;bool valid;
        if(n>=68){model=new GuiLienHeInput{DoiTuongId=Guid.NewGuid(),LoiNhan=new('A',n==68?0:n==69?256:257)};field="LoiNhan";valid=n==69;}
        else if(n>=45){model=new BoLocTimKiemViewModel{TuKhoa=new('A',n==45?100:101)};field="TuKhoa";valid=n==45;}
        else if(n==44){model=new HoSoInput{HoTen="",TrinhDo="Đại học",ChuyenMon="Toán",KinhNghiem="2 năm",MoTa="Giới thiệu",KhuVuc="Hà Nội"};field="HoTen";valid=false;}
        else
        {
            var m=new NhuCauInput{MonHocIds=[1],MucTieu="Ôn đại số lớp 8",SoBuoiTuan=2,HinhThucHoc="TrucTuyen"};model=m;
            if(n<=34){field="SoBuoiTuan";m.SoBuoiTuan=n switch{31=>0,32=>1,33=>7,_=>8};valid=n is 32 or 33;}
            else if(n<=36){field="HinhThucHoc";m.HinhThucHoc=n==35?"Khac":"";valid=false;}
            else if(n<=38){field="MucTieu";m.MucTieu=new('A',n==37?256:257);valid=n==37;}
            else if(n<=42){field="TuoiNguoiHoc";m.TuoiNguoiHoc=n switch{39=>0,40=>1,41=>120,_=>121};valid=n is 40 or 41;}
            else{field="TenMonKhac";m.TenMonKhac=new('A',101);valid=false;}
        }
        var errors=new List<ValidationResult>();bool ok=Validator.TryValidateObject(model,new ValidationContext(model),errors,true);Assert.Equal(valid,ok);
        if(valid)Assert.Empty(errors);else Assert.Contains(errors,e=>e.MemberNames.Contains(field));
        return valid?"Validation hợp lệ; 0 lỗi.":"Validation từ chối đúng trường "+field+"; "+string.Join("; ",errors.Select(e=>e.ErrorMessage));
    }
}
