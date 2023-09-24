using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OrderDAL.Context;
using OrderDAL.Entity;

namespace OrderDAL.Repositories.Shared;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
{
    private readonly OrderDbContext _orderDbContext;
    private readonly DbSet<TEntity> _dbSet;
    public IQueryable<TEntity> Table => _dbSet.AsQueryable();

    public GenericRepository(OrderDbContext orderDbContext)
    {
        _orderDbContext = orderDbContext;
        _dbSet = orderDbContext.Set<TEntity>();
    }
    
    public async Task<bool> ExecuteInTransactionAsync(Func<Task> action)
    {
        using (var transaction = await _orderDbContext.Database.BeginTransactionAsync())
        {
            try
            {
                await action();
                await transaction.CommitAsync();
                await _orderDbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }

    public IDbContextTransaction BeginTransaction()
    {
        return _orderDbContext.Database.BeginTransaction();
    }
    
    public async Task<TEntity?> GetById(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public Task<IQueryable<TEntity>> GetAll()
    {
        return Task.FromResult(Table);
    }

    public Task<IQueryable<TEntity>> GetFiltered(Expression<Func<TEntity, bool>> filter)
    {
        return Task.FromResult(_dbSet.Where(filter));
    }

    public async Task Insert(TEntity item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        
        item.CreatedOnUtc = DateTime.UtcNow;
        item.UpdatedOnUtc = item.CreatedOnUtc;
        _dbSet.Add(item);
        await _orderDbContext.SaveChangesAsync();
    }

    public async Task Update(TEntity item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        
        item.UpdatedOnUtc = DateTime.Now;
        _orderDbContext.Entry(item).State = EntityState.Modified;
        await _orderDbContext.SaveChangesAsync();
    }

    public async Task Delete(TEntity item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        _dbSet.Remove(item);
        await _orderDbContext.SaveChangesAsync();
    }
    
    public Task<bool> DoesExist(Expression<Func<TEntity, bool>> filter)
    {
        return Task.FromResult(_dbSet.Any(filter));
    }
}