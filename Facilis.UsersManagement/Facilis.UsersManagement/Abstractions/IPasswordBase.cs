namespace Facilis.UsersManagement.Abstractions
{
    public interface IPasswordBase : IAuthenticateInput
    {
        public string Username { get; }
        public string Password { get; }
    }

    public class PasswordBase : IPasswordBase
    {
        public const string METHOD_NAME = nameof(PasswordBase);

        public string MethodName => METHOD_NAME;

        public string Username { get; set; }
        public string Password { get; set; }
    }
}