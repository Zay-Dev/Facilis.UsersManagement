using Facilis.Core.Abstractions;
using Facilis.UsersManagement.Abstractions;

namespace Facilis.UsersManagement.Tests.Helpers
{
    public static class UserHelper
    {
        public static User CreateUser(
            this IPasswordHasher passwordHasher,
            string username,
            string password
        )
        {
            var hashed = passwordHasher.Hash(password);

            return new User()
            {
                Username = username,
                HashingMethod = hashed.HashingMethod,
                HashedPassword = hashed.HashedPassword,
                PasswordSalt = hashed.PasswordSalt,
                PasswordIterated = hashed.PasswordIterated,
            };
        }
    }
}