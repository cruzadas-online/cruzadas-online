using Cruzadas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cruzadas.Infrastructure.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("questions");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.QuizId)
            .IsRequired();

        builder.Property(q => q.Text)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(q => q.Explanation)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(q => q.SourceReference)
            .HasMaxLength(500);

        builder.Property(q => q.DisplayOrder)
            .IsRequired();

        builder.HasMany(q => q.Options)
            .WithOne()
            .HasForeignKey(o => o.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(q => q.Options)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
