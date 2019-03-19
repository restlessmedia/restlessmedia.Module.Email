namespace restlessmedia.Module.Email.Configuration
{
  public interface IEmailSettings
  {
    /// <summary>
    /// Specifies what email address is used as the from address when sending mails from the system
    /// </summary>
    string From { get; }

    string FromEmail { get; }

    string AdminEmail { get; }

    IEmailAddress GetAddress(string name);
  }
}