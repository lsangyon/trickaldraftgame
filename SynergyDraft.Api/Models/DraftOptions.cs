namespace SynergyDraft.Api.Models;

public class DraftOptions
{
    public string DefaultModeId { get; set; } = "six";
    public List<DraftModeOptions> Modes { get; set; } = new();
}

public class DraftModeOptions
{
    public string ModeId { get; set; } = "";
    public string DisplayName { get; set; } = "";

    public int CandidateCount { get; set; } = 3;
    public int MaxSelectionCount { get; set; } = 6;
    public int MaxPerRow { get; set; } = 3;
}