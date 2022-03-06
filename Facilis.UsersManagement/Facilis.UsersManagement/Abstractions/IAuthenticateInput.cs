namespace Facilis.UsersManagement.Abstractions
{
    public interface IAuthenticateInput
    {
        string MethodName { get; }
        string IdentifierType { get; }
        string Identifier { get; }
    }
}