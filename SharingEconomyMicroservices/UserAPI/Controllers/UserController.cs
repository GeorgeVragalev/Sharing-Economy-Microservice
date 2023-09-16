using AutoMapper;
using BLL.User;
using DAL.Entity;
using DAL.Entity.Validations;
using Microsoft.AspNetCore.Mvc;
using UserAPI.Models;

namespace UserAPI.Controllers.API;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly ILogger<UserController> _logger;

    public UserController(IMapper mapper, IUserService userService, ILogger<UserController> logger)
    {
        _mapper = mapper;
        _userService = userService;
        _logger = logger;
    }

    
    [HttpGet("status")]
    public IActionResult Get()
    {
        return new JsonResult("Heregh");
    }
    
    // [HttpGet("{id}")]
    // public async Task<IActionResult> GetById(int id)
    // {
    //     var userById = await _userService.GetById(id);
    //     var mappedUserDto = _mapper.Map<User, UserDto>(userById!);
    //
    //     if (userById != null)
    //     {
    //         return Ok(mappedUserDto);
    //     }
    //     
    //     _logger.LogWarning($"User with {id} doesn't exist");
    //     return NotFound($"User with {id} doesn't exist");     
    //
    // }
    //
    // [HttpGet("name/{name}")]
    // public async Task<IActionResult> GetByName(string name)
    // {
    //     var usersByName = await _userService.GetByName(name);
    //     var mappedUsersDto = _mapper.Map<List<User>, List<UserDto>>(usersByName);
    //
    //     if (!mappedUsersDto.IsNullOrEmpty()) 
    //         return Ok(mappedUsersDto);
    //     
    //     _logger.LogWarning($"User with name {name} doesn't exist");
    //     return NotFound($"User with name {name} doesn't exist");
    // }
    //
    // [HttpGet]
    // public async Task<IActionResult> GetAll([FromQuery] UserFilterDto filter)
    // {
    //     var userMap = _mapper.Map<UserFilterDto, UserFilter>(filter);
    //     
    //     var users = await _userService.GetAll(userMap);
    //     
    //     var pagedListModel = _mapper.ToPagedListModel<User, UserDto>(users);
    //     return Ok(pagedListModel);
    // }
    [HttpPost]
    public async Task<ActionResult<User>> PostUser(UserModel userModel)
    {
        var userValidation = new UserValidation();
        
        var user = _mapper.Map<UserModel, User>(userModel);
        
        var validationResult = await userValidation.ValidateAsync(user);
    
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        
        await _userService.Insert(user);
        return Ok(user);
    }

    // public async Task<IActionResult> PutUser(int id, UserDto userDto)
    // {
    //     if (id != userDto.Id)
    //     {
    //         _logger.LogWarning($"User id: {id} doesn't match");
    //         return BadRequest($"User id: {id} doesn't match");     
    //     }
    //     
    //     var userInDb = await _userService.GetById(id);
    //     
    //     if (userInDb == null)
    //     {
    //         _logger.LogWarning($"User with {id} doesn't exist");
    //         return NotFound($"User with {id} doesn't exist");     
    //     }
    //     _mapper.Map(userDto, userInDb);
    //
    //     await _userService.Update(id, userInDb ?? throw new InvalidOperationException());
    //     
    //     return Ok(userDto);
    // }
    //
    // [HttpDelete("{id}")]
    // public async Task<IActionResult> DeleteUser(int id)
    // {
    //     var user = await _userService.GetById(id);
    //     if (user == null)
    //     {
    //         _logger.LogWarning("User doesn't exist");
    //         return NotFound("User doesn't exist");
    //     }
    //     await _userService.Delete(id);
    //     return Ok(id);
    // }
}