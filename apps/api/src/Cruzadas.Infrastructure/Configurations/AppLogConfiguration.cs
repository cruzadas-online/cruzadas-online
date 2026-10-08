using Cruzadas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cruzadas.Infrastructure.Configurations;

public class AppLogConfiguration : IEntityTypeConfiguration<AppLog>
{
    public void Configure(EntityTypeBuilder<AppLog> builder)
    {
        builder.ToTable("app_logs");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id)
            .ValueGeneratedOnAdd();

        builder.Property(l => l.Timestamp)
            .IsRequired();

        builder.Property(l => l.Level)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(l => l.Category)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(l => l.Message)
            .IsRequired();

        builder.Property(l => l.EventName)
            .HasMaxLength(128);

        builder.Property(l => l.TraceId)
            .HasMaxLength(128);

        builder.Property(l => l.PropertiesJson)
            .HasColumnType("text");

        builder.Property(l => l.Exception)
            .HasColumnType("text");

        // Indexes for fast querying by time, level, and event
        builder.HasIndex(l => l.Timestamp);
        builder.HasIndex(l => l.Level);
        builder.HasIndex(l => l.EventName);
    }
}
