using System.Linq.Expressions;

namespace InventoryBLL;

public interface IItemService
{
    Task<InventoryDAL.Entity.Item?> GetById(int id);
    Task<IList<InventoryDAL.Entity.Item>> GetAll();
    Task<IList<InventoryDAL.Entity.Item>> GetFiltered(Expression<Func<InventoryDAL.Entity.Item, bool>> filter);
    Task Insert(InventoryDAL.Entity.Item item);
    Task Update(int id, InventoryDAL.Entity.Item item);
    Task Delete(int id);
    Task<bool> Reserve(int id);
}