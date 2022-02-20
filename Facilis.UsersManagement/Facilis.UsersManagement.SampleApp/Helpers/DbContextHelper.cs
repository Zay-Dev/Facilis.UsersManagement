using Facilis.Core.Abstractions;
using Facilis.UsersManagement.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Facilis.UsersManagement.SampleApp.Helpers
{
    public static class DbContextHelper
    {
        public const string PASSWORD = "password";

        public static void SeedData(this IServiceProvider provider)
        {
            var entities = provider.GetService<IEntities<User>>();
            var operators = provider.GetRequiredService<IOperators>();

            var password = provider.GetService<IPasswordHasher>().Hash(PASSWORD);

            provider.GetService<DbContext>().Database.EnsureCreated();
            entities.CreateUserIfNotExists(
                nameof(User.Username),
                password,
                operators.GetSystemOperatorName()
            );
        }

        private static void CreateUserIfNotExists(
            this IEntities<User> entities,
            string username,
            IPassword password,
            string @operator
        )
        {
            var exists = entities
                .Rows
                .Any(x => x.Username.ToLower() == username.ToLower());
            if (exists) return;

            entities.Add(new User()
            {
                Username = nameof(User.Username),
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