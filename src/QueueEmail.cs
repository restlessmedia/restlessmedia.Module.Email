using System;

namespace restlessmedia.Module.Email
{
  public class QueueEmail : Email
  {
    public DateTime DateCreated { get; set; }

    public DateTime? DateSent { get; set; }

    public short Retries { get; set; }

    [Ignore]
    public int QueueId { get; set; }
  }
}