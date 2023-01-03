using System.Collections.Concurrent;

namespace DataManagerAPI.Helpers
{
    public static class LoggedOutUsersCollection
    {
        private static readonly ConcurrentDictionary<int, bool> _collection = new ConcurrentDictionary<int, bool>();

        public static bool Add(int userId)
        {
            return _collection.TryAdd(userId, true);
        }

        public static bool Remove(int userId)
        {
            return _collection.TryRemove(userId, out bool _);
        }

        public static bool Contains(int userId)
        {
            return _collection.TryGetValue(userId, out bool _);
        }
    }
}
