using DAL.Repositories.Shared;

namespace DAL.Repositories.User;

public interface IUserRepository : ICrudRepository<Entity.User>
{
    Task<Entity.User?> GetByName(string name);
}