using Facilis.Core.Abstractions;
using Facilis.UsersManagement.Abstractions;
using Facilis.UsersManagement.Enums;
using System.Linq;

namespace Facilis.UsersManagement
{
    public class UserIdBasedAuthenticator<TUser> :
        BaseAuthenticator<IUserIdBase, TUser>
        where TUser : IUser
    {
        private IEntities<TUser> users { get; }

        #region Constructor(s)

        public UserIdBasedAuthenticator(IEntities<TUser> users)
        {
            this.users = users;
        }

        #endregion Constructor(s)

        protected override LoginFailureTypes Authenticate(TUser user, IUserIdBase input)
        {
            return LoginFailureTypes.None;
        }

        protected override LoginFailureTypes TryFindUser(IUserIdBase input, out TUser user)
        {
            user = this.users
                .WhereAll(user => user.Id == input.UserId)
                .FirstOrDefault();

            return LoginFailureTypes.None;
        }
    }
}