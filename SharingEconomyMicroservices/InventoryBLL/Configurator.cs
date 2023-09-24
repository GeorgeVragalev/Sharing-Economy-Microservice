using InventoryBLL.Item;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryBLL;

public static class Configurator
{
    public static void AddBll(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IItemService, ItemService>();
    }
}