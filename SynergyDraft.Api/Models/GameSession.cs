namespace SynergyDraft.Api.Models;

public class GameSession
{
    public Guid GameId { get; set; } = Guid.NewGuid();

    public string ModeId { get; set; } = "";
    public string ModeName { get; set; } = "";

    public List<SelectedApostle> SelectedApostles { get; set; } = new();
    public List<string> CandidateApostleIds { get; set; } = new();

    public int CandidateCount { get; set; }
    public int MaxSelectionCount { get; set; }
    public int MaxPerRow { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsFinished => SelectedApostles.Count >= MaxSelectionCount;
}