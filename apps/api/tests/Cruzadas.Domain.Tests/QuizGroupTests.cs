using Cruzadas.Domain.Entities;

namespace Cruzadas.Domain.Tests;

public class QuizGroupTests
{
    [Fact]
    public void CreateQuizGroup_ValidArguments_InitializesCorrectly()
    {
        var group = new QuizGroup(
            Guid.NewGuid(),
            "Sagradas Escrituras",
            "sagradas-escrituras",
            "Quizzes sobre a Bíblia",
            "book",
            1);

        Assert.Equal("Sagradas Escrituras", group.Name);
        Assert.Equal("sagradas-escrituras", group.Slug);
        Assert.Equal("book", group.Icon);
        Assert.Equal(1, group.DisplayOrder);
        Assert.Empty(group.Quizzes);
    }

    [Fact]
    public void AddQuiz_AssignsQuizToGroup()
    {
        var group = new QuizGroup(
            Guid.NewGuid(),
            "Doutrina",
            "doutrina",
            "Quizzes de Doutrina",
            "church",
            1);

        var quiz = new Quiz(
            Guid.NewGuid(),
            "Fundamentos da Fé",
            "fundamentos-da-fe",
            "Descrição",
            true,
            10,
            DateTimeOffset.UtcNow,
            difficultyLevel: "Iniciante");

        group.AddQuiz(quiz);

        Assert.Single(group.Quizzes);
        Assert.Equal(group.Id, quiz.GroupId);
        Assert.Equal("Iniciante", quiz.DifficultyLevel);
    }
}
