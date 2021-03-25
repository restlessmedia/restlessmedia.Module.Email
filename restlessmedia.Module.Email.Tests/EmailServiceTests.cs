using FakeItEasy;
using restlessmedia.Module.Email.Configuration;
using restlessmedia.Module.Email.Data;
using restlessmedia.Test;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace restlessmedia.Module.Email.Tests
{
  public class EmailServiceTests
  {
    public EmailServiceTests()
    {
      _client = A.Fake<ISendGridClient>();
      _emailService = new EmailService(A.Fake<IEmailQueueDataProvider>(), A.Fake<IEmailSettings>(), _client);
    }

    [Theory]
    [InlineData("test-from@test.com", "test-subject", "test-to@test.com")]
    public async void sends_with_simple_message_info(string from, string subject, string to)
    {
      // set-upnp
      await _emailService.SendAsync(from, to, subject, "test-body");

      A.CallTo(() => _client.SendEmailAsync(A<SendGridMessage>.Ignored, A<CancellationToken>.Ignored))
        .WhenArgumentsMatch(args =>
        {
          var message = args.Get<SendGridMessage>(0);
          return message.From.Email.Equals(from)
          && message.Subject.Equals(subject)
          && message.Personalizations.SelectMany(x => x.Tos).Any(x => x.Email == to);
        })
        .MustHaveHappened();
    }

    [Theory]
    [InlineData("body", true)]
    [InlineData("body", false)]
    public async void sends_with_body(string body, bool isHtml)
    {
      // set-upnp
      await _emailService.SendAsync("test-from@test.com", "test-to@test.com", "test-subject", body, isHtml);

      A.CallTo(() => _client.SendEmailAsync(A<SendGridMessage>.Ignored, A<CancellationToken>.Ignored))
        .WhenArgumentsMatch(args =>
        {
          var message = args.Get<SendGridMessage>(0);

          if (isHtml)
          {
            return !string.IsNullOrEmpty(message.HtmlContent);
          }

          return !string.IsNullOrEmpty(message.PlainTextContent);
        })
        .MustHaveHappened();
    }

    [Fact(Skip = "Not sure how to test attachments yet - may be due to them being async")]
    public async void sends_with_attachment()
    {
      // set-up
      IEmail email = A.Fake<IEmail>();
      Stream stream = A.Fake<Stream>();
      A.CallTo(() => email.Attachments)
        .Returns(new IAttachment[]
      {
        new EmailAttachment("attach-name", "attach-type", stream)
      });

      await _emailService.SendAsync(email);

      A.CallTo(() => _client.SendEmailAsync(A<SendGridMessage>.Ignored, A<CancellationToken>.Ignored))
        .WhenArgumentsMatch(args =>
        {
          var message = args.Get<SendGridMessage>(0);
          
          return true;
        })
        .MustHaveHappened();
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

    private readonly ISendGridClient _client;

    private readonly EmailService _emailService;
  }
}