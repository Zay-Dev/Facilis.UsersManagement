using Facilis.Core.Abstractions;
using System;
using System.Linq;

namespace Facilis.UsersManagement.Abstractions
{
    public interface IUserFinder<T> where T : IUser
    {
        IQueryable<T> FindEnabledBy(string type, string text);
    }

    public class UserFinder : IUserFinder<User>
    {
        private IEntities<User> users { get; }
        private IScopedEntities<ExtendedAttribute> attributes { get; }

        #region Constructor(s)

        public UserFinder(IEntities<User> users, IScopedEntities<ExtendedAttribute> attributes)
        {
            this.users = users;
            this.attributes = attributes;
        }

        #endregion Constructor(s)

        public IQueryable<User> FindEnabledBy(string type, string text)
        {
            return this.users
                .WhereEnabled(user =>
                    (type == null || type == nameof(User.Username)) &&
                    user.Username.ToLower().Contains(text)
                )
                .Concat(this.FindEnabledByAttributes(type, text))
                .Distinct();
        }

        private IQueryable<User> FindEnabledByAttributes(string type, string text)
        {
            var userIds = this.attributes
                .WhereEnabled(attribute =>
                    (type == null || attribute.Key == type) &&
                    attribute.Value.ToLower().Contains(text)
                )
                .Select(user => user.Id);

            return this.users.WhereEnabled(user => userIds.Contains(user.Id));
        }
    }
}