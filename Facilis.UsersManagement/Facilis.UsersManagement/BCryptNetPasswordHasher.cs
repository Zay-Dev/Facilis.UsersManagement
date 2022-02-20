using Facilis.Core.Abstractions;

namespace Facilis.UsersManagement
{
    public class BCryptNetPasswordHasher : IPasswordHasher
    {
        public IPassword Hash(string value)
        {
            var salt = BCrypt.Net.BCrypt.GenerateSalt();

            return new Password()
            {
                HashingMethod = nameof(BCrypt),
                HashedPassword = BCrypt.Net.BCrypt.HashPassword(value, salt),
                PasswordSalt = salt
            };
        }

        public bool Verify(IPassword hashed, string value)
        {
            return BCrypt.Net.BCrypt.Verify(value, hashed.HashedPassword);
        }

        private class Password : IPassword
        {
            public string HashingMethod { get; set; }
            public string HashedPassword { get; set; }
            public string PasswordSalt { get; set; }
            public int PasswordIterated { get; set; }
        }
    }
}