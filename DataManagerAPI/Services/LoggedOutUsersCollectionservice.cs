using StackExchange.Redis;
using System.Collections.Concurrent;

namespace DataManagerAPI.Services;

public class LoggedOutUsersCollectionservice : ILoggedOutUsersCollectionService
{
    private readonly ConcurrentDictionary<int, bool> _collection = new ConcurrentDictionary<int, bool>();
    private readonly IDatabase _redisDB;
    private readonly int _dataLifetime;

    public LoggedOutUsersCollectionservice(IConfiguration configuration, ILogger<LoggedOutUsersCollectionservice> logger)
    {
        _dataLifetime = int.Parse(configuration["Tokens:AccessTokenLifetime"]!);

        string connectionString = configuration.GetConnectionString("Redis") ?? string.Empty;
        ConfigurationOptions options = ConfigurationOptions.Parse(connectionString);
        options.AbortOnConnectFail = false;

        ConnectionMultiplexer connector = ConnectionMultiplexer.Connect(options);
        _redisDB = connector.GetDatabase();

        if (!_redisDB.IsConnected(default))
        {
            logger.LogWarning($"Redis is not accessible. Connection string: \"{connectionString}\"");
        }
    }

    public bool Add(int userId)
    {
        if (_redisDB.IsConnected(default))
        {
            return _redisDB.StringSet(userId.ToString(), true, new TimeSpan(0, _dataLifetime, 0));
        }

        return _collection.TryAdd(userId, true);
    }

    public bool Remove(int userId)
    {
        if (_redisDB.IsConnected(default))
        {
            return _redisDB.KeyDelete(userId.ToString());
        }

        return _collection.TryRemove(userId, out bool _);
    }

    public bool Contains(int userId)
    {
        if (_redisDB.IsConnected(default))
        {
            return _redisDB.KeyExists(userId.ToString());
        }

        return _collection.TryGetValue(userId, out bool _);
    }
}
