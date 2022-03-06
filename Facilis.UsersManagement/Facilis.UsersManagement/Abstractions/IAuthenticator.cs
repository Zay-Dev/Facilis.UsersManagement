using Facilis.UsersManagement.Enums;

namespace Facilis.UsersManagement.Abstractions
{
    public delegate void AuthenticatedEventHandler(object sender, IAuthenticateInput input, IUser user);

    public delegate void AuthenticateFailedEventHandler(object sender, IAuthenticateInput input, LoginFailureTypes type);

    public interface IAuthenticator<TInput, TUser>
        where TInput : IAuthenticateInput
        where TUser : IUser
    {
        event AuthenticatedEventHandler Authenticated;

        event AuthenticateFailedEventHandler AuthenticateFailed;

        IAuthenticatedResult<TUser> TryAuthenticate(TInput input);
    }
}