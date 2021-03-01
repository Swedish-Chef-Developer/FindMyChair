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
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;

namespace FindMyChair.Scrapers
{
	public class NAScraper
	{
		public async Task<IEnumerable<Meeting>> MeetingList()
		{
			var scraperUtility = new ScraperUtilities();
			var textUtility = new TextUtility();
			var url = "https://www.nasverige.org/main_server/client_interface/jsonp/?switcher=GetSearchResults&get_used_formats&lang_enum=sv&data_field_key=location_postal_code_1,duration_time,start_time,time_zone,weekday_tinyint,service_body_bigint,longitude,latitude,location_province,location_municipality,location_street,location_info,location_text,location_neighborhood,formats,format_shared_id_list,comments,meeting_name,location_sub_province,worldid_mixed,root_server_uri,id_bigint,meeting_name,location_text,formatted_address,formatted_location_info,virtual_meeting_link,virtual_meeting_link,show_qrcode,virtual_meeting_link,phone_meeting_number,phone_meeting_number,show_qrcode,phone_meeting_number,latitude,longitude,latitude,longitude,map_word,latitude,longitude,contact_name_1,contact_phone_1,contact_email_1,contact_name_2,contact_phone_2,contact_email_2&services[]=2&recursive=1&sort_keys=start_time&callback=jQuery35108984379689974507_1614608665780&_=1614608665781";
			var handler = new HttpClientHandler();
			var httpClient = new HttpClient(handler, false);
			var response = await httpClient.GetStringAsync(url);
			var jsonString = response.Substring(response.IndexOf("(") + 1, response.LastIndexOf(")") - response.IndexOf("(") - 1);
			var jObject = JObject.Parse(jsonString);
			var naMeetings = new List<NAMeeting>();
			var meetingsList = JsonConvert.DeserializeObject<NAJsonToClass> (jObject.ToString());
			var culture = new CultureInfo("sv-SE");
			var meetingList = new List<Meeting>();

			if (null != meetingsList.meetings && meetingsList.meetings.Any())
			{
				foreach (var naMeeting in meetingsList.meetings)
				{

				}
			}

			return meetingList;
		}
	}
}