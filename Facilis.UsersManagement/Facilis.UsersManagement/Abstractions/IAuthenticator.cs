using Facilis.UsersManagement.Enums;

namespace Facilis.UsersManagement.Abstractions
{
    public delegate void OnAuthenticateFailedEventHandler(object sender, string username, string password);

    public interface IAuthenticator
    {
        event OnAuthenticateFailedEventHandler OnAuthenticateFailed;

        LoginFailureTypes TryAuthenticate(string username, string password, out IUser user);
    }
}