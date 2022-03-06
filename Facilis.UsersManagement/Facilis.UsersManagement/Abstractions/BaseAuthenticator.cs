using Facilis.Core.Enums;
using Facilis.UsersManagement.Enums;
using System;

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
            var failure = this.TryFindUser(input, out var user);
            if (failure == LoginFailureTypes.None)
            {
                failure = this.ValidateUserStatus(user);

                if (failure == LoginFailureTypes.None)
                {
                    failure = this.Authenticate(user, input);
                }
            }

            this.InvokePostEvent(failure, input, user);

            return new AuthenticatedResult<TUser>()
            {
                Failure = failure,
                UncastedUser = user,
                User = user,
            };
        }

        protected abstract LoginFailureTypes TryFindUser(TInput input, out TUser user);

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

        private void InvokePostEvent(LoginFailureTypes failure, TInput input, TUser user)
        {
            if (failure != LoginFailureTypes.None)
            {
                this.OnAuthenticateFailed(input, failure);
            }
            else
            {
                this.OnAuthenticated(input, user);
            }
        }

        private static bool IsLocked(TUser user)
        {
            return user.LockedUntilUtc.HasValue &&
                user.LockedUntilUtc > DateTime.UtcNow;
        }
    }
}