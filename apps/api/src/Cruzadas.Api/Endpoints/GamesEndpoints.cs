using Cruzadas.Application.DTOs;
using Cruzadas.Application.Interfaces;

namespace Cruzadas.Api.Endpoints;

public static class GamesEndpoints
{
    public static IEndpointRouteBuilder MapGamesEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/v1/games")
            .WithTags("Games");

        group.MapGet("/", async (IQuizService quizService, CancellationToken ct) =>
        {
            var games = await quizService.GetGamesCatalogAsync(ct);
            return Results.Ok(games);
        })
        .WithName("GetGamesCatalog")
        .WithSummary("Lista o catálogo de jogos disponíveis e futuros do Cruzadas.online")
        .Produces<IReadOnlyList<GameItemDto>>(StatusCodes.Status200OK);

        return routes;
    }
}
