using Xunit;
using Xunit.Abstractions;
namespace EduConnect.KetNoiKetThuc.UnitTests;

[Trait("Suite","ControllerKetNoiKetThuc")]
public sealed class ControllerKetNoiKetThucTest(ITestOutputHelper output)
{
    [Fact,Trait("TestCaseId","UT-106")]
    public Task UT_106_GuiYeuCau_ModelStateLoi()=>ControllerKetNoiKetThucRunner.RunAsync(106,output);
    [Fact,Trait("TestCaseId","UT-107")]
    public Task UT_107_GuiYeuCau_ThanhCong()=>ControllerKetNoiKetThucRunner.RunAsync(107,output);
    [Fact,Trait("TestCaseId","UT-108")]
    public Task UT_108_GuiYeuCau_Service403()=>ControllerKetNoiKetThucRunner.RunAsync(108,output);
    [Fact,Trait("TestCaseId","UT-109")]
    public Task UT_109_GuiYeuCau_Service404()=>ControllerKetNoiKetThucRunner.RunAsync(109,output);
    [Fact,Trait("TestCaseId","UT-110")]
    public Task UT_110_GuiYeuCau_Service409()=>ControllerKetNoiKetThucRunner.RunAsync(110,output);
    [Fact,Trait("TestCaseId","UT-111")]
    public Task UT_111_GuiYeuCau_Service400()=>ControllerKetNoiKetThucRunner.RunAsync(111,output);
    [Fact,Trait("TestCaseId","UT-112")]
    public Task UT_112_GuiYeuCau_Service500()=>ControllerKetNoiKetThucRunner.RunAsync(112,output);
    [Fact,Trait("TestCaseId","UT-113")]
    public Task UT_113_GuiYeuCau_DungThamSoVaToken()=>ControllerKetNoiKetThucRunner.RunAsync(113,output);
    [Fact,Trait("TestCaseId","UT-114")]
    public Task UT_114_GuiDeXuat_ModelStateLoi()=>ControllerKetNoiKetThucRunner.RunAsync(114,output);
    [Fact,Trait("TestCaseId","UT-115")]
    public Task UT_115_GuiDeXuat_ThanhCong()=>ControllerKetNoiKetThucRunner.RunAsync(115,output);
    [Fact,Trait("TestCaseId","UT-116")]
    public Task UT_116_GuiDeXuat_Service403()=>ControllerKetNoiKetThucRunner.RunAsync(116,output);
    [Fact,Trait("TestCaseId","UT-117")]
    public Task UT_117_GuiDeXuat_Service404()=>ControllerKetNoiKetThucRunner.RunAsync(117,output);
    [Fact,Trait("TestCaseId","UT-118")]
    public Task UT_118_GuiDeXuat_Service409()=>ControllerKetNoiKetThucRunner.RunAsync(118,output);
    [Fact,Trait("TestCaseId","UT-119")]
    public Task UT_119_GuiDeXuat_Service400()=>ControllerKetNoiKetThucRunner.RunAsync(119,output);
    [Fact,Trait("TestCaseId","UT-120")]
    public Task UT_120_GuiDeXuat_Service500()=>ControllerKetNoiKetThucRunner.RunAsync(120,output);
    [Fact,Trait("TestCaseId","UT-121")]
    public Task UT_121_GuiDeXuat_DungThamSoVaToken()=>ControllerKetNoiKetThucRunner.RunAsync(121,output);
    [Fact,Trait("TestCaseId","UT-122")]
    public Task UT_122_ChapNhan_ModelStateLoi()=>ControllerKetNoiKetThucRunner.RunAsync(122,output);
    [Fact,Trait("TestCaseId","UT-123")]
    public Task UT_123_ChapNhan_ThanhCong()=>ControllerKetNoiKetThucRunner.RunAsync(123,output);
    [Fact,Trait("TestCaseId","UT-124")]
    public Task UT_124_ChapNhan_Service403()=>ControllerKetNoiKetThucRunner.RunAsync(124,output);
    [Fact,Trait("TestCaseId","UT-125")]
    public Task UT_125_ChapNhan_Service404()=>ControllerKetNoiKetThucRunner.RunAsync(125,output);
    [Fact,Trait("TestCaseId","UT-126")]
    public Task UT_126_ChapNhan_Service409()=>ControllerKetNoiKetThucRunner.RunAsync(126,output);
    [Fact,Trait("TestCaseId","UT-127")]
    public Task UT_127_ChapNhan_Service400()=>ControllerKetNoiKetThucRunner.RunAsync(127,output);
    [Fact,Trait("TestCaseId","UT-128")]
    public Task UT_128_ChapNhan_Service500()=>ControllerKetNoiKetThucRunner.RunAsync(128,output);
    [Fact,Trait("TestCaseId","UT-129")]
    public Task UT_129_ChapNhan_DungThamSoVaToken()=>ControllerKetNoiKetThucRunner.RunAsync(129,output);
    [Fact,Trait("TestCaseId","UT-130")]
    public Task UT_130_TuChoi_ModelStateLoi()=>ControllerKetNoiKetThucRunner.RunAsync(130,output);
    [Fact,Trait("TestCaseId","UT-131")]
    public Task UT_131_TuChoi_ThanhCong()=>ControllerKetNoiKetThucRunner.RunAsync(131,output);
    [Fact,Trait("TestCaseId","UT-132")]
    public Task UT_132_TuChoi_Service403()=>ControllerKetNoiKetThucRunner.RunAsync(132,output);
    [Fact,Trait("TestCaseId","UT-133")]
    public Task UT_133_TuChoi_Service404()=>ControllerKetNoiKetThucRunner.RunAsync(133,output);
    [Fact,Trait("TestCaseId","UT-134")]
    public Task UT_134_TuChoi_Service409()=>ControllerKetNoiKetThucRunner.RunAsync(134,output);
    [Fact,Trait("TestCaseId","UT-135")]
    public Task UT_135_TuChoi_Service400()=>ControllerKetNoiKetThucRunner.RunAsync(135,output);
    [Fact,Trait("TestCaseId","UT-136")]
    public Task UT_136_TuChoi_Service500()=>ControllerKetNoiKetThucRunner.RunAsync(136,output);
    [Fact,Trait("TestCaseId","UT-137")]
    public Task UT_137_TuChoi_DungThamSoVaToken()=>ControllerKetNoiKetThucRunner.RunAsync(137,output);
    [Fact,Trait("TestCaseId","UT-138")]
    public Task UT_138_Rut_ModelStateLoi()=>ControllerKetNoiKetThucRunner.RunAsync(138,output);
    [Fact,Trait("TestCaseId","UT-139")]
    public Task UT_139_Rut_ThanhCong()=>ControllerKetNoiKetThucRunner.RunAsync(139,output);
    [Fact,Trait("TestCaseId","UT-140")]
    public Task UT_140_Rut_Service403()=>ControllerKetNoiKetThucRunner.RunAsync(140,output);
    [Fact,Trait("TestCaseId","UT-141")]
    public Task UT_141_Rut_Service404()=>ControllerKetNoiKetThucRunner.RunAsync(141,output);
    [Fact,Trait("TestCaseId","UT-142")]
    public Task UT_142_Rut_Service409()=>ControllerKetNoiKetThucRunner.RunAsync(142,output);
    [Fact,Trait("TestCaseId","UT-143")]
    public Task UT_143_Rut_Service400()=>ControllerKetNoiKetThucRunner.RunAsync(143,output);
    [Fact,Trait("TestCaseId","UT-144")]
    public Task UT_144_Rut_Service500()=>ControllerKetNoiKetThucRunner.RunAsync(144,output);
    [Fact,Trait("TestCaseId","UT-145")]
    public Task UT_145_Rut_DungThamSoVaToken()=>ControllerKetNoiKetThucRunner.RunAsync(145,output);
    [Fact,Trait("TestCaseId","UT-146")]
    public Task UT_146_XacNhanKetThuc_ModelStateLoi()=>ControllerKetNoiKetThucRunner.RunAsync(146,output);
    [Fact,Trait("TestCaseId","UT-147")]
    public Task UT_147_XacNhanKetThuc_ThanhCong()=>ControllerKetNoiKetThucRunner.RunAsync(147,output);
    [Fact,Trait("TestCaseId","UT-148")]
    public Task UT_148_XacNhanKetThuc_Service403()=>ControllerKetNoiKetThucRunner.RunAsync(148,output);
    [Fact,Trait("TestCaseId","UT-149")]
    public Task UT_149_XacNhanKetThuc_Service404()=>ControllerKetNoiKetThucRunner.RunAsync(149,output);
    [Fact,Trait("TestCaseId","UT-150")]
    public Task UT_150_XacNhanKetThuc_Service409()=>ControllerKetNoiKetThucRunner.RunAsync(150,output);
    [Fact,Trait("TestCaseId","UT-151")]
    public Task UT_151_XacNhanKetThuc_Service400()=>ControllerKetNoiKetThucRunner.RunAsync(151,output);
    [Fact,Trait("TestCaseId","UT-152")]
    public Task UT_152_XacNhanKetThuc_Service500()=>ControllerKetNoiKetThucRunner.RunAsync(152,output);
    [Fact,Trait("TestCaseId","UT-153")]
    public Task UT_153_XacNhanKetThuc_DungThamSoVaToken()=>ControllerKetNoiKetThucRunner.RunAsync(153,output);
    [Fact,Trait("TestCaseId","UT-154")]
    public Task UT_154_HuyKetNoi_ModelStateLoi()=>ControllerKetNoiKetThucRunner.RunAsync(154,output);
    [Fact,Trait("TestCaseId","UT-155")]
    public Task UT_155_HuyKetNoi_ThanhCong()=>ControllerKetNoiKetThucRunner.RunAsync(155,output);
    [Fact,Trait("TestCaseId","UT-156")]
    public Task UT_156_HuyKetNoi_Service403()=>ControllerKetNoiKetThucRunner.RunAsync(156,output);
    [Fact,Trait("TestCaseId","UT-157")]
    public Task UT_157_HuyKetNoi_Service404()=>ControllerKetNoiKetThucRunner.RunAsync(157,output);
    [Fact,Trait("TestCaseId","UT-158")]
    public Task UT_158_HuyKetNoi_Service409()=>ControllerKetNoiKetThucRunner.RunAsync(158,output);
    [Fact,Trait("TestCaseId","UT-159")]
    public Task UT_159_HuyKetNoi_Service400()=>ControllerKetNoiKetThucRunner.RunAsync(159,output);
    [Fact,Trait("TestCaseId","UT-160")]
    public Task UT_160_HuyKetNoi_Service500()=>ControllerKetNoiKetThucRunner.RunAsync(160,output);
    [Fact,Trait("TestCaseId","UT-161")]
    public Task UT_161_HuyKetNoi_DungThamSoVaToken()=>ControllerKetNoiKetThucRunner.RunAsync(161,output);
    [Fact,Trait("TestCaseId","UT-162")]
    public Task UT_162_PhanHoiHuy_ModelStateLoi()=>ControllerKetNoiKetThucRunner.RunAsync(162,output);
    [Fact,Trait("TestCaseId","UT-163")]
    public Task UT_163_PhanHoiHuy_ThanhCong()=>ControllerKetNoiKetThucRunner.RunAsync(163,output);
    [Fact,Trait("TestCaseId","UT-164")]
    public Task UT_164_PhanHoiHuy_Service403()=>ControllerKetNoiKetThucRunner.RunAsync(164,output);
    [Fact,Trait("TestCaseId","UT-165")]
    public Task UT_165_PhanHoiHuy_Service404()=>ControllerKetNoiKetThucRunner.RunAsync(165,output);
    [Fact,Trait("TestCaseId","UT-166")]
    public Task UT_166_PhanHoiHuy_Service409()=>ControllerKetNoiKetThucRunner.RunAsync(166,output);
    [Fact,Trait("TestCaseId","UT-167")]
    public Task UT_167_PhanHoiHuy_Service400()=>ControllerKetNoiKetThucRunner.RunAsync(167,output);
    [Fact,Trait("TestCaseId","UT-168")]
    public Task UT_168_PhanHoiHuy_Service500()=>ControllerKetNoiKetThucRunner.RunAsync(168,output);
    [Fact,Trait("TestCaseId","UT-169")]
    public Task UT_169_PhanHoiHuy_DungThamSoVaToken()=>ControllerKetNoiKetThucRunner.RunAsync(169,output);
}
