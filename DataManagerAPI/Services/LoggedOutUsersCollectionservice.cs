using StackExchange.Redis;
using System.Collections.Concurrent;

namespace DataManagerAPI.Services;

/// <summary>
/// Implementation of <see cref="ILoggedOutUsersCollectionService"/>.
/// </summary>
public class LoggedOutUsersCollectionservice : ILoggedOutUsersCollectionService
{
    // Local collection of user Ids.
    private readonly ConcurrentDictionary<int, bool> _collection = new ConcurrentDictionary<int, bool>();

    // Interface of Redis Database.
    private readonly IDatabase _redisDB;

    // token life time from configuration.
    private readonly int _dataLifetime;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="configuration"><see cref="IConfiguration"/></param>
    /// <param name="logger"><see cref="LoggedOutUsersCollectionservice"/></param>
    public LoggedOutUsersCollectionservice(IConfiguration configuration, ILogger<LoggedOutUsersCollectionservice> logger)
    {
        _dataLifetime = int.Parse(configuration["Tokens:AccessTokenLifetime"]!);    // take from configuration

        // connection string for Redis server
        string connectionString = configuration.GetConnectionString("Redis") ?? string.Empty;
        ConfigurationOptions options = ConfigurationOptions.Parse(connectionString);
        options.AbortOnConnectFail = false;

        // connect to Redis database.
        ConnectionMultiplexer connector = ConnectionMultiplexer.Connect(options);
        _redisDB = connector.GetDatabase();

        if (!_redisDB.IsConnected(default))
        {
            logger.LogWarning($"Redis is not accessible. Connection string: \"{connectionString}\"");
        }
    }

    /// <inheritdoc />
    public bool Add(int userId)
    {
        if (_redisDB.IsConnected(default))  // use Redis if it is accessible.
        {
            return _redisDB.StringSet(userId.ToString(), true, new TimeSpan(0, _dataLifetime, 0));
        }

        return _collection.TryAdd(userId, true);    // use local collection if Redis inaccessible.
    }

    /// <inheritdoc />
    public bool Remove(int userId)
    {
        if (_redisDB.IsConnected(default))  // use Redis if it is accessible.
        {
            return _redisDB.KeyDelete(userId.ToString());
        }

        return _collection.TryRemove(userId, out bool _);   // use local collection if Redis inaccessible.
    }

    /// <inheritdoc />
    public bool Contains(int userId)
    {
        if (_redisDB.IsConnected(default))  // use Redis if it is accessible.
        {
            return _redisDB.KeyExists(userId.ToString());
        }

        return _collection.TryGetValue(userId, out bool _); // use local collection if Redis inaccessible.
    }
}
