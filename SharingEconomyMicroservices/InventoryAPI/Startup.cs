using InventoryBLL;
using InventoryDAL;
using InventoryDAL.Context;
using Microsoft.EntityFrameworkCore;
using ConfigurationManager = Microsoft.Extensions.Configuration.ConfigurationManager;

namespace InventoryAPI;

public class Startup
{
    private readonly ConfigurationManager _configurationManager;

    public Startup(ConfigurationManager configurationManager)
    {
        _configurationManager = configurationManager;
    }

    public void ConfigureServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<InventoryDbContext>(b => b.UseLazyLoadingProxies()
            .UseNpgsql(_configurationManager.GetConnectionString("DefaultConnection")));

        serviceCollection.AddSwaggerGen();

        serviceCollection.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        //add scoped in another file
        serviceCollection.AddDal();
        serviceCollection.AddBll();

        serviceCollection.AddControllers();
    }

    public void ConfigurePipeline(WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
    }
    
    public void ApplyMigrations(IServiceProvider serviceProvider)
    {
        // Here we are manually starting the service scope to apply migrations
        using var scope = serviceProvider.CreateScope();
        
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<InventoryDbContext>();
            context.Database.Migrate();
            Console.WriteLine("Migrated inventory db");
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while migrating the database: " + ex.Message);
        }
    }
}