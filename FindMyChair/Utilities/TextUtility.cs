using FindMyChair.Types;
using HtmlAgilityPack;
using System;
using System.ComponentModel;
using System.Globalization;
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

		public MeetingTypes GetEnumFromInt(int value)
		{
			return (MeetingTypes)Enum.ToObject(typeof(MeetingTypes), value); ;
		}

		public string FormatTimeSpan(TimeSpan timeSpan)
		{
			var dateTime = new DateTime(timeSpan.Ticks);
			var formattedTime = dateTime.ToString("HH:mm", CultureInfo.CurrentCulture);
			return formattedTime;
		}
	}
}