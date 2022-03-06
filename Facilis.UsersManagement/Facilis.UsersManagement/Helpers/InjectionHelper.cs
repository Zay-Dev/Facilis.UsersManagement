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
            return services.AddAuthenticator<IPasswordBase, TAuthenticator, TUser>();
        }

        public static IServiceCollection AddTokenBased<TAuthenticator, TUser>(
            this IServiceCollection services
        )
            where TUser : IUser
            where TAuthenticator : class, IAuthenticator<ITokenBase, TUser>
        {
            return services.AddAuthenticator<ITokenBase, TAuthenticator, TUser>();
        }

        public static IServiceCollection AddDefaultPasswordHasher(
            this IServiceCollection services
        )
        {
            return services.AddSingleton<IPasswordHasher, BCryptNetPasswordHasher>();
        }

        public static IServiceCollection AddDefaultAuthenticationHistories(
            this IServiceCollection services
        )
        {
            return services.AddScoped<IAuthenticationHistoryWriter, AuthenticationHistoryWriter>();
        }

        private static IServiceCollection AddAuthenticator<TInputService, TAuthenticator, TUser>(
            this IServiceCollection services
        )
            where TInputService : IAuthenticateInput
            where TUser : IUser
            where TAuthenticator : class, IAuthenticator<TInputService, TUser>
        {
            return services
                .AddScoped<IAuthenticator<TInputService, TUser>>(provider =>
                {
                    var authenticator = ActivatorUtilities
                        .CreateInstance<TAuthenticator>(provider);
                    var writer = provider.GetService<IAuthenticationHistoryWriter>();
                    if (writer != null) authenticator.AddDefaultHistory(writer);

                    return authenticator;
                });
        }

        private static void AddDefaultHistory<TInput, TUser>(
            this IAuthenticator<TInput, TUser> authenticator,
            IAuthenticationHistoryWriter writer
        )
            where TInput : IAuthenticateInput
            where TUser : IUser
        {
            authenticator.Authenticated += writer.Authenticated;
            authenticator.AuthenticateFailed += writer.AuthenticateFailed;
        }
    }
}