namespace EduConnect.Mvc.Infrastructure;

public sealed class KetQua
{
    private KetQua<bool>? ketQua;
    public bool ThanhCong
    {
        get; private init;
    }
    public string? MaLoi
    {
        get; private init;
    }
    public string? ThongDiep
    {
        get; private init;
    }

    public static async Task<KetQua> ThucHienAsync(Func<Task> action)
    {
        var result = await KetQua<bool>.ThucHienAsync(async () =>
        {
            await action();
            return true;
        });
        return new()
        {
            ketQua = result,
            ThanhCong = result.ThanhCong,
            MaLoi = result.MaLoi,
            ThongDiep = result.ThongDiep
        };
    }

    public void BaoLoiNeuThatBai()
    {
        if (ketQua != null)
        {
            ketQua.BaoLoiNeuThatBai();
            return;
        }
        if (ThanhCong)
            return;

        var status = int.TryParse(MaLoi, out var code) ? code : 400;
        throw new LoiNghiepVu(ThongDiep ?? "Không thể thực hiện thao tác.", status);
    }
}
