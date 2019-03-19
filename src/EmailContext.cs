using restlessmedia.Module.Configuration;
using restlessmedia.Module.Email.Configuration;
using System;

namespace restlessmedia.Module.Email
{
  public class EmailContext : IEmailContext
  {
    public EmailContext(IEmailSettings emailSettings, ILicenseSettings licenseSettings)
    {
      EmailSettings = emailSettings ?? throw new ArgumentNullException(nameof(emailSettings));
      LicenseSettings = licenseSettings ?? throw new ArgumentNullException(nameof(licenseSettings));
    }

    public IEmailSettings EmailSettings { get; private set; }

    public ILicenseSettings LicenseSettings { get; private set; }
  }
}
