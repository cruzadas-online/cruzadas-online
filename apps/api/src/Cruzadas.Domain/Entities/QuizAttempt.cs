using Cruzadas.Domain.Exceptions;

namespace Cruzadas.Domain.Entities;

public class QuizAttempt
{
    private readonly List<AttemptQuestion> _attemptQuestions = [];
    private readonly List<AttemptAnswer> _answers = [];

    public Guid Id { get; private set; }
    public Guid QuizId { get; private set; }
    public DateTimeOffset StartedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public int TotalQuestions { get; private set; }
    public int CorrectAnswersCount { get; private set; }
    public decimal ScorePercentage { get; private set; }
    public bool IsCompleted { get; private set; }

    public IReadOnlyCollection<AttemptQuestion> AttemptQuestions => _attemptQuestions.AsReadOnly();
    public IReadOnlyCollection<AttemptAnswer> Answers => _answers.AsReadOnly();

    // Required by EF Core
    private QuizAttempt() { }

    public QuizAttempt(Guid id, Guid quizId, DateTimeOffset startedAt, IEnumerable<Guid> questionIds)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id não pode ser vazio.", nameof(id));
        if (quizId == Guid.Empty) throw new ArgumentException("QuizId não pode ser vazio.", nameof(quizId));

        Id = id;
        QuizId = quizId;
        StartedAt = startedAt;
        IsCompleted = false;

        int order = 1;
        foreach (var qId in questionIds)
        {
            _attemptQuestions.Add(new AttemptQuestion(Guid.NewGuid(), Id, qId, order++));
        }

        TotalQuestions = _attemptQuestions.Count;
        if (TotalQuestions == 0)
        {
            throw new ArgumentException("Uma tentativa precisa ter ao menos uma questão.", nameof(questionIds));
        }
    }

    public void Complete(
        IReadOnlyDictionary<Guid, Guid> submittedAnswers,
        IReadOnlyCollection<Question> questions,
        DateTimeOffset completedAt)
    {
        if (IsCompleted)
        {
            throw new AttemptAlreadyCompletedException(Id);
        }

        var attemptQuestionIds = _attemptQuestions.Select(aq => aq.QuestionId).ToHashSet();
        var questionLookup = questions.ToDictionary(q => q.Id);

        _answers.Clear();
        int correctCount = 0;

        foreach (var (questionId, selectedOptionId) in submittedAnswers)
        {
            if (!attemptQuestionIds.Contains(questionId))
            {
                throw new InvalidAttemptAnswerException($"A questão '{questionId}' não faz parte desta tentativa.");
            }

            if (!questionLookup.TryGetValue(questionId, out var question))
            {
                throw new InvalidAttemptAnswerException($"Questão '{questionId}' não encontrada.");
            }

            var selectedOption = question.Options.FirstOrDefault(o => o.Id == selectedOptionId);
            if (selectedOption == null)
            {
                throw new InvalidAttemptAnswerException($"A alternativa '{selectedOptionId}' não pertence à questão '{questionId}'.");
            }

            bool isCorrect = selectedOption.IsCorrect;
            if (isCorrect)
            {
                correctCount++;
            }

            _answers.Add(new AttemptAnswer(
                Guid.NewGuid(),
                Id,
                questionId,
                selectedOptionId,
                isCorrect,
                completedAt));
        }

        CorrectAnswersCount = correctCount;
        ScorePercentage = TotalQuestions > 0
            ? Math.Round(((decimal)correctCount / TotalQuestions) * 100m, 2)
            : 0m;
        CompletedAt = completedAt;
        IsCompleted = true;
    }
}
