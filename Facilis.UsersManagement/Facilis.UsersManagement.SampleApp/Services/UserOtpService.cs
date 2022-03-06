using Facilis.Core.Abstractions;
using Facilis.UsersManagement.Abstractions;

namespace Facilis.UsersManagement.SampleApp.Services
{
    public class UserOtpService
    {
        public const int EXPIRY_IN_MINUTES = 5;
        public const int TOKEN_LENGTH = 4;

        public static readonly Random random = new();

        private IEntities<User> users { get; }
        private IEntities<UserToken> tokens { get; }
        private IEntityStampsBinder stampsBinder { get; }
        private IPasswordHasher passwordHasher { get; }

        #region Constructor(s)

        public UserOtpService(
            IEntities<User> users,
            IEntities<UserToken> tokens,
            IEntityStampsBinder stampsBinder,
            IPasswordHasher passwordHasher
        )
        {
            this.users = users;
            this.tokens = tokens;
            this.stampsBinder = stampsBinder;
            this.passwordHasher = passwordHasher;
        }

        #endregion Constructor(s)

        public string? GetGeneratedTokenId(string username, out string? token)
        {
            var user = this.users
                .WhereAll(user => user.Username.ToLower() == username.ToLower())
                .FirstOrDefault();

            token = this.GetRandomizedToken();
            if (user == null) return null;

            var hashedToken = this.passwordHasher.Hash(token);
            var userToken = new UserToken()
            {
                HashingMethod = hashedToken.HashingMethod,
                HashedPassword = hashedToken.HashedPassword,
                PasswordSalt = hashedToken.PasswordSalt,
                PasswordIterated = hashedToken.PasswordIterated,

                UserId = user.Id,
                ExpiredAtUtc = DateTime.UtcNow.AddMinutes(EXPIRY_IN_MINUTES),
            };

            this.stampsBinder.BindCreatedBySystem(userToken);
            this.tokens.Add(userToken);

            return userToken.Id;
        }

        public string GetRandomizedToken()
        {
            var digits = Enumerable.Range(0, TOKEN_LENGTH)
                .Select(_ => $"{random.Next(0, 10)}");

            return string.Concat(digits);
        }
    }
}