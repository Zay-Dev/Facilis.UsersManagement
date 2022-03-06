using Facilis.Core.Abstractions;
using Facilis.UsersManagement.Enums;
using System;

namespace Facilis.UsersManagement.Abstractions
{
    public interface IAuthenticationHistory :
        IEntityWithId,
        IEntityWithCreateStamps,
        IUserRelatedEntity
    {
        LoginFailureTypes Failure { get; }
    }

    public class AuthenticationHistory : IAuthenticationHistory
    {
        public string Id { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedAtUtc { get; set; }

        public string UserId { get; set; }
        public LoginFailureTypes Failure { get; set; }
    }
}