using Facilis.Core.Abstractions;

namespace Facilis.UsersManagement.Abstractions
{
    public interface IInvitations
    {
        void Accept(string id);

        void Send(string from, string to, string type);
    }

    public class Invitations : IInvitations
    {
        private IEntitiesWithId<Invitation> invitations { get; }
        private IEntityStampsBinder stampsBinder { get; }

        #region Constructor(s)

        public Invitations(
            IEntitiesWithId<Invitation> invitations,
            IEntityStampsBinder stampsBinder
        )
        {
            this.invitations = invitations;
            this.stampsBinder = stampsBinder;
        }

        #endregion Constructor(s)

        public void Accept(string id)
        {
            this.invitations.FindById(id).IsAccepted = true;
            this.invitations.Save();
        }

        public void Send(string from, string to, string type)
        {
            var invitation = new Invitation()
            {
                InvitationType = type,
                SentBy = from,
                UserId = to,
            };

            this.stampsBinder.BindCreatedByUser(invitation);
            this.invitations.Add(invitation);
        }
    }
}