namespace SynergyDraft.Api.Models;

public class GameStateResponse
{
    public Guid GameId { get; set; }

    public string ModeId { get; set; } = "";
    public string ModeName { get; set; } = "";

    public int CandidateCount { get; set; }
    public int MaxSelectionCount { get; set; }
    public int SelectedCount { get; set; }
    public int RemainingSelectionCount { get; set; }

    public bool IsFinished { get; set; }

    public List<ApostleData> SelectedApostles { get; set; } = new();
    public List<ApostleData> Candidates { get; set; } = new();

    public Dictionary<string, int> RowCounts { get; set; } = new();
}