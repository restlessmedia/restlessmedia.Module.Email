using System;
using System.IO;

namespace restlessmedia.Module.Email
{
  public class EmailAttachment : IAttachment
  {
    public EmailAttachment(string name, string type)
    {
      if (string.IsNullOrEmpty(name))
      {
        throw new ArgumentNullException(nameof(name));
      }

      if (string.IsNullOrEmpty(type))
      {
        throw new ArgumentNullException(nameof(type));
      }

      Name = name;
      Type = type;
    }

    public EmailAttachment(string name, string type, Stream stream)
      : this(name, type)
    {
      ContentStream = stream;
    }

    public string Name { get; private set; }


    public string Type { get; private set; }


    public virtual Stream ContentStream { get; private set; }
  }
}