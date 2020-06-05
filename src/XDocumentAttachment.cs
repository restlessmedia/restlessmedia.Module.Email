using System;
using System.IO;
using System.Xml.Linq;

namespace restlessmedia.Module.Email
{
  public class XDocumentAttachment : EmailAttachment
  {
    public XDocumentAttachment(string name, XDocument document)
      : base(name, "text/xml")
    {
      _document = document ?? throw new ArgumentNullException(nameof(document));
    }

    public override Stream ContentStream
    {
      get
      {
        Stream stream = new MemoryStream();
        _document.Save(stream);
        stream.Position = 0;
        return stream;
      }
    }

    private readonly XDocument _document;
  }
}