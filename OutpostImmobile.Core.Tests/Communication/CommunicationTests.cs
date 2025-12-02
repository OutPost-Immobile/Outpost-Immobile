using Microsoft.Extensions.Configuration;
using OutpostImmobile.Communication.Options;
using OutpostImmobile.Communication.Services;

namespace OutpostImmobile.Core.Tests.Communication;

public class Tests
{
    private MailService _mailService;
    [SetUp]
    public void Setup()
    {
        // Use data from appsetting.json file
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        var mailOptions = config.GetSection("MailOptions").Get<MailOptions>();
        var options = Microsoft.Extensions.Options.Options.Create(mailOptions);
        _mailService = new MailService(options);
    }
    [Test]
    public void Test_SendMessage()
    {
        var request = new SendEmailRequest
        {
            RecipientMailAddress = "EltonJohn@ethereal.email",
            RecipientName = "Elton John",
            MailSubject = "The king must die",
            MailBody = "Some men are better staying sailors"
        };
        _ = _mailService.SendMessage(request);
        Assert.Pass();
    }
}