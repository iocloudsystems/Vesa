using vesa.SQL.Abstractions;

namespace vesa.SQL.Infrastructure;

public class DbContextConfiguration : IDbContextConfiguration
{
    public string ConnectionStringKey { get; set; }
    public int RetryCount { get; set; }
    public int RetryAttemptIntervalMaxCapInSeconds { get; set; }
    public int TimeoutInSeconds { get; set; }
}