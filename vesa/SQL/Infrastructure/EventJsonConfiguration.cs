using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using vesa.Core.Infrastructure;

namespace vesa.SQL.Infrastructure;

public class EventJsonConfiguration : IEntityTypeConfiguration<EventJson>
{
    public void Configure(EntityTypeBuilder<EventJson> builder)
    {
        builder.ToTable(nameof(EventJson));
        builder.HasKey(o => new { o.Subject, o.Id });
        builder.Property(o => o.Id).HasMaxLength(36);
        builder.HasIndex(e => new { e.Subject, e.SequenceNumber }).IsUnique();
        builder.HasIndex(e => new { e.Subject, e.IdempotencyToken, e.EventTypeName }).IsUnique();
        builder.Property(e => e.Subject).HasMaxLength(100);
        builder.Property(e => e.EventTypeName).HasMaxLength(255);
        builder.Property(e => e.TriggeredBy).HasMaxLength(50);
        builder.Property(e => e.IdempotencyToken).HasMaxLength(36);
    }
}