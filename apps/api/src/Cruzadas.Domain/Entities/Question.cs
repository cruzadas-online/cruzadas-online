namespace Cruzadas.Domain.Entities;

public class Question
{
    private readonly List<AnswerOption> _options = [];

    public Guid Id { get; private set; }
    public Guid QuizId { get; private set; }
    public string Text { get; private set; } = string.Empty;
    public string Explanation { get; private set; } = string.Empty;
    public string? SourceReference { get; private set; }
    public int DisplayOrder { get; private set; }

    public IReadOnlyCollection<AnswerOption> Options => _options.AsReadOnly();

    // Required by EF Core
    private Question() { }

    public Question(
        Guid id,
        Guid quizId,
        string text,
        string explanation,
        string? sourceReference,
        int displayOrder)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id não pode ser vazio.", nameof(id));
        if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("Texto da questão é obrigatório.", nameof(text));
        if (string.IsNullOrWhiteSpace(explanation)) throw new ArgumentException("Explicação da questão é obrigatória.", nameof(explanation));

        Id = id;
        QuizId = quizId;
        Text = text.Trim();
        Explanation = explanation.Trim();
        SourceReference = sourceReference?.Trim();
        DisplayOrder = displayOrder;
    }

    public AnswerOption AddOption(Guid id, string text, bool isCorrect, int displayOrder)
    {
        var option = new AnswerOption(id, Id, text, isCorrect, displayOrder);
        _options.Add(option);
        return option;
    }

    public AnswerOption? GetCorrectOption()
    {
        return _options.FirstOrDefault(o => o.IsCorrect);
    }
}
