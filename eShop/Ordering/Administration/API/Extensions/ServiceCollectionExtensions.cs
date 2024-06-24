using eShop.Ordering.Administration.Service.BuildAllStateViews;
using eShop.Ordering.Administration.Service.BuildStateViewInstance;
using eShop.Ordering.Administration.Service.BuildStateViewInstances;
using eShop.Ordering.Database.SQL.Context;
using eShop.Ordering.Database.SQL.Entities;
using eShop.Ordering.Inquiry.StateViews;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using vesa.Core.Abstractions;
using vesa.Core.Infrastructure;
using vesa.Cosmos.Extensions;
using vesa.Cosmos.Infrastructure;
using vesa.File.Infrastructure;
using vesa.SQL.Extensions;
using vesa.SQL.Infrastructure;
using vesa.File.Extensions;
using vesa.Blob.Extensions;

namespace eShop.Ordering.Administration.API.Extensions;

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
                services.AddScoped<IEventProcessor, EventProcessor>();
                break;
            case "SQL":
                services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
                services.AddSQLStore<OrderingContext>(configuration);
                services.AddTransient(typeof(IStateViewStore<OrderStateView>), typeof(SQLStateViewStore<OrderStateViewJson, OrderStateView>));
                services.AddTransient(typeof(IStateViewStore<CustomerOrdersStateView>), typeof(SQLStateViewStore<CustomerOrdersStateViewJson, CustomerOrdersStateView>));
                services.AddTransient(typeof(IStateViewStore<StatusOrdersStateView>), typeof(SQLStateViewStore<StatusOrdersStateViewJson, StatusOrdersStateView>));
                services.AddTransient(typeof(IStateViewStore<DailyOrdersStateView>), typeof(SQLStateViewStore<DailyOrdersStateViewJson, DailyOrdersStateView>));
                break;
            case "Cosmos":
                services.AddCosmosClient(configuration);
                //services.AddCosmosContainerConfiguration(configuration);
                services.AddCosmosContainerConfiguration<IStateView>(configuration, "StateViewCosmosContainerConfiguration");
                services.InitializeDatabase(configuration);
                services.AddTransient(typeof(IStateViewStore<>), typeof(CosmosStateViewStore<>));
                services.AddScoped<IEventProcessor, EventProcessor>();
                break;
        }

        // Query Handler registrations

        // Command Handler registrations
        services.AddTransient<IDomain<BuildAllStateViewsCommand>, BuildAllStateViewsDomain>();
        services.AddTransient<ICommandHandler<BuildAllStateViewsCommand>, BuildAllStateViewsHandler>();

        services.AddTransient<IDomain<BuildStateViewInstanceCommand>, BuildStateViewInstanceDomain>();
        services.AddTransient<ICommandHandler<BuildStateViewInstanceCommand>, BuildStateViewInstanceHandler>();

        services.AddTransient<IDomain<BuildStateViewInstancesCommand>, BuildStateViewInstancesDomain>();
        services.AddTransient<ICommandHandler<BuildStateViewInstancesCommand>, BuildStateViewInstancesHandler>();

        // State view builders
        services.AddTransient<IStateViewBuilder, StateViewBuilder>();
        services.AddTransient(typeof(IStateViewBuilder<>), typeof(StateViewBuilder<>));

        return services;
    }
}
