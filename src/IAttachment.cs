using System.IO;

namespace restlessmedia.Module.Email
{
  public interface IAttachment
  {
    string Name { get; }

    string Type { get; }

    Stream ContentStream { get; }
  }
}