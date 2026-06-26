namespace SynergyDraft.Api.Models;
// apostles.json의 사도 1명 정보를 C#에서 사용하기 위한 데이터 모델
public class ApostleData
{
    //사도들을 구분하기 위한 ID
    public string Id { get; set; } = "";
    //화면에 보여줄 이름
    public string DisplayName { get; set; } = "";

    //종족분류
    public string Species { get; set; } = "";
    //성격분류
    public string Personality { get; set; } = "";

    //역할분류
    public string Role { get; set; } = "";

    //특별한 특성이 있다면(힐러라던가)
    public List<string> Traits { get; set; } = new();
    public List<string> AvailableRows { get; set; } = new();

    public int Rarity { get; set; }
    //특수한 사도인 엘다인인지 여부 판별
    public bool IsEldain { get; set; }

    //우로스의 공명특성을 활용하기 위한 창
    public List<string> SpecialRules { get; set; } = new();
    //케미계산에 사용하는 것 
    public List<string> ChemistryTags { get; set; } = new();
}
