using Microsoft.EntityFrameworkCore;
using OrderDAL.Configurations;

namespace OrderDAL.Context;

public class OrderDbContext : DbContext
{
    public OrderDbContext() { }
    
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderConfiguration).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}