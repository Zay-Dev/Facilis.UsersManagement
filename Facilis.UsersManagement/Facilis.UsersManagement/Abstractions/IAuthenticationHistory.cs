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
        string MethodName { get; }
        string Information { get; }
    }

    public class AuthenticationHistory : IAuthenticationHistory
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string CreatedBy { get; set; }
        public DateTime CreatedAtUtc { get; set; }

        public string UserId { get; set; }
        public LoginFailureTypes Failure { get; set; }
        public string MethodName { get; set; }
        public string Information { get; set; }
    }
}