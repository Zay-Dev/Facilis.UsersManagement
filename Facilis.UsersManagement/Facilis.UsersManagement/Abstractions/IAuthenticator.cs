using Facilis.UsersManagement.Enums;

namespace Facilis.UsersManagement.Abstractions
{
    public delegate void AuthenticatedEventHandler(object sender, IAuthenticateInput input, IUser user);

    public delegate void AuthenticateFailedEventHandler(object sender, IAuthenticateInput input, LoginFailureTypes type);

    public interface IAuthenticator
    {
        event AuthenticatedEventHandler Authenticated;

        event AuthenticateFailedEventHandler AuthenticateFailed;
    }

    public interface IAuthenticator<TInput, TUser> : IAuthenticator
        where TInput : IAuthenticateInput
        where TUser : IUser
    {
        IAuthenticatedResult<TUser> TryAuthenticate(TInput input);
    }
}