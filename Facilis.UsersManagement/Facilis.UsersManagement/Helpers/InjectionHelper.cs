using Facilis.Core.Abstractions;
using Facilis.UsersManagement.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Facilis.UsersManagement.Helpers
{
    public static class InjectionHelper
    {
        public static IServiceCollection AddPasswordBased<TAuthenticator, TUser>(
            this IServiceCollection services
        )
            where TUser : IUser
            where TAuthenticator : class, IAuthenticator<IPasswordBase, TUser>
        {
            return services
                .AddScoped<IAuthenticator<IPasswordBase, TUser>, TAuthenticator>();
        }

        public static IServiceCollection AddTokenBased<TAuthenticator, TUser>(
            this IServiceCollection services
        )
            where TUser : IUser
            where TAuthenticator : class, IAuthenticator<ITokenBase, TUser>
        {
            return services
                .AddScoped<IAuthenticator<ITokenBase, TUser>, TAuthenticator>();
        }

        public static IServiceCollection AddDefaultPasswordHasher(
            this IServiceCollection services
        )
        {
            return services.AddSingleton<IPasswordHasher, BCryptNetPasswordHasher>();
        }
    }
}