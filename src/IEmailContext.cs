using restlessmedia.Module.Configuration;
using restlessmedia.Module.Email.Configuration;

namespace restlessmedia.Module.Email
{
  public interface IEmailContext
  {
    IEmailSettings EmailSettings { get; }

    ILicenseSettings LicenseSettings { get; }
  }
}