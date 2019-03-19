using restlessmedia.Module.Email.Configuration;
using restlessmedia.Module.Email.Data;
using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace restlessmedia.Module.Email
{
  /// <summary>
  /// Smtp client uses web config for connection information
  /// </summary>
  public sealed class EmailService : IEmailService
  {
    public EmailService(IEmailQueueDataProvider emailQueueDataProvider, IEmailSettings emailSettings)
    {
      _emailQueueDataProvider = emailQueueDataProvider ?? throw new ArgumentNullException(nameof(emailQueueDataProvider));
      _emailSettings = emailSettings ?? throw new ArgumentNullException(nameof(emailSettings));
    }

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
      Send(new Email(from, to, subject, body.ToString(), isHtml));
    }

    public void Send(string from, string to, string subject = null, string body = null, bool isHtml = false)
    {
      Send(new Email(from, to, subject, body.ToString(), isHtml));
    }

    public void SendDefault(string[] to, string subject = null, string body = null, bool isHtml = false)
    {
      Send(_emailSettings.FromEmail, to, subject, body, isHtml);
    }

    public void SendDefault(string to, string subject = null, string body = null, bool isHtml = false)
    {
      Send(_emailSettings.FromEmail, to, subject, body, isHtml);
    }

    public void Send(IEmailAddress email, string subject = null, string body = null)
    {
      if (email == null)
      {
        throw new ArgumentNullException(nameof(email));
      }

      SendDefault(email.EmailAddress, subject: subject, body: body);
    }

    public void Send(params IEmail[] emails)
    {
      foreach (MailMessage mailMessage in CreateMailMessages(emails))
      {
        // smtp client uses web config for connection information
        using (SmtpClient client = new SmtpClient())
        {
          client.Send(mailMessage);
        }
      }
    }

    public void SendAsync(params IEmail[] emails)
    {
      foreach (MailMessage mailMessage in CreateMailMessages(emails))
      {
        using (SmtpClient client = new SmtpClient())
        {
          client.SendAsync(mailMessage, null);
        }
      }
    }

    public void SendAsync(string from, string[] to, string subject = null, string body = null, bool isHtml = false)
    {
      SendAsync(new Email(from, to, subject, body.ToString(), isHtml));
    }

    public void SendAsync(string from, string to, string subject = null, string body = null, bool isHtml = false)
    {
      SendAsync(new Email(from, to, subject, body.ToString(), isHtml));
    }

    public void SendDefaultAsync(string[] to, string subject = null, string body = null, bool isHtml = false)
    {
      SendAsync(_emailSettings.FromEmail, to, subject, body, isHtml);
    }

    public void SendDefaultAsync(string to, string subject = null, string body = null, bool isHtml = false)
    {
      SendAsync(_emailSettings.FromEmail, to, subject, body, isHtml);
    }

    private void Queue(IEmail email)
    {
      if (email == null)
      {
        throw new ArgumentNullException(nameof(email));
      }

      _emailQueueDataProvider.Queue(email);
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

    private static IEnumerable<MailMessage> CreateMailMessages(params IEmail[] emails)
    {
      foreach (IEmail email in emails)
      {
        using (MailMessage mailMessage = new MailMessage())
        {
          foreach (string to in email.To)
          {
            mailMessage.To.Add(to);
          }

          mailMessage.From = new MailAddress(email.From);
          mailMessage.Subject = email.Subject;
          mailMessage.IsBodyHtml = email.IsHtml;
          mailMessage.Body = email.Body;

          if (email.Attachments != null)
          {
            foreach (IAttachment attachment in email.Attachments)
            {
              Attachment mailAttachment = new Attachment(attachment.ContentStream, attachment.Name, attachment.Type)
              {
                ContentId = attachment.Name
              };
              mailMessage.Attachments.Add(mailAttachment);
            }
          }

          yield return mailMessage;
        }
      }
    }

    private readonly IEmailQueueDataProvider _emailQueueDataProvider;

    private readonly IEmailSettings _emailSettings;
  }
}