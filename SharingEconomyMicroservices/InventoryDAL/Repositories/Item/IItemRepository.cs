using InventoryDAL.Entity.Enums;
using InventoryDAL.Repositories.Shared;

namespace InventoryDAL.Repositories.Item;

public interface IItemRepository : ICrudRepository<Entity.Item>
{
    Task<bool> ChangeItemStatusAsync(int itemId, Status status);
}