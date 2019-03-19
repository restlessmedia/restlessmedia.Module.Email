namespace restlessmedia.Module.Email
{
  public interface IEmail
  {
    string From { get; }

    string[] To { get; }

    string Subject { get; }

    string Body { get; }

    bool IsHtml { get; }

    void AddLine(string text = null);

    string ToDelimited(string separator = ";");

    IAttachment[] Attachments { get; }
  }
}