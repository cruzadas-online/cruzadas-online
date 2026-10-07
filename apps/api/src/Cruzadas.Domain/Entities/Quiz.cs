using Cruzadas.Domain.Exceptions;

namespace Cruzadas.Domain.Entities;

public class Quiz
{
    private readonly List<Question> _questions = [];

    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsPublished { get; private set; }
    public int QuestionsPerAttempt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();

    // Required by EF Core
    private Quiz() { }

    public Quiz(
        Guid id,
        string title,
        string slug,
        string description,
        bool isPublished,
        int questionsPerAttempt,
        DateTimeOffset createdAt)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id não pode ser vazio.", nameof(id));
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Título é obrigatório.", nameof(title));
        if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentException("Slug é obrigatório.", nameof(slug));
        if (questionsPerAttempt <= 0) throw new ArgumentOutOfRangeException(nameof(questionsPerAttempt), "Deve ser maior que zero.");

        Id = id;
        Title = title.Trim();
        Slug = slug.Trim().ToLowerInvariant();
        Description = description.Trim();
        IsPublished = isPublished;
        QuestionsPerAttempt = questionsPerAttempt;
        CreatedAt = createdAt;
    }

    public void AddQuestion(Question question)
    {
        if (question == null) throw new ArgumentNullException(nameof(question));
        _questions.Add(question);
    }

    public void Publish() => IsPublished = true;
    public void Unpublish() => IsPublished = false;

    public QuizAttempt StartAttempt(Random? random = null, DateTimeOffset? startedAt = null)
    {
        if (!IsPublished)
        {
            throw new QuizNotPublishedException(Slug);
        }

        if (_questions.Count == 0)
        {
            throw new InvalidOperationException("O quiz não possui perguntas cadastradas.");
        }

        var rng = random ?? Random.Shared;
        var selectedQuestionIds = _questions
            .OrderBy(_ => rng.Next())
            .Take(Math.Min(QuestionsPerAttempt, _questions.Count))
            .Select(q => q.Id)
            .ToList();

        return new QuizAttempt(
            Guid.NewGuid(),
            Id,
            startedAt ?? DateTimeOffset.UtcNow,
            selectedQuestionIds);
    }
}
