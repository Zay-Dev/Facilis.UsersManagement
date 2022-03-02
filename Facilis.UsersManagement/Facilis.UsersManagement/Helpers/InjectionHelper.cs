using Facilis.Core.Abstractions;
using Facilis.UsersManagement.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Facilis.UsersManagement.Helpers
{
    public static class InjectionHelper
    {
        public static IServiceCollection AddAuthenticator<TAuthenticator>(
            this IServiceCollection services
        ) where TAuthenticator : class, IAuthenticator
        {
            return services.AddScoped<IAuthenticator, TAuthenticator>();
        }

        public static IServiceCollection AddAuthenticator<TAuthenticator, TUser>(
            this IServiceCollection services
        )
            where TAuthenticator : class, IAuthenticator<TUser>
            where TUser : IUser
        {
            return services
                .AddScoped<IAuthenticator, TAuthenticator>()
                .AddScoped<IAuthenticator<TUser>, TAuthenticator>();
        }

        public static IServiceCollection AddDefaultPasswordHasher(
            this IServiceCollection services
        )
        {
            return services.AddSingleton<IPasswordHasher, BCryptNetPasswordHasher>();
        }
    }
}