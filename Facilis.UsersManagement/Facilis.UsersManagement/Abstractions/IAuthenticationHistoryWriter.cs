using Facilis.Core.Abstractions;
using Facilis.UsersManagement.Enums;
using System.Text.Json;

namespace Facilis.UsersManagement.Abstractions
{
    public interface IAuthenticationHistoryWriter
    {
        AuthenticatedEventHandler Authenticated { get; }
        AuthenticateFailedEventHandler AuthenticateFailed { get; }
    }

    public class AuthenticationHistoryWriter : IAuthenticationHistoryWriter
    {
        private IEntitiesWithId<AuthenticationHistory> histories { get; }
        private IEntityStampsBinder stampsBinder { get; }

        public AuthenticatedEventHandler Authenticated => this.OnAuthenticated;
        public AuthenticateFailedEventHandler AuthenticateFailed => this.OnAuthenticateFailed;

        #region Constructor(s)

        public AuthenticationHistoryWriter(
            IEntitiesWithId<AuthenticationHistory> histories,
            IEntityStampsBinder stampsBinder
        )
        {
            this.histories = histories;
            this.stampsBinder = stampsBinder;
        }

        #endregion Constructor(s)

        private void OnAuthenticated(object sender, IAuthenticateInput input, IUser user)
        {
            this.histories.Add(this.ToHistory(user.Id, input, LoginFailureTypes.None));
        }

        private void OnAuthenticateFailed(object sender, IAuthenticateInput input, LoginFailureTypes type)
        {
            this.histories.Add(this.ToHistory(null, input, type));
        }

        private AuthenticationHistory ToHistory(
            string userId,
            IAuthenticateInput input,
            LoginFailureTypes type
        )
        {
            var history = new AuthenticationHistory()
            {
                UserId = userId,
                Failure = type,
                MethodName = input.MethodName,
                Identifier = input.Identifier,
                Information = JsonSerializer.Serialize(input, input.GetType()),
            };

            this.stampsBinder.BindCreatedBySystem(history);
            return history;
        }
    }
}