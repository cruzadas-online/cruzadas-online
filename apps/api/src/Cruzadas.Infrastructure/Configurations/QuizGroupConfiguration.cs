using Cruzadas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cruzadas.Infrastructure.Configurations;

public class QuizGroupConfiguration : IEntityTypeConfiguration<QuizGroup>
{
    public void Configure(EntityTypeBuilder<QuizGroup> builder)
    {
        builder.ToTable("quiz_groups");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(g => g.Slug)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(g => g.Slug)
            .IsUnique();

        builder.Property(g => g.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(g => g.Icon)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(g => g.DisplayOrder)
            .IsRequired();

        builder.HasMany(g => g.Quizzes)
            .WithOne(q => q.Group)
            .HasForeignKey(q => q.GroupId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Navigation(g => g.Quizzes)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
