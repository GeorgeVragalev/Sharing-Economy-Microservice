using Microsoft.EntityFrameworkCore.Storage;

namespace InventoryDAL.Repositories.Shared;


public interface IGenericRepository<TEntity> : ICrudRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> Table { get; }
    IDbContextTransaction BeginTransaction();
    Task ExecuteInTransactionAsync(Func<Task> action);
}