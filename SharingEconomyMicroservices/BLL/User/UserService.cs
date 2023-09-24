using DAL.Repositories.User;
using Microsoft.EntityFrameworkCore;

namespace BLL.User;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
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

    public async Task<DAL.Entity.User?> GetByEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return null;
        }
        
        return await _userRepository.GetByEmail(email);
    }

    public async Task<IList<DAL.Entity.User>> GetAll()
    {
        return await _userRepository.GetAll().Result.ToListAsync();
    }

    public async Task Insert(DAL.Entity.User user)
    {
        // var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
        // user.Password = hashedPassword;
        
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

    public async Task<bool> DoesUserExist(string email)
    {
        return await _userRepository.DoesUserExist(email);
    }
}