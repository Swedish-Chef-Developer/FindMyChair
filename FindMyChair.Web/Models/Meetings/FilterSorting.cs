using System;
using System.Collections.Generic;
using FindMyChair.Models.Meetings;
using FindMyChair.Types;

namespace FindMyChair.Web.Models.Meetings
{
	public class FilterSorting
	{
		public List<Meeting> MeetingsList { get; set; }
		public string BingApiKey { get; set; }
		public List<string> CityList { get; set; }
		public List<MeetingTypes> MeetingTypeList { get; set; }
		public List<TimeSpan> StartTimes { get; set; }
	}
}