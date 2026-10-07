using Cruzadas.Application.DTOs;
using Cruzadas.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cruzadas.Api.Endpoints;

public static class QuizEndpoints
{
    public static IEndpointRouteBuilder MapQuizEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/v1/quizzes")
            .WithTags("Quizzes");

        group.MapGet("/{slug}", async (string slug, IQuizService quizService, CancellationToken ct) =>
        {
            var quiz = await quizService.GetQuizBySlugAsync(slug, ct);
            return Results.Ok(quiz);
        })
        .WithName("GetQuizBySlug")
        .WithSummary("Obtém informações do quiz pelo slug")
        .Produces<QuizDetailDto>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPost("/random/attempts", async (IQuizService quizService, CancellationToken ct) =>
        {
            var attempt = await quizService.StartRandomAttemptAsync(ct);
            return Results.Created($"/api/v1/quizzes/random/attempts/{attempt.AttemptId}", attempt);
        })
        .WithName("StartRandomQuizAttempt")
        .WithSummary("Inicia uma partida rápida aleatória de quiz entre todos os quizzes publicados")
        .Produces<StartAttemptResponseDto>(StatusCodes.Status201Created)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

        group.MapPost("/{slug}/attempts", async (string slug, IQuizService quizService, CancellationToken ct) =>
        {
            var attempt = await quizService.StartAttemptAsync(slug, ct);
            return Results.Created($"/api/v1/quizzes/{slug}/attempts/{attempt.AttemptId}", attempt);
        })
        .WithName("StartQuizAttempt")
        .WithSummary("Inicia uma nova partida de quiz para um jogador anônimo")
        .Produces<StartAttemptResponseDto>(StatusCodes.Status201Created)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPost("/{slug}/attempts/{attemptId:guid}/complete", async (
            string slug,
            Guid attemptId,
            [FromBody] SubmitAnswersRequestDto request,
            IQuizService quizService,
            CancellationToken ct) =>
        {
            var result = await quizService.CompleteAttemptAsync(slug, attemptId, request, ct);
            return Results.Ok(result);
        })
        .WithName("CompleteQuizAttempt")
        .WithSummary("Finaliza a partida, avalia as respostas e retorna o resultado detalhado com pontuação")
        .Produces<QuizResultDto>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
        .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity);

        return routes;
    }
}
