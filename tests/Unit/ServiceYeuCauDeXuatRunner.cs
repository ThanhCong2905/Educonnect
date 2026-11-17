using System.Text.Json;
using EduConnect.Mvc.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace EduConnect.ServiceYeuCauDeXuat.UnitTests;

public sealed partial class ServiceYeuCauDeXuatTest(ITestOutputHelper output)
{
    private async Task Run(int id)
    {
        if(id is 195 or 198 or 201)
        {
            foreach(var number in Enumerable.Range(51130,25))await RunOne(id,number);
        }
        else await RunOne(id);
    }
    private async Task RunOne(int id,int? sql=null)
    {
        using var f=new ServiceYeuCauDeXuatFixture();
        int mode=id switch {171 or 176 or >=183 and <=192 or >=196 and <=198=>1,172 or >=199 and <=206=>2,173 or >=207 and <=212 or 219 or 220=>3,174 or >=213 and <=218=>4,_=>0};
        if(mode is 1 or 4)f.Tutor();
        var actor=id<=174?ServiceYeuCauDeXuatFixture.Other:f.User.Id;
        int? expected=null;Guid? data=null;Exception? thrown=null;bool? success=null;string? message=null;object? accepted=null;
        switch(id)
        {
            case >=170 and <=174:expected=403;break;
            case 175:f.Contact=null;expected=404;break;
            case 176:f.Contact!.GiaSuId=ServiceYeuCauDeXuatFixture.Other;expected=403;break;
            case 177:f.Contact!.BenHocId=ServiceYeuCauDeXuatFixture.Other;expected=403;break;
            case 178:f.Contact!.TrangThai="ChoPhanHoi";expected=403;break;
            case 179:f.Accounts[1].TrangThai="BiKhoa";expected=403;break;
            case 181:f.Input.MaDoiTuong=ServiceYeuCauDeXuatFixture.G;f.Input.LienHeId=Guid.Empty;break;
            case 183:f.Proposal.HocPhi=-1;expected=400;break;
            case 184:f.Proposal.LichRanh[0].GioKetThuc=f.Proposal.LichRanh[0].GioBatDau;expected=400;break;
            case 185:f.Proposal.SoBuoiTuan=2;expected=400;break;
            case 186:f.Proposal.LichRanh[0].GioKetThuc=new TimeOnly(18,45);expected=400;break;
            case 187:f.Proposal.BatDauHocLuc=FixedClock.Now.ToOffset(TimeSpan.FromHours(7)).DateTime.AddMinutes(-1);expected=400;break;
            case 188:f.Proposal.HinhThucHoc="TrucTiep";f.Proposal.DiaDiem=" ";expected=400;break;
            case 189:f.Proposal.NhuCauId=f.Need.Id;f.Need.ChuSoHuuId=ServiceYeuCauDeXuatFixture.Other;expected=400;break;
            case 190:f.Proposal.NhuCauId=f.Need.Id;f.Need.TrangThai="DaDong";expected=400;break;
            case 192:f.Proposal.Id=ServiceYeuCauDeXuatFixture.YC;f.Proposal.RowVersion=Convert.ToBase64String(f.RV);break;
            case 193 or 196 or 199:f.CommandError=new DbUpdateConcurrencyException("Xung đột giả lập");expected=409;break;
            case 194 or 197 or 200:f.CommandError=new InvalidOperationException("Hạ tầng lỗi giả lập");break;
            case 195 or 198 or 201:
                f.CommandError=ServiceYeuCauDeXuatFixture.SqlError(sql!.Value);
                expected=sql is 51133 or 51140 or 51151?403:sql is 51132 or 51138 or 51139 or 51141 or 51144 or 51145 or 51152?409:400;break;
            case 202 or 207 or 213:f.RV=new byte[7];expected=400;break;
            case 203:f.LockError=ServiceYeuCauDeXuatFixture.SqlError(51152);expected=409;break;
            case 204:f.Tutor();actor=f.User.Id;expected=403;break;
            case 205:f.Request.TrangThai="DaChapNhan";break;
            case 208 or 214:f.Request.TrangThai="TuChoi";expected=409;break;
            case 209 or 215:f.Request.HetHanLuc=FixedClock.Now.AddMinutes(-1);expected=409;break;
            case 210:f.Tutor();actor=f.User.Id;expected=403;break;
            case 216:f.User=new(){Id=ServiceYeuCauDeXuatFixture.H,HoTen="Bên nhận",LoaiNguoi="HocSinh"};actor=f.User.Id;expected=403;break;
            case 211 or 217:f.RV=[0,0,0,0,0,0,0,1];f.SaveError=new DbUpdateConcurrencyException("RV0 khác RV1");expected=409;break;
            case 220:expected=400;break;
        }
        var stateBefore=f.Request.TrangThai;
        int status=0;
        try
        {
            if(id is >=175 and <=180)
            {
                var r=await f.Accepted(id==176);success=r.ThanhCong;status=r.MaTrangThai;message=r.ThongDiep;if(r.ThanhCong)accepted=r.DuLieu;
            }
            else if(mode==3)
            {
                var reason=new string('a',id==219?2000:id==220?2001:12);
                var r=await f.Service.TuChoiDeXuatAsync(ServiceYeuCauDeXuatFixture.YC,reason,f.RV,actor,f.Token);success=r.ThanhCong;status=r.ThanhCong?200:int.Parse(r.MaLoi!);message=r.ThongDiep;
            }
            else if(mode==4)
            {
                var r=await f.Service.RutYeuCauAsync(ServiceYeuCauDeXuatFixture.YC,f.RV,actor,f.Token);success=r.ThanhCong;status=r.ThanhCong?200:int.Parse(r.MaLoi!);message=r.ThongDiep;
            }
            else
            {
                var r=mode==1?await f.Service.GuiDeXuatAsync(f.Proposal,actor,f.Token):mode==2?await f.Service.ChapNhanDeXuatAsync(ServiceYeuCauDeXuatFixture.YC,f.RV,actor,f.Token):await f.Service.GuiYeuCauAsync(f.Input,actor,f.Token);
                success=r.ThanhCong;status=r.MaTrangThai;message=r.ThongDiep;if(r.ThanhCong)data=r.DuLieu;
            }
        }
        catch(Exception ex){thrown=ex;}
        output.WriteLine("OBSERVATION_JSON:"+JsonSerializer.Serialize(new{id="UT-"+id,sql,expectedStatus=expected,actualStatus=status,success,message,data,acceptedId=(accepted as EduConnect.Mvc.Models.YeuCauLienHe)?.Id,exception=thrown?.GetType().FullName,exceptionMessage=thrown?.Message,stateBefore,actual=f.Snapshot()}));
        if(id is 194 or 197 or 200){Assert.Same(f.CommandError,thrown);Assert.Equal(0,f.Commits);Assert.Empty(f.Notices);return;}
        Assert.Null(thrown);
        if(expected!=null)
        {
            Assert.False(success,"Phải trả KetQua lỗi; thực tế thành công.");Assert.Equal(expected,status);Assert.Equal(0,f.Commits);
            if(id is 193 or 195 or 196 or 198 or 199 or 201){Assert.Single(f.Business);Assert.Equal(0,f.Saves);Assert.Empty(f.Notices);}
            else if(id is 211 or 217){Assert.Equal(Convert.ToBase64String(f.RV),f.Version);Assert.Equal(1,f.Saves);Assert.Empty(f.Business);}
            else {f.NoWrites();Assert.Equal(stateBefore,f.Request.TrangThai);}
            return;
        }
        Assert.True(success,message);
        if(id==180){Assert.Same(f.Contact,accepted);f.NoWrites();return;}
        if(id==205){Assert.Equal(ServiceYeuCauDeXuatFixture.C,data);f.NoWrites();return;}
        if(mode is 3 or 4)
        {
            Assert.Equal(mode==3?"TuChoi":"DaRut",f.Request.TrangThai);Assert.Equal(Convert.ToBase64String(f.RV),f.Version);f.Saved(f.User.Id==ServiceYeuCauDeXuatFixture.G?ServiceYeuCauDeXuatFixture.H:ServiceYeuCauDeXuatFixture.G,ServiceYeuCauDeXuatFixture.YC);Assert.Empty(f.Business);return;
        }
        Assert.Single(f.Business);
        if(mode==2)
        {
            Assert.Equal(ServiceYeuCauDeXuatFixture.C,data);Assert.Equal(2,f.Commands.Count);Assert.Contains("usp_ChapNhanDeXuatTaoLop",f.Business.Single().Format);
            Assert.Equal(ServiceYeuCauDeXuatFixture.YC,f.Business.Single().GetArgument(0));Assert.Equal(f.RV,Assert.IsType<byte[]>(f.Business.Single().GetArgument(2)));f.Saved(ServiceYeuCauDeXuatFixture.G,ServiceYeuCauDeXuatFixture.C);return;
        }
        Assert.Equal(ServiceYeuCauDeXuatFixture.YC,data);
        var cmd=f.Business.Single();Assert.Equal(ServiceYeuCauDeXuatFixture.LH,cmd.GetArgument(0));Assert.Equal(f.User.Id,cmd.GetArgument(1));
        Assert.Contains("LockContact:"+ServiceYeuCauDeXuatFixture.LH,f.Order);
        if(mode==1)
        {
            Assert.Contains("usp_GuiDeXuatLop",cmd.Format);Assert.Equal(JsonSerializer.Serialize(f.Proposal.LichRanh),cmd.GetArgument(11));Assert.Equal(f.Proposal.Id,cmd.GetArgument(14));
            var parameter=Assert.IsType<SqlParameter>(cmd.GetArgument(15));if(id==192)Assert.Equal(f.RV,Assert.IsType<byte[]>(parameter.Value));else Assert.Equal(DBNull.Value,parameter.Value);
            f.Saved(ServiceYeuCauDeXuatFixture.H,ServiceYeuCauDeXuatFixture.YC);
        }
        else
        {
            Assert.Contains("usp_GuiYeuCauKetNoi",cmd.Format);Assert.Equal(f.Input.LoiNhan,cmd.GetArgument(2));Assert.Equal(f.Input.NhuCauId,cmd.GetArgument(3));
            if(id==181){Assert.Equal(ServiceYeuCauDeXuatFixture.LH,f.Input.LienHeId);f.Store.Verify(x=>x.FindAcceptedContactIdAsync(ServiceYeuCauDeXuatFixture.G,ServiceYeuCauDeXuatFixture.H,f.Token),Moq.Times.Once());}
            f.Saved(ServiceYeuCauDeXuatFixture.G,ServiceYeuCauDeXuatFixture.YC);
        }
    }
}
