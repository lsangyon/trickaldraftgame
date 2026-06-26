namespace SynergyDraft.Api.Models;
// 진행 중인 드래프트 게임 1판의 상태를 저장하는 모델
public class GameSession
{
    //그 게임의 고유 ID(추후 랭킹에 적용)
    public Guid GameId { get; set; } = Guid.NewGuid();
    //모드 ID와 명칭
    public string ModeId { get; set; } = "";
    public string ModeName { get; set; } = "";
    //이미선택한 사도목록
    public List<SelectedApostle> SelectedApostles { get; set; } = new();
    //이제 선택할 수 있는 사도목록
    public List<string> CandidateApostleIds { get; set; } = new();
    //한라운드에 보여줄 사도수
    public int CandidateCount { get; set; }
    //최종선택 사도수
    public int MaxSelectionCount { get; set; }
    //한열당 최대 사도수
    public int MaxPerRow { get; set; }
    //게임세션이 생성된 시간
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    //선택한 사도수가, 목표사도수에 도달했는지 확인
    public bool IsFinished => SelectedApostles.Count >= MaxSelectionCount;
}