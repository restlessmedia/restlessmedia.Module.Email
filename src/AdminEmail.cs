namespace restlessmedia.Module.Email
{
  public class AdminEmail : Email
  {
    public AdminEmail(IEmailContext emailContext, string subject = null, string body = null, bool isHtml = false)
      : base(emailContext.EmailSettings.AdminEmail, emailContext.EmailSettings.AdminEmail, subject, body, isHtml)
    {
      EmailContext = emailContext;
    }

    protected readonly IEmailContext EmailContext;
  }
}