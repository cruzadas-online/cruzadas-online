namespace Cruzadas.Domain.Entities;

public class AttemptAnswer
{
    public Guid Id { get; private set; }
    public Guid QuizAttemptId { get; private set; }
    public Guid QuestionId { get; private set; }
    public Guid SelectedOptionId { get; private set; }
    public bool IsCorrect { get; private set; }
    public DateTimeOffset AnsweredAt { get; private set; }

    // Required by EF Core
    private AttemptAnswer() { }

    public AttemptAnswer(
        Guid id,
        Guid quizAttemptId,
        Guid questionId,
        Guid selectedOptionId,
        bool isCorrect,
        DateTimeOffset answeredAt)
    {
        Id = id;
        QuizAttemptId = quizAttemptId;
        QuestionId = questionId;
        SelectedOptionId = selectedOptionId;
        IsCorrect = isCorrect;
        AnsweredAt = answeredAt;
    }
}
