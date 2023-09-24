using InventoryDAL.Configurations;
using Microsoft.EntityFrameworkCore;

namespace InventoryDAL.Context;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext() { }
    
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryConfiguration).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}