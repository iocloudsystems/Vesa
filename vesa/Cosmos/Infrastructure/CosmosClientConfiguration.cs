using vesa.Cosmos.Abstractions;

namespace vesa.Cosmos.Infrastructure;

public class CosmosClientConfiguration : ICosmosClientConfiguration
{
    public CosmosClientConfiguration()
    {
    }

    public CosmosClientConfiguration(string urlKey, string authKey) : this()
    {
        UrlKey = urlKey;
        AuthKey = authKey;
    }

    public string UrlKey { get; set; }
    public string AuthKey { get; set; }
    public int MaxRetryAttemptsOnRateLimitedRequests { get; set; } = 9; // Microsoft Default Value
    public int MaxRetryWaitTimeOnRateLimitedRequestsInSeconds { get; set; } = 30; // Microsoft Default Value
}

public class CosmosClientConfiguration<TEntity> : CosmosClientConfiguration, ICosmosClientConfiguration<TEntity>
    where TEntity : class
{
    public CosmosClientConfiguration() : base()
    {
    }

    public CosmosClientConfiguration(string urlKey, string authKey)
        : base(urlKey, authKey)
    {
    }
}
