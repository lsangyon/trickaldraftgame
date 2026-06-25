using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using SynergyDraft.Api.Models;

namespace SynergyDraft.Api.Services;

public class GameSessionService
{
    private readonly ApostleRepository _apostleRepository;
    private readonly DraftOptions _options;

    private readonly ConcurrentDictionary<Guid, GameSession> _sessions = new();

    public GameSessionService(
        ApostleRepository apostleRepository,
        IOptions<DraftOptions> options)
    {
        _apostleRepository = apostleRepository;
        _options = options.Value;
    }

    public GameStateResponse StartGame(string? requestedModeId)
    {
        var mode = ResolveMode(requestedModeId);

        var session = new GameSession
        {
            ModeId = mode.ModeId,
            ModeName = mode.DisplayName,
            CandidateCount = mode.CandidateCount,
            MaxSelectionCount = mode.MaxSelectionCount,
            MaxPerRow = mode.MaxPerRow
        };

        GenerateCandidates(session);

        _sessions[session.GameId] = session;

        return ToResponse(session);
    }

    public GameStateResponse? GetGame(Guid gameId)
    {
        if (!_sessions.TryGetValue(gameId, out var session))
        {
            return null;
        }

        return ToResponse(session);
    }

    public GameStateResponse? SelectCandidate(Guid gameId, SelectCandidateRequest request)
    {
        if (!_sessions.TryGetValue(gameId, out var session))
        {
            return null;
        }

        if (session.IsFinished)
        {
            throw new InvalidOperationException("이미 종료된 게임입니다.");
        }

        if (!session.CandidateApostleIds.Contains(request.ApostleId))
        {
            throw new InvalidOperationException("현재 후보에 없는 사도는 선택할 수 없습니다.");
        }

        if (session.SelectedApostles.Any(s => s.ApostleId == request.ApostleId))
        {
            throw new InvalidOperationException("이미 선택한 사도입니다.");
        }

        var apostle = _apostleRepository.FindById(request.ApostleId);

        if (apostle == null)
        {
            throw new InvalidOperationException("존재하지 않는 사도입니다.");
        }

        var assignedRow = DecideAssignedRow(session, apostle, request.AssignedRow);

        session.SelectedApostles.Add(new SelectedApostle
        {
            ApostleId = request.ApostleId,
            AssignedRow = assignedRow
        });

        if (session.IsFinished)
        {
            session.CandidateApostleIds.Clear();
        }
        else
        {
            GenerateCandidates(session);
        }

        return ToResponse(session);
    }

    private DraftModeOptions ResolveMode(string? requestedModeId)
    {
        var modeId = string.IsNullOrWhiteSpace(requestedModeId)
            ? _options.DefaultModeId
            : requestedModeId;

        var mode = _options.Modes.FirstOrDefault(m => m.ModeId == modeId);

        if (mode == null)
        {
            throw new InvalidOperationException($"존재하지 않는 드래프트 모드입니다: {modeId}");
        }

        return mode;
    }

    private void GenerateCandidates(GameSession session)
    {
        var selectedIds = session.SelectedApostles
            .Select(s => s.ApostleId)
            .ToHashSet();

        var rowCounts = GetRowCounts(session);

        var pool = _apostleRepository
            .GetAll()
            .Where(a => !selectedIds.Contains(a.Id))
            .Where(a => CanPlaceApostle(session, a, rowCounts))
            .OrderBy(_ => Random.Shared.Next())
            .Take(session.CandidateCount)
            .Select(a => a.Id)
            .ToList();

        session.CandidateApostleIds = pool;
    }

    private Dictionary<string, int> GetRowCounts(GameSession session)
    {
        return new Dictionary<string, int>
        {
            ["전열"] = session.SelectedApostles.Count(s => s.AssignedRow == "전열"),
            ["중열"] = session.SelectedApostles.Count(s => s.AssignedRow == "중열"),
            ["후열"] = session.SelectedApostles.Count(s => s.AssignedRow == "후열")
        };
    }

    private bool CanPlaceApostle(
        GameSession session,
        ApostleData apostle,
        Dictionary<string, int> rowCounts)
    {
        foreach (var row in apostle.AvailableRows)
        {
            if (!rowCounts.ContainsKey(row))
            {
                continue;
            }

            if (rowCounts[row] < session.MaxPerRow)
            {
                return true;
            }
        }

        return false;
    }

    private string DecideAssignedRow(
        GameSession session,
        ApostleData apostle,
        string? requestedRow)
    {
        var rowCounts = GetRowCounts(session);

        var placeableRows = apostle.AvailableRows
            .Where(row => rowCounts.ContainsKey(row))
            .Where(row => rowCounts[row] < session.MaxPerRow)
            .ToList();

        if (placeableRows.Count == 0)
        {
            throw new InvalidOperationException("배치 가능한 열이 없습니다.");
        }

        if (apostle.AvailableRows.Count == 1)
        {
            return placeableRows[0];
        }

        if (string.IsNullOrWhiteSpace(requestedRow))
        {
            throw new InvalidOperationException("다중 배치 사도는 배치할 열을 선택해야 합니다.");
        }

        if (!apostle.AvailableRows.Contains(requestedRow))
        {
            throw new InvalidOperationException("해당 사도가 배치할 수 없는 열입니다.");
        }

        if (!rowCounts.ContainsKey(requestedRow))
        {
            throw new InvalidOperationException("존재하지 않는 배치 열입니다.");
        }

        if (rowCounts[requestedRow] >= session.MaxPerRow)
        {
            throw new InvalidOperationException("해당 열은 이미 최대 인원입니다.");
        }

        return requestedRow;
    }

    private GameStateResponse ToResponse(GameSession session)
    {
        var selectedIds = session.SelectedApostles
            .Select(s => s.ApostleId)
            .ToList();

        var selected = _apostleRepository.FindByIds(selectedIds);
        var candidates = _apostleRepository.FindByIds(session.CandidateApostleIds);

        return new GameStateResponse
        {
            GameId = session.GameId,
            ModeId = session.ModeId,
            ModeName = session.ModeName,
            CandidateCount = session.CandidateCount,
            MaxSelectionCount = session.MaxSelectionCount,
            SelectedCount = session.SelectedApostles.Count,
            RemainingSelectionCount = session.MaxSelectionCount - session.SelectedApostles.Count,
            IsFinished = session.IsFinished,
            SelectedApostles = selected,
            Candidates = candidates,
            RowCounts = GetRowCounts(session)
        };
    }
}