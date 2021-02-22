using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using FindMyChair.Models.Meetings;
using HtmlAgilityPack;
using FindMyChair.Utilities;
using System.Text;
using FindMyChair.Types;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading.Tasks;

namespace FindMyChair.Scrapers
{
	public class NAScraper
	{
		public async Task<IEnumerable<Meeting>> MeetingList()
		{
			var scraperUtility = new ScraperUtilities();
			var textUtility = new TextUtility();
			//choose your website
			var url = "https://www.nasverige.org/moteslista";
			//get the html page source 
			var httpClient = new HttpClient();
			var html = await httpClient.GetStringAsync(url);
			//store the html of the page in a variable
			var doc = new HtmlDocument();
			doc.LoadHtml(html);
			var meetingList = new List<Meeting>();
			var meetingsNode = doc.DocumentNode.SelectSingleNode("//div[@id='tabs-content']");

			return meetingList;
		}
	}
}