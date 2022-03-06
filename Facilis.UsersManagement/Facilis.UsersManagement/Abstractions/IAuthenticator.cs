using Facilis.Core.Abstractions;
using Facilis.Core.Enums;
using Facilis.UsersManagement.Enums;
using System;
using System.Linq;

namespace Facilis.UsersManagement.Abstractions
{
    public delegate void AuthenticatedEventHandler(object sender, IUser user);

    public delegate void AuthenticateFailedEventHandler(object sender, LoginFailureTypes type, string username, string password);

    public interface IAuthenticator
    {
        event AuthenticatedEventHandler Authenticated;

        event AuthenticateFailedEventHandler AuthenticateFailed;

        IAuthenticatedResult TryAuthenticate(string username, string password);
    }

    public interface IAuthenticator<T> : IAuthenticator where T : IUser
    {
        new IAuthenticatedResult<T> TryAuthenticate(string username, string password);
    }

    public class Authenticator<T> : IAuthenticator<T>
        where T : IUser
    {
        private IEntities<T> entities { get; }
        private IPasswordHasher passwordHasher { get; }

        public event AuthenticatedEventHandler Authenticated;

        public event AuthenticateFailedEventHandler AuthenticateFailed;

        #region Constructor(s)

        public Authenticator(IEntities<T> entities, IPasswordHasher passwordHasher)
        {
            this.entities = entities;
            this.passwordHasher = passwordHasher;
        }

        #endregion Constructor(s)

        IAuthenticatedResult IAuthenticator.TryAuthenticate(string username, string password)
        {
            return this.TryAuthenticate(username, password);
        }

        public IAuthenticatedResult<T> TryAuthenticate(string username, string password)
        {
            var failureType = LoginFailureTypes.None;
            var user = this.FindByUsername(username);

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
            else
            {
                this.OnAuthenticated(user);
            }

            return new AuthenticatedResult<T>()
            {
                Failure = failureType,
                UncastedUser = user,
                User = user,
            };
        }

        protected virtual void OnAuthenticated(T user)
        {
            this.Authenticated?.Invoke(this, user);
        }

        protected virtual void OnAuthenticateFailed(
            LoginFailureTypes type,
            string username,
            string password
        )
        {
            this.AuthenticateFailed?.Invoke(this, type, username, password);
        }

        private T FindByUsername(string username)
        {
            return this.entities.Rows.FirstOrDefault(entity =>
                entity.Username.ToLower() == username.ToLower()
            );
        }

        private static bool IsLocked(T user)
        {
            return user.LockedUntilUtc.HasValue &&
                user.LockedUntilUtc > DateTime.UtcNow;
        }
    }
}