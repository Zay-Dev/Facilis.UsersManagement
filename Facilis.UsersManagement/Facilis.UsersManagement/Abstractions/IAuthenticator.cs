using Facilis.Core.Abstractions;
using Facilis.Core.Enums;
using Facilis.UsersManagement.Enums;
using System;
using System.Linq;

namespace Facilis.UsersManagement.Abstractions
{
    public delegate void AuthenticatedEventHandler(object sender, IUser user);

    public delegate void AuthenticateFailedEventHandler(object sender, LoginFailureTypes type, object input);

    public interface IAuthenticator
    {
        event AuthenticatedEventHandler Authenticated;

        event AuthenticateFailedEventHandler AuthenticateFailed;

        IAuthenticatedResult TryAuthenticate(IPasswordBase input);
    }

    public interface IAuthenticator<T> : IAuthenticator where T : IUser
    {
        new IAuthenticatedResult<T> TryAuthenticate(IPasswordBase input);
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

        IAuthenticatedResult IAuthenticator.TryAuthenticate(IPasswordBase input)
        {
            return this.TryAuthenticate(input);
        }

        public IAuthenticatedResult<T> TryAuthenticate(IPasswordBase input)
        {
            var failureType = LoginFailureTypes.None;
            var user = this.FindByUsername(input.Username);

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
            else if (!this.passwordHasher.Verify(user, input.Password))
            {
                failureType = LoginFailureTypes.PasswordMismatch;
            }

            if (failureType != LoginFailureTypes.None)
            {
                this.OnAuthenticateFailed(failureType, input);
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
            object input
        )
        {
            this.AuthenticateFailed?.Invoke(this, type, input);
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