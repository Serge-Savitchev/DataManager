namespace DataManagerAPI.Services;

public interface ILoggedOutUsersCollectionService
{
    bool Add(int userId);
    bool Remove(int userId);
    bool Contains(int userId);
}
