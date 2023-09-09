namespace BLL.User;

public interface IUserService
{
    Task<DAL.Entity.User?> GetById(int id);
    Task<IList<DAL.Entity.User>> GetByName(string name);
    Task Insert(DAL.Entity.User user);
    Task Update(int id, DAL.Entity.User user);
    Task Delete(int id);
}