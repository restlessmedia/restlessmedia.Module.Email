using System;
using System.IO;
using System.Reflection;

namespace restlessmedia.Module.Email
{
  public class ResourceAttachment : IAttachment
  {
    public ResourceAttachment(Assembly assembly, string resource, string name = null, string type = null)
    {
      if (string.IsNullOrWhiteSpace(resource))
      {
        throw new ArgumentNullException(nameof(resource));
      }

      _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
      _resource = resource;
      _name = name ?? resource;
      _type = type;
    }

    public ResourceAttachment(string resource, string type = null)
      : this(Assembly.GetExecutingAssembly(), resource, type) { }

    public virtual string Name
    {
      get
      {
        return _name;
      }
    }

    public virtual string Type
    {
      get
      {
        return _type;
      }
    }

    public virtual Stream ContentStream
    {
      get
      {
        return ResourceHelper.GetResourceStream(_assembly, _resource);
      }
    }

    private readonly Assembly _assembly;

    private readonly string _resource;

    private readonly string _name;

    private readonly string _type;
  }
}