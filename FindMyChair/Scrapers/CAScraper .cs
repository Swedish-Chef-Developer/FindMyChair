﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using FindMyChair.Models.Meetings;
using HtmlAgilityPack;
using FindMyChair.Utilities;
using System.Text;
using FindMyChair.Types;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Configuration;
using System.IO;

namespace FindMyChair.Scrapers
{
	public class CAScraper
	{
		public async Task<IEnumerable<Meeting>> GetMeetingList()
		{
			var daysToUpdate = Convert.ToInt32(ConfigurationManager.AppSettings["MeetingJsonDayspan"]);
			var folderPath = string.Format("{0}MeetingLists", AppDomain.CurrentDomain.BaseDirectory);
			Directory.CreateDirectory(folderPath);
			var filePath = string.Format(@"{0}\CAMeetings.json", folderPath);
			var meetingsList = new List<Meeting>();
			var creation = File.GetCreationTime(filePath);
			var modification = File.GetLastWriteTime(filePath);
			if (null == creation || creation == DateTime.MinValue || null == modification || modification == DateTime.MinValue
				|| !File.Exists(filePath) || DateTime.Now >= creation.AddDays(daysToUpdate)
				|| DateTime.Now >= modification.AddDays(daysToUpdate))
			{
				var meetings = await SetMeetingList();
				if (null == meetings || !meetings.Any()) return meetingsList;
				if (File.Exists(filePath)) File.Delete(filePath);
				File.WriteAllText(filePath, JsonConvert.SerializeObject(meetings));
				using var file = File.CreateText(filePath);
				var serializer = new JsonSerializer();
				serializer.Serialize(file, meetings);
			}
			using (var file = File.OpenText(filePath))
			{
				var serializer = new JsonSerializer();
				meetingsList = serializer.Deserialize(file, typeof(List<Meeting>)) as List<Meeting>;
			}
			return meetingsList;
		}

		private async Task<IEnumerable<Meeting>> SetMeetingList()
		{
			var scraperUtility = new ScraperUtilities();
			var textUtility = new TextUtility();
			var url = "https://meetings.ca.org/api/v1/meetings?area=Sweden&current_day=0&order=city";
			var handler = new HttpClientHandler();
			var httpClient = new HttpClient(handler, false);
			var response = await httpClient.GetAsync(url);
			var caMeetings = new List<CAJsonToClass>();
			var culture = new CultureInfo("sv-SE");
			if (response.IsSuccessStatusCode)
			{
				var stream = await response.Content.ReadAsStreamAsync();
				var ret = scraperUtility.DeserializeFromStream(stream).ToString();
				caMeetings = JsonConvert.DeserializeObject<List<CAJsonToClass>>(ret);
			}
			var meetingList = new List<Meeting>();
			if (null != caMeetings && caMeetings.Any())
			{
				var dayAndTime = new List<MeetingSpecific>();
				foreach (var caMeeting in caMeetings.OrderBy(m => m.group.name, StringComparer.Create(culture, false)))
				{
					var meeting = new Meeting();
					var meetingExist = false;
					if (meetingList.Any(m => m.GroupName.Trim() == caMeeting.group.name.Trim()))
					{
						meeting = meetingList.Where(m => m.GroupName.Trim() == caMeeting.group.name.Trim()).FirstOrDefault();
						meetingExist = true;
					}
					if (!meetingExist)
					{
						meeting.Id = caMeeting.group.id;
						meeting.GroupName = caMeeting.group.name.Trim();
						meeting.Email = caMeeting.group.email;
						meeting.Phone = caMeeting.group.phone;
						meeting.GroupLink = caMeeting.group.web;
						meeting.Address.City = !string.IsNullOrWhiteSpace(caMeeting.group.location.localized_city)
							? caMeeting.group.location.localized_city
							: caMeeting.group.location.city;
						meeting.Address.Street = !string.IsNullOrWhiteSpace(caMeeting.group.location.localized_road)
							? caMeeting.group.location.localized_road
							: caMeeting.group.location.road;
						var district = new District
						{
							DistrictName = !string.IsNullOrWhiteSpace(caMeeting.group.location.localized_county)
							? caMeeting.group.location.localized_county
							: caMeeting.group.location.county,
							Id = caMeeting.group.id
						};
						var districtsList = new List<District> { district };
						meeting.Address.Districts = districtsList.AsEnumerable();
						var success = double.TryParse(caMeeting.group.location.lng, out double longLat);
						meeting.Address.Longitude = (success) ? longLat : 0;
						success = double.TryParse(caMeeting.group.location.lat, out longLat);
						meeting.Address.Lattitude = (success) ? longLat : 0;
						meeting.Language = caMeeting.group.location.language;
						Regex regEx = new Regex(@"(https?://[^\s]+)");
						var description = caMeeting.description.Replace("\r\n", "<br>");
						description = regEx.Replace(description, "<a href=\"$1\" target=\"_blank\">$1</a>");
						meeting.AdditionalInformationHtmlString = description;
						meeting.Note = caMeeting.group.description;
						meeting.Address.LocationLink = string.Format("https://maps.google.com/maps?q={0}",
							caMeeting.group.location.localized_formatted_address);
						meeting.Address.LocalizedAddress = !string.IsNullOrWhiteSpace(caMeeting.group.location.localized_formatted_address)
							? caMeeting.group.location.localized_formatted_address
							: caMeeting.group.location.formatted_address;
					}
					var meetingSpecifics = new MeetingSpecific();
					var tags = new List<MeetingTypes>();
					foreach (var tag in caMeeting.tags)
					{
						var type = scraperUtility.GetMeetingTypesCA(tag.word);
						if (type == MeetingTypes.NotSet) continue;
						tags.Add(type);
					}
					meetingSpecifics.MeetingTypes = tags.AsEnumerable();
					if (tags.Any())
					{
						meetingSpecifics.MeetingType = tags[0];
					}
					var meetingDay = (int)Enum.Parse(typeof(WeekdayNames), caMeeting.day);
					meetingSpecifics.Id = meetingDay;
					meetingSpecifics.MeetingDay = meetingDay;
					DateTime.TryParse(caMeeting.time, out DateTime dateTime);
					TimeSpan startTime = dateTime.TimeOfDay;
					TimeSpan endTime = dateTime.AddMinutes(caMeeting.duration).TimeOfDay;
					meetingSpecifics.StartTime = startTime;
					meetingSpecifics.EndTime = endTime;
					dayAndTime = (null != meeting.DayAndTime && meeting.DayAndTime.Any())
						? meeting.DayAndTime.ToList()
						: new List<MeetingSpecific>();
					if (null != dayAndTime && !dayAndTime.Any())
					{
						for (var t = 0; t < 7; t++)
						{
							dayAndTime.Add(new MeetingSpecific { MeetingDay = t, StartTime = TimeSpan.FromTicks(0), EndTime = TimeSpan.FromTicks(0) });
						}
					}
					if (null != dayAndTime)
					{
						if (dayAndTime.Any() && !dayAndTime.Contains(meetingSpecifics) && null == dayAndTime.FirstOrDefault(d => d.MeetingDay == meetingSpecifics.MeetingDay
								&& meetingSpecifics.StartTime.Ticks == d.StartTime.Ticks) && meetingSpecifics.StartTime.Ticks > 0)
						{
							for (var t = 0; t < 7; t++)
							{
								dayAndTime.Add(new MeetingSpecific { MeetingDay = t, StartTime = TimeSpan.FromTicks(0), EndTime = TimeSpan.FromTicks(0) });
							}
						}
						for (var t = 0; t < dayAndTime.Count; t++)
						{
							if (dayAndTime.Contains(meetingSpecifics) || meetingSpecifics.StartTime.Ticks <= 0) continue;
							if (dayAndTime[t].MeetingDay == meetingSpecifics.MeetingDay && dayAndTime[t].StartTime.Ticks <= 0 && meetingSpecifics.StartTime.Ticks > 0)
							{
								dayAndTime[t] = meetingSpecifics;
							}
						}
					}
					var lastRow = (dayAndTime.Count > 7) ? dayAndTime.GetRange(dayAndTime.Count - 7, 7).Sum(t => t.StartTime.Ticks) : -1;
					if (lastRow <= 0)
					{
						dayAndTime.RemoveRange(dayAndTime.Count - 7, 7);
					}
					meeting.DayAndTime = dayAndTime.AsEnumerable();

					if (meetingExist)
					{
						var indexMeeting = meetingList.Where(m => m.GroupName.Trim() == caMeeting.group.name.Trim()).FirstOrDefault();
						var index = meetingList.IndexOf(indexMeeting);
						if (index != -1)
							meetingList[index] = meeting;
					}
					else
					{
						meetingList.Add(meeting);
					}
				}
			}

			return meetingList;
		}

		private void AddDayTimeWeek(MeetingSpecific meetingSpecific, ref List<MeetingSpecific> list)
		{
			if (null != list && list.Any())
			{
				var workList = list.OrderBy(m => m.MeetingDay).ToList();
				for (var i = 0; i < 7; i++)
				{
					list.Add(new MeetingSpecific { MeetingDay = i });
				}
				for (var i = 0; i < 7; i++)
				{
					if (workList[i].MeetingDay == meetingSpecific.MeetingDay
						&& workList[i].StartTime.Ticks <= 0)
					{
						list[i] = meetingSpecific;
					}
				}
			}
		}
	}
}