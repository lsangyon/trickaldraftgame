using System.Text.Json;
using SynergyDraft.Api.Models;

namespace SynergyDraft.Api.Services;

public class ApostleRepository
{
    private readonly List<ApostleData> _apostles;

    public ApostleRepository()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Data", "apostles.json");

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"apostles.json 파일을 찾을 수 없습니다: {path}");
        }

        var json = File.ReadAllText(path);

        _apostles = JsonSerializer.Deserialize<List<ApostleData>>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        ) ?? new List<ApostleData>();
    }

    public IReadOnlyList<ApostleData> GetAll()
    {
        return _apostles;
    }

    public ApostleData? FindById(string id)
    {
        return _apostles.FirstOrDefault(a => a.Id == id);
    }

    public List<ApostleData> FindByIds(IEnumerable<string> ids)
    {
        var map = _apostles.ToDictionary(a => a.Id);

        return ids
            .Where(id => map.ContainsKey(id))
            .Select(id => map[id])
            .ToList();
    }
}