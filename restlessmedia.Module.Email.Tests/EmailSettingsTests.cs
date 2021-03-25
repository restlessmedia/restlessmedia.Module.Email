using restlessmedia.Module.Email.Configuration;
using restlessmedia.Test;
using System.Configuration;
using Xunit;

namespace restlessmedia.Module.Email.Tests
{
  /// <summary>
  /// These tests rely on the app.config file in the test project.
  /// </summary>
  public class EmailSettingsTests
  {
    public EmailSettingsTests()
    {
      _settings = ConfigurationManager.GetSection("restlessmedia/email") as EmailSettings;
    }
    
    [Fact]
    public void apiKey()
    {
      _settings.ApiKey.MustBe("test-key");
    }

    private readonly EmailSettings _settings;
  }
}
