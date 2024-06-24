namespace vesa.Cosmos.Abstractions;

public interface ICosmosContainerConfiguration
{
    string DatabaseName { get; set; }
    string ContainerName { get; set; }
    string PartitionKeyPath { get; set; }
    string[] UniqueKeyPaths { get; set; }

}

public interface ICosmosContainerConfiguration<TEntity> : ICosmosContainerConfiguration
    where TEntity : class
{
}
