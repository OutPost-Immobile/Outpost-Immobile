namespace OutpostImmobile.Communication.Interfaces;

public interface IMailService
{
    public void SendMessage(string mailAddress, string recipientName, string subject, string body);
}