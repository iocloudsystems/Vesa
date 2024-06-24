using eShop.Ordering.Database.SQL.Context;
using eShop.Ordering.Database.SQL.Entities;
using eShop.Ordering.Inquiry.Service.GetCustomerOrders;
using eShop.Ordering.Inquiry.Service.GetDailyOrders;
using eShop.Ordering.Inquiry.Service.GetOrder;
using eShop.Ordering.Inquiry.Service.GetStatusOrders;
using eShop.Ordering.Inquiry.StateViews;
using eShop.Ordering.Management.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using vesa.Blob.Extensions;
using vesa.Core.Abstractions;
using vesa.Core.Infrastructure;
using vesa.Cosmos.Extensions;
using vesa.Cosmos.Infrastructure;
using vesa.File.Extensions;
using vesa.File.Infrastructure;
using vesa.SQL.Extensions;
using vesa.SQL.Infrastructure;

namespace eShop.Ordering.Inquiry.Worker.Extensions;

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

        //Initialize stores - these creates the required filesystem directories on startup if they do not exist.

        // EventStore and EventStoreListener Registration
        switch (configuration["EventStore"])
        {
            case "File":
                services.AddFileEventStore(configuration);
                services.AddFileEventListeners(configuration);
                break;
            case "Blob":
                services.AddBlobEventStore(configuration, ServiceLifetime.Singleton);
                services.AddBlobEventStoreListener(configuration);
                break;
            case "SQL":
                services.AddSQLStore<OrderingContext>(configuration, ServiceLifetime.Singleton);
                services.AddSQLEventListeners(configuration);
                services.AddScoped<IEventStore, SQLEventStore>();
                break;
            case "Cosmos":
                services.AddCosmosEventStore(configuration, ServiceLifetime.Singleton);
                services.AddCosmosEventStoreListener(configuration);
                break;
        }

        switch (configuration["StateViewStore"])
        {
            case "File":
                services.AddSingleton(typeof(IStateViewStore<>), typeof(FileStateViewStore<>));
                break;
            case "SQL":
                services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
                if (configuration["EventStore"] != "SQL")
                {
                    services.AddSQLStore<OrderingContext>(configuration, ServiceLifetime.Scoped);
                }
                services.AddSingleton(typeof(IStateViewStore<OrderStateView>), typeof(SQLStateViewStore<OrderStateViewJson, OrderStateView>));
                services.AddSingleton(typeof(IStateViewStore<CustomerOrdersStateView>), typeof(SQLStateViewStore<CustomerOrdersStateViewJson, CustomerOrdersStateView>));
                services.AddSingleton(typeof(IStateViewStore<StatusOrdersStateView>), typeof(SQLStateViewStore<StatusOrdersStateViewJson, StatusOrdersStateView>));
                services.AddSingleton(typeof(IStateViewStore<DailyOrdersStateView>), typeof(SQLStateViewStore<DailyOrdersStateViewJson, DailyOrdersStateView>));
                break;
            case "Cosmos":
                if (configuration["EventStore"] != "Cosmos")
                {
                    services.AddCosmosClient(configuration);
                    services.AddCosmosContainerConfiguration(configuration);
                    services.InitializeDatabase(configuration);
                }
                services.AddCosmosStateViewStore(configuration, ServiceLifetime.Singleton);
                break;
        }

        // OrderStateView updaters
        services.AddSingleton<IEventHandler<OrderPlacedEvent>, OrderStateViewUpdater>();
        services.AddSingleton<IEventHandler<OrderCancelledEvent>, OrderStateViewUpdater>();
        services.AddSingleton<IEventHandler<OrderReturnedEvent>, OrderStateViewUpdater>();

        // CustomerOrdersStateView updaters
        services.AddSingleton<IEventHandler<OrderPlacedEvent>, CustomerOrdersStateViewUpdater>();
        services.AddSingleton<IEventHandler<OrderCancelledEvent>, CustomerOrdersStateViewUpdater>();
        services.AddSingleton<IEventHandler<OrderReturnedEvent>, CustomerOrdersStateViewUpdater>();

        // StatusOrdersStateView updaters
        services.AddSingleton<IEventHandler<OrderPlacedEvent>, StatusOrdersStateViewUpdater>();
        services.AddSingleton<IEventHandler<OrderCancelledEvent>, StatusOrdersStateViewUpdater>();
        services.AddSingleton<IEventHandler<OrderReturnedEvent>, StatusOrdersStateViewUpdater>();

        //DailyOrdersStateView updaters
        services.AddSingleton<IEventHandler<OrderPlacedEvent>, DailyOrdersStateViewUpdater>();
        services.AddSingleton<IEventHandler<OrderCancelledEvent>, DailyOrdersStateViewUpdater>();
        services.AddSingleton<IEventHandler<OrderReturnedEvent>, DailyOrdersStateViewUpdater>();

        // Event observers
        services.AddSingleton<IEventObservers, EventHandlerObservers<OrderPlacedEvent>>();
        services.AddSingleton<IEventObservers, EventHandlerObservers<OrderCancelledEvent>>();
        services.AddSingleton<IEventObservers, EventHandlerObservers<OrderReturnedEvent>>();

        return services;
    }
}