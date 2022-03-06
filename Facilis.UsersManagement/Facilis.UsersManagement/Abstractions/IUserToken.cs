using Facilis.Core.Abstractions;
using Facilis.Core.Enums;
using System;

namespace Facilis.UsersManagement.Abstractions
{
    public interface IUserToken :
        IEntityWithId,
        IEntityWithStatus,
        IEntityWithCreateStamps,
        IPassword,
        IUserRelatedEntity,
        IEntityWithExpiration
    {
    }

    public class UserToken : IUserToken
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public StatusTypes Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public string HashingMethod { get; set; }
        public string HashedPassword { get; set; }
        public string PasswordSalt { get; set; }
        public int PasswordIterated { get; set; }

        public string UserId { get; set; }
        public DateTime ExpiredAtUtc { get; set; }
    }
}