using Facilis.Core.Abstractions;
using Facilis.Core.Enums;
using System;

namespace Facilis.UsersManagement.Abstractions
{
    public interface IInvitation :
        IEntityWithId,
        IUserRelatedEntity,
        IEntityWithStatus,
        IEntityWithCreateStamps
    {
        string InvitationType { get; }
        string SentBy { get; }
        bool IsAccepted { get; }
    }

    public class Invitation : IInvitation
    {
        public string InvitationType { get; set; }
        public string SentBy { get; set; }
        public bool IsAccepted { get; set; }

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }

        public StatusTypes Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}