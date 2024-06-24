using eShop.Ordering.Database.SQL.Entities;
using eShop.Ordering.Inquiry.StateViews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using vesa.SQL.Infrastructure;

namespace eShop.Ordering.Database.SQL.EntityConfigurations;

public class CustomerOrdersStateViewJsonConfiguration : StateViewJsonConfiguration<CustomerOrdersStateViewJson, CustomerOrdersStateView>
{
    public override void Configure(EntityTypeBuilder<CustomerOrdersStateViewJson> builder)
    {
        builder.ToTable(nameof(CustomerOrdersStateViewJson));
        base.Configure(builder);
    }
}