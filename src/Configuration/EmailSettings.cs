using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Configuration;

namespace restlessmedia.Module.Email.Configuration
{
  public class EmailSettings : SerializableConfigurationSection, IEmailSettings
  {
    public EmailSettings()
      : base() { }

    [ConfigurationProperty(_fromProperty, IsRequired = true)]
    public string From
    {
      get
      {
        return (string)this[_fromProperty];
      }
    }

    /// <summary>
    /// The email address of the mail setting set to be the default 'from' address
    /// </summary>
    public string FromEmail
    {
      get
      {
        IEmailAddress address = Addresses[From];

        if (address == null)
        {
          throw new ConfigurationErrorsException("No from email address has been set-up.");
        }

        return address.EmailAddress;
      }
    }

    /// <summary>
    /// The email address of the mail setting 'admin' address
    /// </summary>
    public string AdminEmail
    {
      get
      {
        IEmailAddress address = Addresses["admin"];

        if (address == null)
        {
          throw new ConfigurationErrorsException("No admin email address has been set-up");
        }

        return address.EmailAddress;
      }
    }

    public IEmailAddress GetAddress(string name)
    {
      if (!Addresses.Exists(name))
      {
        throw new ConfigurationErrorsException($"There is no address configured with the name '{name}'");
      }

      return Addresses[name];
    }

    [ConfigurationProperty(_emailsProperty, IsRequired = false)]
    [ConfigurationCollection(typeof(EmailCollection), AddItemName = _addItemName)]
    private EmailCollection Addresses
    {
      get { return (EmailCollection)base[_emailsProperty]; }
    }

    private const string _addItemName = "add";

    private const string _adminEmailProperty = "adminEmail";

    private const string _emailsProperty = "addresses";

    private const string _fromProperty = "from";
  }
}