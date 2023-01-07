using System.Collections.Concurrent;

namespace DataManagerAPI.Services;

public class LoggedOutUsersCollectionservice : ILoggedOutUsersCollectionService
{
    private readonly ConcurrentDictionary<int, bool> _collection = new ConcurrentDictionary<int, bool>();

    public bool Add(int userId)
    {
        return _collection.TryAdd(userId, true);
    }

    public bool Remove(int userId)
    {
        return _collection.TryRemove(userId, out bool _);
    }

    public bool Contains(int userId)
    {
        return _collection.TryGetValue(userId, out bool _);
    }
}
