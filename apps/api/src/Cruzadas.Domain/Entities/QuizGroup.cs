namespace Cruzadas.Domain.Entities;

public class QuizGroup
{
    private readonly List<Quiz> _quizzes = [];

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Icon { get; private set; } = string.Empty;
    public int DisplayOrder { get; private set; }

    public IReadOnlyCollection<Quiz> Quizzes => _quizzes.AsReadOnly();

    // Requerido pelo EF Core
    private QuizGroup() { }

    public QuizGroup(
        Guid id,
        string name,
        string slug,
        string description,
        string icon,
        int displayOrder)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id não pode ser vazio.", nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nome é obrigatório.", nameof(name));
        if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentException("Slug é obrigatório.", nameof(slug));

        Id = id;
        Name = name.Trim();
        Slug = slug.Trim().ToLowerInvariant();
        Description = description.Trim();
        Icon = string.IsNullOrWhiteSpace(icon) ? "cross" : icon.Trim();
        DisplayOrder = displayOrder;
    }

    public void AddQuiz(Quiz quiz)
    {
        if (quiz == null) throw new ArgumentNullException(nameof(quiz));
        _quizzes.Add(quiz);
        quiz.AssignToGroup(Id);
    }
}
