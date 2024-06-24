using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using vesa.Core.Abstractions;
using vesa.Core.Infrastructure;
using vesa.Kafka.Abstractions;
using vesa.Kafka.Infrastructure;

namespace vesa.Kafka.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventMappings
    (
        this IServiceCollection services,
        IConfiguration configuration,
        ServiceLifetime serviceLifetime = ServiceLifetime.Singleton,
        string eventMappingsName = "EventMappings"
    )
    {
        var eventMappings = configuration.GetSection(eventMappingsName).Get<List<EventMapping>>();
        services.Add(new ServiceDescriptor(typeof(IEnumerable<IEventMapping>), sp => eventMappings, serviceLifetime));
        return services;
    }

    public static IServiceCollection AddKafkaEventConsumption
    (
        this IServiceCollection services,
        IConfiguration configuration,
        ServiceLifetime serviceLifetime = ServiceLifetime.Singleton,
        string kafkaConsumerConfigurationSectionName = "KafkaConsumerConfiguration"
    )
    {
        var kafkaConsumerConfiguration = configuration.GetSection(kafkaConsumerConfigurationSectionName).Get<KafkaConsumerConfiguration>();

        var config = new ConsumerConfig
        {
            BootstrapServers = kafkaConsumerConfiguration.BootstrapServers,
            GroupId = kafkaConsumerConfiguration.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            SaslPassword = configuration[kafkaConsumerConfiguration.SaslPassword],
            SaslUsername = configuration[kafkaConsumerConfiguration.SaslUsername],
            SaslMechanism = SaslMechanism.ScramSha256,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            EnableAutoCommit = false,
            EnableAutoOffsetStore = false
        };

        var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe(kafkaConsumerConfiguration.Topic);

        services.AddSingleton<IKafkaConsumerConfiguration>(kafkaConsumerConfiguration);
        services.AddSingleton(consumer);

        services.Add(new ServiceDescriptor(typeof(IEventListener), typeof(KafkaEventListener), serviceLifetime));
        services.Add(new ServiceDescriptor(typeof(IKafkaEventConsumer), typeof(KafkaEventConsumer), serviceLifetime));
        services.Add(new ServiceDescriptor(typeof(IEventProcessor), typeof(EventProcessor), serviceLifetime));

        return services;
    }


    public static IServiceCollection AddKafkaEventPublication
    (
        this IServiceCollection services,
        IConfiguration configuration,
        ServiceLifetime serviceLifetime = ServiceLifetime.Singleton,
        string kafkaPublisherConfigurationSectionName = "KafkaPublisherConfiguration"
    )
    {
        var kafkaPublisherConfiguration = configuration.GetSection(kafkaPublisherConfigurationSectionName).Get<KafkaPublisherConfiguration>();

        var config = new ProducerConfig
        {
            BootstrapServers = kafkaPublisherConfiguration.BootstrapServers,
            SaslPassword = configuration[kafkaPublisherConfiguration.SaslPassword],
            SaslUsername = configuration[kafkaPublisherConfiguration.SaslUsername],
            SaslMechanism = SaslMechanism.ScramSha256,
            SecurityProtocol = SecurityProtocol.SaslSsl

        };

        var producer = new ProducerBuilder<Ignore, string>(config)
            .SetValueSerializer(Serializers.Utf8)
            .SetKeySerializer(new IgnoreSerializer())
            .Build();


        services.AddSingleton<IKafkaPublisherConfiguration>(kafkaPublisherConfiguration);
        //services.AddSingleton(kafkaPublisherConfiguration);

        services.AddSingleton(producer);

        services.Add(new ServiceDescriptor(typeof(IEventPublisher), typeof(KafkaEventPublisher), serviceLifetime));

        return services;
    }
}
