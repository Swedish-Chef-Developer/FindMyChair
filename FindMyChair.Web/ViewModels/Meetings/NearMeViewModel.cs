using FindMyChair.Models.Mapping;
using FindMyChair.Models.Meetings;
using FindMyChair.Web.Models.Meetings;
using System.Collections.Generic;

namespace FindMyChair.Web.ViewModels.Meetings
{
	public class NearMeViewModel
	{
		public List<Meeting> UpcomingMeetingsList { get; set; }
		public string BingApiKey { get; set; }
		public LocationLists LocationLists { get; set; }
		public FilterSorting FilterSorting { get; set; }
	}
}