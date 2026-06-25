namespace SynergyDraft.Api.Models;

public class ChemistryTagData
{
    public string TagId { get; set; } = "";
    public string DisplayName { get; set; } = "";

    public int RequiredCount { get; set; }
    public int BonusScore { get; set; }

    public List<string> MemberIds { get; set; } = new();
}