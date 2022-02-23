using Facilis.Core.Abstractions;
using Facilis.UsersManagement.Abstractions;
using Facilis.UsersManagement.Models;

namespace Facilis.UsersManagement.Tests.Helpers
{
    public static class UserHelper
    {
        public static User<UserProfile> CreateUser(
            this IPasswordHasher passwordHasher,
            string username,
            string password
        )
        {
            var hashed = passwordHasher.Hash(password);

            return new User<UserProfile>()
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