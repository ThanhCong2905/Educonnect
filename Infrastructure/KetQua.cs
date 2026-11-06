using Microsoft.EntityFrameworkCore;

namespace EduConnect.Mvc.Infrastructure;

public sealed class KetQua<T>
{
    private readonly T? duLieu;
    private readonly LoiNghiepVu? loi;

    private KetQua(T value) => duLieu = value;
    private KetQua(LoiNghiepVu error) => loi = error;

    public bool ThanhCong => loi == null;
    public string? ThongBao => loi?.Message;
    public string? ThongDiep => ThongBao;
    public string? MaLoi => loi?.Status.ToString(System.Globalization.CultureInfo.InvariantCulture);
    public int MaTrangThai => loi?.Status ?? 200;
    public T DuLieu => loi == null ? duLieu! : throw loi;

    public static async Task<KetQua<T>> ThucHienAsync(Func<Task<T>> action)
    {
        try
        {
            return new(await action());
        }
        catch (LoiNghiepVu ex) { return new(ex); }
        catch (DbUpdateConcurrencyException)
        {
            return new(new LoiNghiepVu("Dữ liệu vừa được cập nhật. Hãy tải lại trước khi lưu.", 409));
        }
        catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number is >= 51130 and <= 51154)
        {
            var status = ex.Number switch
            {
                51133 or 51140 or 51151 => 403,
                51132 or 51138 or 51139 or 51141 or 51144 or 51145 or 51152 => 409,
                _ => 400
            };
            return new(new LoiNghiepVu(ex.Message, status));
        }
    }
}

public sealed class Page<T>
{
    public List<T> Items { get; init; } = [];
    public int Total
    {
        get; init;
    }
    public int Trang
    {
        get; init;
    }
    public int KichThuocTrang { get; init; } = 9;
}
