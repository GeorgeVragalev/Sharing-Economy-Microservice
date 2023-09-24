using InventoryDAL.Context;
using InventoryDAL.Entity;
using Microsoft.EntityFrameworkCore;

namespace InventoryDAL.Repositories.Shared;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
{
    private readonly InventoryDbContext _inventoryDbContext;
    private readonly DbSet<TEntity> _dbSet;
    public IQueryable<TEntity> Table => _dbSet.AsQueryable();

    public GenericRepository(InventoryDbContext inventoryDbContext)
    {
        _inventoryDbContext = inventoryDbContext;
        _dbSet = inventoryDbContext.Set<TEntity>();
    }

    public async Task<TEntity?> GetById(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public Task<IQueryable<TEntity>> GetAll()
    {
        return Task.FromResult(Table);
    }

    public async Task Insert(TEntity item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        _dbSet.Add(item);
        await _inventoryDbContext.SaveChangesAsync();
    }

    public async Task Update(TEntity item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        _inventoryDbContext.Entry(item).State = EntityState.Modified;
        await _inventoryDbContext.SaveChangesAsync();
    }

    public async Task Delete(TEntity item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        _dbSet.Remove(item);
        await _inventoryDbContext.SaveChangesAsync();
    }
}