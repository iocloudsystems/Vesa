namespace vesa.SQL.Abstractions;

public interface IDbContextConfiguration
{
    string ConnectionStringKey { get; set; }
    int RetryCount { get; set; }
    int RetryAttemptIntervalMaxCapInSeconds { get; set; }
    int TimeoutInSeconds { get; set; }
}
