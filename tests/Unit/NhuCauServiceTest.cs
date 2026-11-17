using EduConnect.Mvc.Areas.NhuCau.Services;
using EduConnect.Mvc.Areas.NhuCau.ViewModels;
using EduConnect.Mvc.Data;
using EduConnect.Mvc.Infrastructure;
using EduConnect.Mvc.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Xunit;

namespace EduConnect.UnitTests;

public sealed class NhuCauServiceTest
{
    [Fact, Trait("TestCaseId", "UT-003")]
    public async Task UT_003_TaoCongKhai_DungChuMonLich_MotGiaoDich()
    {
        using var f = new Fixture();
        var input = f.Input();
        var result = await f.Service.TaoNhuCauHocAsync(input, f.User.Id, f.Token);
        f.AssertCreated(result, input, "DangMo");
        f.Store.Verify(x => x.AddPosting(It.Is<LuotDangNhuCau>(p => p.NhuCauId == result.DuLieu && p.TaiKhoanId == f.User.Id && p.MaThaoTac == input.MaThaoTac && p.HanhDong == "Dang")), Times.Once);
    }

    [Fact, Trait("TestCaseId", "UT-004")]
    public async Task UT_004_TaoNhap_DungChuMonLich_KhongTinhLuotDang()
    {
        using var f = new Fixture();
        var input = f.Input(); input.BanNhap = true;
        var result = await f.Service.TaoNhuCauHocAsync(input, f.User.Id, f.Token);
        f.AssertCreated(result, input, "BanNhap");
        f.Store.Verify(x => x.AddPosting(It.IsAny<LuotDangNhuCau>()), Times.Never);
    }

    [Fact, Trait("TestCaseId", "UT-005")]
    public async Task UT_005_MonKhongTonTai_KhongGhi()
    {
        using var f = new Fixture(); f.Subjects.Clear();
        var result = await f.Service.TaoNhuCauHocAsync(f.Input(), f.User.Id, f.Token);
        f.AssertRejectedWithoutWrite(result, 400, "Môn học không hợp lệ.");
        f.Store.Verify(x => x.CountActiveSubjectsAsync(It.Is<List<int>>(ids => ids.SequenceEqual(new[] {1})), f.Token), Times.Once);
    }

    [Fact, Trait("TestCaseId", "UT-006")]
    public async Task UT_006_MonNgungSuDung_KhongGhi()
    {
        using var f = new Fixture(); f.Subjects[0].DangSuDung = false;
        var result = await f.Service.TaoNhuCauHocAsync(f.Input(), f.User.Id, f.Token);
        f.AssertRejectedWithoutWrite(result, 400, "Môn học không hợp lệ.");
    }

    [Fact, Trait("TestCaseId", "UT-007")]
    public async Task UT_007_LichDaoNguoc_KhongGhi()
    {
        using var f = new Fixture(); var input = f.Input();
        input.LichRanh[0].GioBatDau = new(19, 0); input.LichRanh[0].GioKetThuc = new(18, 0);
        var result = await f.Service.TaoNhuCauHocAsync(input, f.User.Id, f.Token);
        f.AssertRejectedWithoutWrite(result, 400, "Thứ hoặc giờ của khung giờ không hợp lệ.");
    }

    [Fact, Trait("TestCaseId", "UT-011")]
    public async Task UT_011_NapSua_DuIdPhienBanMonVaHaiLich()
    {
        using var f = new Fixture(); f.SeedNeed();
        var result = await f.Service.LayDeSuaAsync(f.NeedId);
        Assert.True(result.ThanhCong, result.ThongBao);
        var model = result.DuLieu;
        Assert.Equal(f.NeedId, model.Id);
        Assert.Equal(Convert.ToBase64String(Fixture.Version), model.RowVersion);
        Assert.Equal(new[] {1, 2}, model.MonHocIds.OrderBy(x => x));
        Assert.Equal(f.Existing!.MucTieu, model.MucTieu);
        Assert.Equal(f.Existing.KhoiLop, model.KhoiLop);
        Assert.Equal(f.Existing.HocLuc, model.HocLuc);
        Assert.Equal(f.Existing.SoBuoiTuan, model.SoBuoiTuan);
        Assert.Equal(f.Existing.HinhThucHoc, model.HinhThucHoc);
        Assert.Equal(f.Existing.HocPhiDuKien, model.HocPhiDuKien);
        Assert.Equal(f.Existing.DonViHocPhi, model.DonViHocPhi);
        Assert.Equal(f.OldSlots.Select(s => (s.ThuTrongTuan, s.GioBatDau, s.GioKetThuc)), model.LichRanh.Select(s => (s.ThuTrongTuan, s.GioBatDau, s.GioKetThuc)));
        f.Store.Verify(x => x.FindOwnedAsync(f.NeedId, f.User.Id, false, default), Times.Once);
        f.AssertNoWrite();
    }

    [Fact, Trait("TestCaseId", "UT-012")]
    public async Task UT_012_NhuCauCuaNguoiKhac_KhongLoNoiDung()
    {
        using var f = new Fixture(); f.SeedNeed(owner: Guid.NewGuid());
        var result = await f.Service.LayDeSuaAsync(f.NeedId);
        f.AssertRejectedWithoutWrite(result, 404, "Không tìm thấy nhu cầu của bạn.");
        Assert.Throws<LoiNghiepVu>(() => result.DuLieu);
        f.Store.Verify(x => x.FindOwnedAsync(f.NeedId, f.User.Id, false, default), Times.Once);
        f.Store.Verify(x => x.ReadSlotsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact, Trait("TestCaseId", "UT-014")]
    public async Task UT_014_MoBanNhap_ThayToanBoLich_KhongCongDon()
    {
        using var f = new Fixture(); f.SeedNeed(state: "BanNhap");
        var input = f.Input(); input.MaNhuCau = f.NeedId;
        input.LichRanh = [new() { ThuTrongTuan = 3, GioBatDau = new(18,0), GioKetThuc = new(19,0) }, new() { ThuTrongTuan = 5, GioBatDau = new(18,0), GioKetThuc = new(19,0) }];
        var result = await f.Service.CapNhatAsync(f.NeedId, input, Fixture.Version, f.User.Id, f.Token);
        Assert.True(result.ThanhCong, result.ThongBao);
        Assert.Equal(f.NeedId, result.DuLieu);
        Assert.Equal("DangMo", f.Tracked!.TrangThai);
        Assert.Equal(input.MucTieu, f.Tracked.MucTieu);
        Assert.Equal(new byte[] {3, 5}, f.AddedSlots.Select(s => s.ThuTrongTuan));
        Assert.All(f.AddedSlots, s => Assert.Equal(f.NeedId, s.NhuCauId));
        Assert.Equal(new[] {"save", "delete-slots", "add-slot", "add-slot", "save", "save", "commit"}, f.Events);
        f.Store.Verify(x => x.AddNeed(It.IsAny<NhuCauHoc>()), Times.Never);
        f.Store.Verify(x => x.SetVersion(f.Tracked, It.Is<byte[]>(v => v.SequenceEqual(Fixture.Version))), Times.Once);
        f.Transaction.Verify(x => x.CommitAsync(f.Token), Times.Once);
        Assert.Equal("BanNhap", f.Existing!.TrangThai); // repository snapshot is not the tracked copy
    }

    [Fact, Trait("TestCaseId", "UT-015")]
    public async Task UT_015_CapNhatSaiChu_KhongGhiDe()
    {
        using var f = new Fixture(); f.SeedNeed(owner: Guid.NewGuid());
        var result = await f.Service.CapNhatAsync(f.NeedId, f.Input(), Fixture.Version, f.User.Id, f.Token);
        f.AssertRejectedWithoutWrite(result, 403, "Nhu cầu không thuộc về bạn.");
        f.Store.Verify(x => x.FindOwnedAsync(f.NeedId, f.User.Id, true, f.Token), Times.Once);
        Assert.Equal("Muc tieu cu", f.Existing!.MucTieu);
    }

    [Fact, Trait("TestCaseId", "UT-016")]
    public async Task UT_016_PhienBanCu_ChuyenDungRV_VaXuLyXungDot()
    {
        using var f = new Fixture(); f.SeedNeed();
        byte[] stale = [8,7,6,5,4,3,2,1];
        var result = await f.Service.CapNhatAsync(f.NeedId, f.Input(), stale, f.User.Id, f.Token);
        Assert.False(result.ThanhCong);
        Assert.Equal(409, result.MaTrangThai);
        Assert.Equal("Dữ liệu vừa được cập nhật. Hãy tải lại trước khi lưu.", result.ThongBao);
        f.Store.Verify(x => x.SetVersion(It.IsAny<NhuCauHoc>(), It.Is<byte[]>(v => v.SequenceEqual(stale))), Times.Once);
        f.Store.Verify(x => x.SaveAsync(f.Token), Times.Once);
        f.Store.Verify(x => x.DeleteSlotsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        f.Transaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        Assert.Equal("Muc tieu cu", f.Existing!.MucTieu);
        Assert.Empty(f.AddedSlots);
    }

    [Fact, Trait("TestCaseId", "UT-017")]
    public async Task UT_017_CapNhatMonNgungSuDung_KhongGhiDe()
    {
        using var f = new Fixture(); f.SeedNeed(); f.Subjects[0].DangSuDung = false;
        var result = await f.Service.CapNhatAsync(f.NeedId, f.Input(), Fixture.Version, f.User.Id, f.Token);
        f.AssertRejectedWithoutWrite(result, 400, "Môn học không hợp lệ.");
        Assert.Equal("Muc tieu cu", f.Existing!.MucTieu);
    }

    [Fact, Trait("TestCaseId", "UT-018")]
    public async Task UT_018_CapNhatLichBangGio_KhongGhiDe()
    {
        using var f = new Fixture(); f.SeedNeed(); var input = f.Input();
        input.LichRanh[0].GioKetThuc = input.LichRanh[0].GioBatDau;
        var result = await f.Service.CapNhatAsync(f.NeedId, input, Fixture.Version, f.User.Id, f.Token);
        f.AssertRejectedWithoutWrite(result, 400, "Thứ hoặc giờ của khung giờ không hợp lệ.");
        Assert.Equal("Muc tieu cu", f.Existing!.MucTieu);
    }

    private sealed class Fixture : IDisposable
    {
        public static readonly byte[] Version = [1,2,3,4,5,6,7,8];
        private readonly CancellationTokenSource cancellation = new();
        private readonly AppDbContext db = new(new DbContextOptionsBuilder<AppDbContext>().Options);
        public CancellationToken Token => cancellation.Token;
        public NguoiThamGia User { get; } = new() {Id=Guid.NewGuid(), HoTen="U1", LoaiNguoi="HocSinh"};
        public Guid NeedId { get; } = Guid.NewGuid();
        public NhuCauHoc? Existing { get; private set; }
        public NhuCauHoc? Tracked { get; private set; }
        public NhuCauHoc? Added { get; private set; }
        public List<MonHoc> Subjects { get; } = [new() {Id=1, TenMon="Toán Học", DangSuDung=true}, new() {Id=2, TenMon="Tiếng Anh", DangSuDung=true}];
        public List<KhungGioRanh> OldSlots { get; } = [];
        public List<KhungGioRanh> AddedSlots { get; } = [];
        public List<string> Events { get; } = [];
        public Mock<INhuCauStore> Store { get; } = new(MockBehavior.Strict);
        public Mock<IDbContextTransaction> Transaction { get; } = new(MockBehavior.Strict);
        public Mock<INguoiDungHienTaiService> Current { get; } = new(MockBehavior.Strict);
        public NhuCauService Service { get; }
        private byte[]? suppliedVersion;

        public Fixture()
        {
            Current.Setup(x => x.HienTaiAsync(It.IsAny<CancellationToken>())).ReturnsAsync(User);
            Current.Setup(x => x.KiemTraNguoiThucHienAsync(User.Id, Token)).ReturnsAsync(User);
            Store.Setup(x => x.BeginAsync(Token)).ReturnsAsync(Transaction.Object);
            Store.Setup(x => x.LockOwnerAsync(User.Id, Token)).Returns(Task.CompletedTask);
            Store.Setup(x => x.FindOperationAsync(User.Id, It.IsAny<Guid>(), Token)).ReturnsAsync((LuotDangNhuCau?)null);
            Store.Setup(x => x.FindOwnedAsync(It.IsAny<Guid>(), User.Id, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid id, Guid owner, bool tracking, CancellationToken ct) => Existing?.Id == id && Existing.ChuSoHuuId == owner ? tracking ? Tracked = Clone(Existing) : Clone(Existing) : null);
            Store.Setup(x => x.CountNeedsAsync(User.Id, It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>())).ReturnsAsync(0);
            Store.Setup(x => x.CountPostingsAsync(User.Id, It.IsAny<DateOnly>(), It.IsAny<CancellationToken>())).ReturnsAsync(0);
            Store.Setup(x => x.CountActiveSubjectsAsync(It.IsAny<List<int>>(), Token)).ReturnsAsync((List<int> ids, CancellationToken ct) => Subjects.Count(s => ids.Contains(s.Id) && s.DangSuDung));
            Store.Setup(x => x.FindDuplicateCandidatesAsync(It.IsAny<NhuCauInput>(), User.Id, Token)).ReturnsAsync([]);
            Store.Setup(x => x.AddNeed(It.IsAny<NhuCauHoc>())).Callback<NhuCauHoc>(n => Added=n);
            Store.Setup(x => x.SetVersion(It.IsAny<NhuCauHoc>(), It.IsAny<byte[]>())).Callback<NhuCauHoc,byte[]>((n,v) => suppliedVersion=[..v]);
            Store.Setup(x => x.SaveAsync(Token)).Returns(() =>
            {
                Events.Add("save");
                return suppliedVersion != null && !suppliedVersion.SequenceEqual(Version)
                    ? Task.FromException(new DbUpdateConcurrencyException("stale rowversion from persistence")) : Task.CompletedTask;
            });
            Store.Setup(x => x.DeleteSlotsAsync(It.IsAny<Guid>(), Token)).Callback(() => Events.Add("delete-slots")).Returns(Task.CompletedTask);
            Store.Setup(x => x.AddSlot(It.IsAny<KhungGioRanh>())).Callback<KhungGioRanh>(s => {AddedSlots.Add(s); Events.Add("add-slot");});
            Store.Setup(x => x.AddPosting(It.IsAny<LuotDangNhuCau>()));
            Store.Setup(x => x.ReadLinksAsync(It.IsAny<Guid>(), Token)).ReturnsAsync([]);
            Store.Setup(x => x.RemoveLinks(It.IsAny<IEnumerable<NhuCauMonHoc>>()));
            Store.Setup(x => x.AddLinks(It.IsAny<IEnumerable<NhuCauMonHoc>>()));
            Store.Setup(x => x.ReadSubjectAsync(1, default)).ReturnsAsync(() => Subjects[0]);
            Store.Setup(x => x.IsCommonSubjectAsync(1, default)).ReturnsAsync(true);
            Store.Setup(x => x.ReadSlotsAsync(NeedId, default)).ReturnsAsync(() => OldSlots);
            Transaction.Setup(x => x.CommitAsync(Token)).Callback(() => Events.Add("commit")).Returns(Task.CompletedTask);
            Transaction.Setup(x => x.DisposeAsync()).Returns(ValueTask.CompletedTask);
            Service = new NhuCauService(db, Current.Object, new MonHocCatalog(db), Store.Object);
        }

        public TaoNhuCauBenHocViewModel Input() => new()
        {
            MonHocId=1, MonHocIds=[1], MucTieu="On tap dai so", KhoiLop="10", HocLuc="Kha", SoBuoiTuan=2,
            HinhThucHoc="TrucTuyen", HocPhiDuKien=150000m, DonViHocPhi="VND/Buoi",
            LichRanh=[new() {ThuTrongTuan=1, GioBatDau=new(18,0), GioKetThuc=new(19,0)}]
        };
        public void SeedNeed(Guid? owner=null, string state="DangMo")
        {
            Existing = new() {Id=NeedId, ChuSoHuuId=owner??User.Id, LoaiNhuCau="TimGiaSu", MonHocId=1, MonHocs=[new() {NhuCauId=NeedId, MonHocId=1},new() {NhuCauId=NeedId, MonHocId=2}], RowVersion=[..Version], TrangThai=state, MucTieu="Muc tieu cu", KhoiLop="9", HocLuc="Kha", SoBuoiTuan=2, HinhThucHoc="TrucTuyen", HocPhiDuKien=120000, DonViHocPhi="VND/Buoi"};
            OldSlots.AddRange([new() {Id=Guid.NewGuid(), NhuCauId=NeedId, ThuTrongTuan=1,GioBatDau=new(16,0),GioKetThuc=new(17,0)},new() {Id=Guid.NewGuid(),NhuCauId=NeedId,ThuTrongTuan=2,GioBatDau=new(16,0),GioKetThuc=new(17,0)}]);
        }
        private static NhuCauHoc Clone(NhuCauHoc n) => new() {Id=n.Id,ChuSoHuuId=n.ChuSoHuuId,LoaiNhuCau=n.LoaiNhuCau,MonHocId=n.MonHocId,MonHocs=[..n.MonHocs],RowVersion=[..n.RowVersion],TrangThai=n.TrangThai,MucTieu=n.MucTieu,KhoiLop=n.KhoiLop,HocLuc=n.HocLuc,SoBuoiTuan=n.SoBuoiTuan,HinhThucHoc=n.HinhThucHoc,HocPhiDuKien=n.HocPhiDuKien,DonViHocPhi=n.DonViHocPhi};
        public void AssertCreated(KetQua<Guid> result, TaoNhuCauBenHocViewModel input, string state)
        {
            Assert.True(result.ThanhCong, result.ThongBao);
            Assert.NotEqual(Guid.Empty, result.DuLieu);
            Assert.NotNull(Added);
            Assert.Equal(result.DuLieu, Added.Id); Assert.Equal(User.Id, Added.ChuSoHuuId);
            Assert.Equal(state, Added.TrangThai); Assert.Equal(1, Added.MonHocId);
            Assert.Equal(input.MucTieu, Added.MucTieu);
            var slot=Assert.Single(AddedSlots);
            Assert.Equal(Added.Id,slot.NhuCauId); Assert.Equal((byte)1,slot.ThuTrongTuan);
            Assert.Equal(new TimeOnly(18,0),slot.GioBatDau); Assert.Equal(new TimeOnly(19,0),slot.GioKetThuc);
            Store.Verify(x => x.AddNeed(It.IsAny<NhuCauHoc>()), Times.Once);
            Store.Verify(x => x.AddLinks(It.Is<IEnumerable<NhuCauMonHoc>>(links => links.Count()==1 && links.Single().NhuCauId==result.DuLieu && links.Single().MonHocId==1)), Times.Once);
            Transaction.Verify(x => x.CommitAsync(Token), Times.Once);
            Assert.Equal("commit",Events.Last());
            Current.Verify(x => x.KiemTraNguoiThucHienAsync(User.Id,Token),Times.Once);
        }
        public void AssertRejectedWithoutWrite<T>(KetQua<T> result,int status,string message)
        {
            Assert.False(result.ThanhCong); Assert.Equal(status,result.MaTrangThai); Assert.Equal(message,result.ThongBao); AssertNoWrite();
        }
        public void AssertNoWrite()
        {
            Store.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()),Times.Never);
            Store.Verify(x => x.AddNeed(It.IsAny<NhuCauHoc>()),Times.Never);
            Store.Verify(x => x.DeleteSlotsAsync(It.IsAny<Guid>(),It.IsAny<CancellationToken>()),Times.Never);
            Store.Verify(x => x.AddSlot(It.IsAny<KhungGioRanh>()),Times.Never);
            Store.Verify(x => x.AddPosting(It.IsAny<LuotDangNhuCau>()),Times.Never);
            Transaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()),Times.Never);
            Assert.Empty(AddedSlots);
        }
        public void Dispose() {db.Dispose();cancellation.Dispose();}
    }
}
