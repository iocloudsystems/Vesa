using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using vesa.Core.Abstractions;

namespace vesa.SQL.Infrastructure;

public abstract class StateViewJsonConfiguration<TStateViewJson, TStateView> : IEntityTypeConfiguration<TStateViewJson>
    where TStateViewJson : StateViewJson<TStateView>
    where TStateView : class, IStateView
{
    public virtual void Configure(EntityTypeBuilder<TStateViewJson> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasMaxLength(36);
        builder.HasAlternateKey(o => o.Subject);
        builder.Property(o => o.Subject).HasMaxLength(100);
    }
}