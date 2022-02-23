using Facilis.Core.Attributes;
using System;

namespace Facilis.UsersManagement.Models
{
    public class UserProfile
    {
        public string[] Roles { get; set; }

        [Immutable]
        public DateTime LastSignInAtUtc { get; set; }
    }
}