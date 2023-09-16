using AutoMapper;
using BLL.Authentication;
using BLL.User;
using DAL.Entity;
using Microsoft.AspNetCore.Mvc;
using UserAPI.Models;
using UserAPI.Validations;

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
    
    [HttpPost("validate-token")]
    public async Task<ActionResult> ValidateToken(string token)
    {
        var isValid = await _authenticationService.ValidateToken(token);

        if (isValid)
        {
            return Ok(token);
        }

        return Unauthorized("Invalid token");
    }
    
    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] UserLogin userLogin)
    {
        var user = await _userService.GetByEmail(userLogin.Email);

        if (user == null)
        {
            return Unauthorized("Wrong login credentials");
        }

        var login = await _authenticationService.VerifyPassword(userLogin.Password);

        if (!login)
        {
            return Unauthorized("Wrong login credentials");
        }

        var token = await _authenticationService.GenerateToken(user);

        if (string.IsNullOrEmpty(token))
        {
            return Problem("Failed to create a token for user login details");
        }
        
        return Ok(token);
    }

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(UserModel userModel)
    {
        var userValidation = new UserValidation();

        var validationResult = await userValidation.ValidateAsync(userModel);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        if (await _userService.DoesUserExist(userModel.Email))
        {
            return ValidationProblem("User already exists with this email");
        }
        
        var user = _mapper.Map<UserModel, User>(userModel);
        
        await _userService.Insert(user);
        return Ok(user);
    }
    
    #endregion
}