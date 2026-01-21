using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using netDumbster.smtp;
using OutpostImmobile.Communication.Options;
using OutpostImmobile.Communication.Services;
using OutpostImmobile.Communication.Interfaces;

namespace OutpostImmobile.Core.Tests.Communication;
    
[TestFixture]
public class BetterMailServiceTests
{
    private SimpleSmtpServer _smtpServer;
    private ServiceProvider _serviceProvider;
    private IMailService _mailService;
    private int _port = 2525;

    [OneTimeSetUp]
    public void GlobalSetup()
    {
        _smtpServer = SimpleSmtpServer.Start(_port);
        
        var inMemoryConfig = new Dictionary<string, string>
        {
            {"MailOptions:SmtpHost", "localhost"},
            {"MailOptions:Port", _port.ToString()},
            {"MailOptions:Secure", "false"},
            {"MailOptions:SenderMailAddress", "noreply@outpost.local"}, 
            {"MailOptions:DisplayName", "Outpost System"},
            {"MailOptions:SenderPassword", "password"}
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemoryConfig)
            .Build();

        // 3. Setup DI
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.Configure<MailOptions>(configuration.GetSection("MailOptions"));
        services.AddTransient<IMailService, MailService>();
        services.AddLogging(l => l.AddConsole());

        _serviceProvider = services.BuildServiceProvider();
        _mailService = _serviceProvider.GetRequiredService<IMailService>();
    }

    [OneTimeTearDown]
    public void GlobalTeardown()
    {
        _smtpServer?.Dispose();
        if (_serviceProvider is IDisposable d)
        {
            d.Dispose();
        }
    }

    [SetUp]
    public void TestSetup()
    {
        _smtpServer.ClearReceivedEmail();
    }

    [Test]
    public async Task SendMessage_ShouldActuallyDeliverToInbox()
    {
        // Arrange
        var uniqueSubject = $"Integration Test {Guid.NewGuid()}";
        var request = new SendEmailRequest
        {
            RecipientMailAddress = "integration@test.local",
            RecipientName = "Test Robot",
            MailSubject = uniqueSubject,
            MailBody = "Verifying side effects."
        };

        // Act
        await _mailService.SendMessage(request);

        // Assert
        var emailCount = _smtpServer.ReceivedEmailCount;
        var lastEmail = _smtpServer.ReceivedEmail.LastOrDefault();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(emailCount, Is.GreaterThan(0), "No email was received by the server.");
            Assert.That(lastEmail.Headers["Subject"], Is.EqualTo(uniqueSubject), "Subjects do not match.");
        }
    }
}