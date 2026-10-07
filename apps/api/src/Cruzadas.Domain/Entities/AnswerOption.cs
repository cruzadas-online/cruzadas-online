namespace Cruzadas.Domain.Entities;

public class AnswerOption
{
    public Guid Id { get; private set; }
    public Guid QuestionId { get; private set; }
    public string Text { get; private set; } = string.Empty;
    public bool IsCorrect { get; private set; }
    public int DisplayOrder { get; private set; }

    // Required by EF Core
    private AnswerOption() { }

    public AnswerOption(Guid id, Guid questionId, string text, bool isCorrect, int displayOrder)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id não pode ser vazio.", nameof(id));
        if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("Texto da alternativa é obrigatório.", nameof(text));

        Id = id;
        QuestionId = questionId;
        Text = text.Trim();
        IsCorrect = isCorrect;
        DisplayOrder = displayOrder;
    }
}
