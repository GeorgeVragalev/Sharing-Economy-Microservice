using DAL.Repositories.Shared;

namespace DAL.Repositories.User;

public class UserRepository : IUserRepository
{
    private readonly IGenericRepository<Entity.User> _genericRepository;

    public UserRepository(IGenericRepository<Entity.User> genericRepository)
    {
        _genericRepository = genericRepository;
    }
    
    public async Task<Entity.User?> GetById(int id)
    {
        return await _genericRepository.GetById(id);
    }

    public async Task<IQueryable<Entity.User>> GetAll()
    {
        return await _genericRepository.GetAll();
    }

    public Task<IQueryable<Entity.User>> GetByName(string name)
    {
        var query = _genericRepository.Table;
        return Task.FromResult(query.Where(it => string.Equals(it.Name, name)));
    }

    public async Task Insert(Entity.User item)
    {
        //clear cache
        //emit a event (EventBus)
        item.CreatedOnUtc = DateTime.UtcNow;
        item.UpdatedOnUtc = item.CreatedOnUtc;
        await _genericRepository.Insert(item);
    }

    public async Task Update(Entity.User item)
    {
        item.UpdatedOnUtc = DateTime.Now;
        await _genericRepository.Update(item);
    }

    public async Task Delete(Entity.User item)
    {
        await _genericRepository.Delete(item);
    }
}