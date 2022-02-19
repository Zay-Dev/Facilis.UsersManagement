using Facilis.Core.Abstractions;
using Facilis.Core.Enums;
using Facilis.UsersManagement.Enums;
using System;
using System.Linq;

namespace Facilis.UsersManagement.Abstractions
{
    public delegate void AuthenticateFailedEventHandler(object sender, LoginFailureTypes type, string username, string password);

    public interface IAuthenticator
    {
        event AuthenticateFailedEventHandler AuthenticateFailed;

        LoginFailureTypes TryAuthenticate(string username, string password, out IUser user);
    }

    public class Authenticator<T> : IAuthenticator where T : IUser
    {
        private IEntities<T> entities { get; }
        private IPasswordHasher passwordHasher { get; }

        public event AuthenticateFailedEventHandler AuthenticateFailed;

        #region Constructor(s)

        public Authenticator(IEntities<T> entities, IPasswordHasher passwordHasher)
        {
            this.entities = entities;
            this.passwordHasher = passwordHasher;
        }

        #endregion Constructor(s)

        public LoginFailureTypes TryAuthenticate(string username, string password, out IUser user)
        {
            var failureType = LoginFailureTypes.None;
            user = this.FindByUsername(username);

            if (user == null)
            {
                failureType = LoginFailureTypes.UserNotFound;
            }
            else if (user.Status == StatusTypes.Disabled)
            {
                failureType = LoginFailureTypes.DisabledUser;
            }
            else if (IsLocked(user))
            {
                failureType = LoginFailureTypes.LockedUser;
            }
            else if (!this.passwordHasher.Verify(user, password))
            {
                failureType = LoginFailureTypes.PasswordMismatch;
            }

            if (failureType != LoginFailureTypes.None)
            {
                this.OnAuthenticateFailed(failureType, username, password);
            }

            return failureType;
        }

        private IUser FindByUsername(string username)
        {
            return this.entities.Rows.FirstOrDefault(entity =>
                entity.Username.ToLower() == username.ToLower()
            );
        }

        protected virtual void OnAuthenticateFailed(
            LoginFailureTypes type,
            string username,
            string password
        )
        {
            this.AuthenticateFailed?.Invoke(this, type, username, password);
        }

        private static bool IsLocked(IUser user)
        {
            return user.LockedUntilUtc.HasValue &&
                user.LockedUntilUtc > DateTime.UtcNow;
        }
    }
}