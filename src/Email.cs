using restlessmedia.Module.Address;
using System;

namespace restlessmedia.Module.Email
{
  public class Email : IEmail
  {
    public Email() { }

    public Email(string from, string[] to, string subject = null, string body = null, bool isHtml = false)
    {
      if (string.IsNullOrWhiteSpace(from))
      {
        throw new ArgumentNullException(nameof(from));
      }

      if (to == null || to.Length == 0)
      {
        throw new ArgumentException(nameof(to));
      }

      From = from;
      To = to;
      Subject = subject;
      Body = body;
      IsHtml = isHtml;
    }

    public Email(string from, string to, string subject = null, string body = null, bool isHtml = false)
      : this(from, new string[1] { to }, subject, body, isHtml) { }

    public virtual string From { get; private set; }

    public virtual string[] To { get; private set; }

    public virtual string Subject { get; private set; }

    public virtual string Body { get; private set; }

    public string ToDelimited(string separator = Separator)
    {
      return To != null && To.Length > 0 ? string.Join(Separator, To) : null;
    }

    public virtual bool IsHtml { get; private set; }

    public virtual IAttachment[] Attachments { get; private set; }

    public void AddLine(AddressEntity address, bool addIfEmpty = true)
    {
      if (address != null)
      {
        if (addIfEmpty || (!addIfEmpty && !string.IsNullOrEmpty(address.Address01)))
        {
          AddLine($"Address 01 = {address.Address01}");
        }

        if (addIfEmpty || (!addIfEmpty && !string.IsNullOrEmpty(address.Address02)))
        {
          AddLine($"Address 02 = {address.Address02}");
        }

        if (addIfEmpty || (!addIfEmpty && !string.IsNullOrEmpty(address.Town)))
        {
          AddLine($"Town = {address.Town}");
        }

        if (addIfEmpty || (!addIfEmpty && !string.IsNullOrEmpty(address.PostCode)))
        {
          AddLine($"Post Code = {address.PostCode}");
        }
      }
    }

    public void AddLine(string text = null)
    {
      if (IsHtml)
      {
        throw new InvalidOperationException("Attempting to add a text only line to an html email");
      }

      if (string.IsNullOrEmpty(Body))
      {
        Body = string.Empty;
      }

      if (!string.IsNullOrEmpty(text))
      {
        Body += text;
      }

      Body += NewLineEnding;
    }

    public const string Separator = ",";

    public const string NewLineEnding = "\t\r\n";
  }
}