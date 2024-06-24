using eShop.Ordering.Database.SQL.Entities;
using eShop.Ordering.Inquiry.StateViews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using vesa.SQL.Infrastructure;

namespace eShop.Ordering.Database.SQL.EntityConfigurations;

public class StatusOrdersStateViewJsonConfiguration : StateViewJsonConfiguration<StatusOrdersStateViewJson, StatusOrdersStateView>
{
    public override void Configure(EntityTypeBuilder<StatusOrdersStateViewJson> builder)
    {
        builder.ToTable(nameof(StatusOrdersStateViewJson));
        base.Configure(builder);
    }
}