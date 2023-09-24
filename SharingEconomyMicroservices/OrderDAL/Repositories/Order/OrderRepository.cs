using System.Linq.Expressions;
using OrderDAL.Entity.Enums;
using OrderDAL.Exceptions;
using OrderDAL.Repositories.Shared;

namespace OrderDAL.Repositories.Order;

public class OrderRepository : IOrderRepository
{
    private readonly IGenericRepository<Entity.Order> _genericRepository;

    public OrderRepository(IGenericRepository<Entity.Order> genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<Entity.Order?> GetById(int id)
    {
        return await _genericRepository.GetById(id);
    }

    public async Task<IQueryable<Entity.Order>> GetAll()
    {
        return await _genericRepository.GetAll();
    }

    public async Task<IQueryable<Entity.Order>> GetFiltered(Expression<Func<Entity.Order, bool>> filter)
    {
        return await _genericRepository.GetFiltered(filter);
    }

    public async Task Insert(Entity.Order order)
    {
        await _genericRepository.Insert(order);
    }

    public async Task Update(Entity.Order order)
    {
        await _genericRepository.Update(order);
    }

    public async Task Delete(Entity.Order order)
    {
        await _genericRepository.Delete(order);
    }

    public async Task<bool> DoesExist(Expression<Func<Entity.Order, bool>> filter)
    {
        return await _genericRepository.DoesExist(filter);
    }

    public async Task<bool> ChangeOrderStatusAsync(int id, OrderStatus orderStatus)
    {
        return await _genericRepository.ExecuteInTransactionAsync(async () =>
        {
            var order = await _genericRepository.GetById(id);

            if (order != null)
            {
                order.OrderStatus = orderStatus;
            }
            else
            {
                // Rollback will happen automatically if an exception is thrown
                throw new OrderNotFoundException($"Order: {id} was not found");
            }
        });
    }
}