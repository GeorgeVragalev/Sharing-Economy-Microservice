using Microsoft.Extensions.DependencyInjection;
using OrderBLL.Order;

namespace OrderBLL;

public static class Configurator
{
    public static void AddBll(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IOrderService, OrderService>();
    }
}