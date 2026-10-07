using Cruzadas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cruzadas.Infrastructure.Configurations;

public class QuizAttemptConfiguration : IEntityTypeConfiguration<QuizAttempt>
{
    public void Configure(EntityTypeBuilder<QuizAttempt> builder)
    {
        builder.ToTable("quiz_attempts");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .ValueGeneratedNever();

        builder.Property(a => a.QuizId)
            .IsRequired();

        builder.Property(a => a.StartedAt)
            .IsRequired();

        builder.Property(a => a.CompletedAt);

        builder.Property(a => a.TotalQuestions)
            .IsRequired();

        builder.Property(a => a.CorrectAnswersCount)
            .IsRequired();

        builder.Property(a => a.ScorePercentage)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(a => a.IsCompleted)
            .IsRequired();

        builder.HasMany(a => a.AttemptQuestions)
            .WithOne()
            .HasForeignKey(aq => aq.QuizAttemptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Answers)
            .WithOne()
            .HasForeignKey(aa => aa.QuizAttemptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(a => a.AttemptQuestions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(a => a.Answers)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class AttemptQuestionConfiguration : IEntityTypeConfiguration<AttemptQuestion>
{
    public void Configure(EntityTypeBuilder<AttemptQuestion> builder)
    {
        builder.ToTable("attempt_questions");

        builder.HasKey(aq => aq.Id);
        builder.Property(aq => aq.Id)
            .ValueGeneratedNever();

        builder.Property(aq => aq.QuizAttemptId)
            .IsRequired();

        builder.Property(aq => aq.QuestionId)
            .IsRequired();

        builder.Property(aq => aq.Order)
            .IsRequired();

        builder.HasOne(aq => aq.Question)
            .WithMany()
            .HasForeignKey(aq => aq.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class AttemptAnswerConfiguration : IEntityTypeConfiguration<AttemptAnswer>
{
    public void Configure(EntityTypeBuilder<AttemptAnswer> builder)
    {
        builder.ToTable("attempt_answers");

        builder.HasKey(aa => aa.Id);
        builder.Property(aa => aa.Id)
            .ValueGeneratedNever();

        builder.Property(aa => aa.QuizAttemptId)
            .IsRequired();

        builder.Property(aa => aa.QuestionId)
            .IsRequired();

        builder.Property(aa => aa.SelectedOptionId)
            .IsRequired();

        builder.Property(aa => aa.IsCorrect)
            .IsRequired();

        builder.Property(aa => aa.AnsweredAt)
            .IsRequired();
    }
}
