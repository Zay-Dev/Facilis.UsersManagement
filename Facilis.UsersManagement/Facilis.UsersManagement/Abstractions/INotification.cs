using Facilis.Core.Abstractions;
using System;

namespace Facilis.UsersManagement.Abstractions
{
    public interface INotification :
        IEntityWithId,
        IUserRelatedEntity,
        IEntityWithExpiration,
        IEntityWithCreateStamps
    {
        string NotificationType { get; }
        string ReferenceId { get; }
    }

    public class Notification : INotification
    {
        public string NotificationType { get; set; }
        public string ReferenceId { get; set; }

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }

        public DateTime ExpiredAtUtc { get; set; } = DateTime.MaxValue;
        public string CreatedBy { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}