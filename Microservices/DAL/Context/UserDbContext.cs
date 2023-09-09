using DAL.Configurations;
using Microsoft.EntityFrameworkCore;

namespace DAL.Context;

public class UserDbContext : DbContext
{
    public UserDbContext() { }
    
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
        base.OnModelCreating(modelBuilder);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { 
        optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("DefaultConnection"));
    }
}