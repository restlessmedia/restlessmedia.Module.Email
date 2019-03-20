using RazorEngine;
using RazorEngine.Templating;
using System;
using System.IO;
using System.Reflection;

namespace restlessmedia.Module.Email
{
  public class TemplateEngine
  {
    public static string Parse<T>(string template, T model)
    {
      Type modelType = typeof(T);

      string key = template.GetHashCode().ToString();

      if (Engine.Razor.IsTemplateCached(key, modelType))
      {
        return Engine.Razor.Run(key, modelType, model);
      }

      return Engine.Razor.RunCompile(template, key, modelType, model);
    }

    public static string ParseResource<T>(Assembly assembly, string resource, T model)
    {
      const string period = ".";

      if (!resource.Contains(period))
      {
        resource = string.Concat(_templateNamespace, period, resource);
      }

      return Parse(GetResource(assembly, resource), model);
    }

    public static string ParseResource<T>(string resource, T model)
    {
      return ParseResource(Assembly.GetExecutingAssembly(), resource, model);
    }

    public static void AddLayout(Assembly assembly, string resource, Type modelType = null)
    {
      if (!Engine.Razor.IsTemplateCached(resource, modelType))
      {
        Engine.Razor.Compile(GetResource(assembly, resource), resource, modelType);
      }
    }

    private static string GetResource(Assembly assembly, string resource)
    {
      using (StreamReader reader = GetResourceReader(assembly, resource))
      {
        return reader.ReadToEnd();
      }
    }

    private static StreamReader GetResourceReader(Assembly assembly, string resource)
    {
      if (!resource.EndsWith(_extension))
      {
        resource = string.Concat(resource, _extension);
      }

      return new StreamReader(ResourceHelper.GetResourceStream(assembly, resource));
    }

    private const string _templateNamespace = "restlessmedia.Business.Email.Templates";

    private const string _extension = ".cshtml";
  }
}