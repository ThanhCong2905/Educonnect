using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

public partial class IntegrationTests
{
    private static Dictionary<string, string> NeedForm(Mt1Case c) => new()
    {
        ["MonHocId"] = c.Math.ToString(),
        ["MonHocIds[0]"] = c.Math.ToString(),
        ["KhoiLop"] = "Lớp 8",
        ["HocLuc"] = "Trung bình",
        ["MucTieu"] = c.Tag,
        ["SoBuoiTuan"] = "2",
        ["HinhThucHoc"] = "TrucTuyen",
        ["HocPhiDuKien"] = "150000",
        ["DonViHocPhi"] = "VND/Gio",
        ["LichRanh[0].ThuTrongTuan"] = "1",
        ["LichRanh[0].GioBatDau"] = "19:00",
        ["LichRanh[0].GioKetThuc"] = "20:00",
        ["LichRanh[1].ThuTrongTuan"] = "3",
        ["LichRanh[1].GioBatDau"] = "19:00",
        ["LichRanh[1].GioKetThuc"] = "20:00"
    };
    private static async Task<HttpResponseMessage> Post(Mt1Case c, HttpClient client, string path, Dictionary<string, string> form, bool ajax = false, string? tokenPath = null, bool includeToken = true)
    {
        if (includeToken)
            form["__RequestVerificationToken"] = await Mt1Case.Token(client, tokenPath ?? "/NhuCau/NhuCau/Tao");
        using var request = new HttpRequestMessage(HttpMethod.Post, path) { Content = new FormUrlEncodedContent(form) };
        if (ajax)
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
        var response = await client.SendAsync(request);
        await c.RecordHttp(response);
        return response;
    }
    private static int Cards(string html) => Regex.Matches(html, "<article\\s+class=\"card candidate\"").Count;
    private static async Task Http(Mt1Case c, int n)
    {
        var actor = n == 62 ? c.GS : n == 75 ? (Guid?)null : c.HS;
        if (n is 61 or 72 or 73 or 74)
            await c.ProfileSet();
        if (n == 62)
            await c.NeedSet();
        using var client = await c.Client(actor);
        if (n is 61 or 62 or 63 or 72 or 73 or 74 or 86)
        {
            var path = "/TimKiem/TimKiem/Index?TuKhoa=" + c.Tag;
            if (n == 62)
                path += "&MonHocId=" + c.Math;
            if (n == 63)
                path += "&Trang=0";
            if (n == 86)
                path += "&MonHocId=abc&Trang=1";
            if (n == 72)
                path += "&MonHocId=" + c.Math;
            if (n is 73 or 74)
            {
                var ids = n == 73 ? new[] { c.P1, c.Profiles["P2"] } : new[] { c.P1, c.Profiles["P2"], c.Profiles["P3"], c.Profiles["P7"] };
                path = "/TimKiem/TimKiem/SoSanh?" + string.Join("&", ids.Select(x => "ids=" + x));
            }
            using var request = new HttpRequestMessage(HttpMethod.Get, path);
            if (n is 72 or 73 or 74 or 86)
                request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            if (n == 72)
                request.Headers.Add("X-Edu-Partial", "true");
            if (n == 73)
                request.Headers.Add("X-Edu-Modal", "true");
            using var response = await client.SendAsync(request);
            await c.RecordHttp(response);
            var body = await response.Content.ReadAsStringAsync();
            if (n is 63 or 74 or 86)
            {
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                if (n == 63)
                    Assert.Contains("Trang không hợp lệ", WebUtility.HtmlDecode(body));
                else
                {
                    using var json = JsonDocument.Parse(body);
                    Assert.False(json.RootElement.GetProperty("ok").GetBoolean());
                }
            }
            else
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
                if (n is 61 or 62 or 72)
                {
                    Assert.Equal(n == 62 ? 9 : n == 72 ? 3 : 4, Cards(body));
                    Assert.Contains(c.Tag, body);
                }
                if (n == 62)
                    Assert.Contains(">12 ", body);
                if (n is 72 or 73)
                {
                    Assert.DoesNotContain("<html", body, StringComparison.OrdinalIgnoreCase);
                    Assert.DoesNotContain("<head", body, StringComparison.OrdinalIgnoreCase);
                }
                if (n == 73)
                {
                    Assert.Contains(c.P1.ToString(), body);
                    Assert.Contains(c.Profiles["P2"].ToString(), body);
                }
            }
            return;
        }
        if (n == 75)
        {
            using var get = await client.GetAsync("/NhuCau/NhuCau/Tao");
            Assert.Equal(HttpStatusCode.Redirect, get.StatusCode);
            Assert.Contains("/TaiKhoan/DangNhap", get.Headers.Location!.ToString());
            using var response = await Post(c, client, "/NhuCau/NhuCau/Luu", NeedForm(c), tokenPath: "/TaiKhoan/DangNhap");
            Assert.Contains(response.StatusCode, new[] { HttpStatusCode.Redirect, HttpStatusCode.Unauthorized });
            Assert.Equal(0, await c.NeedCount());
            return;
        }
        if (n == 85)
        {
            using var response = await Post(c, client, "/KetNoi/KetNoi/GuiLienHe", new()
            {
                ["DoiTuongId"] = c.GS.ToString(),
                ["LoiNhan"] = ""
            }, true);
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            Assert.False(json.RootElement.GetProperty("ok").GetBoolean());
            Assert.True(json.RootElement.GetProperty("errors").TryGetProperty("LoiNhan", out _));
            await using var db = c.Db();
            Assert.False(await db.Set<YeuCauLienHe>().AnyAsync(x => x.BenHocId == c.HS));
            Assert.False(await db.Set<ThongBao>().AnyAsync(x => x.NguoiNhanId == c.GS));
            return;
        }
        var form = NeedForm(c);
        if (n is 70 or 71)
        {
            var id = await c.CreateNeed();
            var old = await c.ReadNeed(id);
            form["Id"] = id.ToString();
            form["RowVersion"] = Convert.ToBase64String(old.RowVersion);
            var tokenPath = "/NhuCau/NhuCau/Sua/" + id;
            if (n == 70)
                form["RowVersion"] = "@@@";
            else
            {
                using var first = await c.Client(c.HS);
                var a = NeedForm(c);
                a["Id"] = id.ToString();
                a["RowVersion"] = form["RowVersion"];
                a["MucTieu"] = "Nội dung A";
                using var saved = await Post(c, first, "/NhuCau/NhuCau/Luu", a, tokenPath: tokenPath);
                Assert.Equal(HttpStatusCode.Redirect, saved.StatusCode);
                form["MucTieu"] = "Nội dung B";
            }
            using var response = await Post(c, client, "/NhuCau/NhuCau/Luu", form, true, tokenPath);
            if (n == 71)
                Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            else
                Assert.True((int)response.StatusCode >= 400, "Sai phiên bản phải bị từ chối");
            var row = await c.ReadNeed(id);
            Assert.Equal(n == 70 ? old.MucTieu : "Nội dung A", row.MucTieu);
            Assert.Equal(2, (await c.ReadSlots(id)).Count);
            return;
        }
        if (n is 67 or 68)
            form["SoBuoiTuan"] = "8";
        if (n == 69)
        {
            foreach (var key in form.Keys.Where(x => x.StartsWith("LichRanh[1]")).ToArray())
                form.Remove(key);
        }
        using (var response = await Post(c, client, "/NhuCau/NhuCau/Luu", form, n is 66 or 67, includeToken: n != 65))
        {
            var body = await response.Content.ReadAsStringAsync();
            switch (n)
            {
                case 64:
                    Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
                    Assert.Contains("/NhuCau", response.Headers.Location!.ToString());
                    break;
                case 65:
                    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                    Assert.Equal(0, await c.NeedCount());
                    break;
                case 66:
                    {
                        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                        using var json = JsonDocument.Parse(body);
                        Assert.True(json.RootElement.GetProperty("ok").GetBoolean());
                        Assert.False(string.IsNullOrWhiteSpace(json.RootElement.GetProperty("redirectUrl").GetString()));
                        Assert.Equal(1, await c.NeedCount());
                        break;
                    }
                case 67:
                    {
                        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
                        using var json = JsonDocument.Parse(body);
                        Assert.False(json.RootElement.GetProperty("ok").GetBoolean());
                        Assert.True(json.RootElement.GetProperty("errors").TryGetProperty("SoBuoiTuan", out _));
                        Assert.Equal(0, await c.NeedCount());
                        break;
                    }
                case 68:
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    Assert.Equal(0, await c.NeedCount());
                    Assert.Contains("field-validation-error", body);
                    Assert.DoesNotContain("<h1>Tạo nhu cầu", WebUtility.HtmlDecode(body));
                    break;
                case 69:
                    Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
                    break;
                default:
                    throw new InvalidOperationException("HTTP case chưa triển khai: " + n);
            }
        }
        if (n is 64 or 66 or 69)
        {
            await using var db = c.Db();
            var row = await db.Set<NhuCauHoc>().AsNoTracking().SingleAsync(x => x.ChuSoHuuId == c.HS);
            Assert.Equal(c.Tag, row.MucTieu);
            Assert.Equal("DangMo", row.TrangThai);
            var slots = await c.ReadSlots(row.Id);
            Assert.Equal(n == 69 ? 1 : 2, slots.Count);
            Assert.Equal(1, slots[0].ThuTrongTuan);
            Assert.Equal(new TimeOnly(19, 0), slots[0].GioBatDau);
            Assert.Equal(new TimeOnly(20, 0), slots[0].GioKetThuc);
        }
    }
}
