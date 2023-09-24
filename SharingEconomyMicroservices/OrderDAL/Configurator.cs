using Microsoft.Extensions.DependencyInjection;
using OrderDAL.Repositories.Order;
using OrderDAL.Repositories.Shared;

namespace OrderDAL;

public static class Configurator
{
    public static void AddDal(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IOrderRepository, OrderRepository>();
        serviceCollection.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    }
}