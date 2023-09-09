using BLL;
using DAL;
using DAL.Context;
using Microsoft.EntityFrameworkCore;

namespace UserAPI;

public class Startup
{
    private readonly ConfigurationManager _configurationManager;

    public Startup(ConfigurationManager configurationManager)
    {
        _configurationManager = configurationManager;
    }

    public void ConfigureServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<UserDbContext>(b => b.UseLazyLoadingProxies()
            .UseNpgsql(@"Data Source=.\\SQLEXPRESS;Initial Catalog=MarketWebsite;Trusted_Connection=True;MultipleActiveResultSets=true"));

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

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
    }
}