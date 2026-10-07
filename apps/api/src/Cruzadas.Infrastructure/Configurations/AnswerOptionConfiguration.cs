using Cruzadas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cruzadas.Infrastructure.Configurations;

public class AnswerOptionConfiguration : IEntityTypeConfiguration<AnswerOption>
{
    public void Configure(EntityTypeBuilder<AnswerOption> builder)
    {
        builder.ToTable("answer_options");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.QuestionId)
            .IsRequired();

        builder.Property(o => o.Text)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(o => o.IsCorrect)
            .IsRequired();

        builder.Property(o => o.DisplayOrder)
            .IsRequired();
    }
}
