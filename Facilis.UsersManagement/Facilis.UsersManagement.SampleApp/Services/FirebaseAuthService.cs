using Facilis.Core.Abstractions;
using Facilis.Core.Enums;
using Facilis.UsersManagement.Abstractions;
using Facilis.UsersManagement.Models;
using Facilis.UsersManagement.SampleApp.Enums;
using FirebaseAdmin.Auth;

namespace Facilis.UsersManagement.SampleApp.Services
{
    public class FirebaseAuthService
    {
        private IEntities<User> users { get; }
        private IPasswordHasher hasher { get; }
        private IEntityStampsBinder stampsBinder { get; }
        private IAuthenticator<IUserIdBase, User> authenticator { get; }

        #region Constructor(s)

        public FirebaseAuthService(
            IEntities<User> users,
            IPasswordHasher hasher,
            IEntityStampsBinder stampsBinder,
            IAuthenticator<IUserIdBase, User> authenticator
        )
        {
            this.users = users;
            this.hasher = hasher;
            this.stampsBinder = stampsBinder;
            this.authenticator = authenticator;
        }

        #endregion Constructor(s)

        public async Task<IAuthenticatedResult<User>> SignIn(string accessToken)
        {
            var decoded = await FirebaseAuth.DefaultInstance
                .VerifyIdTokenAsync(accessToken);

            var user = this.users.FindById(decoded.Uid);
            if (user == null || user.Status == StatusTypes.Deleted)
            {
                this.CreateUser(decoded);
            }

            return this.authenticator.TryAuthenticate(new UserIdBase()
            {
                MethodName = nameof(FirebaseAuthService),
                UserId = decoded.Uid,
            });
        }

        private void CreateUser(FirebaseToken decoded)
        {
            var password = Guid.NewGuid().ToString();
            var user = new User(this.hasher.Hash(password))
            {
                Id = decoded.Uid,
                Username = $"{decoded.Claims["email"]}",
            };
            user.SetProfile(new UserProfile()
            {
                Nickname = $"{decoded.Claims["name"]}",
                Email = $"{decoded.Claims["email"]}",
                FirstName = $"{decoded.Claims["name"]}".Split(' ').FirstOrDefault(),
                LastName = $"{decoded.Claims["name"]}".Split(' ').LastOrDefault(),
                Roles = new[] { nameof(RoleTypes.User) },
                LastSignInAtUtc = DateTime.UtcNow,
            });

            this.stampsBinder.BindCreatedBySystem(user);
            this.users.Add(user);
        }
    }
}