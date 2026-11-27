using System.Reflection;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

internal static partial class Cases
{
    static ServiceProvider MaintenanceServices(Fixture f) => new ServiceCollection()
        .AddLogging()
        .AddScoped(_ => f.Db())
        .AddScoped<IHttpContextAccessor>(_ => new PlainAccessor())
        .AddScoped<NguoiDungHienTaiService>()
        .AddScoped<ThongBaoService>()
        .BuildServiceProvider();

    static async Task RunMaintenance(Fixture f, string method, params object[] args)
    {
        using var provider = MaintenanceServices(f);
        var worker = new BaoTriWorker(provider.GetRequiredService<IServiceScopeFactory>(), NullLogger<BaoTriWorker>.Instance);
        var task = (Task)(typeof(BaoTriWorker).GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(worker, args)
            ?? throw new MissingMethodException(typeof(BaoTriWorker).FullName, method));
        await task;
    }

    internal static async Task WorkerCases(Fixture f, string key)
    {
        switch (key)
        {
            case "worker_proposal_expire":
            {
                await f.SeedProposal();
                await f.Sql("UPDATE dbo.YeuCauKetNoiLopHoc SET GuiLuc=DATEADD(hour,-73,SYSDATETIMEOFFSET()),DeXuatLuc=DATEADD(hour,-73,SYSDATETIMEOFFSET()),HetHanLuc=DATEADD(hour,-1,SYSDATETIMEOFFSET()) WHERE Id=@id", new() { ["@id"] = f.Request });
                await using var future = new Fixture(f.Id + "-future"); await future.Init(); await future.SeedProposal();
                await using var rejected = new Fixture(f.Id + "-rejected"); await rejected.Init(); await rejected.SeedProposal();
                await rejected.Sql("UPDATE dbo.YeuCauKetNoiLopHoc SET TrangThai='TuChoi',GuiLuc=DATEADD(hour,-74,SYSDATETIMEOFFSET()),DeXuatLuc=DATEADD(hour,-74,SYSDATETIMEOFFSET()),HetHanLuc=DATEADD(hour,-2,SYSDATETIMEOFFSET()) WHERE Id=@id", new() { ["@id"] = rejected.Request });
                await RunMaintenance(f, "XuLyHetHanYeuCauAsync", DateTimeOffset.UtcNow, CancellationToken.None);
                await using (var db = f.Db()) Assert.Equal("HetHan", (await db.Set<YeuCauKetNoiLopHoc>().SingleAsync(x => x.Id == f.Request)).TrangThai);
                await using (var db = future.Db()) Assert.Equal("ChoBenHoc", (await db.Set<YeuCauKetNoiLopHoc>().SingleAsync(x => x.Id == future.Request)).TrangThai);
                await using (var db = rejected.Db()) Assert.Equal("TuChoi", (await db.Set<YeuCauKetNoiLopHoc>().SingleAsync(x => x.Id == rejected.Request)).TrangThai);
                break;
            }
            case "worker_need_expire":
            {
                var n1 = await f.SeedNeed();
                var n2 = await f.SeedNeed();
                var n3 = await f.SeedNeed();
                await f.Sql("UPDATE dbo.NhuCauHoc SET TaoLuc=DATEADD(day,-31,SYSDATETIMEOFFSET()),XacNhanConNhuCauLuc=NULL WHERE Id=@id", new() { ["@id"] = n1 });
                await f.Sql("UPDATE dbo.NhuCauHoc SET TaoLuc=DATEADD(day,-40,SYSDATETIMEOFFSET()),XacNhanConNhuCauLuc=DATEADD(day,-38,SYSDATETIMEOFFSET()),NhacXacNhanLuc=DATEADD(day,-8,SYSDATETIMEOFFSET()) WHERE Id=@id", new() { ["@id"] = n2 });
                await f.Sql("UPDATE dbo.NhuCauHoc SET TaoLuc=DATEADD(day,-40,SYSDATETIMEOFFSET()),XacNhanConNhuCauLuc=SYSDATETIMEOFFSET() WHERE Id=@id", new() { ["@id"] = n3 });
                await RunMaintenance(f, "XuLyHieuLucNhuCauAsync", CancellationToken.None);
                await using var verify = f.Db();
                var a = await verify.Set<NhuCauHoc>().SingleAsync(x => x.Id == n1); var b = await verify.Set<NhuCauHoc>().SingleAsync(x => x.Id == n2); var c = await verify.Set<NhuCauHoc>().SingleAsync(x => x.Id == n3);
                Assert.NotNull(a.NhacXacNhanLuc); Assert.Equal("DaDong", b.TrangThai); Assert.Equal("HetHieuLucTimKiem", b.LyDoDong); Assert.Null(c.NhacXacNhanLuc); Assert.Equal("DangMo", c.TrangThai);
                Assert.True(await verify.Set<ThongBao>().AnyAsync(x => x.DoiTuongId == n1 && x.NoiDungTomTat.Contains("còn tìm gia sư")));
                Assert.True(await verify.Set<ThongBao>().AnyAsync(x => x.DoiTuongId == n2 && x.NoiDungTomTat.Contains("đã đóng")));
                break;
            }
            case "worker_end":
            {
                await f.SeedClass();
                await using var refused = new Fixture(f.Id + "-refused"); await refused.Init(); await refused.SeedClass();
                await using var future = new Fixture(f.Id + "-future"); await future.Init(); await future.SeedClass();
                var old = DateTimeOffset.UtcNow.AddHours(-73); var now = DateTimeOffset.UtcNow;
                var q1 = new YeuCauHuyKetNoi { Id = Guid.NewGuid(), LopHocId = f.Class, NguoiGuiId = f.G, LyDo = "Kết thúc lớp", TrangThai = "ChoPhanHoi", GuiLuc = old, HetHanLuc = old.AddHours(72) };
                var q2 = new YeuCauHuyKetNoi { Id = Guid.NewGuid(), LopHocId = refused.Class, NguoiGuiId = refused.G, LyDo = "Đã từ chối", TrangThai = "TuChoi", GuiLuc = old, HetHanLuc = old.AddHours(72), PhanHoiLuc = old.AddHours(1), NguoiPhanHoiId = refused.H };
                var q3 = new YeuCauHuyKetNoi { Id = Guid.NewGuid(), LopHocId = future.Class, NguoiGuiId = future.G, LyDo = "Còn hạn", TrangThai = "ChoPhanHoi", GuiLuc = now, HetHanLuc = now.AddHours(72) };
                await using (var db = f.Db()) { db.Add(q1); db.Add(q2); db.Add(q3); await db.SaveChangesAsync(); }
                await RunMaintenance(f, "XuLyTuHuyKetNoiAsync", now, CancellationToken.None);
                await using (var db = f.Db())
                {
                    Assert.Equal("TuDongChapNhan", (await db.Set<YeuCauHuyKetNoi>().SingleAsync(x => x.Id == q1.Id)).TrangThai);
                    Assert.Equal("DaKetThuc", (await db.Set<LopHoc>().SingleAsync(x => x.Id == f.Class)).TrangThai);
                    Assert.Equal("HetHan", (await db.Set<LopHoc>().SingleAsync(x => x.Id == f.Class)).CachKetThuc);
                    Assert.Equal("TuChoi", (await db.Set<YeuCauHuyKetNoi>().SingleAsync(x => x.Id == q2.Id)).TrangThai);
                    Assert.Equal("DangHoatDong", (await db.Set<LopHoc>().SingleAsync(x => x.Id == refused.Class)).TrangThai);
                    Assert.Equal("ChoPhanHoi", (await db.Set<YeuCauHuyKetNoi>().SingleAsync(x => x.Id == q3.Id)).TrangThai);
                    Assert.Equal("DangHoatDong", (await db.Set<LopHoc>().SingleAsync(x => x.Id == future.Class)).TrangThai);
                }
                break;
            }
            case "worker_learning_reminder":
            {
                await f.SeedLesson(); await f.SeedAssignment();
                await f.Sql("UPDATE dbo.LichHoc SET KetThucLuc=DATEADD(hour,-30,SYSDATETIMEOFFSET()),BatDauLuc=DATEADD(hour,-31,SYSDATETIMEOFFSET()) WHERE Id=@id", new() { ["@id"] = f.Schedule });
                await f.Sql("UPDATE dbo.BaiTapGiao SET HanNopLuc=DATEADD(day,-1,SYSDATETIMEOFFSET()) WHERE Id=@id", new() { ["@id"] = f.Assignment });
                Guid futureSchedule = Guid.NewGuid();
                await using (var db = f.Db())
                {
                    db.Add(new LichHoc { Id = futureSchedule, LopHocId = f.Class, TieuDe = f.Tag + " future", BatDauLuc = DateTimeOffset.UtcNow.AddDays(2), KetThucLuc = DateTimeOffset.UtcNow.AddDays(2).AddHours(1), TrangThai = "DaLenLich" });
                    await db.SaveChangesAsync();
                }
                await RunMaintenance(f, "XuLyNhacHocTapAsync", CancellationToken.None);
                await using var verify = f.Db();
                Assert.True(await verify.Set<ThongBao>().AnyAsync(x => x.DoiTuongId == f.Schedule && x.LoaiThongBao == "NhacViec" && x.NguoiNhanId == f.G));
                Assert.True(await verify.Set<ThongBao>().AnyAsync(x => x.DoiTuongId == f.Assignment && x.LoaiThongBao == "BaiTap"));
                Assert.False(await verify.Set<ThongBao>().AnyAsync(x => x.DoiTuongId == futureSchedule));
                Assert.False(await verify.Set<BaiTapNop>().AnyAsync(x => x.BaiTapGiaoId == f.Assignment));
                Assert.False(await (from result in verify.Set<KetQuaChamBai>() join submission in verify.Set<BaiTapNop>() on result.BaiTapNopId equals submission.Id where submission.BaiTapGiaoId == f.Assignment select result).AnyAsync());
                break;
            }
            case "worker_end_race":
            {
                // DL01: người nhận commit trước hạn; worker đến sau không được ghi đè kết quả.
                var firstId = await EndRequest(f);
                var firstRv = await f.Version<YeuCauHuyKetNoi>(firstId);
                await using (var reply = f.Scope(f.H))
                {
                    var result = await reply.Connections.PhanHoiHuyAsync(firstId, true, firstRv, f.H, default);
                    Assert.True(result.ThanhCong, result.ThongDiep);
                }
                await EndExpired(f, firstId);
                await RunMaintenance(f, "XuLyTuHuyKetNoiAsync", DateTimeOffset.UtcNow.AddDays(1), CancellationToken.None);
                await using (var db = f.Db())
                {
                    Assert.Equal("ChapNhan", (await db.Set<YeuCauHuyKetNoi>().SingleAsync(x => x.Id == firstId)).TrangThai);
                    Assert.Equal("DongY", (await db.Set<LopHoc>().SingleAsync(x => x.Id == f.Class)).CachKetThuc);
                }

                // DL02: hết hạn được worker chốt trước; phản hồi đến sau phải nhận 409.
                await using var late = new Fixture(f.Id + "-late"); await late.Init();
                var lateId = await EndRequest(late); var lateRv = await late.Version<YeuCauHuyKetNoi>(lateId);
                await EndExpired(late, lateId);
                await RunMaintenance(late, "XuLyTuHuyKetNoiAsync", DateTimeOffset.UtcNow, CancellationToken.None);
                await using (var reply = late.Scope(late.H))
                {
                    var result = await reply.Connections.PhanHoiHuyAsync(lateId, true, lateRv, late.H, default);
                    Assert.False(result.ThanhCong); Assert.Equal("409", result.MaLoi);
                }
                await using (var db = late.Db())
                {
                    Assert.Equal("TuDongChapNhan", (await db.Set<YeuCauHuyKetNoi>().SingleAsync(x => x.Id == lateId)).TrangThai);
                    Assert.Equal("HetHan", (await db.Set<LopHoc>().SingleAsync(x => x.Id == late.Class)).CachKetThuc);
                }
                break;
            }
            default: throw new InvalidOperationException(key);
        }
    }
}
