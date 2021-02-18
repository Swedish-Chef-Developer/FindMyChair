using System;
using System.ComponentModel;
using FindMyChair.Types;

namespace FindMyChair.Models.Meetings
{
	public class MeetingSpecific : IComparable
	{
		public long Compare(MeetingSpecific x, MeetingSpecific y)
		{
			return x.StartTime.CompareTo(y.StartTime);
		}

		public int CompareTo(object obj)
		{
			throw new NotImplementedException();
		}

		public int Id { get; set; }
		public int Row { get; set; }
		[DisplayName("Start Time")]
		public TimeSpan StartTime { get; set; }
		public TimeSpan EndTime { get; set; }
		public int MeetingDay { get; set; }
		public MeetingTypes MeetingType { get; set; }
	}
}