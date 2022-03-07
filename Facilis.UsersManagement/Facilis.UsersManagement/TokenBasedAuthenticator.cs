using Facilis.Core.Abstractions;
using Facilis.Core.Enums;
using Facilis.UsersManagement.Abstractions;
using Facilis.UsersManagement.Enums;
using System;

namespace Facilis.UsersManagement
{
    public class TokenBasedAuthenticator<TUserToken, TUser> :
        BaseAuthenticator<ITokenBase, TUser>
        where TUserToken : IUserToken
        where TUser : IUser
    {
        private IEntities<TUser> users { get; }
        private IEntities<TUserToken> tokens { get; }
        private IPasswordHasher passwordHasher { get; }

        #region Constructor(s)

        public TokenBasedAuthenticator(
            IEntities<TUser> users,
            IEntities<TUserToken> tokens,
            IPasswordHasher passwordHasher
        )
        {
            this.users = users;
            this.tokens = tokens;
            this.passwordHasher = passwordHasher;

            this.Authenticated += this.TokenBasedAuthenticator_Authenticated;
        }

        #endregion Constructor(s)

        protected override LoginFailureTypes Authenticate(TUser user, ITokenBase input)
        {
            return this.passwordHasher.Verify(input.UserToken, input.Value) ?
                LoginFailureTypes.None :
                LoginFailureTypes.TokenMismatch;
        }

        protected override LoginFailureTypes TryFindUser(ITokenBase input, out TUser user)
        {
            user = default;

            input.UserToken = this.tokens.FindEnabledById(input.TokenId);
            if (input.UserToken == null) return LoginFailureTypes.TokenNotFound;

            user = this.users.FindById(input.UserToken.UserId);
            if (user == null) return LoginFailureTypes.UserNotFound;

            input.UserId = user.Id;

            if (input.UserToken.ExpiredAtUtc <= DateTime.UtcNow)
            {
                return LoginFailureTypes.TokenIsExpired;
            }

            return LoginFailureTypes.None;
        }

        protected virtual void TokenBasedAuthenticator_Authenticated(
            object sender,
            IAuthenticateInput input,
            IUser user
        )
        {
            foreach (var token in this.tokens
                .WhereEnabled(token => token.UserId == user.Id)
            )
            {
                token.Status = StatusTypes.Disabled;
                this.tokens.UpdateNoSave(token);
            }

            this.tokens.Save();
        }
    }
}