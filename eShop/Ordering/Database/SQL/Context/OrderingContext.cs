using eShop.Ordering.Database.SQL.Entities;
using eShop.Ordering.Database.SQL.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using vesa.Core.Infrastructure;
using vesa.SQL.Infrastructure;

namespace eShop.Ordering.Database.SQL.Context;

public class OrderingContext : DbContext
{
    private readonly DbContextOptions<OrderingContext> _options;

    public OrderingContext(DbContextOptions<OrderingContext> options) : base(options)
    {
        ChangeTracker.LazyLoadingEnabled = false;
        _options = options;
    }

    protected OrderingContext() : base()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new EventJsonConfiguration().Configure(modelBuilder.Entity<EventJson>());
        new OrderStateViewJsonConfiguration().Configure(modelBuilder.Entity<OrderStateViewJson>());
        new CustomerOrdersStateViewJsonConfiguration().Configure(modelBuilder.Entity<CustomerOrdersStateViewJson>());
        new StatusOrdersStateViewJsonConfiguration().Configure(modelBuilder.Entity<StatusOrdersStateViewJson>());
        new DailyOrdersStateViewJsonConfiguration().Configure(modelBuilder.Entity<DailyOrdersStateViewJson>());
    }
}
