namespace Facilis.UsersManagement.Abstractions
{
    public interface IAuthenticateInput
    {
        string MethodName { get; }
        string UserId { get; set; }
    }
}