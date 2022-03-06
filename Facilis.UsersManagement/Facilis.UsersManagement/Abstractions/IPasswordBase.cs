using System.Text.Json.Serialization;

namespace Facilis.UsersManagement.Abstractions
{
    public interface IPasswordBase : IAuthenticateInput
    {
        string Username { get; }
        string Password { get; }
    }

    public class PasswordBase : IPasswordBase
    {
        public const string METHOD_NAME = nameof(PasswordBase);

        public string MethodName => METHOD_NAME;
        public string UserId { get; set; }

        public string Username { get; set; }

        [JsonIgnore]
        public string Password { get; set; }
    }
}