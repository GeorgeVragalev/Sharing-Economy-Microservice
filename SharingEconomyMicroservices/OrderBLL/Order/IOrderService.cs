using System.Linq.Expressions;
using OrderBLL.Models;
using OrderDAL.Entity.Enums;

namespace OrderBLL.Order;

public interface IOrderService
{
    Task<OrderDAL.Entity.Order?> GetById(int id);
    Task<IList<OrderDAL.Entity.Order>> GetAll();
    Task<IList<OrderDAL.Entity.Order>> GetFiltered(Expression<Func<OrderDAL.Entity.Order, bool>> filter);
    Task Insert(OrderDAL.Entity.Order order);
    Task Update(OrderDAL.Entity.Order order);
    Task Delete(int id);
    Task<ResponseMessage> PlaceOrder(OrderDAL.Entity.Order order);
    Task<bool> ChangeStatus(int id, OrderStatus status);
}