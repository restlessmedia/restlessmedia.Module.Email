using System.Net.Mail;

namespace restlessmedia.Module.Email
{
  public interface ISmtpClientFactory
  {
    SmtpClient Create();
  }
}