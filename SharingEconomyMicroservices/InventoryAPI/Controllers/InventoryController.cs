using AutoMapper;
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

    [HttpPost("create")]
    public async Task<ActionResult<Item>> Create(ItemModel itemModel)
    {
        var itemValidation = new ItemValidation();

        var validationResult = await itemValidation.ValidateAsync(itemModel);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var item = _mapper.Map<ItemModel, Item>(itemModel);
        
        await _itemService.Insert(item);
        return Ok(item);
    }

    [HttpPut]
    public async Task<IActionResult> Update(ItemModel itemModel)
    {
        int id = itemModel.Id;
        
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _itemService.GetById(id);
        if (item == null)
        {
            _logger.LogWarning("Item doesn't exist");
            return NotFound("Item doesn't exist");
        }
        
        await _itemService.Delete(id);
        return Ok(id);
    }

    #endregion
    
    [HttpPost("reserve")]
    public async Task<IActionResult> Reserve([FromBody] int id)
    {
        return await ChangeStatus(id, InventoryDAL.Entity.Enums.Status.Reserved);
    }
    
    [HttpPost("change-status")]
    public async Task<IActionResult> ChangeStatus(int id, string status)
    {
        var isValidStatus = Enum.TryParse<Status>(status, out var newStatus);

        if (!isValidStatus)
        {
            return BadRequest($"Status: {status} doesn't exist");
        }
        
        return await ChangeStatus(id, newStatus);
    }

    private async Task<IActionResult> ChangeStatus(int id, Status newStatus)
    {
        try
        {
            await _itemService.ChangeStatus(id, newStatus);
            return Ok(id);
        }
        catch (ItemReservedException)
        {
            return Ok("Item is already reserved");
        }
        catch (Exception e)
        {
            _logger.LogWarning("Something failed", e);
            return Problem($"Something failed. {e.Message}");
        }
    }
}