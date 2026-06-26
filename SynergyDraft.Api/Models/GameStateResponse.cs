namespace SynergyDraft.Api.Models;
// 현재 드래프트 게임 상태를 클라이언트에게 반환하기 위한 응답 모델
public class GameStateResponse
{
    //지금 게임ID
    public Guid GameId { get; set; }

    public string ModeId { get; set; } = "";
    public string ModeName { get; set; } = "";
    //후보수
    public int CandidateCount { get; set; }
    //최종선택수
    public int MaxSelectionCount { get; set; }
    //지금까지 뽑은수
    public int SelectedCount { get; set; }
    //남은 수
    public int RemainingSelectionCount { get; set; }
    //게임이 끝났는가
    public bool IsFinished { get; set; }
    //이미 뽑힌 사도들의 상세정보
    public List<ApostleData> SelectedApostles { get; set; } = new();
    //지금 후보로 올라온 사도들의 상세정보
    public List<ApostleData> Candidates { get; set; } = new();
    //전열 중열 후열에 몇명이 배치되어있는가
    public Dictionary<string, int> RowCounts { get; set; } = new();
    // 게임이 끝났을 때 최종 점수 결과를 응답에 포함한다.
    public ScoreResult? ScoreResult { get; set; }
}