using DAL.Repositories.Shared;
using DAL.Repositories.User;
using Microsoft.Extensions.DependencyInjection;

namespace DAL;

public static class Configurator
{
    public static void AddDal(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IUserRepository, UserRepository>();
        serviceCollection.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    }
}