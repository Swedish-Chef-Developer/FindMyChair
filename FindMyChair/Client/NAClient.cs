using FindMyChair.Models.Meetings;
using FindMyChair.Scrapers;
using FindMyChair.Types;
using FindMyChair.Utilities;
using OsmSharp.Streams;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace FindMyChair.Client
{
	public class NAClient : INAClient
	{
		private NAScraper _naScraper;

		public NAClient()
		{
			_naScraper = new NAScraper();
		}

		public async Task<IEnumerable<Meeting>> GetMeetingsList()
		{
			return await SetMeetingsList();
		}

		public async Task<IEnumerable<Meeting>> GetUpcomingMeetingsList(List<Meeting> meetingList)
		{
			return await SetUpcomingMeetingsList(meetingList);
		}

		public async Task<List<string>> GetCities(List<Meeting> meetingList)
		{
			return await SetCities(meetingList);
		}

		public async Task<List<MeetingTypes>> GetMeetingTypes(List<Meeting> meetingList)
		{
			return await SetMeetingTpes(meetingList);
		}

		public async Task<IEnumerable<TimeSpan>> GetTimes(List<Meeting> meetingList)
		{
			return await SetTimes(meetingList);
		}

		public int GetCurrentDay()
		{
			return SetCurrentDay();
		}

		private async Task<IEnumerable<Meeting>> SetMeetingsList()
		{
			return await _naScraper.MeetingList();
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
				return Castings.ToList(SortedOnStartTime(upcomingList));
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

		private async Task<List<string>> SetCities(List<Meeting> meetingList)
		{
			var cities = new List<string>();
			var culture = new CultureInfo("sv-SE");
			cities = Castings.ToList(meetingList.Select(c => c.Address.City.Trim())
				.Where(s => s.Trim() != "")
				.Where(m => m.Any(m => char.IsUpper(m.ToString()[0])))
				.Distinct()
				.OrderBy(s => s, StringComparer.Create(culture, false)));
			return cities;
		}

		private async Task<List<MeetingTypes>> SetMeetingTpes(List<Meeting> meetingList)
		{
			return Castings.ToList(Enum.GetValues(typeof(MeetingTypes)).Cast<MeetingTypes>());
		}

		private async Task<IEnumerable<TimeSpan>> SetTimes(List<Meeting> meetingList)
		{
			var times = meetingList.SelectMany(n => n.DayAndTime
			.Select(p => p.StartTime)
			.Where(t => t.Ticks > 0))
			.Distinct()
			.OrderBy(t => t.Ticks);
			return times;
		}

		private IEnumerable<Meeting> SortedOnStartTime(List<Meeting> meetings)
		{
			var today = SetCurrentDay();
			var sortedList = from meeting in meetings
					from daytime in meeting.DayAndTime
					where (daytime.MeetingDay == today)  && (daytime.StartTime.Ticks >= DateTime.Now.TimeOfDay.Ticks)
					orderby daytime.StartTime.Ticks
					select meeting;
			return sortedList;
		}
	}


}