namespace Facilis.UsersManagement.Abstractions
{
    public interface IUserIdBase : IAuthenticateInput
    {
    }

    public class UserIdBase : IUserIdBase
    {
        public string MethodName { get; set; }

        public string UserId { get; set; }
    }
}