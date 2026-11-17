namespace EduConnect.Mvc.Areas.QuanTri.Services;
public interface IQuyenQuanTriService
{
    Task<bool> CoQuyenAsync(string permission);
    Task<Guid> KiemTraAsync(string permission);
    void GhiNhatKy(Guid actor, string hanhDong, string loai, object id, string ketQua = "ThanhCong", string? chiTiet = null);
}
