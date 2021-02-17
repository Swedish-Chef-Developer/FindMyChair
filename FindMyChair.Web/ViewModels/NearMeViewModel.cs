using FindMyChair.Models.Mapping;
using FindMyChair.Models.Meetings;
using System.Collections.Generic;

namespace FindMyChair.Web.ViewModels
{
	public class NearMeViewModel
	{
		public List<Meeting> UpcomingMeetingsList { get; set; }
		public string LongitudesAndLattitudesListString { get; set; }
		public string BingApiKey { get; set; }
		public LocationLists LocationLists { get; set; }
	}
}