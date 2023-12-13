using Microsoft.Extensions.Logging;
using OrderBLL.Http;
using OrderBLL.Order;
using OrderDAL.Context;
using OrderDAL.Entity;
using OrderDAL.Entity.Enums;
using OrderDAL.Repositories.Order;
using UnitTests.Tool;

namespace UnitTests.ServiceTests;

public class OrderServiceTests : TestWithOrderPostgres<OrderDbContext>
{
    private readonly OrderRepository _orderRepository;
    private readonly OrderService _orderService;
    private readonly OrderDbContext _context;
    private readonly HttpService _httpService;
    private readonly ILogger<OrderService> _logger;

    public OrderServiceTests()
    {
        _context = new OrderDbContext(Options);
        _orderRepository = new OrderRepository(new OrderDAL.Repositories.Shared.GenericRepository<Order>(_context));
        _httpService = new HttpService(new HttpClient());
        _logger = new Logger<OrderService>(new LoggerFactory());
        _orderService = new OrderService(_orderRepository, _httpService, _logger);
    }
    
    [Fact]
    public async Task Insert_Order_Should_Succeed()
    {
        await _context.Database.EnsureCreatedAsync();
        
        var order = await InsertOrder();

        // Assert
        Assert.True(order.Id > 0);
    }
    
    [Fact]
    public async Task Update_Order_Status_Should_Succeed()
    {
        await _context.Database.EnsureCreatedAsync();
        
        var order = await InsertOrder();
        
        var updatedStatus = OrderStatus.Reserved;
        
        await _orderService.ChangeStatus(order.Id, updatedStatus);
        
        var result = await _orderRepository.GetById(order.Id);

        // Assert
        Assert.Equal(result?.OrderStatus, updatedStatus);
    }
    
    private async Task<Order> InsertOrder()
    {
        var order = new Order
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

        await _orderService.Insert(order);
        return order;
    }
}