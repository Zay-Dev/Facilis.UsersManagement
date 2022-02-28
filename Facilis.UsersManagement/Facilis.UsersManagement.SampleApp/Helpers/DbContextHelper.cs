using Facilis.Core.Abstractions;
using Facilis.UsersManagement.Abstractions;
using Facilis.UsersManagement.Models;
using Facilis.UsersManagement.SampleApp.Enums;
using Microsoft.EntityFrameworkCore;

namespace Facilis.UsersManagement.SampleApp.Helpers
{
    public static class DbContextHelper
    {
        public const string ADMIN = nameof(ADMIN);
        public const string USERNAME = nameof(USERNAME);
        public const string PASSWORD = "password";

        public static void SeedData(this IServiceProvider provider)
        {
            var entities = provider.GetService<IEntities<User>>();
            var entityStampsBinder = provider.GetRequiredService<IEntityStampsBinder>();

            var password = provider.GetService<IPasswordHasher>().Hash(PASSWORD);

            provider.GetService<DbContext>().Database.EnsureCreated();

            entities.CreateUserIfNotExists(
                USERNAME,
                password,
                new[] { RoleTypes.User },
                entityStampsBinder
            );

            entities.CreateUserIfNotExists(
                ADMIN,
                password,
                new[] { RoleTypes.Administrator, RoleTypes.User },
                entityStampsBinder
            );
        }

        private static void CreateUserIfNotExists(
            this IEntities<User> entities,
            string username,
            IPassword password,
            RoleTypes[] roles,
            IEntityStampsBinder entityStampsBinder
        )
        {
            var exists = entities
                .Rows
                .Any(x => x.Username.ToLower() == username.ToLower());
            if (exists) return;

            var user = entityStampsBinder
                .BindCreatedBySystem(new User()
                {
                    Username = username,
                    HashingMethod = password.HashingMethod,
                    HashedPassword = password.HashedPassword,
                    PasswordSalt = password.PasswordSalt,
                    PasswordIterated = password.PasswordIterated,
                });
            user.SetProfile(new UserProfile()
            {
                Roles = roles.Select(x => x.ToString()).ToArray(),
                LastSignInAtUtc = DateTime.UtcNow,
            });

            entities.Add(user);
        }
    }
}