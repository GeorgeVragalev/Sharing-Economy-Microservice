using System.Linq.Expressions;
using InventoryDAL.Repositories.Item;
using Microsoft.EntityFrameworkCore;

namespace InventoryBLL.Item;

public class ItemService : IItemService
{
    private readonly IItemRepository _itemRepository;
    
    public ItemService(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task<InventoryDAL.Entity.Item?> GetById(int id)
    {
        if (id <= 0)
        {
            return null;
        }
        
        var item = await _itemRepository.GetById(id);

        return item;
    }

    public async Task<IList<InventoryDAL.Entity.Item>> GetAll()
    {
        return await _itemRepository.GetAll().Result.ToListAsync();
    }
    
    public async Task<IList<InventoryDAL.Entity.Item>> GetFiltered(Expression<Func<InventoryDAL.Entity.Item, bool>> filter)
    {
        return await _itemRepository.GetFiltered(filter).Result.ToListAsync();
    }

    public async Task Insert(InventoryDAL.Entity.Item item)
    {
        await _itemRepository.Insert(item);
    }

    public async Task Update(int id, InventoryDAL.Entity.Item item)
    {
        var itemInDb = await _itemRepository.GetById(id);
        await _itemRepository.Update(itemInDb ?? throw new InvalidOperationException());
    }

    public async Task Delete(int id)
    {
        var itemById = await _itemRepository.GetById(id);

        await _itemRepository.Delete(itemById ?? throw new InvalidOperationException());
    }

    public async Task<bool> Reserve(int id)
    {
        return await _itemRepository.ReserveItemAsync(id);
    }
}