namespace SynergyDraft.Api.Models;
// 서버 내부에서 선택 완료된 사도와 실제 배치 열을 저장하는 모델
public class SelectedApostle
{
    public string ApostleId { get; set; } = "";
    // 서버 검증을 거쳐 확정된 배치 열
    // 예: "전열", "중열", "후열"
    public string AssignedRow { get; set; } = "";
}