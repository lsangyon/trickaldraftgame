using System.Text.Json;
using SynergyDraft.Api.Models;

namespace SynergyDraft.Api.Services;

public class ChemistryRepository
{
    private readonly List<ChemistryTagData> _chemistryTags;

    public ChemistryRepository()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Data", "chemistryTags.json");

        if (!File.Exists(path))
        {
            _chemistryTags = new List<ChemistryTagData>();
            return;
        }

        var json = File.ReadAllText(path);

        _chemistryTags = JsonSerializer.Deserialize<List<ChemistryTagData>>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        ) ?? new List<ChemistryTagData>();
    }

    public IReadOnlyList<ChemistryTagData> GetAll()
    {
        return _chemistryTags;
    }
}