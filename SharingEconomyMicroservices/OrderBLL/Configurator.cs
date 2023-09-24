using Microsoft.Extensions.DependencyInjection;
using OrderBLL.Http;
using OrderBLL.Order;

namespace OrderBLL;

public static class Configurator
{
    public static void AddBll(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IOrderService, OrderService>();
        serviceCollection.AddScoped<IHttpService, HttpService>();
    }
}