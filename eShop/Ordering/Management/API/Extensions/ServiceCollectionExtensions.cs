using eShop.Ordering.Database.SQL.Context;
using eShop.Ordering.Database.SQL.Entities;
using eShop.Ordering.Inquiry.StateViews;
using eShop.Ordering.Management.Application.Abstractions;
using eShop.Ordering.Management.Application.Infrastructure;
using eShop.Ordering.Management.Service.CancelOrder;
using eShop.Ordering.Management.Service.PlaceOrder;
using eShop.Ordering.Management.Service.ReorderStock;
using eShop.Ordering.Management.Service.ReturnOrder;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using vesa.Blob.Extensions;
using vesa.Core.Abstractions;
using vesa.Cosmos.Extensions;
using vesa.Cosmos.Infrastructure;
using vesa.EventHub.Extensions;
using vesa.File.Extensions;
using vesa.File.Infrastructure;
using vesa.Kafka.Extensions;
using vesa.SQL.Extensions;
using vesa.SQL.Infrastructure;

namespace eShop.Ordering.Management.API.Extensions;

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

        // EventStore and EventStoreListener Registration
        switch (configuration["EventStore"])
        {
            case "File":
                services.AddFileEventStore(configuration);
                services.AddFileEventListeners(configuration);
                break;
            case "Blob":
                services.AddBlobEventStore(configuration);
                break;
            case "SQL":
                services.AddSQLStore<OrderingContext>(configuration);
                services.AddScoped<IEventStore, SQLEventStore>();
                break;
            case "Cosmos":
                services.AddCosmosEventStore(configuration);
                //services.AddCosmosEventStoreListener(configuration);
                break;
        }

        switch (configuration["StateViewStore"])
        {
            case "File":
                services.AddScoped(typeof(IStateViewStore<>), typeof(FileStateViewStore<>));
                break;
            case "SQL":
                services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
                if (configuration["EventStore"] != "SQL")
                {
                    services.AddSQLStore<OrderingContext>(configuration);
                }
                services.AddScoped(typeof(IStateViewStore<OrderStateView>), typeof(SQLStateViewStore<OrderStateViewJson, OrderStateView>));
                services.AddScoped(typeof(IStateViewStore<CustomerOrdersStateView>), typeof(SQLStateViewStore<CustomerOrdersStateViewJson, CustomerOrdersStateView>));
                services.AddScoped(typeof(IStateViewStore<StatusOrdersStateView>), typeof(SQLStateViewStore<StatusOrdersStateViewJson, StatusOrdersStateView>));
                services.AddScoped(typeof(IStateViewStore<DailyOrdersStateView>), typeof(SQLStateViewStore<DailyOrdersStateViewJson, DailyOrdersStateView>));
                break;
            case "Cosmos":
                if (configuration["EventStore"] != "Cosmos")
                {
                    services.AddCosmosClient(configuration);
                    services.AddCosmosContainerConfiguration(configuration, "EventCosmosContainerConfiguration");
                    services.InitializeDatabase(configuration);
                }
                services.AddCosmosContainerConfiguration<IStateView>(configuration, "StateViewCosmosContainerConfiguration");
                services.AddScoped(typeof(IStateViewStore<>), typeof(CosmosStateViewStore<>));
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
                services.AddEventHubConsumption(configuration);
                break;

        }

        // Domain and Handler registrations

        // Place Order Slice
        services.AddTransient<ICommandHandler<PlaceOrderCommand>, PlaceOrderHandler>();
        services.AddTransient<IDomain<PlaceOrderCommand>, PlaceOrderDomain>();

        // Cancel Order Slice
        services.AddTransient<ICommandHandler<CancelOrderCommand, OrderStateView>, CancelOrderHandler>();
        services.AddTransient<IDomain<CancelOrderCommand, OrderStateView>, CancelOrderDomain>();

        // Return Order Slice
        services.AddTransient<ICommandHandler<ReturnOrderCommand, OrderStateView>, ReturnOrderHandler>();
        services.AddTransient<IDomain<ReturnOrderCommand, OrderStateView>, ReturnOrderDomain>();


        // Reorder Slice
        services.AddTransient<ICommandHandler<ReorderStockCommand>, ReorderStockHandler>();
        services.AddTransient<IDomain<ReorderStockCommand>, ReorderStockDomain>();

        // Business components
        services.AddScoped<IOrderNumberGenerator, OrderNumberGenerator>();
        services.AddScoped<IInventoryChecker, InventoryChecker>();
        services.AddScoped<IPaymentProcessor, PaymentProcessor>();
        services.AddScoped<IDeliveryScheduler, DeliveryScheduler>();

        return services;
    }
}
