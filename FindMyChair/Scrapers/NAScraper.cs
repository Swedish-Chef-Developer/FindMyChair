using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using FindMyChair.Models.Meetings;
using FindMyChair.Utilities;
using FindMyChair.Types;
using System.Web.UI.WebControls;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;

namespace FindMyChair.Scrapers
{
	public class NAScraper
	{
		public async Task<IEnumerable<Meeting>> GetMeetingList()
		{
			return await SetMeetingList();
		}
		private async Task<IEnumerable<Meeting>> SetMeetingList()
		{
			var scraperUtility = new ScraperUtilities();
			var textUtility = new TextUtility();
			var url = "https://www.nasverige.org/main_server/client_interface/jsonp/?switcher=GetSearchResults&get_used_formats&lang_enum=sv&data_field_key=location_postal_code_1,duration_time,start_time,time_zone,weekday_tinyint,service_body_bigint,longitude,latitude,location_province,location_municipality,location_street,location_info,location_text,location_neighborhood,formats,format_shared_id_list,comments,meeting_name,location_sub_province,worldid_mixed,root_server_uri,id_bigint,meeting_name,location_text,formatted_address,formatted_location_info,virtual_meeting_link,virtual_meeting_link,show_qrcode,virtual_meeting_link,phone_meeting_number,phone_meeting_number,show_qrcode,phone_meeting_number,latitude,longitude,latitude,longitude,map_word,latitude,longitude,contact_name_1,contact_phone_1,contact_email_1,contact_name_2,contact_phone_2,contact_email_2&services[]=2&recursive=1&sort_keys=start_time&callback=jQuery35108984379689974507_1614608665780&_=1614608665781";
			var handler = new HttpClientHandler();
			var httpClient = new HttpClient(handler, false);
			var response = await httpClient.GetStringAsync(url);
			var jsonString = response.Substring(response.IndexOf("(") + 1, response.LastIndexOf(")") - response.IndexOf("(") - 1);
			var jObject = JObject.Parse(jsonString);
			var naMeetings = new List<NAMeeting>();
			var meetingsList = JsonConvert.DeserializeObject<NAJsonToClass>(jObject.ToString());
			var culture = new CultureInfo("sv-SE");
			var meetingList = new List<Meeting>();

			if (null != meetingsList.meetings && meetingsList.meetings.Any())
			{
				foreach (var naMeeting in meetingsList.meetings.OrderBy(m => HttpUtility.HtmlDecode(m.meeting_name), StringComparer.Create(culture, false)))
				{
					var meeting = new Meeting();
					var meetingExist = false;
					var meetingSpecifics = new List<MeetingSpecific>();
					if (null != naMeeting.meeting_name.Trim() && meetingList.Any(m => m.GroupName.Trim() == HttpUtility.HtmlDecode(naMeeting.meeting_name.Trim())))
					{
						meeting = meetingList.Where(m => m.GroupName.Trim() == HttpUtility.HtmlDecode(naMeeting.meeting_name.Trim())).FirstOrDefault();
						meetingExist = true;
					}
					if (!meetingExist)
					{
						var dayAndTime = new List<MeetingSpecific>();
						meeting.GroupName = naMeeting.meeting_name.Trim();
						meeting.Id = Convert.ToInt32(naMeeting.id_bigint);
						meeting.Address.Street = naMeeting.location_street.Trim();
						meeting.Address.City = naMeeting.location_municipality.Trim();
						meeting.Address.Zip = naMeeting.location_postal_code_1.Trim();
						meeting.Address.FullAddress = string.Format("{0}, {1} {2}", naMeeting.location_street.Trim(),
							naMeeting.location_postal_code_1.Trim(),
							naMeeting.location_municipality.Trim());
						meeting.Place = naMeeting.location_text.Trim();
						meeting.Address.Longitude = double.Parse(naMeeting.longitude.Trim());
						meeting.Address.Lattitude = double.Parse(naMeeting.latitude.Trim());
						meeting.Address.LocationLink = string.Format("https://www.google.com/maps/search/?api=1&query={0}.0090711&q={1},17.0090711"
																		, meeting.Address.Longitude
																		, meeting.Address.Lattitude);
						meeting.GroupLink = naMeeting.root_server_uri.Trim();
						meeting.Email = naMeeting.contact_email_1.Trim();
						meeting.Phone = naMeeting.phone_meeting_number.Trim();
						meeting.Note = naMeeting.comments.Trim();
						meeting.Language = "sv";
						var districts = new List<District> { new District { DistrictName = naMeeting.location_province } };
						meeting.Address.Districts = districts.AsEnumerable();
						meeting.AdditionalInformationHtmlString = naMeeting.location_info.Trim();
						var meetingSpecific = new MeetingSpecific();
						DateTime.TryParse(naMeeting.start_time.Trim(), out DateTime dateTime);
						TimeSpan startTime = dateTime.TimeOfDay;
						TimeSpan endTime = dateTime.Add(TimeSpan.Parse(naMeeting.duration_time)).TimeOfDay;
						meeting.OnlineLink = naMeeting.virtual_meeting_link.Trim();
						meetingSpecific.MeetingDay = Convert.ToInt32(naMeeting.weekday_tinyint.Trim()) -1;
						meetingSpecific.StartTime = startTime;
						meetingSpecific.EndTime = endTime;
						var tags = new List<MeetingTypes>();
						var typesArray = naMeeting.formats.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
						foreach (var tag in typesArray)
						{
							var type = scraperUtility.GetMeetingTypesCA(tag.Trim());
							if (type == MeetingTypes.NotSet) continue;
							tags.Add(type);
						}
						meetingSpecific.MeetingTypes = tags.AsEnumerable();
						if (tags.Any())
						{
							meetingSpecific.MeetingType = tags[0];
						}
						meeting.HandicapFriendly = meetingSpecific.MeetingTypes.ToList().Contains(MeetingTypes.HandicapFriendly);
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
							if (dayAndTime.Any() && !dayAndTime.Contains(meetingSpecific) && null == dayAndTime.FirstOrDefault(d => d.MeetingDay == meetingSpecific.MeetingDay
									&& meetingSpecific.StartTime.Ticks == d.StartTime.Ticks) && meetingSpecific.StartTime.Ticks > 0)
							{
								for (var t = 0; t < 7; t++)
								{
									dayAndTime.Add(new MeetingSpecific { MeetingDay = t, StartTime = TimeSpan.FromTicks(0), EndTime = TimeSpan.FromTicks(0) });
								}
							}
							for (var t = 0; t < dayAndTime.Count; t++)
							{
								if (dayAndTime.Contains(meetingSpecific) || meetingSpecific.StartTime.Ticks <= 0) continue;
								if (dayAndTime[t].MeetingDay == meetingSpecific.MeetingDay && dayAndTime[t].StartTime.Ticks <= 0 && meetingSpecific.StartTime.Ticks > 0)
								{
									dayAndTime[t] = meetingSpecific;
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
							var indexMeeting = meetingList.Where(m => m.GroupName.Trim() == naMeeting.meeting_name.Trim()).FirstOrDefault();
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
			}

			return meetingList;
		}
	}
}