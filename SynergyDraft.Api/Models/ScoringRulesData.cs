namespace SynergyDraft.Api.Models;
// scoringRules.json의 점수 계산 규칙을 C#에서 사용하기 위한 모델
public class ScoringRulesData
{
    //기본점수
    public int BaseScorePerApostle { get; set; }
    //성격보너스, 종족보너스
    public List<CountScoreRule> PersonalitySynergyScores { get; set; } = new();
    public List<CountScoreRule> SpeciesSynergyScores { get; set; } = new();
    //캐미스트리 보너스
    public List<RequiredCountScoreRule> ChemistryScores { get; set; } = new();
    //직업이 0명일경우 페널티
    public int MissingRolePenalty { get; set; }
    //과도하게 집중된경우 페널티
    public List<CountScoreRule> RoleOverConcentrationPenalties { get; set; } = new();
    //엘다인 보너스
    public int EldainBonusPerApostle { get; set; }
    public int EldainBonusMax { get; set; }
}
// 특정 개수 이상일 때 적용되는 점수 규칙
// 성격, 종족, 역할 과집중 규칙에서 사용한다.
public class CountScoreRule
{
    //해당되는 숫자와 그때의 점수계산
    public int Count { get; set; }
    public int Score { get; set; }
}
// 공식 케미때 필요한 인원 수가 정해진 점수 규칙
public class RequiredCountScoreRule
{
    //필요숫자와 그 점수
    public int RequiredCount { get; set; }
    public int Score { get; set; }
}