using FakeItEasy;
using restlessmedia.Module.Email.Configuration;
using restlessmedia.Module.Email.Data;
using System;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace restlessmedia.Module.Email.Tests
{
  public class EmailServiceTests : IDisposable
  {
    public EmailServiceTests()
    {
      string binDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\", "");
      
      _testDirectory = Path.Combine(binDirectory, "temp");
      _smtpClientFactory = A.Fake<ISmtpClientFactory>();
      _emailService = new EmailService(A.Fake<IEmailQueueDataProvider>(), A.Fake<IEmailSettings>(), _smtpClientFactory);
      
      Directory.CreateDirectory(_testDirectory);

      A.CallTo(() => _smtpClientFactory.Create()).Returns(new SmtpClient
      {
        DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
        PickupDirectoryLocation = _testDirectory,
      });
    }

    [Fact]
    public async void SendAsync()
    {
      // set-up
      await _emailService.SendAsync("test-from@test.com", "test-to@test.com", "test-subject", "test-body");
    }

    [Fact]
    public async void SendAllAsync()
    {
      IEmail email = A.Fake<IEmail>();
      A.CallTo(() => email.To).Returns(new[] { "test-to@test.com" });
      A.CallTo(() => email.From).Returns("test-from@test.com");
      A.CallTo(() => email.Subject).Returns("test-subject");
      A.CallTo(() => email.Body).Returns("test-body");

      // set-up
      await Task.WhenAll(_emailService.SendAllAsync(new[] { email, email }));
    }

    public void Dispose()
    {
      Directory.Delete(_testDirectory, true);
    }

    private readonly ISmtpClientFactory _smtpClientFactory;

    private readonly EmailService _emailService;

    private readonly string _testDirectory;
  }
}