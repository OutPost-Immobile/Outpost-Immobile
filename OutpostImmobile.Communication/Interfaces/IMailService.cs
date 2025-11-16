using OutpostImmobile.Communication.Services;

namespace OutpostImmobile.Communication.Interfaces;

public interface IMailService
{
    public void SendMessage(SendEmailRequest request);
}