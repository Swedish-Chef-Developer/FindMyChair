using HtmlAgilityPack;

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
	}
}