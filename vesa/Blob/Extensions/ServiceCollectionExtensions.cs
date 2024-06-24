using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using vesa.Blob.Abstractions;
using vesa.Blob.Infrastructure;
using vesa.Core.Abstractions;
using vesa.Core.Infrastructure;
using vesa.File.Infrastructure;

namespace vesa.Blob.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlobEventStore(this IServiceCollection services, IConfiguration configuration, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
    {
        services.AddSingleton<IStreamSerializer<IEvent>, TextEventStreamSerializer>();
        services.AddSingleton(typeof(IStreamSerializer<>), typeof(TextStateViewStreamSerializer<>));
        services.AddBlobServiceClient(configuration);
        services.Add(new ServiceDescriptor(typeof(IEventStore), typeof(BlobEventStore), serviceLifetime));
        return services;
    }

    public static IServiceCollection AddBlobEventStoreListener(this IServiceCollection services, IConfiguration configuration, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        services.Add(new ServiceDescriptor(typeof(IEventListener), typeof(BlobEventStoreListener), serviceLifetime));
        services.Add(new ServiceDescriptor(typeof(IEventConsumerBase), typeof(BlobEventConsumer), serviceLifetime));
        services.Add(new ServiceDescriptor(typeof(IEventProcessor), typeof(EventProcessor), serviceLifetime));
        return services;
    }

    public static IServiceCollection AddBlobServiceClient
    (
        this IServiceCollection services,
        IConfiguration configuration,
        string blobStorageConfigurationSectionName = "BlobStorageConfiguration"
    )
    {
        var blobStorageConfiguration = configuration.GetSection(blobStorageConfigurationSectionName).Get<BlobStorageConfiguration>();
        services.AddSingleton<IBlobStorageConfiguration>(blobStorageConfiguration);

        var blobClientOptions = new BlobClientOptions();
        blobClientOptions.Retry.MaxRetries = blobStorageConfiguration.MaxRetries;
        blobClientOptions.Retry.Delay = TimeSpan.FromSeconds(blobStorageConfiguration.DelayInSeconds);
        blobClientOptions.Diagnostics.IsLoggingEnabled = blobStorageConfiguration.IsLoggingEnabled;
        blobClientOptions.Diagnostics.ApplicationId = blobStorageConfiguration.ApplicationId;
        //blobClientOptions.AddPolicy(provider.GetService<TracingPolicy>(), HttpPipelinePosition.PerCall);
        var blobServiceClient = new BlobServiceClient(configuration[blobStorageConfiguration.ConnectionStringKey], blobClientOptions);
        services.AddSingleton(blobServiceClient);

        return services;
    }

    public static IServiceCollection AddBlobContainerClient
    (
        this IServiceCollection services,
        IConfiguration configuration,
        string blobContainerClientConfigurationSectionName = "BlobContainerClientConfiguration"
    )
    {
        var blobContainerClientConfiguration = configuration.GetSection(blobContainerClientConfigurationSectionName).Get<BlobContainerClientConfiguration>();
        services.AddSingleton<IBlobContainerClientConfiguration>(blobContainerClientConfiguration);

        var blobContainerClient = new BlobContainerClient(configuration[blobContainerClientConfiguration.ConnectionStringKey], blobContainerClientConfiguration.ContainerName);
        services.AddSingleton(blobContainerClient);

        return services;
    }
}
