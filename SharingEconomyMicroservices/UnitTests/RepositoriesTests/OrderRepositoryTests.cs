using InventoryDAL.Context;
using InventoryDAL.Entity;
using InventoryDAL.Entity.Enums;
using InventoryDAL.Repositories.Item;
using InventoryDAL.Repositories.Shared;
using Moq;
using OrderDAL.Context;
using OrderDAL.Entity;
using OrderDAL.Entity.Enums;
using OrderDAL.Repositories.Order;
using UnitTests.Tool;

namespace UnitTests.RepositoriesTests;

public class OrderRepositoryTests : TestWithOrderPostgres<OrderDbContext>
{
    private readonly OrderRepository _orderRepository;
    private readonly OrderDbContext _context;

    public OrderRepositoryTests()
    {
        _context = new OrderDbContext(Options);
        _orderRepository = new OrderRepository(new OrderDAL.Repositories.Shared.GenericRepository<Order>(_context));
    }

    [Fact]
    public async Task Insert_Item_Should_Succeed()
    {
        await _context.Database.EnsureCreatedAsync();

        var order = await InsertOrder();

        // Assert
        Assert.True(order.Id > 0);
    }

    [Fact]
    public async Task Get_Item_Should_Succeed()
    {
        await _context.Database.EnsureCreatedAsync();

        var order = await InsertOrder();
        
        var result = await _orderRepository.GetById(order.Id);

        // Assert
        Assert.True(result != null && result.Id > 0);
    }

    [Fact]
    public async Task Update_Item_Should_Succeed()
    {
        await _context.Database.EnsureCreatedAsync();

        var order = await InsertOrder();
        
        var updatedStatus = OrderStatus.Reserved;
        
        order.OrderStatus = updatedStatus;
        
        await _orderRepository.Update(order);
        
        var result = await _orderRepository.GetById(order.Id);

        // Assert
        Assert.Equal(result?.OrderStatus, updatedStatus);
    }

    [Fact] public async Task Delete_Item_Should_Succeed()
    {
        await _context.Database.EnsureCreatedAsync();

        var order = await InsertOrder();
        
        var result = await _orderRepository.GetById(order.Id);
        
        Assert.NotNull(result);
        
        await _orderRepository.Delete(order);
        
        var deleted = await _orderRepository.GetById(order.Id);

        // Assert
        Assert.Null(deleted);
    }

    private async Task<Order> InsertOrder()
    {
        var order = new Order()
        {
            Description = "Test Order",
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow,

            EndTime = DateTime.UtcNow.AddHours(2),
            OrderStatus = OrderStatus.Available,
            StartTime = DateTime.UtcNow,
            UserId = 1,
            ItemId = 1
        };

        await _orderRepository.Insert(order);
        return order;
    }
}