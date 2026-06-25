using SynergyDraft.Api.Models;
using SynergyDraft.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DraftOptions>(
    builder.Configuration.GetSection("DraftOptions"));

builder.Services.AddSingleton<ApostleRepository>();
builder.Services.AddSingleton<ChemistryRepository>();
builder.Services.AddSingleton<GameSessionService>();

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

app.MapGet("/api/games/{gameId:guid}", (Guid gameId, GameSessionService gameService) =>
{
    var game = gameService.GetGame(gameId);

    if (game == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(game);
});

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