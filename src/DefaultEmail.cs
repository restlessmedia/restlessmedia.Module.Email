namespace restlessmedia.Module.Email
{
  /// <summary>
  /// Sends an email to and from the default 'from' address
  /// </summary>
  public class DefaultEmail : Email
  {
    public DefaultEmail(IEmailContext emailContext, string subject = null, string body = null, bool isHtml = false)
      : base(emailContext.EmailSettings.FromEmail, emailContext.EmailSettings.FromEmail, subject, body, isHtml) { }
  }
}