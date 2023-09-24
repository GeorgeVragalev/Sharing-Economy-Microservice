using System.Linq.Expressions;
using InventoryDAL.Entity.Enums;

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
    Task<bool> ChangeStatus(int id, Status status);
}