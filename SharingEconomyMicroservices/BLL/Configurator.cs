using BLL.Authentication;
using BLL.User;
using Microsoft.Extensions.DependencyInjection;

namespace BLL;

public static class Configurator
{
    public static void AddBll(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IUserService, UserService>();
        serviceCollection.AddScoped<IAuthenticationService, AuthenticationService>();
    }
}