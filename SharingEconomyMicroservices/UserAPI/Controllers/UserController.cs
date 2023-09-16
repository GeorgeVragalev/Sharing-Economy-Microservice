using AutoMapper;
using BLL.Authentication;
using BLL.User;
using DAL.Entity;
using DAL.Entity.Validations;
using Microsoft.AspNetCore.Mvc;
using UserAPI.Models;

namespace UserAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthenticationService _authenticationService;
    private readonly IMapper _mapper;
    private readonly ILogger<UserController> _logger;
    
    public UserController(IMapper mapper, IUserService userService, ILogger<UserController> logger, IAuthenticationService authenticationService)
    {
        _mapper = mapper;
        _userService = userService;
        _logger = logger;
        _authenticationService = authenticationService;
    }

    [HttpGet("status")]
    public IActionResult Status()
    {
        return Ok();
    }
    
    #region CRUD
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (id <= 0)
        {
            return BadRequest("Id cannot be less than 0");
        }

        var user = await _userService.GetById(id);

        if (user != null)
        {
            return Ok(user);
        }
        
        return NotFound($"Not found user with id {id}");
    }

    [HttpPut]
    public async Task<IActionResult> PutUser(UserModel userModel)
    {
        int id = userModel.Id;
        
        var userInDb = await _userService.GetById(id);
        
        if (userInDb == null)
        {
            _logger.LogWarning($"User with {id} doesn't exist");
            return NotFound($"User with {id} doesn't exist");     
        }
        
        _mapper.Map(userModel, userInDb);
    
        await _userService.Update(id, userInDb);
        
        return Ok(userModel);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _userService.GetById(id);
        if (user == null)
        {
            _logger.LogWarning("User doesn't exist");
            return NotFound("User doesn't exist");
        }
        
        await _userService.Delete(id);
        return Ok(id);
    }

    #endregion
    
    #region Auth
    
    [HttpPost]
    public async Task<ActionResult> Login([FromBody] UserLogin userLogin)
    {
        var user = await _userService.GetByName(userLogin.Username);

        if (user == null)
        {
            return NotFound($"User not found with username {userLogin.Username}");
        }

        var token = _authenticationService.GenerateToken(user);

        if (string.IsNullOrEmpty(token))
        {
            return Problem("Failed to create a token for user login details");
        }
        
        return Ok(token);
    }

    [HttpPost]
    public async Task<ActionResult<User>> Register(UserModel userModel)
    {
        var userValidation = new UserValidation();
        
        var user = _mapper.Map<UserModel, User>(userModel);
        
        var validationResult = await userValidation.ValidateAsync(user);
    
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        
        await _userService.Insert(user);
        return Ok(user);
    }
    
    #endregion
}