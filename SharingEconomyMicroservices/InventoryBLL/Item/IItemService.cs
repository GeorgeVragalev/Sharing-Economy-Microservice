namespace InventoryBLL.Item;

public interface IItemService
{
    Task<InventoryDAL.Entity.Item?> GetById(int id);
    Task Insert(InventoryDAL.Entity.Item item);
    Task Update(int id, InventoryDAL.Entity.Item item);
    Task Delete(int id);
}