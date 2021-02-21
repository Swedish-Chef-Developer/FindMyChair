using FindMyChair.Client;
using FindMyChair.Models.Meetings;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using static FindMyChair.Types.FilterSortTypes;
using static FindMyChair.Types.MeetingTypes;

namespace FindMyChair.Utilities
{
	public class FilterAndSortingUtility
	{
		private TextUtility _textUtility;
		private AAClient _aaClient;

		public FilterAndSortingUtility()
		{
			_textUtility = new TextUtility();
			_aaClient = new AAClient();
		}

		public List<Meeting> GetListFiltered(List<Meeting> meetings, List<string> terms, FilterTypes type, bool onlyToday = false)
		{
			return SetListFiltered(meetings, terms, type, onlyToday);
		}

		public List<Meeting> GetListSorted(List<Meeting> meetings, SortingTypes type, bool onlyToday = false)
		{
			return SetListSorted(meetings, type, onlyToday);
		}

		public List<Meeting> GetTodaysMeetings(List<Meeting> meetings)
		{
			return SetTodaysMeetings(meetings);
		}

		private List<Meeting> SetListFiltered(List<Meeting> meetings, List<string> terms, FilterTypes type, bool onlyToday = false)
		{
			var filteredList = new List<Meeting>();
			var workList = new List<Meeting>();
			if (null != terms && terms.Any())
			{
				switch (type)
				{
					case FilterTypes.Cities:
						workList = new List<Meeting>();
						foreach (var term in terms)
						{
							var filteredMeetingsCity = filteredList.Any() ? filteredList.AsQueryable() : meetings.AsQueryable();
							if (string.IsNullOrWhiteSpace(term)) continue;
							filteredMeetingsCity = filteredMeetingsCity
								.Where(r => r.Address.City.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)
								.ToList()
								.Contains(term));
							workList = workList.Concat(filteredMeetingsCity).ToList();
						}
						filteredList = workList;
						break;
					case FilterTypes.Meetings:
						workList = new List<Meeting>();
						foreach (var term in terms)
						{
							var filteredMeetings = filteredList.Any() ? filteredList.AsQueryable() : meetings.AsQueryable();
							if (string.IsNullOrWhiteSpace(term)) continue;
							filteredMeetings = filteredMeetings
								.Where(m => m.DayAndTime.ToList().Select(m => m.MeetingType)
								.Contains(_textUtility.GetEnumFromInt(int.Parse(term))));
							workList = workList.Concat(filteredMeetings).ToList();
						}
						filteredList = workList;
						break;
					case FilterTypes.EarliestTime:
						workList = new List<Meeting>();
						var filteredMeetingsEarly = filteredList.Any() ? filteredList.AsQueryable() : meetings.AsQueryable();
						foreach (var term in terms)
						{
							TimeSpan.TryParse(term, out TimeSpan timeSpan);
							var meetingsFiltered = (from meeting in filteredMeetingsEarly
													from day in meeting.DayAndTime
													where day.StartTime.Ticks > 0 && day.StartTime >= timeSpan
													orderby day.StartTime.Ticks
													select meeting).ToList().Distinct();
							workList = workList.Concat(meetingsFiltered).ToList();
						}
						filteredList = workList;
						break;
					case FilterTypes.LatestTime:
						workList = new List<Meeting>();
						var filteredMeetingsLate = filteredList.Any() ? filteredList.AsQueryable() : meetings.AsQueryable();
						foreach (var term in terms)
						{
							TimeSpan.TryParse(term, out TimeSpan timeSpan);
							var meetingsFiltered = (from meeting in filteredMeetingsLate
													from day in meeting.DayAndTime
													where day.StartTime.Ticks > 0 &&
													day.StartTime <= timeSpan
													orderby day.StartTime.Ticks
													select meeting).ToList().Distinct();
							workList = workList.Concat(meetingsFiltered).ToList();
						}
						filteredList = workList;
						break;
				}
			}
			if (onlyToday)
			{
				filteredList = SetTodaysMeetings(filteredList);
			}
			return filteredList;
		}

		private List<Meeting> SetListSorted(List<Meeting> meetings, SortingTypes type, bool onlyToday = false)
		{
			var sortedList = meetings;
			if (null != sortedList && sortedList.Any())
			{
				switch (type)
				{
					case SortingTypes.NameAZ:
						sortedList = Castings.ToList(sortedList.OrderBy(m => m.GroupName));
						break;
					case SortingTypes.NameZA:
						sortedList = Castings.ToList(sortedList.OrderByDescending(m => m.GroupName));
						break;
					case SortingTypes.TimeEarlyToLate:
						var sortedMeetings = (from meeting in sortedList
											  from day in meeting.DayAndTime
											  where day.StartTime.Ticks > 0
											  orderby day.StartTime.Ticks
											  select meeting).ToList().Distinct();
						sortedList = Castings.ToList(sortedMeetings);
						break;
					case SortingTypes.TimeLateToEarly:
						var sortedMeetingList = (from meeting in sortedList
												 from day in meeting.DayAndTime
												 where day.StartTime.Ticks > 0
												 orderby day.StartTime.Ticks descending
												 select meeting).ToList().Distinct();
						sortedList = Castings.ToList(sortedMeetingList);
						break;
				}
			}
			if (onlyToday)
			{
				sortedList = SetTodaysMeetings(sortedList);
			}
			return sortedList;
		}

		private List<Meeting> SetTodaysMeetings(List<Meeting> meetings)
		{
			if (null != meetings && meetings.Any())
			{
				var curentDay = _aaClient.GetCurrentDay();
				var filteredMeetings = meetings.Where(m => m.DayAndTime.Any(dt => dt.StartTime.Ticks > 0 && dt.MeetingDay == curentDay))
								 .ToList();
				return filteredMeetings;
			}
			return meetings;
		}
	}
}