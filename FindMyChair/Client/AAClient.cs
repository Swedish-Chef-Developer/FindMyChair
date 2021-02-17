using FindMyChair.Models.Meetings;
using FindMyChair.Scrapers;
using FindMyChair.Types;
using FindMyChair.Utilities;
using OsmSharp.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

		public async Task<List<Meeting>> GetUpcomingMeetingsList(List<Meeting> meetingList)
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

		private async Task<List<Meeting>> SetUpcomingMeetingsList(List<Meeting> meetingList)
		{
			if (null != meetingList && meetingList.Any())
			{
				var currentTimeString = string.Format("{0}:{1}", DateTime.Now.Hour, DateTime.Now.Minute);
				var currentTimeSpan = TimeSpan.Parse(currentTimeString);
				var curentDay = SetCurrentDay();
				var upcomingList = new List<Meeting>();
				foreach (var meeting in meetingList)
				{
					foreach (var meetingDay in meeting.DayAndTime.Values)
					{
						foreach (var meetings in meetingDay.Values)
						{
							if (!upcomingList.Contains(meeting) 
								&& meetings.MeetingDay == curentDay 
								&& meetings.StartTime > currentTimeSpan)
							{
								upcomingList.Add(meeting);
								continue;
							}
						}
					}
				}
				return upcomingList;
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
	}
}