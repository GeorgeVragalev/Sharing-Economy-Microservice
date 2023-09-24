using OrderDAL.Entity.Enums;
using OrderDAL.Repositories.Shared;

namespace OrderDAL.Repositories.Order;

public interface IOrderRepository : ICrudRepository<Entity.Order>
{
    Task<bool> ChangeOrderStatusAsync(int id, OrderStatus orderStatus);
}