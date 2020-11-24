﻿using restlessmedia.Module.Email.Configuration;
using restlessmedia.Module.Email.Data;
using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace restlessmedia.Module.Email
{
  /// <summary>
  /// Smtp client uses web config for connection information
  /// </summary>
  public sealed class EmailService : IEmailService
  {
    public EmailService(IEmailQueueDataProvider emailQueueDataProvider, IEmailSettings emailSettings, ISmtpClientFactory smtpClientFactory)
    {
      _emailQueueDataProvider = emailQueueDataProvider ?? throw new ArgumentNullException(nameof(emailQueueDataProvider));
      _emailSettings = emailSettings ?? throw new ArgumentNullException(nameof(emailSettings));
      _smtpClientFactory = smtpClientFactory ?? throw new ArgumentNullException(nameof(smtpClientFactory));
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

    public void Send(IEmailAddress email, string subject = null, string body = null)
    {
      if (email == null)
      {
        throw new ArgumentNullException(nameof(email));
      }

      SendDefault(email.EmailAddress, subject: subject, body: body);
    }

    public void Send(IEmail email)
    {
      using (MailMessage mailMessage = CreateMailMessage(email))
      {
        using (SmtpClient client = _smtpClientFactory.Create())
        {
          client.Send(mailMessage);
        }
      }
    }

    public void SendAll(params IEmail[] emails)
    {
      foreach (IEmail email in emails)
      {
        Send(email);
      }
    }

    public async Task SendAsync(IEmail email)
    {
      using (MailMessage mailMessage = CreateMailMessage(email))
      {
        using (SmtpClient client = _smtpClientFactory.Create())
        {
          await client.SendMailAsync(mailMessage);
        }
      }
    }

    public async Task SendAllAsync(params IEmail[] emails)
    {
      await Task.WhenAll(emails.Select(SendAsync).ToArray());
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

    private static MailMessage CreateMailMessage(IEmail email)
    {
      MailMessage mailMessage = new MailMessage
      {
        From = new MailAddress(email.From),
        Subject = email.Subject,
        IsBodyHtml = email.IsHtml,
        Body = email.Body,
      };

      foreach (string to in email.To)
      {
        mailMessage.To.Add(to);
      }

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

      return mailMessage;
    }

    private readonly ISmtpClientFactory _smtpClientFactory;

    private readonly IEmailQueueDataProvider _emailQueueDataProvider;

    private readonly IEmailSettings _emailSettings;
  }
}