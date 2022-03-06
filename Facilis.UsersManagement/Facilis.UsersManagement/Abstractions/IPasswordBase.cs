namespace Facilis.UsersManagement.Abstractions
{
    public interface IPasswordBase
    {
        public string Username { get; }
        public string Password { get; }
    }

    public class PasswordBase : IPasswordBase
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }
}