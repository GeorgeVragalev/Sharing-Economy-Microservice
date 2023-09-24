using InventoryDAL.Entity.Enums;
using InventoryDAL.Repositories.Shared;

namespace InventoryDAL.Repositories.Item;

public class ItemRepository : IItemRepository
{
    private readonly IGenericRepository<Entity.Item> _genericRepository;

    public ItemRepository(IGenericRepository<Entity.Item> genericRepository)
    {
        _genericRepository = genericRepository;
    }
    
    public async Task<Entity.Item?> GetById(int id)
    {
        return await _genericRepository.GetById(id);
    }

    public async Task<IQueryable<Entity.Item>> GetAll()
    {
        return await _genericRepository.GetAll();
    }

    public Task<bool> DoesExistByName(string name)
    {
        return Task.FromResult(_genericRepository.Table.Any(item => string.Equals(item.Name, name)));
    }

    public async Task Insert(Entity.Item item)
    {
        item.CreatedOnUtc = DateTime.UtcNow;
        item.UpdatedOnUtc = item.CreatedOnUtc;
        await _genericRepository.Insert(item);
    }

    public async Task Update(Entity.Item item)
    {
        item.UpdatedOnUtc = DateTime.Now;
        await _genericRepository.Update(item);
    }

    public async Task Delete(Entity.Item item)
    {
        await _genericRepository.Delete(item);
    }

    public async Task<bool> IsAvailable(int id)
    {
        var item = await GetById(id);

        return item?.Status == Status.Available;
    }
}