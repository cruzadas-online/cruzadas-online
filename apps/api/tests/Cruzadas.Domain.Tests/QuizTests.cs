using Cruzadas.Domain.Entities;
using Cruzadas.Domain.Exceptions;

namespace Cruzadas.Domain.Tests;

public class QuizTests
{
    private Quiz CreateSampleQuiz(bool isPublished = true, int questionsPerAttempt = 2)
    {
        var quiz = new Quiz(
            Guid.NewGuid(),
            "Fundamentos da Fé",
            "fundamentos-da-fe",
            "Quiz introdutório",
            isPublished,
            questionsPerAttempt,
            DateTimeOffset.UtcNow);

        for (int i = 1; i <= 3; i++)
        {
            var q = new Question(
                Guid.NewGuid(),
                quiz.Id,
                $"Pergunta {i}?",
                $"Explicação {i}",
                $"Fonte {i}",
                i);

            q.AddOption(Guid.NewGuid(), "Opção A (Correta)", true, 1);
            q.AddOption(Guid.NewGuid(), "Opção B", false, 2);
            quiz.AddQuestion(q);
        }

        return quiz;
    }

    [Fact]
    public void StartAttempt_WhenPublished_CreatesAttemptWithConfiguredQuestions()
    {
        var quiz = CreateSampleQuiz(isPublished: true, questionsPerAttempt: 2);

        var attempt = quiz.StartAttempt();

        Assert.NotNull(attempt);
        Assert.Equal(quiz.Id, attempt.QuizId);
        Assert.Equal(2, attempt.TotalQuestions);
        Assert.False(attempt.IsCompleted);
    }

    [Fact]
    public void StartAttempt_WhenNotPublished_ThrowsQuizNotPublishedException()
    {
        var quiz = CreateSampleQuiz(isPublished: false);

        Assert.Throws<QuizNotPublishedException>(() => quiz.StartAttempt());
    }

    [Fact]
    public void PublishAndUnpublish_UpdatesIsPublishedState()
    {
        var quiz = CreateSampleQuiz(isPublished: false);
        Assert.False(quiz.IsPublished);

        quiz.Publish();
        Assert.True(quiz.IsPublished);

        quiz.Unpublish();
        Assert.False(quiz.IsPublished);
    }
}
