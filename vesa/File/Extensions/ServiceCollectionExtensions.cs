using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using vesa.Core.Abstractions;
using vesa.Core.Infrastructure;
using vesa.File.Abstractions;
using vesa.File.Infrastructure;

namespace vesa.File.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFileSequenceNumberGenerator(this IServiceCollection services, IConfiguration configuration)
    {
        var sequenceNumberPath = configuration["SequenceNumberPath"];
        if (!@Directory.Exists(sequenceNumberPath))
        {
            Directory.CreateDirectory(sequenceNumberPath);
        }
        services.AddSingleton<ISequenceNumberGenerator>(_ => new FileSequenceNumberGenerator(configuration["SequenceNumberPath"]));

        return services;
    }

    public static IServiceCollection AddFileEventStore
    (
        this IServiceCollection services,
        IConfiguration configuration,
        ServiceLifetime serviceLifetime = ServiceLifetime.Scoped
    )
    {
        var eventStorePath = configuration["EventStorePath"];

        if (!@Directory.Exists(eventStorePath))
        {
            Directory.CreateDirectory(eventStorePath);
        }

        services.Add(new ServiceDescriptor
        (
            typeof(IEventStore),
            sp => new FileEventStore
            (
                configuration["EventStorePath"],
                sp.GetService<ILogger<FileEventStore>>()
            ),
            serviceLifetime
        ));

        services.Add(new ServiceDescriptor
        (
            typeof(IFileEventStore),
            sp => new FileEventStore
            (
                configuration["EventStorePath"],
                sp.GetService<ILogger<FileEventStore>>()
            ),
            serviceLifetime
        ));

        return services;
    }

    public static IServiceCollection AddFileEventListeners
    (
        this IServiceCollection services,
        IConfiguration configuration,
        ServiceLifetime serviceLifetime = ServiceLifetime.Singleton
    )
    {
        services.Add(new ServiceDescriptor(typeof(IEventListener), typeof(FileEventStoreListener), serviceLifetime));
        services.Add(new ServiceDescriptor(typeof(IEventProcessor), typeof(EventProcessor), serviceLifetime));
        //services.AddScoped<FileEventStoreListener>();
        //services.AddSingleton<IEventListener, FileEventHubListener>();
        //services.AddSingleton<FileEventHubListener>();
        return services;
    }

    public static IServiceCollection AddFileEventHub
    (
        this IServiceCollection services,
        IConfiguration configuration,
        ServiceLifetime serviceLifetime = ServiceLifetime.Singleton
    )
    {
        //services.AddSingleton<IEventConsumer>(_ => new FileEventConsumer(configuration["EventHubPath"], configuration["EventConsumerId"],));
        //services.AddSingleton<IFileEventConsumer>(_ => new FileEventConsumer(configuration["EventHubPath"], configuration["EventConsumerId"]));

        // Event Publisher
        services.Add(new ServiceDescriptor
        (
            typeof(IEventPublisher),
            sp => new FileEventPublisher(configuration["EventHubPath"]),
            serviceLifetime
        ));
        services.AddSingleton<IEventPublisher>(_ => new FileEventPublisher(configuration["EventHubPath"]));
        return services;
    }
}
