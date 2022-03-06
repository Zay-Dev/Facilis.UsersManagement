namespace Facilis.UsersManagement.Abstractions
{
    public interface ITokenBase : IAuthenticateInput
    {
        string TokenId { get; }
        string Value { get; }

        IUserToken UserToken { get; set; }
    }

    public class TokenBase : ITokenBase
    {
        public const string METHOD_NAME = nameof(TokenBase);

        public string MethodName => METHOD_NAME;
        public string IdentifierType { get; set; }
        public string Identifier { get; set; }

        public string TokenId { get; set; }
        public string Value { get; set; }

        public IUserToken UserToken { get; set; }
    }
}