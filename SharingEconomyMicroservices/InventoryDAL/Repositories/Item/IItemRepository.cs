using InventoryDAL.Repositories.Shared;

namespace InventoryDAL.Repositories.Item;

public interface IItemRepository : ICrudRepository<Entity.Item>
{
    Task ReserveItemAsync(int itemId);
}