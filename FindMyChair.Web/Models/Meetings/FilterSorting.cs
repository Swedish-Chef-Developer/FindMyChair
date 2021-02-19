using System.Collections.Generic;
using FindMyChair.Models.Meetings;

namespace FindMyChair.Web.Models.Meetings
{
	public class FilterSorting
	{
		public List<Meeting> MeetingsList { get; set; }
		public string BingApiKey { get; set; }
		public List<string> CityList { get; set; }
	}
}