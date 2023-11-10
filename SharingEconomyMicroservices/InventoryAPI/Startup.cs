using InventoryBLL;
using InventoryDAL;
using InventoryDAL.Context;
using Microsoft.EntityFrameworkCore;
using Polly;
using Prometheus;
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
            // .UseNpgsql(_configurationManager.GetConnectionString("LocalConnection")));

        serviceCollection.AddSwaggerGen();

        serviceCollection.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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

        app.UseHttpMetrics();

        app.UseAuthorization();
        
        app.MapMetrics();

        app.MapControllers();
    }

    public void ApplyMigrations(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Startup>>();

        try
        {
            var policy = Policy.Handle<Exception>()
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(15),
                });

            policy.Execute(() =>
            {
                var context = services.GetRequiredService<InventoryDbContext>();
                context.Database.Migrate();
            });

            logger.LogInformation("Migrated inventory db");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the inventory database");
        }
    }
}