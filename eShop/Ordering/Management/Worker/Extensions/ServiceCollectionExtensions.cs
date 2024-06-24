using eShop.Ordering.Database.SQL.Context;
using eShop.Ordering.Database.SQL.Entities;
using eShop.Ordering.Inquiry.StateViews;
using eShop.Ordering.Management.Application.Abstractions;
using eShop.Ordering.Management.Application.Infrastructure;
using eShop.Ordering.Management.Events;
using eShop.Ordering.Management.Service.CancelOrder;
using eShop.Ordering.Management.Service.PlaceOrder;
using eShop.Ordering.Management.Service.ReorderStock;
using eShop.Ordering.Management.Service.ReturnOrder;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using vesa.Blob.Extensions;
using vesa.Core.Abstractions;
using vesa.Core.Infrastructure;
using vesa.Cosmos.Extensions;
using vesa.Cosmos.Infrastructure;
using vesa.EventHub.Extensions;
using vesa.File.Extensions;
using vesa.File.Infrastructure;
using vesa.Kafka.Extensions;
using vesa.SQL.Extensions;
using vesa.SQL.Infrastructure;

namespace eShop.Ordering.Management.Worker.Extensions;

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
                services.InitializeDatabase(configuration);
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

        switch (configuration["MessageHub"])
        {
            case "File":
                services.AddFileEventHub(configuration);
                break;
            case "Kafka":
                services.AddKafkaEventPublication(configuration);
                break;
            case "EventHub":
                //TODO
                services.AddEventHubPublication(configuration);
                //services.AddBlobContainerClient(configuration);
                //services.AddEventHubConsumption(configuration);
                break;

        }

        // Event Hub Consumers and Publishers Registration


        // Add Event Hub Publication
        //

        // Add Event Hub Consumption with CheckPointing and event processed check
        //
        //// Blob Storage client needed for CloudEvent processing
        //

        //// Add Event Hub Consumption Service with CheckPointing and event processed check
        //;

        // Registry Generic Factory
        services.AddTransient(typeof(IFactory<>), typeof(GenericFactory<>));

        // Domain and Command handlers

        // PlaceOrder Slice
        services.AddTransient<ICommandHandler<PlaceOrderCommand>, PlaceOrderHandler>();
        services.AddTransient<IDomain<PlaceOrderCommand>, PlaceOrderDomain>();

        // CancelOrder Slice
        services.AddTransient<ICommandHandler<CancelOrderCommand, OrderStateView>, CancelOrderHandler>();
        services.AddTransient<IDomain<CancelOrderCommand, OrderStateView>, CancelOrderDomain>();

        // ReturnOrder Slice
        services.AddTransient<ICommandHandler<ReturnOrderCommand, OrderStateView>, ReturnOrderHandler>();
        services.AddTransient<IDomain<ReturnOrderCommand, OrderStateView>, ReturnOrderDomain>();

        // ReorderStock Slice
        services.AddTransient<ICommandHandler<ReorderStockCommand>, ReorderStockHandler>();
        services.AddTransient<IDomain<ReorderStockCommand>, ReorderStockDomain>();

        // Event handlers
        services.AddSingleton<IEventHandler<OrderPlacedEvent>, EventPropagationHandler<OrderPlacedEvent, OrderStateView>>();
        services.AddSingleton<IEventHandler<OrderCancelledEvent>, EventPropagationHandler<OrderCancelledEvent, OrderStateView>>();
        services.AddSingleton<IEventHandler<OrderReturnedEvent>, EventPropagationHandler<OrderReturnedEvent, OrderStateView>>();
        services.AddSingleton<IEventHandler<OutOfStockExceptionEvent>, OutOfStockExceptionHandler>();
        services.AddSingleton<IEventHandler<StockReorderedEvent>, EventPublicationHandler<StockReorderedEvent>>();

        // Event observers
        services.AddSingleton<IEventObservers, EventHandlerObservers<OrderPlacedEvent>>();
        services.AddSingleton<IEventObservers, EventHandlerObservers<OrderCancelledEvent>>();
        services.AddSingleton<IEventObservers, EventHandlerObservers<OrderReturnedEvent>>();
        services.AddSingleton<IEventObservers, EventHandlerObservers<OutOfStockExceptionEvent>>();
        services.AddSingleton<IEventObservers, EventHandlerObservers<StockReorderedEvent>>();

        // We need the state views' Subject in order to write the events to a partition that the state view can be hydrated from
        services.AddSingleton(typeof(IEventPropagationService<>), typeof(EventPropagationService<>));

        // Mapping that OrderStateView is interested in OrderPlacedEvent, OrderCancelledEvent and OrderReturnedEvent
        services.AddTransient<IStateView<OrderPlacedEvent>, OrderStateView>();
        services.AddTransient<IStateView<OrderCancelledEvent>, OrderStateView>();
        services.AddTransient<IStateView<OrderReturnedEvent>, OrderStateView>();

        // Mapping that CustomerOrdersStateView is interested in OrderPlacedEvent, OrderCancelledEvent and OrderReturnedEvent
        services.AddTransient<IStateView<OrderPlacedEvent>, CustomerOrdersStateView>();
        services.AddTransient<IStateView<OrderCancelledEvent>, CustomerOrdersStateView>();
        services.AddTransient<IStateView<OrderReturnedEvent>, CustomerOrdersStateView>();

        // Mapping that StatusOrdersStateView is interested in OrderPlacedEvent, OrderCancelledEvent and OrderReturnedEvent
        services.AddTransient<IStateView<OrderPlacedEvent>, StatusOrdersStateView>();
        services.AddTransient<IStateView<OrderCancelledEvent>, StatusOrdersStateView>();
        services.AddTransient<IStateView<OrderReturnedEvent>, StatusOrdersStateView>();

        // Mapping that DailyOrdersStateView is interested in OrderPlacedEvent, OrderCancelledEvent and OrderReturnedEvent
        services.AddTransient<IStateView<OrderPlacedEvent>, DailyOrdersStateView>();
        services.AddTransient<IStateView<OrderCancelledEvent>, DailyOrdersStateView>();
        services.AddTransient<IStateView<OrderReturnedEvent>, DailyOrdersStateView>();

        // Business components
        services.AddScoped<IOrderNumberGenerator, OrderNumberGenerator>();
        services.AddScoped<IInventoryChecker, InventoryChecker>();
        services.AddScoped<IPaymentProcessor, PaymentProcessor>();
        services.AddScoped<IDeliveryScheduler, DeliveryScheduler>();

        return services;
    }
}