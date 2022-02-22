using Facilis.Core.Abstractions;
using Facilis.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;
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
        DateTime? LockedUntilUtc { get; }
    }

    public class User<TProfile> : IUser
    {
        public string Username { get; set; }
        public DateTime? LockedUntilUtc { get; set; }

        [NotMapped]
        public object Profile { get; set; }

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

        #region Constructor(s)

        public User()
        {
            this.Profile = this.GetProfile<TProfile>();
        }

        #endregion Constructor(s)

        public T GetProfile<T>()
        {
            return this.SerializedProfile == null ? default :
                JsonSerializer.Deserialize<T>(this.SerializedProfile);
        }

        public void SetProfile(object profile)
        {
            this.Profile = profile;
            this.SerializedProfile = JsonSerializer.Serialize(profile);
        }
    }
}