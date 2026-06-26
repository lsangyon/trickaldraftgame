using System.Text.Json;
using SynergyDraft.Api.Models;

namespace SynergyDraft.Api.Services;
// scoringRules.json에 저장된 점수 계산 규칙을 읽고 조회하는 저장소 클래스
public class ScoringRulesRepository
{
    private readonly ScoringRulesData _rules;

    public ScoringRulesRepository()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Data", "scoringRules.json");

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"scoringRules.json 파일을 찾을 수 없습니다: {path}");
        }

        var json = File.ReadAllText(path);

        _rules = JsonSerializer.Deserialize<ScoringRulesData>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        ) ?? new ScoringRulesData();
    }

    public ScoringRulesData GetRules()
    {
        return _rules;
    }
}