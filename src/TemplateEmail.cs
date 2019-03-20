using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace restlessmedia.Module.Email
{
  public abstract class TemplateEmail<T> : Email
  {
    public TemplateEmail(Assembly assembly, string resource, string from, string[] to, string subject = null)
      : base(from, to, subject, isHtml: true)
    {
      _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
      _resource = resource ?? throw new ArgumentNullException(nameof(resource));
      _attachments = new List<IAttachment>(0);
    }

    public TemplateEmail(Assembly assembly, string resource, string from, string to, string subject = null)
      : this(assembly, resource, from, new[] { to }, subject) { }

    public TemplateEmail(string resource, string from, string[] to, string subject = null)
      : this(Assembly.GetExecutingAssembly(), resource, from, to, subject) { }

    public TemplateEmail(string resource, string from, string to, string subject = null)
      : this(resource, from, new[] { to }, subject) { }

    public virtual T Model { get; set; }

    public override string Body
    {
      get
      {
        return Parse();
      }
    }

    public override IAttachment[] Attachments
    {
      get
      {
        return _attachments.ToArray();
      }
    }

    public void AddImage(string resource, string fileName)
    {
      _attachments.Add(new ResourceAttachment(_assembly, resource, fileName));
    }

    public void AddLayout(string resource, Type modelType = null)
    {
      TemplateEngine.AddLayout(_assembly, resource, modelType);
    }

    private string Parse()
    {
      _attachments.Clear();
      string template = TemplateEngine.ParseResource(_assembly, _resource, Model);
      return ParseImages(template);
    }

    private string ParseImages(string template)
    {
      if (string.IsNullOrWhiteSpace(template))
      {
        return template;
      }

      const string pattern = "(<img.*src=\")([^\"]*)(\".*>)";

      return Regex.Replace(template, pattern, MatchHandler, RegexOptions.IgnoreCase | RegexOptions.Multiline);
    }

    private string Replace(Match match, string fileName)
    {
      const string prefix = "cid:";
      return string.Concat(match.Groups[1].Value, string.Concat(prefix, fileName), match.Groups[3].Value);
    }

    private string MatchHandler(Match match)
    {
      string resource = match.Groups[2].Value;

      if (!ResourceHelper.Exists(_assembly, resource))
      {
        return match.Value;
      }

      string fileName = GetFileName(resource);
      AddImage(resource, fileName);
      return Replace(match, fileName);
    }

    private static string GetFileName(string resource)
    {
      const char separator = '.';
      string[] parts = resource.Split(separator);
      return string.Join(separator.ToString(), parts.Skip(parts.Length - 2));
    }

    private readonly IList<IAttachment> _attachments;

    private readonly Assembly _assembly;

    private readonly string _resource;
  }
}