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
            var token = this.tokens.FindById(input.TokenId);
            user = default;

            if (token == null)
            {
                return LoginFailureTypes.TokenNotFound;
            }
            else if (token.ExpiredAtUtc <= DateTime.UtcNow)
            {
                return LoginFailureTypes.TokenIsExpired;
            }

            input.UserToken = token;
            user = this.users.FindById(input.UserToken.UserId);
            return LoginFailureTypes.None;
        }

        private void TokenBasedAuthenticator_Authenticated(object sender, IAuthenticateInput input, IUser user)
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