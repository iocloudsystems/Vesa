using vesa.Core.Abstractions;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using System.Net;

namespace vesa.Core.Infrastructure;

public class RedisSequenceNumberGenerator : ISequenceNumberGenerator
{
    private readonly IDatabase _redisDatabase;
    private readonly RedLockFactory _redLockFactory;
    private const int LOCK_EXPIRY_IN_SECONDS = 3;   //TODO: read from configuration

    public RedisSequenceNumberGenerator(string connectionString)
    {
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(connectionString);
        _redisDatabase = redis.GetDatabase();

        var endpoints = new List<RedLockEndPoint>
        {
            new DnsEndPoint(redis.Configuration, (redis.GetEndPoints().First() as DnsEndPoint).Port)
        };

        _redLockFactory = RedLockFactory.Create(endpoints);
    }

    public async Task<long> GetNextSequenceNumberAsync(string sequenceName)
    {
        var redLock = _redLockFactory.CreateLock(sequenceName, TimeSpan.FromSeconds(LOCK_EXPIRY_IN_SECONDS));
        try
        {
            if (redLock.IsAcquired)
            {
                return _redisDatabase.StringIncrement(sequenceName);
            }
            else
            {
                throw new Exception($"Unable to acquire lock on sequence {sequenceName}");
            }
        }
        finally
        {
            if (redLock.IsAcquired)
            {
                redLock.Dispose();
            }
        }
    }
}