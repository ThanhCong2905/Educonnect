using Xunit;

namespace EduConnect.UnitTests;

public sealed class NhuCauSuggestionTest
{
    [Fact(Skip = "UT-020: chưa có đơn vị điều phối sau tạo gọi nguồn gợi ý theo N1. NhuCauController chỉ chuyển Index; INhuCauService chưa có hợp đồng nhận/trả H1,H2. Không dùng test GoiYRules để thay thế luồng này.")]
    [Trait("TestCaseId", "UT-020")]
    public void UT_020_KhongBoSotGoiYBanDau_SauTaoNhuCau()
    {
        // Intentionally blocked, not a passing placeholder. Implement when the
        // production orchestration contract for the approved UC exists.
        throw new InvalidOperationException("Chưa có hợp đồng điều phối gợi ý theo nhu cầu mới.");
    }
}
