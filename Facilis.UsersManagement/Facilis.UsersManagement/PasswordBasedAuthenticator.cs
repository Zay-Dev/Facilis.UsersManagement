using Facilis.Core.Abstractions;
using Facilis.UsersManagement.Abstractions;
using Facilis.UsersManagement.Enums;
using System.Linq;

namespace Facilis.UsersManagement
{
    public class PasswordBasedAuthenticator<T> : BaseAuthenticator<IPasswordBase, T>
        where T : IUser
    {
        private IEntities<T> users { get; }
        private IPasswordHasher passwordHasher { get; }

        #region Constructor(s)

        public PasswordBasedAuthenticator(IEntities<T> entities, IPasswordHasher passwordHasher)
        {
            this.users = entities;
            this.passwordHasher = passwordHasher;
        }

        #endregion Constructor(s)

        protected override LoginFailureTypes Authenticate(T user, IPasswordBase input)
        {
            return this.passwordHasher.Verify(user, input.Password) ?
                LoginFailureTypes.None :
                LoginFailureTypes.PasswordMismatch;
        }

        protected override LoginFailureTypes TryFindUser(IPasswordBase input, out T user)
        {
            user = this.users.Rows.FirstOrDefault(entity =>
                entity.Username.ToLower() == input.Username.ToLower()
            );
            return LoginFailureTypes.None;
        }
    }
}