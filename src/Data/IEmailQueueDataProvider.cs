using restlessmedia.Module.Data;
using System;

namespace restlessmedia.Module.Email.Data
{
  public interface IEmailQueueDataProvider : IDataProvider
  {
    void Queue(IEmail  email);

    void QueueError(int queueId, string message);

    void QueueItemProcessed(int queueId, DateTime dateSent);

    ModelCollection<QueueEmail> ListMailQueue(int max = 10, int? maxTries = 2, bool includeSent = false);

    void FlushQueue();
  }
}