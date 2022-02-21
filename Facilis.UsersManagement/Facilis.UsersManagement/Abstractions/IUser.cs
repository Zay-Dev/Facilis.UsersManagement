using Facilis.Core.Abstractions;
using Facilis.Core.Enums;
using System;
using System.Text.Json;

namespace Facilis.UsersManagement.Abstractions
{
    public interface IUser :
        IEntityWithId,
        IEntityWithProfile,
        IEntityWithStatus,
        IEntityWithCreateStamps,
        IEntityWithUpdateStamps,
        IPassword
    {
        string Username { get; }
        string Nickname { get; }
        DateTime? LockedUntilUtc { get; }
    }

    public class User : IUser
    {
        public string Username { get; set; }
        public string Nickname { get; set; }
        public DateTime? LockedUntilUtc { get; set; }

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string SerializedProfile { get; set; }
        public StatusTypes Status { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public string UpdatedBy { get; set; }
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        public string HashingMethod { get; set; }
        public string HashedPassword { get; set; }
        public string PasswordSalt { get; set; }
        public int PasswordIterated { get; set; }

        public T GetProfile<T>() where T : class, new()
        {
            return JsonSerializer.Deserialize<T>(this.SerializedProfile);
        }

        public void SetProfile(object profile)
        {
            this.SerializedProfile = JsonSerializer.Serialize(profile);
        }
    }
}