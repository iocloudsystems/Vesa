using vesa.Cosmos.Abstractions;

namespace vesa.Cosmos.Infrastructure;

public class CosmosContainerConfiguration : ICosmosContainerConfiguration
{
    public CosmosContainerConfiguration()
    {
    }

    public CosmosContainerConfiguration(string databaseName, string containerName, string partitionKeyPath) : this()
    {
        DatabaseName = databaseName;
        ContainerName = containerName;
        PartitionKeyPath = partitionKeyPath;
    }

    public string DatabaseName { get; set; }
    public string ContainerName { get; set; }
    public string PartitionKeyPath { get; set; }
    public string[] UniqueKeyPaths { get; set; } = null;
}

public class CosmosContainerConfiguration<TItem> : CosmosContainerConfiguration, ICosmosContainerConfiguration<TItem>
    where TItem : class
{
    public CosmosContainerConfiguration() : base()
    {
    }

    public CosmosContainerConfiguration(string databaseName, string containerName, string partitionKeyPath)
        : base(databaseName, containerName, partitionKeyPath)
    {
    }
}