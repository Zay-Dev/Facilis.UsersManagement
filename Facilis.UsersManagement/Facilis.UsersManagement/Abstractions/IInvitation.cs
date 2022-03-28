using Facilis.Core.Abstractions;
using System;

namespace Facilis.UsersManagement.Abstractions
{
    public interface IInvitation :
        IEntityWithId,
        IUserRelatedEntity,
        IEntityWithCreateStamps
    {
        string InvitationType { get; }
        string SentBy { get; }
        bool IsAccepted { get; set; }
    }

    public class Invitation : IInvitation
    {
        public string InvitationType { get; set; }
        public string SentBy { get; set; }
        public bool IsAccepted { get; set; }

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}