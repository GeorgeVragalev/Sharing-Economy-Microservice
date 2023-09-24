using System.Linq.Expressions;
using OrderDAL.Entity.Enums;

namespace OrderBLL.Order;

public interface IOrderService
{
    Task<OrderDAL.Entity.Order?> GetById(int id);
    Task<IList<OrderDAL.Entity.Order>> GetAll();
    Task<IList<OrderDAL.Entity.Order>> GetFiltered(Expression<Func<OrderDAL.Entity.Order, bool>> filter);
    Task Insert(OrderDAL.Entity.Order order);
    Task Update(int id, OrderDAL.Entity.Order order);
    Task Delete(int id);
    Task<bool> Reserve(int id);
    Task<bool> ChangeStatus(int id, OrderStatus status);
}