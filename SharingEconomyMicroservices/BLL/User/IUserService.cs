namespace BLL.User;

public interface IUserService
{
    Task<DAL.Entity.User?> GetById(int id);
    Task<DAL.Entity.User?> GetByEmail(string email);
    Task Insert(DAL.Entity.User user);
    Task Update(int id, DAL.Entity.User user);
    Task Delete(int id);
    Task<bool> DoesUserExist(string email);
}