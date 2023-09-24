namespace InventoryDAL.Repositories.Shared;

public interface ICrudRepository<TEntity> where TEntity : class 
{
    Task<TEntity?> GetById(int id);
    Task<IQueryable<TEntity>> GetAll();
    Task Insert(TEntity item);
    Task Update(TEntity item);
    Task Delete(TEntity item);
}