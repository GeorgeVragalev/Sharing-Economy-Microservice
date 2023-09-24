namespace InventoryBLL.Item;

public interface IItemService
{
    Task<InventoryDAL.Entity.Item?> GetById(int id);
    Task Insert(InventoryDAL.Entity.Item user);
    Task Update(int id, InventoryDAL.Entity.Item user);
    Task Delete(int id);
    Task<bool> GetByEmail(int id);
}