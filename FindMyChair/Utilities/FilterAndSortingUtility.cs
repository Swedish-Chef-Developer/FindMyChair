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
						filteredList = onlyToday ? SetTodaysMeetings(filteredList) : filteredList;
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
						filteredList = onlyToday ? SetTodaysMeetings(filteredList) : filteredList;
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
						filteredList = onlyToday ? SetTodaysMeetings(filteredList) : filteredList;
						var filteredMeetingsEarly = filteredList.Any() ? filteredList.AsQueryable() : meetings.AsQueryable();
						foreach (var term in terms)
						{
							TimeSpan.TryParse(term, out TimeSpan timeSpan);
							var meetingsFilterEarly = (from meeting in filteredMeetingsEarly
													from day in meeting.DayAndTime
													where day.StartTime.Ticks > 0 && 
													day.StartTime.Ticks >= timeSpan.Ticks
													orderby day.StartTime.Ticks
													select meeting).ToList().Distinct();
							workList = workList.Concat(meetingsFilterEarly).ToList();
						}
						filteredList = workList;
						break;
					case FilterTypes.LatestTime:
						workList = new List<Meeting>();
						filteredList = onlyToday ? SetTodaysMeetings(filteredList) : filteredList;
						var filteredMeetingsLate = filteredList.Any() ? filteredList.AsQueryable() : meetings.AsQueryable();
						foreach (var term in terms)
						{
							TimeSpan.TryParse(term, out TimeSpan timeSpan);
							var meetingsFilteredLatest = (from meeting in filteredMeetingsLate
													from day in meeting.DayAndTime
													where day.StartTime.Ticks > 0 &&
													day.StartTime.Ticks <= timeSpan.Ticks
													orderby day.StartTime.Ticks
													select meeting).ToList().Distinct();
							workList = workList.Concat(meetingsFilteredLatest).ToList();
						}
						filteredList = workList;
						break;
					case FilterTypes.TimeBetweenEarlyAndLate:
						workList = new List<Meeting>();
						var filteredMeetingsTimes = filteredList.Any() ? filteredList.AsQueryable() : meetings.AsQueryable();
						if (null == terms || !terms.Any() || terms.Count != 2) return filteredList;
						var earlyTime = terms[0];
						var lateTime = terms[1];
						TimeSpan.TryParse(earlyTime, out TimeSpan timeSpanEarly);
						TimeSpan.TryParse(lateTime, out TimeSpan timeSpanLate);
						var meetingsFiltered = (from meeting in filteredMeetingsTimes
												from day in meeting.DayAndTime
												where day.StartTime.Ticks > 0 &&
												day.StartTime >= timeSpanEarly && 
												day.StartTime <= timeSpanLate
												orderby day.StartTime.Ticks
												select meeting).ToList().Distinct();
						workList = workList.Concat(meetingsFiltered).ToList();
						if (onlyToday) workList = SetTodaysMeetings(workList);
						filteredList = workList;
						break;
				}
			}
			return filteredList;
		}

		private List<Meeting> SetListSorted(List<Meeting> meetings, SortingTypes type, bool onlyToday = false)
		{
			var sortedList = onlyToday ? SetTodaysMeetings(meetings) : meetings;
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
						sortedList = onlyToday ? SetTodaysMeetings(Castings.ToList(sortedMeetings)) : Castings.ToList(sortedMeetings);
						break;
					case SortingTypes.TimeLateToEarly:
						var sortedMeetingList = (from meeting in sortedList
												 from day in meeting.DayAndTime
												 where day.StartTime.Ticks > 0
												 orderby day.StartTime.Ticks descending
												 select meeting).ToList().Distinct();
						sortedList = onlyToday ? SetTodaysMeetings(Castings.ToList(sortedMeetingList)) : Castings.ToList(sortedMeetingList);
						break;
				}
			}
			return sortedList;
		}

		private List<Meeting> SetTodaysMeetings(List<Meeting> meetings)
		{
			if (null != meetings && meetings.Any())
			{
				var curentDay = _aaClient.GetCurrentDay();
				var filteredMeetings = meetings.Where(m => m.DayAndTime.All(dt => dt.MeetingDay == curentDay && dt.StartTime.Ticks > 0))
					.Select(m => m)
					.Distinct()
					.ToList();
				return filteredMeetings;
			}
			return meetings;
		}
	}
}