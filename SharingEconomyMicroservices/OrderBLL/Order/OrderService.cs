using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderBLL.Http;
using OrderBLL.Models;
using OrderDAL.Entity.Enums;
using OrderDAL.Exceptions;
using OrderDAL.Repositories.Order;

namespace OrderBLL.Order;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IHttpService _httpService;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IOrderRepository orderRepository, IHttpService httpService, ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _httpService = httpService;
        _logger = logger;
    }

    public async Task<ResponseMessage> PlaceOrder(OrderDAL.Entity.Order order)
    {
        var json = JsonConvert.SerializeObject(order.ItemId, Formatting.Indented);

        var url = $"{GlobalConstants.InventoryUrl}/reserve";
        
        var response = await _httpService.Post(url, json);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning($"Failed to send post request to inventory api. {response.Content}");
            return new ResponseMessage()
            {
                Message = $"Failed to send post request to inventory api. {response.Content}"
            };
        }

        await _orderRepository.Insert(order);

        return new ResponseMessage()
        {
            Message = $"Successfully placed an order with id {order.Id}"
        };
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

    public async Task Update(OrderDAL.Entity.Order order)
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