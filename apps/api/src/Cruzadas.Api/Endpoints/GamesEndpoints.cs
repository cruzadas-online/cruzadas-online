using Cruzadas.Application.DTOs;
using Cruzadas.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cruzadas.Api.Endpoints;

public static class GamesEndpoints
{
    public static IEndpointRouteBuilder MapGamesEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/v1/games")
            .WithTags("Games");

        group.MapGet("/", async ([FromQuery] string? group, IQuizService quizService, CancellationToken ct) =>
        {
            var games = await quizService.GetGamesCatalogAsync(group, ct);
            return Results.Ok(games);
        })
        .WithName("GetGamesCatalog")
        .WithSummary("Lista o catálogo de jogos disponíveis e futuros do Cruzadas.online, com filtro opcional por grupo")
        .Produces<IReadOnlyList<GameItemDto>>(StatusCodes.Status200OK);

        group.MapGet("/groups", async (IQuizService quizService, CancellationToken ct) =>
        {
            var groups = await quizService.GetQuizGroupsAsync(ct);
            return Results.Ok(groups);
        })
        .WithName("GetQuizGroups")
        .WithSummary("Lista os grupos e categorias temáticas de quiz")
        .Produces<IReadOnlyList<QuizGroupDto>>(StatusCodes.Status200OK);

        return routes;
    }
}
