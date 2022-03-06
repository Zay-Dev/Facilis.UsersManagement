using Facilis.Core.Abstractions;
using Facilis.Core.Enums;
using Facilis.UsersManagement.Enums;
using System;
using System.Linq;

namespace Facilis.UsersManagement.Abstractions
{
    public abstract class BaseAuthenticator<TInput, TUser> :
        IAuthenticator<TInput, TUser>
        where TInput : IAuthenticateInput
        where TUser : IUser
    {
        public event AuthenticatedEventHandler Authenticated;

        public event AuthenticateFailedEventHandler AuthenticateFailed;

        public IAuthenticatedResult<TUser> TryAuthenticate(TInput input)
        {
            var user = this.FindUser(input);
            var failure = this.ValidateUserStatus(user);

            if (failure == LoginFailureTypes.None)
            {
                failure = this.Authenticate(user, input);
            }

            if (failure != LoginFailureTypes.None)
            {
                this.OnAuthenticateFailed(input, failure);
            }
            else
            {
                this.OnAuthenticated(input, user);
            }

            return new AuthenticatedResult<TUser>()
            {
                Failure = failure,
                UncastedUser = user,
                User = user,
            };
        }

        protected abstract TUser FindUser(TInput input);

        protected abstract LoginFailureTypes Authenticate(TUser user, TInput input);

        protected virtual LoginFailureTypes ValidateUserStatus(TUser user)
        {
            if (user == null)
            {
                return LoginFailureTypes.UserNotFound;
            }
            else if (user.Status == StatusTypes.Disabled)
            {
                return LoginFailureTypes.DisabledUser;
            }
            else if (IsLocked(user))
            {
                return LoginFailureTypes.LockedUser;
            }

            return LoginFailureTypes.None;
        }

        protected virtual void OnAuthenticated(IAuthenticateInput input, TUser user)
        {
            this.Authenticated?.Invoke(this, input, user);
        }

        protected virtual void OnAuthenticateFailed(
            IAuthenticateInput input,
            LoginFailureTypes type
        )
        {
            this.AuthenticateFailed?.Invoke(this, input, type);
        }

        private static bool IsLocked(TUser user)
        {
            return user.LockedUntilUtc.HasValue &&
                user.LockedUntilUtc > DateTime.UtcNow;
        }
    }

    public class PasswordBasedAuthenticator<TUser> : BaseAuthenticator<IPasswordBase, TUser>
        where TUser : IUser
    {
        private IEntities<TUser> entities { get; }
        private IPasswordHasher passwordHasher { get; }

        #region Constructor(s)

        public PasswordBasedAuthenticator(IEntities<TUser> entities, IPasswordHasher passwordHasher)
        {
            this.entities = entities;
            this.passwordHasher = passwordHasher;
        }

        #endregion Constructor(s)

        protected override TUser FindUser(IPasswordBase input)
        {
            return this.entities.Rows.FirstOrDefault(entity =>
                entity.Username.ToLower() == input.Username.ToLower()
            );
        }

        protected override LoginFailureTypes Authenticate(TUser user, IPasswordBase input)
        {
            return this.passwordHasher.Verify(user, input.Password) ?
                LoginFailureTypes.None :
                LoginFailureTypes.PasswordMismatch;
        }
    }
}