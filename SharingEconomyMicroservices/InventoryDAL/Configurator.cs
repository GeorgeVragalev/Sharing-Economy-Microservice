using InventoryDAL.Repositories.Item;
using InventoryDAL.Repositories.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryDAL;

public static class Configurator
{
    public static void AddDal(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IItemRepository, ItemRepository>();
        serviceCollection.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    }
}