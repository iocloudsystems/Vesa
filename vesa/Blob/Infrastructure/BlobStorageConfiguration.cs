using vesa.Blob.Abstractions;

namespace vesa.Blob.Infrastructure;

public class BlobStorageConfiguration : IBlobStorageConfiguration
{
    public string ConnectionStringKey { get; set; }
    public int MaxRetries { get; set; } = 5;
    public int DelayInSeconds { get; set; } = 3;
    public bool IsLoggingEnabled { get; set; }
    public string ApplicationId { get; set; }
    public int PollingIntervalInMilliseconds { get; set; } = 60000;
}
