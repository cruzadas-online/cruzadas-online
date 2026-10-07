using Cruzadas.Domain.Entities;
using Cruzadas.Domain.Exceptions;

namespace Cruzadas.Domain.Tests;

public class QuizAttemptTests
{
    private (Quiz Quiz, List<Question> Questions, List<AnswerOption> CorrectOptions, List<AnswerOption> IncorrectOptions) SetupQuizData()
    {
        var quizId = Guid.NewGuid();
        var quiz = new Quiz(
            quizId,
            "Fundamentos da Fé",
            "fundamentos-da-fe",
            "Descrição",
            true,
            2,
            DateTimeOffset.UtcNow);

        var questions = new List<Question>();
        var correctOptions = new List<AnswerOption>();
        var incorrectOptions = new List<AnswerOption>();

        for (int i = 1; i <= 2; i++)
        {
            var q = new Question(
                Guid.NewGuid(),
                quizId,
                $"Pergunta {i}",
                $"Explicação {i}",
                $"CIC {i}",
                i);

            var optCorrect = q.AddOption(Guid.NewGuid(), "Opção Correta", true, 1);
            var optWrong = q.AddOption(Guid.NewGuid(), "Opção Incorreta", false, 2);

            correctOptions.Add(optCorrect);
            incorrectOptions.Add(optWrong);
            questions.Add(q);
            quiz.AddQuestion(q);
        }

        return (quiz, questions, correctOptions, incorrectOptions);
    }

    [Fact]
    public void Complete_WhenAllAnswersCorrect_Calculates100Percent()
    {
        var (quiz, questions, correctOptions, _) = SetupQuizData();
        var attempt = quiz.StartAttempt();

        var answers = new Dictionary<Guid, Guid>
        {
            [questions[0].Id] = correctOptions[0].Id,
            [questions[1].Id] = correctOptions[1].Id,
        };

        attempt.Complete(answers, questions, DateTimeOffset.UtcNow);

        Assert.True(attempt.IsCompleted);
        Assert.Equal(2, attempt.TotalQuestions);
        Assert.Equal(2, attempt.CorrectAnswersCount);
        Assert.Equal(100.00m, attempt.ScorePercentage);
        Assert.NotNull(attempt.CompletedAt);
    }

    [Fact]
    public void Complete_WhenHalfAnswersCorrect_Calculates50Percent()
    {
        var (quiz, questions, correctOptions, incorrectOptions) = SetupQuizData();
        var attempt = quiz.StartAttempt();

        var answers = new Dictionary<Guid, Guid>
        {
            [questions[0].Id] = correctOptions[0].Id,
            [questions[1].Id] = incorrectOptions[1].Id,
        };

        attempt.Complete(answers, questions, DateTimeOffset.UtcNow);

        Assert.True(attempt.IsCompleted);
        Assert.Equal(2, attempt.TotalQuestions);
        Assert.Equal(1, attempt.CorrectAnswersCount);
        Assert.Equal(50.00m, attempt.ScorePercentage);
    }

    [Fact]
    public void Complete_WhenAlreadyCompleted_ThrowsAttemptAlreadyCompletedException()
    {
        var (quiz, questions, correctOptions, _) = SetupQuizData();
        var attempt = quiz.StartAttempt();

        var answers = new Dictionary<Guid, Guid>
        {
            [questions[0].Id] = correctOptions[0].Id,
            [questions[1].Id] = correctOptions[1].Id,
        };

        attempt.Complete(answers, questions, DateTimeOffset.UtcNow);

        Assert.Throws<AttemptAlreadyCompletedException>(() =>
            attempt.Complete(answers, questions, DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Complete_WhenQuestionDoesNotBelongToAttempt_ThrowsInvalidAttemptAnswerException()
    {
        var (quiz, questions, _, _) = SetupQuizData();
        var attempt = quiz.StartAttempt();

        var alienQuestionId = Guid.NewGuid();
        var answers = new Dictionary<Guid, Guid>
        {
            [alienQuestionId] = Guid.NewGuid()
        };

        Assert.Throws<InvalidAttemptAnswerException>(() =>
            attempt.Complete(answers, questions, DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Complete_WhenOptionDoesNotBelongToQuestion_ThrowsInvalidAttemptAnswerException()
    {
        var (quiz, questions, _, _) = SetupQuizData();
        var attempt = quiz.StartAttempt();

        var answers = new Dictionary<Guid, Guid>
        {
            [questions[0].Id] = Guid.NewGuid(), // Invalid option
            [questions[1].Id] = questions[1].Options.First().Id
        };

        Assert.Throws<InvalidAttemptAnswerException>(() =>
            attempt.Complete(answers, questions, DateTimeOffset.UtcNow));
    }
}
