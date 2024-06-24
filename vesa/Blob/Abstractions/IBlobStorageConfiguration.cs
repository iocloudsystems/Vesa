namespace vesa.Blob.Abstractions;

public interface IBlobStorageConfiguration
{
    string ConnectionStringKey { get; set; }
    int MaxRetries { get; set; }
    int DelayInSeconds { get; set; }
    bool IsLoggingEnabled { get; set; }
    string ApplicationId { get; set; }
    int PollingIntervalInMilliseconds { get; set; }
}
