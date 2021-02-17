﻿using FindMyChair.Models.Meetings;
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
			return SetMeetingDay();
		}

		private async Task<List<Meeting>> SetMeetingsList()
		{
			using (var fileStream = File.OpenRead("path/to/file.osm.pbf"))
			{
				// create source stream.
				var source = new PBFOsmStreamSource(fileStream);

				// filter all powerlines and keep all nodes.
				var filtered = from osmGeo in source
							   where osmGeo.Type == OsmSharp.OsmGeoType.Node ||
								(osmGeo.Type == OsmSharp.OsmGeoType.Way && osmGeo.Tags != null && osmGeo.Tags.Contains("power", "line"))
							   select osmGeo;

				// convert to complete stream.
				// WARNING: nodes that are partof powerlines will be kept in-memory.
				//          it's important to filter only the objects you need **before** 
				//          you convert to a complete stream otherwise all objects will 
				//          be kept in-memory.
				var complete = filtered.ToComplete();
			}
			return Castings.ToList(_aaScraper.MeetingList());
		}

		private async Task<List<Meeting>> SetUpcomingMeetingsList(List<Meeting> meetingList)
		{
			if (null != meetingList && meetingList.Any())
			{
				var currentTimeString = string.Format("{0}:{1}", DateTime.Now.Hour, DateTime.Now.Minute);
				var currentTimeSpan = TimeSpan.Parse(currentTimeString);
				var curentDay = SetMeetingDay();
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

		private int SetMeetingDay()
		{
			var dayOfWeek = DateTime.Now.DayOfWeek.ToString();
			Enum.TryParse(dayOfWeek, out WeekdayNames weekDay);
			var dayInt = (int)Enum.Parse(typeof(WeekdayNames), weekDay.ToString());
			return dayInt;
		}
	}
}