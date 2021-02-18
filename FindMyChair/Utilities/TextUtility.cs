using HtmlAgilityPack;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace FindMyChair.Utilities
{
	public class TextUtility
	{
		public string ConfirmHtml(string html)
		{
			var outHtml = string.Empty;
			var cleanHtml = new HtmlDocument();
			cleanHtml.LoadHtml(html);		
			return cleanHtml.DocumentNode.OuterHtml; ;
		}

		public string GetEnumDescription(Enum value)
		{
			var fi = value.GetType().GetField(value.ToString());
			var attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
			if (attributes != null && attributes.Any())
			{
				return attributes.First().Description;
			}
			return value.ToString();
		}
	}
}