using Facilis.UsersManagement.Enums;

namespace Facilis.UsersManagement.Abstractions
{
    public interface IAuthenticatedResult
    {
        LoginFailureTypes Failure { get; }

        IUser UncastedUser { get; }

        bool HasFailure();
    }

    public interface IAuthenticatedResult<T> : IAuthenticatedResult
        where T : IUser
    {
        T User { get; }
    }

    public class AuthenticatedResult : IAuthenticatedResult
    {
        public LoginFailureTypes Failure { get; set; }

        public IUser UncastedUser { get; set; }

        public virtual bool HasFailure() => this.Failure != LoginFailureTypes.None;
    }

    public class AuthenticatedResult<T> :
        AuthenticatedResult, IAuthenticatedResult<T>
        where T : IUser
    {
        public T User { get; set; }
    }
}