using restlessmedia.Module.Configuration;
using System.Configuration;

namespace restlessmedia.Module.Email.Configuration
{
  public class EmailCollection : TypedCollection<Email>
  {
    protected override object GetElementKey(ConfigurationElement element)
    {
      return ((Email)element).Name;
    }

    public override void Remove(Email item)
    {
      if (BaseIndexOf(item) > 0)
      {
        BaseRemove(item.Name);
      }
    }
  }
}