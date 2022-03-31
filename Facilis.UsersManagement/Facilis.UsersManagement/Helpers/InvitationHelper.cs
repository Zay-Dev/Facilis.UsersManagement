using Facilis.Core.Abstractions;
using Facilis.UsersManagement.Abstractions;
using System.Linq;

namespace Facilis.UsersManagement.Helpers
{
    public static class InvitationHelper
    {
        public static IQueryable<T> FindSentInvitations<T>(
            this IEntitiesWithId<T> entities,
            string type,
            string userId,
            bool onlyAccepted = false
        ) where T : IInvitation
        {
            return entities
                .Where(entity => entity.InvitationType == type &&
                    entity.SentBy == userId &&
                    (
                        onlyAccepted == false ||
                        entity.IsAccepted
                    )
                );
        }

        public static IQueryable<T> FindReceivedInvitations<T>(
            this IEntitiesWithId<T> entities,
            string type,
            string userId,
            bool onlyAccepted = false
        ) where T : IInvitation
        {
            return entities
                .Where(entity => entity.InvitationType == type &&
                    entity.UserId == userId &&
                    (
                        onlyAccepted == false ||
                        entity.IsAccepted
                    )
                );
        }
    }
}