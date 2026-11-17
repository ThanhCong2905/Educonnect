using System.Reflection;
using Xunit;
namespace EduConnect.HocTap.UnitTests;

internal static class HocTapDesignContract
{
    public static void RequireMethod(string id,Type type,string name,Type? firstParameter,string actualCounterpart)
    {
        var matches=type.GetMethods(BindingFlags.Public|BindingFlags.Instance)
            .Where(m=>m.Name==name)
            .Where(m=>firstParameter==null||m.GetParameters().Length>0&&m.GetParameters()[0].ParameterType==firstParameter)
            .ToArray();
        string signature=type.Name+"."+name+(firstParameter==null?"(...)":"("+firstParameter.Name+")");
        Assert.True(matches.Length>0,
            $"{id} — FAIL ĐỐI CHIẾU THIẾT KẾ.\nMong đợi: code khai báo public {signature}.\nThực tế: không tìm thấy phương thức/overload này.\nCode hiện có: {actualCounterpart}.\nKhông tự coi phương thức khác tên là tương đương. Chưa chạy phần kiểm hành vi vì hợp đồng thiết kế chưa được triển khai đúng.");
    }
    public static void RequireModel(string id,Assembly assembly,string name,string actualCounterpart)
    {
        Assert.True(assembly.GetTypes().Any(t=>t.IsPublic&&t.Name==name),
            $"{id} — FAIL ĐỐI CHIẾU THIẾT KẾ.\nMong đợi: có model public {name} để kiểm validation.\nThực tế: không tìm thấy model {name}; code hiện có {actualCounterpart}.\nKhông thay model khác tên để báo Pass cho model trong thiết kế.");
    }
}
