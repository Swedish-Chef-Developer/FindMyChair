using System;
using System.Collections.Generic;
using System.ComponentModel;
using FindMyChair.Types;

namespace FindMyChair.Models.Meetings
{
	public class MeetingSpecific
	{
		public int Id { get; set; }
		public int Row { get; set; }
		public TimeSpan StartTime { get; set; }
		public TimeSpan EndTime { get; set; }
		public int MeetingDay { get; set; }
		public MeetingTypes MeetingType { get; set; }
		public IEnumerable<MeetingTypes> MeetingTypes { get; set; }
	}
}