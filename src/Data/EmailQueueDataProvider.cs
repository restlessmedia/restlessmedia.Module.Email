using restlessmedia.Module.Data;

namespace restlessmedia.Module.Email.Data
{
  public class EmailQueueDataProvider : EmailQueueSqlDataProvider, IEmailQueueDataProvider
  {
    public EmailQueueDataProvider(IDataContext context)
      : base(context) { }
  }
}