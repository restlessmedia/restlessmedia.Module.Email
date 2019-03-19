using restlessmedia.Module.Data;
using restlessmedia.Module.Email.Configuration;

namespace restlessmedia.Module.Email.Data
{
  public class EmailQueueDataProvider : EmailQueueSqlDataProvider, IEmailQueueDataProvider
  {
    public EmailQueueDataProvider(IDataContext context)
      : base(context) { }
  }
}