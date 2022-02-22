using System;

namespace Facilis.UsersManagement.Models
{
    public class UserProfile
    {
        public string[] Roles { get; set; }
        public DateTime LastSignInAtUtc { get; set; }
    }
}