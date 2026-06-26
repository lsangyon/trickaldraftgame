using System.Text.Json;
using SynergyDraft.Api.Models;

namespace SynergyDraft.Api.Services;
// apostles.json에 저장된 사도 데이터를 읽고 조회하는 저장소
public class ApostleRepository
{
     // 사용할 전체 사도 목록
    private readonly List<ApostleData> _apostles;

    public ApostleRepository()
    {
        // 실행 폴더 기준으로 Data/apostles.json 파일 경로를 생성
        var path = Path.Combine(AppContext.BaseDirectory, "Data", "apostles.json");

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"apostles.json 파일을 찾을 수 없습니다: {path}");
        }
        // 사도 데이터 파일 내용을 문자열로 읽는다.
        var json = File.ReadAllText(path);

        // JSON 데이터를 ApostleData 목록으로 변환한다.
        // PropertyNameCaseInsensitive를 true로 두어 JSON 필드명의 대소문자 차이를 허용한다.
        _apostles = JsonSerializer.Deserialize<List<ApostleData>>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        ) ?? new List<ApostleData>();
    }
    // 전체 사도 목록을 반환
    public IReadOnlyList<ApostleData> GetAll()
    {
        return _apostles;
    }
    // 사도 ID로 사도 1명을 검색
    // 없으면 null을 반환
    public ApostleData? FindById(string id)
    {
        return _apostles.FirstOrDefault(a => a.Id == id);
    }
    // 여러 사도 ID를 받아 해당하는 사도 목록을 반환
    // 선택된 사도나 후보 사도의 상세 정보를 만들 때 사용
    public List<ApostleData> FindByIds(IEnumerable<string> ids)
    {
        var map = _apostles.ToDictionary(a => a.Id);

        return ids
            .Where(id => map.ContainsKey(id))
            .Select(id => map[id])
            .ToList();
    }
}