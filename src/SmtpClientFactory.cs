using System.Net.Mail;

namespace restlessmedia.Module.Email
{
  internal class SmtpClientFactory : ISmtpClientFactory
  {
    public SmtpClient Create()
    {
      return new SmtpClient();
    }
  }
}