using restlessmedia.Module.Data;
using restlessmedia.Module.Data.Sql;
using System;

namespace restlessmedia.Module.Email.Data
{
  public class EmailQueueSqlDataProvider : SqlDataProviderBase
  {
    internal EmailQueueSqlDataProvider(IDataContext context)
      : base(context) { }

    public void Queue(IEmail email)
    {
      Execute("dbo.SPQueueMail", new
      {
        from = email.From,
        to = email.ToDelimited(),
        subject = email.Subject,
        body = email.Body
      });
    }

    public void QueueError(int queueId, string message)
    {
      Execute("dbo.SPQueueMailError", new { queueId = queueId, message = message });
    }

    public void QueueItemProcessed(int queueId, DateTime dateSent)
    {
      Execute("dbo.SPQueueMailProcessed", new { queueId = queueId, dateSent = dateSent });
    }

    public ModelCollection<QueueEmail> ListMailQueue(int max = 10, int? maxTries = 2, bool includeSent = false)
    {
      return ModelQuery<QueueEmail>("dbo.SPGetMailQueue", new { max = max, maxTries = maxTries, includeSent = includeSent });
    }

    public void FlushQueue()
    {
      Execute("dbo.SPFlushMailQueue");
    }
  }
}