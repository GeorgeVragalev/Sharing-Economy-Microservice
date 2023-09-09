using Serilog;
using UserAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => 
    configuration
        .WriteTo
        .Console()
        .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
);

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.ConfigurePipeline(app);

app.Run();