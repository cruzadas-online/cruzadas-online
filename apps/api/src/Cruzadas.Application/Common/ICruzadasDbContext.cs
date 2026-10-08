using Cruzadas.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cruzadas.Application.Common;

public interface ICruzadasDbContext
{
    DbSet<Quiz> Quizzes { get; }
    DbSet<QuizGroup> QuizGroups { get; }
    DbSet<Question> Questions { get; }
    DbSet<QuizAttempt> QuizAttempts { get; }
    DbSet<AttemptAnswer> AttemptAnswers { get; }
    DbSet<AppLog> AppLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
