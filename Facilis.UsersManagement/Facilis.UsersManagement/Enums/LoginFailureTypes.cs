namespace Facilis.UsersManagement.Enums
{
    public enum LoginFailureTypes
    {
        None,
        UserNotFound,
        DisabledUser,
        LockedUser,
        PasswordMismatch,
        TokenMismatch,
        TokenNotFound,
        TokenIsExpired,
    }
}