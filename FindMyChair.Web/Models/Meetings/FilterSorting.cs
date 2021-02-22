using System;
using System.Collections.Generic;
using FindMyChair.Models.Meetings;
using FindMyChair.Types;

namespace FindMyChair.Web.Models.Meetings
{
	public class FilterSorting
	{
		public IEnumerable<Meeting> MeetingsList { get; set; }
		public string BingApiKey { get; set; }
		public IEnumerable<string> CityList { get; set; }
		public IEnumerable<MeetingTypes> MeetingTypeList { get; set; }
		public IEnumerable<TimeSpan> StartTimes { get; set; }
	}
}