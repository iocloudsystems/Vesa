namespace vesa.Cosmos.Abstractions;

public interface IChangeFeedProcessorConfiguration
{
    string ProcessorName { get; set; }
    string SourceDatabaseName { get; set; }
    string SourceContainerName { get; set; }
    string LeaseDatabaseName { get; set; }
    string LeaseContainerName { get; set; }
    DateTimeOffset? StartDateTimeOffset { get; set; }
}