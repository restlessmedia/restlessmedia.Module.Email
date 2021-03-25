using restlessmedia.Module.Email.Configuration;
using restlessmedia.Module.Email.Data;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace restlessmedia.Module.Email
{
  /// <summary>
  /// Smtp client uses web config for connection information
  /// </summary>
  public sealed class EmailService : IEmailService
  {
    public EmailService(IEmailQueueDataProvider emailQueueDataProvider, IEmailSettings emailSettings, ISendGridClient client)
    {
      _emailQueueDataProvider = emailQueueDataProvider ?? throw new ArgumentNullException(nameof(emailQueueDataProvider));
      _emailSettings = emailSettings ?? throw new ArgumentNullException(nameof(emailSettings));
      _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    [Obsolete("Queue is deprecated")]
    public void ProcessQueue()
    {
      foreach (QueueEmail item in Queue())
      {
        try
        {
          Send(item);
        }
        catch (Exception exception)
        {
          QueueItemError(item, exception);
          continue;
        }

        QueueItemProcessed(item);
      }
    }

    public void FlushQueue()
    {
      _emailQueueDataProvider.FlushQueue();
    }

    public ModelCollection<QueueEmail> Queue(int max = 10, int? maxTries = 3, bool includeSent = false)
    {
      return _emailQueueDataProvider.ListMailQueue(max, maxTries, includeSent);
    }

    public void Send(string from, string[] to, string subject = null, string body = null, bool isHtml = false)
    {
      Send(new Email(from, to, subject, body, isHtml));
    }

    public void Send(string from, string to, string subject = null, string body = null, bool isHtml = false)
    {
      Send(new Email(from, to, subject, body, isHtml));
    }

    public void SendDefault(string[] to, string subject = null, string body = null, bool isHtml = false)
    {
      Send(_emailSettings.FromEmail, to, subject, body, isHtml);
    }

    public void SendDefault(string to, string subject = null, string body = null, bool isHtml = false)
    {
      Send(_emailSettings.FromEmail, to, subject, body, isHtml);
    }

    [Obsolete("Use async methods")]
    public void Send(IEmailAddress email, string subject = null, string body = null)
    {
      if (email == null)
      {
        throw new ArgumentNullException(nameof(email));
      }

      SendDefault(email.EmailAddress, subject: subject, body: body);
    }

    [Obsolete("Use async methods")]
    public void Send(IEmail email)
    {
      SendAsync(email).GetAwaiter().GetResult();
    }

    [Obsolete("Use async methods")]
    public void SendAll(params IEmail[] emails)
    {
      foreach (IEmail email in emails)
      {
        Send(email);
      }
    }

    public async Task SendAsync(IEmail email)
    {
      SendGridMessage mailMessage = CreateMailMessage(email);
      await _client.SendEmailAsync(mailMessage);
    }

    public async Task SendAllAsync(params IEmail[] emails)
    {
      await Task.WhenAll(emails.Select(SendAsync));
    }

    public Task SendAsync(string from, string[] to, string subject = null, string body = null, bool isHtml = false)
    {
      return SendAsync(new Email(from, to, subject, body.ToString(), isHtml));
    }

    public Task SendAsync(string from, string to, string subject = null, string body = null, bool isHtml = false)
    {
      return SendAsync(new Email(from, to, subject, body.ToString(), isHtml));
    }

    public Task SendDefaultAsync(string[] to, string subject = null, string body = null, bool isHtml = false)
    {
      return SendAsync(_emailSettings.FromEmail, to, subject, body, isHtml);
    }

    public Task SendDefaultAsync(string to, string subject = null, string body = null, bool isHtml = false)
    {
      return SendAsync(_emailSettings.FromEmail, to, subject, body, isHtml);
    }

    private void QueueItemError(QueueEmail item, Exception exception)
    {
      if (exception == null)
      {
        return;
      }

      if (item == null)
      {
        throw new ArgumentNullException(nameof(item));
      }

      _emailQueueDataProvider.QueueError(item.QueueId, exception.Messages());
    }

    private void QueueItemProcessed(QueueEmail item)
    {
      if (item == null)
      {
        throw new ArgumentNullException(nameof(item));
      }

      _emailQueueDataProvider.QueueItemProcessed(item.QueueId, DateTime.Now);
    }

    private static SendGridMessage CreateMailMessage(IEmail email)
    {
      SendGridMessage mailMessage = new SendGridMessage
      {
        From = new EmailAddress(email.From),
        Subject = email.Subject,
      };

      if (email.IsHtml)
      {
        mailMessage.HtmlContent = email.Body;
      }
      else
      {
        mailMessage.PlainTextContent = email.Body;
      }

      foreach (string to in email.To)
      {
        mailMessage.AddTo(to);
      }

      if (email.Attachments != null)
      {
        foreach (IAttachment attachment in email.Attachments.Where(x => x.ContentStream != null))
        {
          mailMessage.AddAttachmentAsync(attachment.Name, attachment.ContentStream, attachment.Type);
        }
      }

      return mailMessage;
    }

    private readonly ISendGridClient _client;

    private readonly IEmailQueueDataProvider _emailQueueDataProvider;

    private readonly IEmailSettings _emailSettings;
  }
}