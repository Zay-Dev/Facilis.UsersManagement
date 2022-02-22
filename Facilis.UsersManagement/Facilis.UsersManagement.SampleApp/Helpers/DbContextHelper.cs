using Facilis.Core.Abstractions;
using Facilis.UsersManagement.Abstractions;
using Facilis.UsersManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace Facilis.UsersManagement.SampleApp.Helpers
{
    public static class DbContextHelper
    {
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
                operators.GetSystemOperatorName()
            );
        }

        private static void CreateUserIfNotExists(
            this IEntities<User<UserProfile>> entities,
            string username,
            IPassword password,
            string @operator
        )
        {
            var exists = entities
                .Rows
                .Any(x => x.Username.ToLower() == username.ToLower());
            if (exists) return;

            entities.Add(new User<UserProfile>()
            {
                Username = username,
                CreatedBy = @operator,
                UpdatedBy = @operator,

                HashingMethod = password.HashingMethod,
                HashedPassword = password.HashedPassword,
                PasswordSalt = password.PasswordSalt,
                PasswordIterated = password.PasswordIterated,
            });
        }
    }
}