namespace SynergyDraft.Api.Models;
// 클라이언트가 현재 후보 중 하나를 선택할 때 서버로 보내는 요청 모델
public class SelectCandidateRequest
{
    // 선택하려는 사도의 ID
    // 반드시 현재 candidates 안에 포함된 사도여야 한다.
    public string ApostleId { get; set; } = "";

    // 다중 배치 사도일 때만 사용.
    // 예: "전열", "중열", "후열"
    public string? AssignedRow { get; set; }
}