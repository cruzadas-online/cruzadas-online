using System.Net;
using System.Net.Http.Json;
using Cruzadas.Application.DTOs;

namespace Cruzadas.Api.IntegrationTests;

public class ApiEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ApiEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsHealthy()
    {
        var response = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Healthy", content);
    }

    [Fact]
    public async Task GetGamesCatalog_ReturnsOkWithPublishedGames()
    {
        var response = await _client.GetAsync("/api/v1/games");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var games = await response.Content.ReadFromJsonAsync<List<GameItemDto>>();
        Assert.NotNull(games);
        Assert.Contains(games, g => g.Slug == "fundamentos-da-fe" && g.IsAvailable);
        Assert.Contains(games, g => !g.IsAvailable);
    }

    [Fact]
    public async Task GetQuizBySlug_WhenExists_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/v1/quizzes/fundamentos-da-fe");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var quiz = await response.Content.ReadFromJsonAsync<QuizDetailDto>();
        Assert.NotNull(quiz);
        Assert.Equal("fundamentos-da-fe", quiz.Slug);
        Assert.Equal("Quiz Católico — Fundamentos da Fé", quiz.Title);
    }

    [Fact]
    public async Task GetQuizBySlug_WhenNotFound_Returns404()
    {
        var response = await _client.GetAsync("/api/v1/quizzes/nao-existe");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetQuizBySlug_WhenNotPublished_Returns400()
    {
        var response = await _client.GetAsync("/api/v1/quizzes/quiz-rascunho");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task StartAttempt_WhenPublishedQuiz_Returns201AndAttemptWithQuestions()
    {
        var response = await _client.PostAsync("/api/v1/quizzes/fundamentos-da-fe/attempts", null);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var attempt = await response.Content.ReadFromJsonAsync<StartAttemptResponseDto>();
        Assert.NotNull(attempt);
        Assert.NotEqual(Guid.Empty, attempt.AttemptId);
        Assert.Equal(2, attempt.TotalQuestions);
        Assert.Equal(2, attempt.Questions.Count);
    }

    [Fact]
    public async Task CompleteFlow_StartAnswerAndCompleteQuiz_ReturnsScoresAndReviews()
    {
        // 1. Start attempt
        var startResponse = await _client.PostAsync("/api/v1/quizzes/fundamentos-da-fe/attempts", null);
        Assert.Equal(HttpStatusCode.Created, startResponse.StatusCode);
        var attempt = await startResponse.Content.ReadFromJsonAsync<StartAttemptResponseDto>();
        Assert.NotNull(attempt);

        // 2. Select first option for each question
        var answers = attempt.Questions.ToDictionary(
            q => q.Id,
            q => q.Options.First().Id);

        var completeRequest = new SubmitAnswersRequestDto(answers);

        // 3. Submit
        var completeResponse = await _client.PostAsJsonAsync(
            $"/api/v1/quizzes/fundamentos-da-fe/attempts/{attempt.AttemptId}/complete",
            completeRequest);

        Assert.Equal(HttpStatusCode.OK, completeResponse.StatusCode);

        var result = await completeResponse.Content.ReadFromJsonAsync<QuizResultDto>();
        Assert.NotNull(result);
        Assert.Equal(attempt.AttemptId, result.AttemptId);
        Assert.Equal(2, result.TotalQuestions);
        Assert.Equal(2, result.Questions.Count);
        Assert.InRange(result.ScorePercentage, 0m, 100m);

        // 4. Duplicate complete should return Conflict (409)
        var duplicateResponse = await _client.PostAsJsonAsync(
            $"/api/v1/quizzes/fundamentos-da-fe/attempts/{attempt.AttemptId}/complete",
            completeRequest);
        Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);
    }

    [Fact]
    public async Task CompleteAttempt_WhenAttemptNotFound_Returns404()
    {
        var request = new SubmitAnswersRequestDto(new Dictionary<Guid, Guid>());
        var response = await _client.PostAsJsonAsync(
            $"/api/v1/quizzes/fundamentos-da-fe/attempts/{Guid.NewGuid()}/complete",
            request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
