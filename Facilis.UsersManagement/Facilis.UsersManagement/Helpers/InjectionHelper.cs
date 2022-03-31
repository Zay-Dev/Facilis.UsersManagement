using Facilis.Core.Abstractions;
using Facilis.UsersManagement.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public static IServiceCollection AddUserIdBased<TAuthenticator, TUser>(
            this IServiceCollection services
        )
            where TUser : IUser
            where TAuthenticator : class, IAuthenticator<IUserIdBase, TUser>
        {
            return services.AddAuthenticator<IUserIdBase, TAuthenticator, TUser>();
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

        public static void UseAuthenticationHistories(
            this IEnumerable<IAuthenticator> authenticators,
            IAuthenticationHistoryWriter writer
        )
        {
            foreach (var authenticator in authenticators)
            {
                authenticator.Authenticated += writer.OnAuthenticated;
                authenticator.AuthenticateFailed += writer.OnAuthenticateFailed;
            }
        }

        public static async Task UseAuthenticationHistories(
            this IServiceProvider provider,
            Func<Task> next
        )
        {
            provider.GetRequiredService<IEnumerable<IAuthenticator>>()
                .UseAuthenticationHistories(provider
                    .GetRequiredService<IAuthenticationHistoryWriter>()
                );
            await next();
        }

        public static async Task UseInvitationNotifier(
            this IServiceProvider provider,
            Func<Task> next
        )
        {
            var notifier = provider.GetRequiredService<IInvitationNotifier>();
            var service = provider.GetRequiredService<IInvitationService>();

            service.InvitationSent += (sender, invitation) => notifier.Notify(invitation);
            service.InvitationAccepted += (sender, invitation) => notifier.Notify(invitation);

            await next();
        }

        private static IServiceCollection AddAuthenticator<TInputService, TAuthenticator, TUser>(
            this IServiceCollection services
        )
            where TInputService : IAuthenticateInput
            where TUser : IUser
            where TAuthenticator : class, IAuthenticator<TInputService, TUser>
        {
            return services
                .AddScoped<IAuthenticator<TInputService, TUser>, TAuthenticator>()
                .AddScoped<IAuthenticator>(provider =>
                    provider.GetRequiredService<IAuthenticator<TInputService, TUser>>()
                );
        }
    }
}