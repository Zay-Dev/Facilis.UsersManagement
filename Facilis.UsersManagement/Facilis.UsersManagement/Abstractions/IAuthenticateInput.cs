namespace Facilis.UsersManagement.Abstractions
{
    public interface IAuthenticateInput
    {
        string MethodName { get; }
        string Identifier { get; set; }
    }
}