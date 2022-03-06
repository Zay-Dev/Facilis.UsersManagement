using Facilis.Core.Abstractions;

namespace Facilis.UsersManagement.Tests.Services
{
    public class PlainPasswordHashService : IPasswordHasher
    {
        public IPassword Hash(string value)
        {
            return new Password()
            {
                HashedPassword = value,
            };
        }

        public bool Verify(IPassword hashed, string value)
        {
            return hashed.HashedPassword == value;
        }
    }
}