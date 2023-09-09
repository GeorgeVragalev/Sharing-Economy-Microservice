using DAL.Context;
using DAL.Entity;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.Shared;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
{
    private readonly UserDbContext _userDbContext;
    private readonly DbSet<TEntity> _dbSet;
    public IQueryable<TEntity> Table => _dbSet.AsQueryable();

    public GenericRepository(UserDbContext userDbContext)
    {
        _userDbContext = userDbContext;
        _dbSet = userDbContext.Set<TEntity>();
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
        await _userDbContext.SaveChangesAsync();
    }

    public async Task Update(TEntity item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        _userDbContext.Entry(item).State = EntityState.Modified;
        await _userDbContext.SaveChangesAsync();
    }

    public async Task Delete(TEntity item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        _dbSet.Remove(item);
        await _userDbContext.SaveChangesAsync();
    }
}