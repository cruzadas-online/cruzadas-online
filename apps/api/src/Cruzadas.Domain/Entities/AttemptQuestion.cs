namespace Cruzadas.Domain.Entities;

public class AttemptQuestion
{
    public Guid Id { get; private set; }
    public Guid QuizAttemptId { get; private set; }
    public Guid QuestionId { get; private set; }
    public int Order { get; private set; }

    // Navigation property for EF Core
    public Question? Question { get; private set; }

    // Required by EF Core
    private AttemptQuestion() { }

    public AttemptQuestion(Guid id, Guid quizAttemptId, Guid questionId, int order)
    {
        Id = id;
        QuizAttemptId = quizAttemptId;
        QuestionId = questionId;
        Order = order;
    }
}
