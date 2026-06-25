namespace SynergyDraft.Api.Models;

public class ApostleData
{
    public string Id { get; set; } = "";
    public string DisplayName { get; set; } = "";

    public string Species { get; set; } = "";
    public string Personality { get; set; } = "";

    public string Role { get; set; } = "";

    public List<string> Traits { get; set; } = new();
    public List<string> AvailableRows { get; set; } = new();

    public int Rarity { get; set; }
    public bool IsEldain { get; set; }

    public List<string> SpecialRules { get; set; } = new();
    public List<string> ChemistryTags { get; set; } = new();
}
