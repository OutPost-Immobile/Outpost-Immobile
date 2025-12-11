using OutpostImmobile.Communication.Services;

namespace OutpostImmobile.Communication.Interfaces;

public interface IMailService
{
    public Task SendMessage(SendEmailRequest request);
}