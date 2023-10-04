using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OrderAPI.Helpers;
using OrderAPI.Models;
using OrderAPI.Validations;
using OrderBLL.Order;
using OrderDAL.Entity;
using OrderDAL.Exceptions;

namespace OrderAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderController> _logger;

    public OrderController(IOrderService orderService, IMapper mapper, ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet("status")]
    public IActionResult Status()
    {
        return Ok("Ok");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("Id cannot be less than 0");
            }

            var order = await _orderService.GetById(id);

            if (order != null)
            {
                return Ok(order);
            }

            return NotFound($"Not found order with id {id}");
        }
        catch (Exception e)
        {
            _logger.LogWarning($"Couldn't get order with id: {id}", e);
            return Problem($"Couldn't get order with id: {id}. {e.Message}");
        }
      
    }

    [HttpPost("create")]
    public async Task<ActionResult<Order>> PlaceOrder([FromBody] PlaceOrderRequestModel orderModel)
    {
        try
        {
            var orderValidation = new OrderValidation();

            var validationResult = await orderValidation.ValidateAsync(orderModel);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var order = _mapper.Map<PlaceOrderRequestModel, Order>(orderModel);

            order.MapOrder(orderModel);

            await _orderService.PlaceOrder(order);

            return Ok(order);
        }
        catch (Exception e)
        {
            _logger.LogWarning($"Couldn't place order", e);
            return Problem($"Couldn't place order. {e.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] OrderModel orderModel, int id)
    {
        try
        {
            var orderInDb = await _orderService.GetById(id);

            if (orderInDb == null)
            {
                _logger.LogWarning($"Order with {id} doesn't exist");
                return NotFound($"Order with {id} doesn't exist");
            }

            _mapper.Map(orderModel, orderInDb);

            await _orderService.Update(id, orderInDb);

            return Ok(orderModel);
        }
        catch (OrderNotFoundException)
        {
            _logger.LogWarning($"Order with {id} doesn't exist");
            return NotFound($"Order with {id} doesn't exist");
        }
        catch (Exception e)
        {
            _logger.LogWarning($"Couldn't update order with id: {id}", e);
            return Problem($"Couldn't update order with id: {id}. {e.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _orderService.Delete(id);
            return Ok(id);
        }
        catch (OrderNotFoundException)
        {
            _logger.LogWarning($"Order: {id} was not found and can't be deleted");
            return NotFound($"Order was not found and can't be deleted: {id}");
        }
        catch (Exception e)
        {
            _logger.LogWarning($"Couldn't delete order with id: {id}", e);
            return Problem($"Couldn't delete order with id: {id}. {e.Message}");
        }
    }
}