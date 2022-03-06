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
        private IEntitiesWithId<IAuthenticationHistory> histories { get; }
        private IEntityStampsBinder stampsBinder { get; }

        public AuthenticatedEventHandler Authenticated => this.OnAuthenticated;
        public AuthenticateFailedEventHandler AuthenticateFailed => this.OnAuthenticateFailed;

        #region Constructor(s)

        public AuthenticationHistoryWriter(
            IEntitiesWithId<IAuthenticationHistory> histories,
            IEntityStampsBinder stampsBinder
        )
        {
            this.histories = histories;
            this.stampsBinder = stampsBinder;
        }

        #endregion Constructor(s)

        private void OnAuthenticated(object sender, IAuthenticateInput input, IUser user)
        {
            var history = new AuthenticationHistory()
            {
                UserId = user.Id,
                Failure = LoginFailureTypes.None,
                MethodName = input.MethodName,
                Information = JsonSerializer.Serialize(input)
            };

            this.stampsBinder.BindCreatedBySystem(history);
            this.histories.Add(history);
        }

        private void OnAuthenticateFailed(object sender, IAuthenticateInput input, LoginFailureTypes type)
        {
            var history = new AuthenticationHistory()
            {
                Failure = type,
                MethodName = input.MethodName,
                Information = JsonSerializer.Serialize(input)
            };

            this.stampsBinder.BindCreatedBySystem(history);
            this.histories.Add(history);
        }
    }
}