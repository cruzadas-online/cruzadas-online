using Cruzadas.Application.Common;
using Cruzadas.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cruzadas.Infrastructure.Persistence;

public class CruzadasDbContext : DbContext, ICruzadasDbContext
{
    public CruzadasDbContext(DbContextOptions<CruzadasDbContext> options)
        : base(options)
    {
    }

    public DbSet<Quiz> Quizzes => Set<Quiz>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<AnswerOption> AnswerOptions => Set<AnswerOption>();
    public DbSet<QuizAttempt> QuizAttempts => Set<QuizAttempt>();
    public DbSet<AttemptQuestion> AttemptQuestions => Set<AttemptQuestion>();
    public DbSet<AttemptAnswer> AttemptAnswers => Set<AttemptAnswer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CruzadasDbContext).Assembly);
    }
}
