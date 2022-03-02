using Facilis.Core.Abstractions;
using Facilis.Core.Enums;
using Facilis.UsersManagement.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Facilis.UsersManagement.Abstractions
{
    public interface IUser :
        IEntityWithId,
        IEntityWithStatus,
        IEntityWithCreateStamps,
        IEntityWithUpdateStamps,
        IPassword,
        IEntityWithProfile
    {
        string Username { get; }
        DateTime? LockedUntilUtc { get; }
    }

    public interface IUser<T> : IUser, IEntityWithProfile<T>
    {
    }

    public class User<T> : IUser<T>
    {
        private T profile;
        private string serializedProfile;

        public string Username { get; set; }
        public DateTime? LockedUntilUtc { get; set; }

        [NotMapped]
        public T Profile
        {
            get
            {
                this.profile ??= this.GetProfile();
                return this.profile;
            }
        }

        [NotMapped]
        public object UncastedProfile => this.Profile;

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string SerializedProfile
        {
            get => this.serializedProfile;
            set
            {
                this.serializedProfile = value;
                this.profile = this.GetProfile();
            }
        }

        public StatusTypes Status { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public string UpdatedBy { get; set; }
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        public string HashingMethod { get; set; }
        public string HashedPassword { get; set; }
        public string PasswordSalt { get; set; }
        public int PasswordIterated { get; set; }

        public T GetProfile()
        {
            return this.serializedProfile == null ? default :
                JsonSerializer.Deserialize<T>(this.serializedProfile);
        }

        public void SetProfile(object profile)
        {
            this.serializedProfile = JsonSerializer.Serialize(profile);
        }

        public void SetProfile(T profile)
        {
            this.SetProfile((object)profile);
        }
    }

    public class User : User<UserProfile>
    {
    }
}