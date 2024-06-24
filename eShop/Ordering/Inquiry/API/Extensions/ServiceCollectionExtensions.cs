using eShop.Ordering.Database.SQL.Context;
using eShop.Ordering.Database.SQL.Entities;
using eShop.Ordering.Inquiry.Service.GetCustomerOrders;
using eShop.Ordering.Inquiry.Service.GetDailyOrders;
using eShop.Ordering.Inquiry.Service.GetOrder;
using eShop.Ordering.Inquiry.Service.GetStatusOrders;
using eShop.Ordering.Inquiry.StateViews;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using vesa.Core.Abstractions;
using vesa.Core.Infrastructure;
using vesa.Cosmos.Extensions;
using vesa.Cosmos.Infrastructure;
using vesa.File.Extensions;
using vesa.File.Infrastructure;
using vesa.SQL.Extensions;
using vesa.SQL.Infrastructure;

namespace eShop.Ordering.Inquiry.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection Configure(this IServiceCollection services, IConfiguration configuration)
    {
        // settings will automatically be used by JsonConvert.SerializeObject/DeserializeObject
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        // Add EventStore
        services.AddFileEventStore(configuration);

        switch (configuration["StateViewStore"])
        {
            case "File":
                services.AddScoped(typeof(IStateViewStore<>), typeof(FileStateViewStore<>));
                services.AddScoped<IEventProcessor, EventProcessor>();
                break;
            case "SQL":
                services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
                services.AddSQLStore<OrderingContext>(configuration);
                services.AddTransient(typeof(IStateViewStore<OrderStateView>), typeof(SQLStateViewStore<OrderStateViewJson, OrderStateView>));
                services.AddScoped(typeof(IStateViewStore<CustomerOrdersStateView>), typeof(SQLStateViewStore<CustomerOrdersStateViewJson, CustomerOrdersStateView>));
                services.AddScoped(typeof(IStateViewStore<StatusOrdersStateView>), typeof(SQLStateViewStore<StatusOrdersStateViewJson, StatusOrdersStateView>));
                services.AddScoped(typeof(IStateViewStore<DailyOrdersStateView>), typeof(SQLStateViewStore<DailyOrdersStateViewJson, DailyOrdersStateView>));
                break;
            case "Cosmos":
                services.AddCosmosClient(configuration);
                //services.AddCosmosContainerConfiguration(configuration);
                services.AddCosmosContainerConfiguration<IStateView>(configuration, "StateViewCosmosContainerConfiguration");
                services.InitializeDatabase(configuration);
                services.AddScoped(typeof(IStateViewStore<>), typeof(CosmosStateViewStore<>));
                services.AddScoped<IEventProcessor, EventProcessor>();
                break;
        }

        // Query Handler registrations
        services.AddTransient<IQueryHandler<GetCustomerOrdersQuery, CustomerOrdersStateView>, GetCustomerOrdersHandler>();
        services.AddTransient<IQueryHandler<GetOrderQuery, OrderStateView>, GetOrderHandler>();
        services.AddTransient<IQueryHandler<GetStatusOrdersQuery, StatusOrdersStateView>, GetStatusOrdersHandler>();
        services.AddTransient<IQueryHandler<GetDailyOrdersQuery, DailyOrdersStateView>, GetDailyOrdersHandler>();

        return services;
    }
}
