using Microsoft.EntityFrameworkCore;
using OrderBLL;
using OrderDAL;
using OrderDAL.Context;
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
}