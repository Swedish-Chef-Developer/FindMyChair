using FindMyChair.Models.Meetings;
using FindMyChair.Scrapers;
using FindMyChair.Types;
using FindMyChair.Utilities;
using OsmSharp.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace FindMyChair.Client
{
	public class AAClient : IAAClient
	{
		private AAScraper _aaScraper;

		public AAClient()
		{
			_aaScraper = new AAScraper();
		}

		public async Task<List<Meeting>> GetMeetingsList()
		{
			return await SetMeetingsList();
		}

		public async Task<IEnumerable<Meeting>> GetUpcomingMeetingsList(List<Meeting> meetingList)
		{
			return await SetUpcomingMeetingsList(meetingList);
		}

		public int GetCurrentDay()
		{
			return SetCurrentDay();
		}

		private async Task<List<Meeting>> SetMeetingsList()
		{
			return Castings.ToList(_aaScraper.MeetingList());
		}

		private async Task<IEnumerable<Meeting>> SetUpcomingMeetingsList(List<Meeting> meetingList)
		{
			if (null != meetingList && meetingList.Any())
			{
				var currentTimeString = string.Format("{0}:{1}", DateTime.Now.Hour, DateTime.Now.Minute);
				var currentTimeSpan = TimeSpan.Parse(currentTimeString);
				var curentDay = SetCurrentDay();
				var upcomingList = new List<Meeting>();
				foreach (var meeting in meetingList)
				{
					foreach (var meetingDay in meeting.DayAndTime.OrderByDescending(m => m.StartTime))
					{
						if (!upcomingList.Contains(meeting)
							&& meetingDay.MeetingDay == curentDay
							&& meetingDay.StartTime.Ticks >= currentTimeSpan.Ticks)
						{
							upcomingList.Add(meeting);
							continue;
						}
					}
				}
				return SortedOnStartTime(upcomingList);
			}
			return meetingList;
		}

		private int SetCurrentDay()
		{
			var dayOfWeek = DateTime.Now.DayOfWeek.ToString();
			Enum.TryParse(dayOfWeek, out WeekdayNames weekDay);
			var dayInt = (int)Enum.Parse(typeof(WeekdayNames), weekDay.ToString());
			return dayInt;
		}

		private IEnumerable<Meeting> SortedOnStartTime(List<Meeting> meetings)
		{
			var today = SetCurrentDay();
			var enumMeetings = meetings.OrderByDescending(m => m.DayAndTime[today].StartTime.Ticks).Where(m => m.DayAndTime[today].StartTime.Ticks > 0);
			var g = meetings.OrderByDescending(m => m.DayAndTime.Any(t => t.StartTime.Ticks >= DateTime.Now.TimeOfDay.Ticks));
			return meetings;
		}
	}


}