using OutpostImmobile.Communication.Options;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using OutpostImmobile.Communication.Interfaces;

namespace OutpostImmobile.Communication.Services;

public record SendEmailRequest
{
    public required string RecipientMailAddress { get; init; }
    public required string RecipientName { get; init; }
    public required string MailSubject { get; init; }
    public required string MailBody { get; init; }
}

public class MailService : IMailService
{
    private readonly MailOptions _mailOptions;
    public MailService(IOptions<MailOptions> mailOptions)
    {
        _mailOptions = mailOptions.Value;
    }
    public void SendMessage(SendEmailRequest request)
    {
        // Create and configure email
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_mailOptions.Sender, _mailOptions.SenderMailAddress));
        message.To.Add(new MailboxAddress(request.RecipientName, request.RecipientMailAddress));
        message.Subject = request.MailSubject;
        message.Body = new TextPart("plain")  { Text = request.MailBody };
        // Log onto client and send the email
        using (var client = new SmtpClient()) {
            client.Connect(_mailOptions.SmtpHost, _mailOptions.Port, false);
            client.Authenticate(_mailOptions.SenderMailAddress, _mailOptions.SenderPassword);
            client.Send(message);
            client.Disconnect(true);
        }
    }
}