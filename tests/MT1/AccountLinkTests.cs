using System.Security.Claims;
using EduConnect.Mvc.Areas.HocTap.Services;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Xunit;

public sealed class AccountLinkTests
{
    private static LienKetTaiKhoanService Service(AppDbContext db, Guid actor)
    {
        var http = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, actor.ToString())], "Test")) };
        var current = new NguoiDungHienTaiService(db, new HttpContextAccessor { HttpContext = http });
        return new(db, current, new ThongBaoService(db, current));
    }

    private static string Email(Guid id) => id.ToString("N") + "@example.test";

    private static async Task<(Guid Parent, Guid Student, Guid Outsider)> Seed(AppDbContext db)
    {
        var parent = Guid.NewGuid(); var student = Guid.NewGuid(); var outsider = Guid.NewGuid();
        foreach (var (id, roleName) in new[] { (parent, "PhuHuynh"), (student, "HocSinh"), (outsider, "HocSinh") })
        {
            var email = Email(id);
            db.Users.Add(new TaiKhoan { Id = id, HoTen = "Kiểm thử liên kết", UserName = email, NormalizedUserName = email.ToUpperInvariant(), Email = email, NormalizedEmail = email.ToUpperInvariant(), SecurityStamp = Guid.NewGuid().ToString() });
            var role = await db.Roles.SingleAsync(x => x.Name == roleName);
            db.UserRoles.Add(new() { UserId = id, RoleId = role.Id });
        }
        await db.SaveChangesAsync();
        db.Add(new PhuHuynh { TaiKhoanId = parent });
        db.Add(new HocSinh { TaiKhoanId = student });
        db.Add(new HocSinh { TaiKhoanId = outsider });
        await db.SaveChangesAsync();
        return (parent, student, outsider);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    public async Task Either_side_sends_and_only_recipient_responds(bool parentSends, bool accept)
    {
        await using var db = ProposalRegressionTests.Open();
        var seed = await Seed(db);
        var sender = parentSends ? seed.Parent : seed.Student;
        var receiver = parentSends ? seed.Student : seed.Parent;
        await Service(db, sender).GuiDeNghiAsync(" " + Email(receiver).ToUpperInvariant() + " ");
        var link = await db.Set<LienKetPhuHuynhHocSinh>().SingleAsync(x => x.PhuHuynhId == seed.Parent && x.HocSinhId == seed.Student);
        Assert.Equal(sender, link.NguoiGuiId);
        var version = Convert.ToBase64String(link.RowVersion);
        var self = await Assert.ThrowsAsync<LoiNghiepVu>(() => Service(db, sender).PhanHoiAsync(link.Id, true, version));
        Assert.Equal(404, Rules.ErrorStatus(self));
        var other = await Assert.ThrowsAsync<LoiNghiepVu>(() => Service(db, seed.Outsider).PhanHoiAsync(link.Id, true, version));
        Assert.Equal(404, Rules.ErrorStatus(other));
        var duplicate = await Assert.ThrowsAsync<LoiNghiepVu>(() => Service(db, receiver).GuiDeNghiAsync(Email(sender)));
        Assert.Equal(409, Rules.ErrorStatus(duplicate));
        await Service(db, receiver).PhanHoiAsync(link.Id, accept, version);
        await db.Entry(link).ReloadAsync();
        Assert.Equal(accept ? "DaLienKet" : "TuChoi", link.TrangThai);
        Assert.Equal(accept, link.XacNhanLuc.HasValue);
        Assert.Equal(!accept, link.KetThucLuc.HasValue);
        Assert.Contains(await db.Set<ThongBao>().Where(x => x.NguoiNhanId == sender).Select(x => x.NoiDungTomTat).ToListAsync(), x => x.Contains(accept ? "đã đồng ý" : "đã từ chối"));
        var repeated = await Assert.ThrowsAsync<LoiNghiepVu>(() => Service(db, receiver).PhanHoiAsync(link.Id, accept, Convert.ToBase64String(link.RowVersion)));
        Assert.Equal(409, Rules.ErrorStatus(repeated));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Sender_can_withdraw_but_recipient_cannot_cancel_pending_request(bool parentSends)
    {
        await using var db = ProposalRegressionTests.Open();
        var seed = await Seed(db);
        var sender = parentSends ? seed.Parent : seed.Student;
        var receiver = parentSends ? seed.Student : seed.Parent;
        await Service(db, sender).GuiDeNghiAsync(Email(receiver));
        var link = await db.Set<LienKetPhuHuynhHocSinh>().SingleAsync(x => x.PhuHuynhId == seed.Parent && x.HocSinhId == seed.Student);
        var version = Convert.ToBase64String(link.RowVersion);
        var denied = await Assert.ThrowsAsync<LoiNghiepVu>(() => Service(db, receiver).HuyAsync(link.Id, version));
        Assert.Equal(409, Rules.ErrorStatus(denied));
        Assert.True(await Service(db, sender).HuyAsync(link.Id, version));
        await db.Entry(link).ReloadAsync();
        Assert.Equal("DaHuy", link.TrangThai);
        Assert.Equal(sender, link.NguoiKetThucId);
    }

    [Fact]
    public async Task Student_cannot_link_to_self_or_another_student()
    {
        await using var db = ProposalRegressionTests.Open();
        var seed = await Seed(db);
        foreach (var target in new[] { seed.Student, seed.Outsider })
        {
            var error = await Assert.ThrowsAsync<LoiNghiepVu>(() => Service(db, seed.Student).GuiDeNghiAsync(Email(target)));
            Assert.Equal(404, Rules.ErrorStatus(error));
        }
    }
}
