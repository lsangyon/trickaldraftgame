namespace SynergyDraft.Api.Models;
// 최종 점수 계산 결과의 세부 항목을 클라이언트에게 전달하기 위한 모델
public class ScoreBreakdown
{
    // 점수 항목의 큰 분류
    // 예: 기본 점수, 성격 시너지, 종족 시너지, 공식 케미, 역할 감점
    public string Category { get; set; } = "";
    // 점수 항목의 이름
    // 예: 선택 사도 수, 활발 2명 시너지, 엘다인 포함
    public string Name { get; set; } = "";
    // 해당 항목에서 더하거나 뺀 점수
    public int Score { get; set; }
    // 클라이언트나 결과 화면에 보여줄 점수 산정 이유
    public string Description { get; set; } = "";
}