using Cruzadas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cruzadas.Infrastructure.Configurations;

public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
{
    public void Configure(EntityTypeBuilder<Quiz> builder)
    {
        builder.ToTable("quizzes");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(q => q.Slug)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(q => q.Slug)
            .IsUnique();

        builder.Property(q => q.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(q => q.IsPublished)
            .IsRequired();

        builder.Property(q => q.QuestionsPerAttempt)
            .IsRequired();

        builder.Property(q => q.CreatedAt)
            .IsRequired();

        builder.Property(q => q.DifficultyLevel)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Iniciante");

        builder.HasOne(q => q.Group)
            .WithMany(g => g.Quizzes)
            .HasForeignKey(q => q.GroupId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(q => q.Questions)
            .WithOne()
            .HasForeignKey(q => q.QuizId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(q => q.Questions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
