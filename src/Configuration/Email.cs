using System.Configuration;

namespace restlessmedia.Module.Email.Configuration
{
  public class Email : ConfigurationElement, IEmailAddress
  {
    /// <summary>
    /// Serves as the key
    /// </summary>
    [ConfigurationProperty(nameProperty, IsRequired = true)]
    public string Name
    {
      get
      {
        return (string)this[nameProperty];
      }
    }

    [ConfigurationProperty(emailAddressProperty, IsRequired = true)]
    public string EmailAddress
    {
      get
      {
        return (string)this[emailAddressProperty];
      }
    }

    private const string nameProperty = "name";

    private const string emailAddressProperty = "email";
  }
}