using AutoMapper;
using DefaultNamespace;
using InventoryAPI.Models;
using InventoryAPI.Validations;
using InventoryBLL.Item;
using InventoryDAL.Entity;
using InventoryDAL.Entity.Enums;
using InventoryDAL.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InventoryController : ControllerBase
{
    private readonly IItemService _itemService;
    private readonly IMapper _mapper;
    private readonly ILogger<InventoryController> _logger;

    public InventoryController(IItemService itemService, IMapper mapper, ILogger<InventoryController> logger)
    {
        _itemService = itemService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet("status")]
    public IActionResult Status()
    {
        return Ok("Ok");
    }

    #region CRUD

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("Id cannot be less than 0");
            }

            var item = await _itemService.GetById(id);

            if (item != null)
            {
                return Ok(item);
            }

            return NotFound($"Not found item with id {id}");
        }
        catch (Exception e)
        {
            _logger.LogWarning($"Couldn't get item with id: {id}", e);
            return Problem($"Couldn't get item with id: {id}. {e.Message}");
        }
    }

    [HttpPost("create")]
    public async Task<ActionResult<Item>> Create([FromBody] ItemModel itemModel)
    {
        try
        {
            var itemValidation = new ItemValidation();

            var validationResult = await itemValidation.ValidateAsync(itemModel);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var item = _mapper.Map<ItemModel, Item>(itemModel);

            await _itemService.Insert(item);
            return Ok(item);
        }
        catch (Exception e)
        {
            _logger.LogWarning($"Couldn't create item", e);
            return Problem($"Couldn't create item. {e.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] ItemModel itemModel, int id)
    {
        try
        {
            var itemInDb = await _itemService.GetById(id);

            if (itemInDb == null)
            {
                _logger.LogWarning($"Item with {id} doesn't exist");
                return NotFound($"Item with {id} doesn't exist");
            }

            _mapper.Map(itemModel, itemInDb);

            await _itemService.Update(id, itemInDb);

            return Ok(itemModel);
        }
        catch (Exception e)
        {
            _logger.LogWarning($"Couldn't update item with id: {id}", e);
            return Problem($"Couldn't update item with id: {id}. {e.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var item = await _itemService.GetById(id);
            if (item == null)
            {
                _logger.LogWarning("Item doesn't exist");
                return NotFound("Item doesn't exist");
            }

            await _itemService.Delete(id);
            return Ok($"Deleted item {id}");
        }
        catch (Exception e)
        {
            _logger.LogWarning($"Couldn't delete item with id: {id}", e);
            return Problem($"Couldn't delete item with id: {id}. {e.Message}");
        }
    }

    #endregion

    [HttpPost("reserve")]
    public async Task<IActionResult> Reserve([FromBody] int id)
    {
        return await ChangeStatus(id, InventoryDAL.Entity.Enums.Status.Reserved);
    }

    [HttpPost("change-status")]
    public async Task<IActionResult> ChangeStatus([FromBody] ChangeStatusRequest changeStatusRequest)
    {
        var status = changeStatusRequest.Status;
        
        var isValidStatus = Enum.TryParse<Status>(status, out var newStatus);

        if (!isValidStatus)
        {
            return BadRequest($"Status: {status} doesn't exist");
        }

        return await ChangeStatus(changeStatusRequest.Id, newStatus);
    }

    private async Task<IActionResult> ChangeStatus(int id, Status newStatus)
    {
        try
        {
            await _itemService.ChangeStatus(id, newStatus);
            return Ok($"Changed item {id} status to {newStatus}");
        }
        catch (NotFoundException)
        {
            _logger.LogWarning($"Item {id} doesn't exist");
            return NotFound($"Item {id} doesn't exist");
        }
        catch (ItemReservedException)
        {
            return Problem("Item is already reserved");
        }
        catch (Exception e)
        {
            _logger.LogWarning("Something failed", e);
            return Problem($"Something failed. {e.Message}");
        }
    }
}