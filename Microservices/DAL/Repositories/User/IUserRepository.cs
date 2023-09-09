using DAL.Repositories.Shared;

namespace DAL.Repositories.User;

public interface IUserRepository : ICrudRepository<Entity.User>
{
    Task<IQueryable<Entity.User>> GetByName(string name);
}