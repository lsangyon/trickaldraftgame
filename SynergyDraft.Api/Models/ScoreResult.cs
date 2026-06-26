namespace SynergyDraft.Api.Models;
// 최종 점수 계산 결과를 클라이언트에게 전달하기 위한 모델
public class ScoreResult
{
    //전체점수, 등급, 코멘트 전송
    public int TotalScore { get; set; }
    public string Grade { get; set; } = "";
    public string Comment { get; set; } = "";

    //점수 계산식 보여주기
    public List<ScoreBreakdown> Breakdown { get; set; } = new();
}