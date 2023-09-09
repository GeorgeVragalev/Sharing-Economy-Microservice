namespace DAL.Repositories.Shared;

public interface ICrudRepository<TEntity> where TEntity : class 
{
    Task Insert(TEntity item);
    Task Update(TEntity item);
    Task Delete(TEntity item);

    Task<TEntity?> GetById(int id);
    Task<IQueryable<TEntity>> GetAll();
}