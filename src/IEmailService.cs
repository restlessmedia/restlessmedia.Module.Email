using System;
using System.Threading.Tasks;

namespace restlessmedia.Module.Email
{
  public interface IEmailService : IService
  {
    /// <summary>
    /// Process unsent or failed emails
    /// </summary>
    [Obsolete("Queue is deprecated")]
    void ProcessQueue();

    /// <summary>
    /// Flushes the queue of any sent emails
    /// </summary>
    void FlushQueue();

    /// <summary>
    /// Returns a list of mail queue items
    /// </summary>
    /// <param name="max"></param>
    /// <param name="maxTries"></param>
    /// <param name="includeSent"></param>
    /// <returns></returns>
    ModelCollection<QueueEmail> Queue(int max = 10, int? maxTries = 3, bool includeSent = false);

    [Obsolete("Use async methods")]
    void Send(IEmail email);

    [Obsolete("Use async methods")]
    void Send(string from, string[] to, string subject = null, string body = null, bool isHtml = false);

    [Obsolete("Use async methods")]
    void Send(string from, string to, string subject = null, string body = null, bool isHtml = false);

    [Obsolete("Use async methods")]
    void SendDefault(string[] to, string subject = null, string body = null, bool isHtml = false);

    [Obsolete("Use async methods")]
    void SendDefault(string to, string subject = null, string body = null, bool isHtml = false);

    [Obsolete("Use async methods")]
    void Send(IEmailAddress email, string subject = null, string body = null);

    [Obsolete("Use async methods")]
    void SendAll(params IEmail[] emails);

    Task SendAsync(IEmail email);

    Task SendAsync(string from, string[] to, string subject = null, string body = null, bool isHtml = false);

    Task SendAsync(string from, string to, string subject = null, string body = null, bool isHtml = false);

    Task SendDefaultAsync(string[] to, string subject = null, string body = null, bool isHtml = false);

    Task SendDefaultAsync(string to, string subject = null, string body = null, bool isHtml = false);

    Task SendAllAsync(params IEmail[] emails);
  }
}