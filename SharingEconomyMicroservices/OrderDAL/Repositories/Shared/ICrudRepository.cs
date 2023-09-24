using System.Linq.Expressions;

namespace OrderDAL.Repositories.Shared;

public interface ICrudRepository<TEntity> where TEntity : class 
{
    Task<TEntity?> GetById(int id);
    Task<IQueryable<TEntity>> GetAll();
    Task<IQueryable<TEntity>> GetFiltered(Expression<Func<TEntity, bool>> filter);
    Task Insert(TEntity item);
    Task Update(TEntity item);
    Task Delete(TEntity item);
    Task<bool> DoesExist(Expression<Func<TEntity, bool>> filter);
}