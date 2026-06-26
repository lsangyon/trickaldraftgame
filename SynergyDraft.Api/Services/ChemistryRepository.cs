using System.Text.Json;
using SynergyDraft.Api.Models;

namespace SynergyDraft.Api.Services;
// 공식 케미 데이터를 읽고 조회하는 저장소 클래스
public class ChemistryRepository
{
    private readonly List<ChemistryTagData> _chemistryTags;
    
    public ChemistryRepository()
    {
        //경로생성
        var path = Path.Combine(AppContext.BaseDirectory, "Data", "chemistryTags.json");

        if (!File.Exists(path))
        {
            _chemistryTags = new List<ChemistryTagData>();
            return;
        }

        var json = File.ReadAllText(path);
        //목록변환
        _chemistryTags = JsonSerializer.Deserialize<List<ChemistryTagData>>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        ) ?? new List<ChemistryTagData>();
    }
    //케미데이터 반환
    public IReadOnlyList<ChemistryTagData> GetAll()
    {
        return _chemistryTags;
    }
}