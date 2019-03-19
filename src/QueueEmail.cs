using System;

namespace restlessmedia.Module.Email
{
  public class QueueEmail : Email
  {
    public new string From { get; set; }

    public new string[] To { get; set; }

    public new string Subject { get; set; }

    public new string Body { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? DateSent { get; set; }

    public short Retries { get; set; }

    [Ignore]
    public int QueueId { get; set; }
  }
}