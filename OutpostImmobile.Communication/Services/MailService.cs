using Communication.Options;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using OutpostImmobile.Communication.Interfaces;

namespace OutpostImmobile.Communication.Services;

public class MailService(IOptions<MailOptions> mailOptions) : IMailService
{
    private readonly MailOptions _mailOptions = mailOptions.Value;

    public void SendMessage(string mailAddress, string recipientName, string subject, string body)
    {
        // Create and configure email
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_mailOptions.Sender, _mailOptions.SenderMailAddress));
        message.To.Add(new MailboxAddress(recipientName, mailAddress));
        message.Subject = subject;
        message.Body = new TextPart("plain")  { Text = body };
        // Log onto client and send the email
        using (var client = new SmtpClient()) {
            client.Connect(_mailOptions.SmtpHost, _mailOptions.port, true);
            client.Authenticate(_mailOptions.Sender, _mailOptions.SenderPassword);
            client.Send(message);
            client.Disconnect(true);
        }
    }
}