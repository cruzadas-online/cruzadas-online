using Cruzadas.Application.DTOs;

namespace Cruzadas.Application.Interfaces;

public interface IQuizService
{
    Task<IReadOnlyList<GameItemDto>> GetGamesCatalogAsync(CancellationToken cancellationToken = default);
    Task<QuizDetailDto> GetQuizBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<StartAttemptResponseDto> StartAttemptAsync(string slug, CancellationToken cancellationToken = default);
    Task<QuizResultDto> CompleteAttemptAsync(string slug, Guid attemptId, SubmitAnswersRequestDto request, CancellationToken cancellationToken = default);
}
