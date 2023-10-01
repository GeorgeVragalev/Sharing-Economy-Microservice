using Microsoft.EntityFrameworkCore;
using OrderBLL;
using OrderDAL;
using OrderDAL.Context;
using Polly;
using ConfigurationManager = Microsoft.Extensions.Configuration.ConfigurationManager;

namespace OrderAPI;

public class Startup
{
    private readonly ConfigurationManager _configurationManager;

    public Startup(ConfigurationManager configurationManager)
    {
        _configurationManager = configurationManager;
    }

    public void ConfigureServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient();
        
        serviceCollection.AddDbContext<OrderDbContext>(b => b.UseLazyLoadingProxies()
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
                var context = services.GetRequiredService<OrderDbContext>();
                context.Database.Migrate();
            });

            logger.LogInformation("Migrated order db");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the order database");
        }
    }
}