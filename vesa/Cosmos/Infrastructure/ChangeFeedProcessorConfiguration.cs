using vesa.Cosmos.Abstractions;

namespace vesa.Cosmos.Infrastructure;
public class ChangeFeedProcessorConfiguration : IChangeFeedProcessorConfiguration
{
    public string ProcessorName { get; set; }
    public string SourceDatabaseName { get; set; }
    public string SourceContainerName { get; set; }
    public string LeaseDatabaseName { get; set; }
    public string LeaseContainerName { get; set; }

    // Example: "2022/08/20 10:45:00 PM -05:00"
    public DateTimeOffset? StartDateTimeOffset { get; set; }
}
