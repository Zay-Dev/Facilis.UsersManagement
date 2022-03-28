using Facilis.Core.Abstractions;

namespace Facilis.UsersManagement.Abstractions
{
    public delegate void InvitationSentEventHandler(object sender, IInvitation invitation);

    public delegate void InvitationAcceptedEventHandler(object sender, IInvitation invitation);

    public interface IInvitationService
    {
        event InvitationSentEventHandler InvitationSent;

        event InvitationAcceptedEventHandler InvitationAccepted;

        void Accept(string id);

        void Send(string from, string to, string type, bool isAutoAccept = false);
    }

    public class InvitationService : IInvitationService
    {
        private IEntitiesWithId<Invitation> invitations { get; }
        private IEntityStampsBinder stampsBinder { get; }

        public event InvitationSentEventHandler InvitationSent;

        public event InvitationAcceptedEventHandler InvitationAccepted;

        #region Constructor(s)

        public InvitationService(
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
            var invitation = this.invitations.FindById(id);
            invitation.IsAccepted = true;

            this.invitations.Update(invitation);
            this.invitations.Save();

            this.InvitationAccepted(this, invitation);
        }

        public void Send(string from, string to, string type, bool isAutoAccept = false)
        {
            var invitation = new Invitation()
            {
                InvitationType = type,
                SentBy = from,
                UserId = to,
            };

            this.stampsBinder.BindCreatedByUser(invitation);
            this.invitations.Add(invitation);

            this.InvitationSent(this, invitation);
            if (isAutoAccept) this.Accept(invitation.Id);
        }
    }
}