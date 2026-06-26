using SynergyDraft.Api.Models;
using SynergyDraft.Api.Services;
// 웹 서버를 만들기 위한 기본 설정 객체
var builder = WebApplication.CreateBuilder(args);
// DraftOptions 항목을 DraftOptions 모델로 읽어오도록 등록
// 6명 모드, 9명 모드, 후보 수, 최대 선택 수 같은 설정을 가져올 때 사용한다.
builder.Services.Configure<DraftOptions>(
    builder.Configuration.GetSection("DraftOptions"));
// 서버에서 사용할 주요 서비스들을 등록한다.
// AddSingleton은 서버 실행 중 하나의 인스턴스를 계속 재사용한다.
builder.Services.AddSingleton<ApostleRepository>();
builder.Services.AddSingleton<ChemistryRepository>();
builder.Services.AddSingleton<ScoringRulesRepository>();
builder.Services.AddSingleton<ScoreService>();
builder.Services.AddSingleton<GameSessionService>();
// 위에서 등록한 설정과 서비스를 바탕으로 실제 웹 애플리케이션 생성
var app = builder.Build();

app.MapGet("/", () => "SynergyDraft API Server");

app.MapGet("/api/apostles", (ApostleRepository repo) =>
{
    return Results.Ok(repo.GetAll());
});

app.MapGet("/api/chemistry-tags", (ChemistryRepository repo) =>
{
    return Results.Ok(repo.GetAll());
});
// 새 드래프트 게임을 시작하는 API
// POST /api/games
// 요청 예시: { "modeId": "six" } 또는 { "modeId": "nine" }
app.MapPost("/api/games", (StartGameRequest? request, GameSessionService gameService) =>
{
    try
    {
        var game = gameService.StartGame(request?.ModeId);
        return Results.Ok(game);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new
        {
            message = ex.Message
        });
    }
});
// 현재 진행 중인 게임 상태를 조회하는 API
// GET /api/games/{gameId}
app.MapGet("/api/games/{gameId:guid}", (Guid gameId, GameSessionService gameService) =>
{
    var game = gameService.GetGame(gameId);

    if (game == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(game);
});
// 현재 후보 중 하나를 선택하는 API
// POST /api/games/{gameId}/select
// 요청 예시: { "apostleId": "AP019" }
app.MapPost("/api/games/{gameId:guid}/select",
    (Guid gameId, SelectCandidateRequest request, GameSessionService gameService) =>
{
    try
    {
        var game = gameService.SelectCandidate(gameId, request);

        if (game == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(game);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new
        {
            message = ex.Message
        });
    }
});

app.Run();