using eShop.Ordering.Database.SQL.Entities;
using eShop.Ordering.Inquiry.StateViews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using vesa.SQL.Infrastructure;

namespace eShop.Ordering.Database.SQL.EntityConfigurations;

public class DailyOrdersStateViewJsonConfiguration : StateViewJsonConfiguration<DailyOrdersStateViewJson, DailyOrdersStateView>
{
    public override void Configure(EntityTypeBuilder<DailyOrdersStateViewJson> builder)
    {
        builder.ToTable(nameof(DailyOrdersStateViewJson));
        base.Configure(builder);
    }
}