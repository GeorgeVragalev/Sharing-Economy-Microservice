using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OrderDAL.Entity.Enums;
using OrderDAL.Exceptions;
using OrderDAL.Repositories.Order;

namespace OrderBLL.Order;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<bool> Reserve(int id)
    {
        return await _orderRepository.ChangeOrderStatusAsync(id, OrderStatus.Reserved);
    }

    public async Task<bool> ChangeStatus(int id, OrderStatus status)
    {
        return await _orderRepository.ChangeOrderStatusAsync(id, status);
    }
    
    #region CRUD

    public async Task<OrderDAL.Entity.Order?> GetById(int id)
    {
        if (id <= 0)
        {
            return null;
        }
        
        var order = await _orderRepository.GetById(id);

        return order;
    }

    public async Task<IList<OrderDAL.Entity.Order>> GetAll()
    {
        return await _orderRepository.GetAll().Result.ToListAsync();
    }
    
    public async Task<IList<OrderDAL.Entity.Order>> GetFiltered(Expression<Func<OrderDAL.Entity.Order, bool>> filter)
    {
        return await _orderRepository.GetFiltered(filter).Result.ToListAsync();
    }

    public async Task Insert(OrderDAL.Entity.Order order)
    {
        await _orderRepository.Insert(order);
    }

    public async Task Update(int id, OrderDAL.Entity.Order order)
    {
        await _orderRepository.Update(order ?? throw new OrderNotFoundException());
    }

    public async Task Delete(int id)
    {
        var orderById = await _orderRepository.GetById(id);

        await _orderRepository.Delete(orderById ?? throw new OrderNotFoundException());
    }

    #endregion
}