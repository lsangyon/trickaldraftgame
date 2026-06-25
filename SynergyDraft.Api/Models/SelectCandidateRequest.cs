namespace SynergyDraft.Api.Models;

public class SelectCandidateRequest
{
    public string ApostleId { get; set; } = "";

    // 다중 배치 사도일 때만 사용.
    // 예: "전열", "중열", "후열"
    public string? AssignedRow { get; set; }
}