using SynergyDraft.Api.Models;

namespace SynergyDraft.Api.Services;
// 선택된 사도 조합에 scoringRules.json의 점수 규칙을 적용하여
// 최종 점수, 등급, 평가 문구, 세부 점수 내역을 계산하는 서비스
public class ScoreService
{
    private readonly ScoringRulesRepository _scoringRulesRepository;
    private readonly ChemistryRepository _chemistryRepository;

    public ScoreService(
        ScoringRulesRepository scoringRulesRepository,
        ChemistryRepository chemistryRepository)
    {
        _scoringRulesRepository = scoringRulesRepository;
        _chemistryRepository = chemistryRepository;
    }

    public ScoreResult Calculate(IReadOnlyList<ApostleData> selectedApostles)
    {
        var rules = _scoringRulesRepository.GetRules();

        var breakdowns = new List<ScoreBreakdown>();
        var totalScore = 0;

        totalScore += AddBaseScore(selectedApostles, rules, breakdowns);
        totalScore += AddPersonalitySynergyScore(selectedApostles, rules, breakdowns);
        totalScore += AddSpeciesSynergyScore(selectedApostles, rules, breakdowns);
        totalScore += AddChemistryScore(selectedApostles, rules, breakdowns);
        totalScore += AddMissingRolePenalty(selectedApostles, rules, breakdowns);
        totalScore += AddRoleOverConcentrationPenalty(selectedApostles, rules, breakdowns);
        totalScore += AddEldainBonus(selectedApostles, rules, breakdowns);

        return new ScoreResult
        {
            TotalScore = totalScore,
            Grade = GetGrade(totalScore),
            Comment = GetComment(totalScore),
            Breakdown = breakdowns
        };
    }

    private int AddBaseScore(
        IReadOnlyList<ApostleData> selectedApostles,
        ScoringRulesData rules,
        List<ScoreBreakdown> breakdowns)
    {
        var score = selectedApostles.Count * rules.BaseScorePerApostle;

        breakdowns.Add(new ScoreBreakdown
        {
            Category = "기본 점수",
            Name = "선택 사도 수",
            Score = score,
            Description = $"사도 {selectedApostles.Count}명 × {rules.BaseScorePerApostle}점"
        });

        return score;
    }

    private int AddPersonalitySynergyScore(
        IReadOnlyList<ApostleData> selectedApostles,
        ScoringRulesData rules,
        List<ScoreBreakdown> breakdowns)
    {
        var total = 0;

        var groups = selectedApostles
            .GroupBy(a => a.Personality)
            .Where(g => !string.IsNullOrWhiteSpace(g.Key));

        foreach (var group in groups)
        {
            var count = group.Count();

            var matchedRule = rules.PersonalitySynergyScores
                .OrderByDescending(r => r.Count)
                .FirstOrDefault(r => count >= r.Count);

            if (matchedRule == null)
            {
                continue;
            }

            total += matchedRule.Score;

            breakdowns.Add(new ScoreBreakdown
            {
                Category = "성격 시너지",
                Name = $"{group.Key} {matchedRule.Count}명 시너지",
                Score = matchedRule.Score,
                Description = $"{group.Key} 성격 사도 {count}명"
            });
        }

        return total;
    }

    private int AddSpeciesSynergyScore(
        IReadOnlyList<ApostleData> selectedApostles,
        ScoringRulesData rules,
        List<ScoreBreakdown> breakdowns)
    {
        var total = 0;

        var groups = selectedApostles
            .GroupBy(a => a.Species)
            .Where(g => !string.IsNullOrWhiteSpace(g.Key));

        foreach (var group in groups)
        {
            var count = group.Count();

            var matchedRule = rules.SpeciesSynergyScores
                .OrderByDescending(r => r.Count)
                .FirstOrDefault(r => count >= r.Count);

            if (matchedRule == null)
            {
                continue;
            }

            total += matchedRule.Score;

            breakdowns.Add(new ScoreBreakdown
            {
                Category = "종족 시너지",
                Name = $"{group.Key} {matchedRule.Count}명 시너지",
                Score = matchedRule.Score,
                Description = $"{group.Key} 종족 사도 {count}명"
            });
        }

        return total;
    }

    private int AddChemistryScore(
        IReadOnlyList<ApostleData> selectedApostles,
        ScoringRulesData rules,
        List<ScoreBreakdown> breakdowns)
    {
        var total = 0;

        var selectedIds = selectedApostles
            .Select(a => a.Id)
            .ToHashSet();

        foreach (var chemistry in _chemistryRepository.GetAll())
        {
            var matchedCount = chemistry.MemberIds
                .Count(id => selectedIds.Contains(id));

            if (matchedCount < chemistry.RequiredCount)
            {
                continue;
            }

            var matchedRule = rules.ChemistryScores
                .FirstOrDefault(r => r.RequiredCount == chemistry.RequiredCount);

            if (matchedRule == null)
            {
                continue;
            }

            total += matchedRule.Score;

            breakdowns.Add(new ScoreBreakdown
            {
                Category = "공식 케미",
                Name = chemistry.DisplayName,
                Score = matchedRule.Score,
                Description = $"{chemistry.RequiredCount}인 케미 발동"
            });
        }

        return total;
    }

    private int AddMissingRolePenalty(
        IReadOnlyList<ApostleData> selectedApostles,
        ScoringRulesData rules,
        List<ScoreBreakdown> breakdowns)
    {
        var total = 0;

        var requiredRoles = new[] { "탱커", "딜러", "서포터" };

        foreach (var role in requiredRoles)
        {
            var exists = selectedApostles.Any(a => a.Role == role);

            if (exists)
            {
                continue;
            }

            total += rules.MissingRolePenalty;

            breakdowns.Add(new ScoreBreakdown
            {
                Category = "역할 감점",
                Name = $"{role} 없음",
                Score = rules.MissingRolePenalty,
                Description = $"{role} 역할 사도가 한 명도 없습니다."
            });
        }

        return total;
    }

    private int AddRoleOverConcentrationPenalty(
        IReadOnlyList<ApostleData> selectedApostles,
        ScoringRulesData rules,
        List<ScoreBreakdown> breakdowns)
    {
        var total = 0;

        var groups = selectedApostles
            .GroupBy(a => a.Role)
            .Where(g => !string.IsNullOrWhiteSpace(g.Key));

        foreach (var group in groups)
        {
            var count = group.Count();

            var matchedRule = rules.RoleOverConcentrationPenalties
                .OrderByDescending(r => r.Count)
                .FirstOrDefault(r => count >= r.Count);

            if (matchedRule == null)
            {
                continue;
            }

            total += matchedRule.Score;

            breakdowns.Add(new ScoreBreakdown
            {
                Category = "역할 과집중 감점",
                Name = $"{group.Key} {count}명",
                Score = matchedRule.Score,
                Description = $"{group.Key} 역할이 {count}명으로 과도하게 집중되었습니다."
            });
        }

        return total;
    }

    private int AddEldainBonus(
        IReadOnlyList<ApostleData> selectedApostles,
        ScoringRulesData rules,
        List<ScoreBreakdown> breakdowns)
    {
        var eldainCount = selectedApostles.Count(a => a.IsEldain);

        if (eldainCount == 0)
        {
            return 0;
        }

        var rawScore = eldainCount * rules.EldainBonusPerApostle;
        var score = Math.Min(rawScore, rules.EldainBonusMax);

        breakdowns.Add(new ScoreBreakdown
        {
            Category = "엘다인 보너스",
            Name = "엘다인 포함",
            Score = score,
            Description = $"엘다인 사도 {eldainCount}명, 최대 {rules.EldainBonusMax}점"
        });

        return score;
    }

    private string GetGrade(int totalScore)
    {
        if (totalScore >= 180) return "S";
        if (totalScore >= 140) return "A";
        if (totalScore >= 100) return "B";
        if (totalScore >= 70) return "C";
        return "D";
    }

    private string GetComment(int totalScore)
    {
        if (totalScore >= 180) return "최고급 시너지가 잘 맞은 조합입니다.";
        if (totalScore >= 140) return "강한 시너지를 가진 좋은 조합입니다.";
        if (totalScore >= 100) return "무난하게 완성된 조합입니다.";
        if (totalScore >= 70) return "일부 시너지가 부족한 조합입니다.";
        return "역할이나 시너지가 크게 부족한 조합입니다.";
    }
}