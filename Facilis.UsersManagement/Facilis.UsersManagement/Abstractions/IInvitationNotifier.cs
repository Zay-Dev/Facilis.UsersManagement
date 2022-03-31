namespace Facilis.UsersManagement.Abstractions
{
    public interface IInvitationNotifier
    {
        public void Notify(IInvitation invitation);
    }
}