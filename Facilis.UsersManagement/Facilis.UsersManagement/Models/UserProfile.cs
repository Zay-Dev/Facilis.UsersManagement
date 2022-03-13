using Facilis.Core.Attributes;
using System;

namespace Facilis.UsersManagement.Models
{
    public class UserProfile
    {
        public string Avatar { get; set; }
        public string Nickname { get; set; }
        public string Email { get; set; }
        public string CountryCode { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string[] Roles { get; set; }

        [Immutable]
        public DateTime LastSignInAtUtc { get; set; }
    }
}