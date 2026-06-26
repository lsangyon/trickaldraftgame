namespace SynergyDraft.Api.Models;
//드래프트옵션 설정을 C#에서 사용하기 위한 모델 
public class DraftOptions
{
    // 모드설정을 안했을때 기본 드래프트 모드
    public string DefaultModeId { get; set; } = "six";
    public List<DraftModeOptions> Modes { get; set; } = new();
}

public class DraftModeOptions
{
    //모드ID
    public string ModeId { get; set; } = "";
    //모드명
    public string DisplayName { get; set; } = "";
    //제시되는 사도수
    public int CandidateCount { get; set; } = 3;
    //뽑는 사도수
    public int MaxSelectionCount { get; set; } = 6;
    public int MaxPerRow { get; set; } = 3;
}