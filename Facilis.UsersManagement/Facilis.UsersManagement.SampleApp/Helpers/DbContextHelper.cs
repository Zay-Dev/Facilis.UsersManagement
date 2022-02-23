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
            var entities = provider.GetService<IEntities<User<UserProfile>>>();
            var operators = provider.GetRequiredService<IOperators>();

            var password = provider.GetService<IPasswordHasher>().Hash(PASSWORD);

            provider.GetService<DbContext>().Database.EnsureCreated();

            entities.CreateUserIfNotExists(
                USERNAME,
                password,
                new[] { RoleTypes.User },
                operators.GetSystemOperatorName()
            );

            entities.CreateUserIfNotExists(
                ADMIN,
                password,
                new[] { RoleTypes.Administrator, RoleTypes.User },
                operators.GetSystemOperatorName()
            );
        }

        private static void CreateUserIfNotExists(
            this IEntities<User<UserProfile>> entities,
            string username,
            IPassword password,
            RoleTypes[] roles,
            string @operator
        )
        {
            var exists = entities
                .Rows
                .Any(x => x.Username.ToLower() == username.ToLower());
            if (exists) return;

            var user = new User<UserProfile>()
            {
                Username = username,
                CreatedBy = @operator,
                UpdatedBy = @operator,

                HashingMethod = password.HashingMethod,
                HashedPassword = password.HashedPassword,
                PasswordSalt = password.PasswordSalt,
                PasswordIterated = password.PasswordIterated,
            };
            user.SetProfile(new UserProfile()
            {
                Roles = roles.Select(x => x.ToString()).ToArray(),
                LastSignInAtUtc = DateTime.UtcNow,
            });

            entities.Add(user);
        }
    }
}