using DAL.Repositories.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BLL.User;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<DAL.Entity.User> _passwordHasher;
    
    public UserService(IUserRepository userRepository, IPasswordHasher<DAL.Entity.User> passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<DAL.Entity.User?> GetById(int id)
    {
        if (id <= 0)
        {
            return null;
        }
        
        var userById = await _userRepository.GetById(id);

        return userById;
    }

    public async Task<DAL.Entity.User?> GetByName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }
        
        return await _userRepository.GetByName(name);
    }

    public async Task<IList<DAL.Entity.User>> GetAll()
    {
        return await _userRepository.GetAll().Result.ToListAsync();
    }

    public async Task Insert(DAL.Entity.User user)
    {
        var hashedPassword = _passwordHasher.HashPassword(user, user.Password);
        
        user.Password = hashedPassword;
        
        await _userRepository.Insert(user);
    }


    public async Task Update(int id, DAL.Entity.User user)
    {
        var userInDb = await _userRepository.GetById(id);
        await _userRepository.Update(userInDb ?? throw new InvalidOperationException());
    }

    public async Task Delete(int id)
    {
        var userById = await _userRepository.GetById(id);

        await _userRepository.Delete(userById ?? throw new InvalidOperationException());
    }
}