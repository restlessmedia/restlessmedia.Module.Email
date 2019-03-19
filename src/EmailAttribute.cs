using System;
using System.ComponentModel.DataAnnotations;

namespace restlessmedia.Module.Email
{
  [AttributeUsage(AttributeTargets.Property)]
  public class EmailAttribute : RegularExpressionAttribute
  {
    public EmailAttribute()
      : base(_pattern) { }

    private const string _pattern = "^([0-9a-zA-Z]([-.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$";
  }
}