using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using FindMyChair.Models.Meetings;
using HtmlAgilityPack;
using FindMyChair.Utilities;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Configuration;
using System.Linq.Expressions;

namespace FindMyChair.Scrapers
{
	public class AAScraper
	{

		public async Task<IEnumerable<Meeting>> GetMeetingList()
		{
			var daysToUpdate = Convert.ToInt32(ConfigurationManager.AppSettings["MeetingJsonDayspan"]);
			var folderPath = string.Format("{0}MeetingLists", AppDomain.CurrentDomain.BaseDirectory);
			Directory.CreateDirectory(folderPath);
			var filePath = string.Format(@"{0}\AAMeetings.json", folderPath);
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
			//choose your website
			var url = "http://www.aa.se/aa-moten?search_term=&days_selection=&";
			//get the html page source 
			var httpClient = new HttpClient();
			var html = await httpClient.GetStringAsync(url);
			//store the html of the page in a variable
			var doc = new HtmlDocument();
			doc.LoadHtml(html);
			var meetingList = new List<Meeting>();
			var meetingNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'search-meetings-results-item')]");
			var culture = new CultureInfo("sv-SE");
			var orderedList = meetingNodes.OrderBy(mn => mn.Descendants("h3").FirstOrDefault().InnerText.Split(new string[] { "," }, 
													StringSplitOptions.RemoveEmptyEntries).FirstOrDefault().Trim());
			foreach (var meetingNode in orderedList)
			{
				var meeting = new Meeting
				{
					Id = int.Parse(meetingNode.Attributes["id"].Value.Replace("node-", "").Trim())
				};
				var fields = meetingNode.SelectNodes(".//div[contains(@class, 'field')]");
				var title = meetingNode.Descendants("h3").FirstOrDefault().InnerText;
				var isHandicapFriendly = null != meetingNode.Descendants("h3").FirstOrDefault().ChildNodes.Where(n => n.HasAttributes
										&& null != n.Attributes["class"]
										&& n.Attributes["class"].Value.Contains("circle")).FirstOrDefault();
				meeting.HandicapFriendly = isHandicapFriendly;
				var titleArray = title.Split(new string[] { "," },
													StringSplitOptions.RemoveEmptyEntries);
				meeting.GroupName = !string.IsNullOrWhiteSpace(titleArray.FirstOrDefault())
					? titleArray.FirstOrDefault().Trim()
					: string.Empty;
				if (null != titleArray && titleArray.Length > 1)
				{
					var districtsList = new List<District>();
					for (int i = 1; i < titleArray.Length; i++)
					{
						var district = new District();
						district.Id = i;
						district.DistrictName = titleArray[i];
						districtsList.Add(district);
					}
					meeting.Address.Districts = new List<District>();
					meeting.Address.Districts = districtsList;
				}
				var locationNode = fields.Where(n => n.HasAttributes
								&& null != n.Attributes["class"]
								&& !n.Attributes["class"].Value.Contains("field-label")).FirstOrDefault();
				var locationLink = (null != locationNode) ? locationNode.SelectNodes(".//a") : null;
				var locationUrl = (null != locationLink
							&& null != locationLink.FirstOrDefault()
							&& locationLink.FirstOrDefault().HasAttributes
							&& null != locationLink.FirstOrDefault().Attributes["href"])
								? locationLink.FirstOrDefault().Attributes["href"].Value.Trim()
								: string.Empty;
				meeting.Address.LocationLink = (null != locationLink) ? locationUrl : string.Empty;
				var address = null != locationLink
							&& null != locationLink.FirstOrDefault()
							&& locationLink.FirstOrDefault().HasAttributes
							&& null != locationLink.FirstOrDefault().InnerText
								? locationLink.FirstOrDefault().InnerText.Trim()
								: string.Empty;
				if (address.Contains(','))
				{
					var addressArray = address.Split(',');
					meeting.Address.Street = addressArray[0].Trim();
					meeting.Address.City = addressArray[1].Trim();
				}
				else
				{
					meeting.Address.Street = address;
					meeting.Address.City = string.Empty;
				}
				var place = fields.Where(n => n.HasAttributes
								&& null != n.Attributes["class"]
								&& n.ParentNode.Attributes["class"].Value.Contains("field-name-field-place")
								&& !n.Attributes["class"].Value.Contains("field-label")).FirstOrDefault();
				meeting.Place = null != place && !string.IsNullOrWhiteSpace(place.InnerText.Trim())
								? place.InnerText.Trim()
								: string.Empty;
				var phone = fields.Where(n => n.HasAttributes
							   && null != n.Attributes["class"]
							   && n.ParentNode.Attributes["class"].Value.Contains("field-name-field-phone")
							   && !n.Attributes["class"].Value.Contains("field-label")).FirstOrDefault();
				meeting.Phone = null != phone && !string.IsNullOrWhiteSpace(phone.InnerText.Trim())
								? phone.InnerText.Trim()
								: string.Empty;
				var email = fields.Where(n => n.HasAttributes
							   && null != n.Attributes["class"]
							   && n.ParentNode.Attributes["class"].Value.Contains("field-name-field-email")
							   && !n.Attributes["class"].Value.Contains("field-label")).FirstOrDefault();
				meeting.Email = null != email && !string.IsNullOrWhiteSpace(email.InnerText.Trim())
								? email.InnerText.Trim()
								: string.Empty;
				var daysOpenMeeting = fields.Where(n => null != n.ParentNode
								&& n.HasAttributes
							   && null != n.Attributes["class"]
							   && n.ParentNode.HasAttributes
							   && null != n.ParentNode.Attributes["class"]
							   && n.ParentNode.Attributes["class"].Value.Contains("field-type-text")).FirstOrDefault();
				try
				{
					if (null != daysOpenMeeting
										&& null != daysOpenMeeting.ChildNodes
										&& daysOpenMeeting.HasChildNodes
										&& daysOpenMeeting.ChildNodes.Count > -1)
					{
						meeting.DaysOpenMeeting = (daysOpenMeeting.ChildNodes.Count > 0
										&& null != daysOpenMeeting.ChildNodes[1]
										&& !string.IsNullOrWhiteSpace(daysOpenMeeting.ChildNodes[1].InnerText.Trim()))
										? daysOpenMeeting.ChildNodes[1].InnerText.Trim()
										: (daysOpenMeeting.ChildNodes.Count > -1
											&& null != daysOpenMeeting.ChildNodes[0]
											&& !string.IsNullOrWhiteSpace(daysOpenMeeting.ChildNodes[0].InnerText.Trim()))
											? daysOpenMeeting.ChildNodes[1].InnerText.Trim()
											: string.Empty;
					}
				}
				catch
				{

				}
				var note = fields.Where(n => n.HasAttributes
							   && null != n.Attributes["class"]
							   && n.ParentNode.Attributes["class"].Value.Contains("field-collection-view")).FirstOrDefault();
				meeting.Note = null != note && !string.IsNullOrWhiteSpace(note.InnerText.Trim())
								? note.InnerHtml.Trim()
								: string.Empty;
				var additionalInformation = new List<string>();
				var notesList = new List<string>();
				var infoFields = meetingNode.SelectNodes(".//div[contains(@class, 'field-item')]")
										.Where(n => n.HasAttributes
										&& null != n.Attributes["class"]
										&& n.Attributes["class"].Value.Contains("even")
										&& !n.Name.ToLower().StartsWith("<a"));
				var additionalInfoNode = meetingNode.SelectSingleNode(".//div[contains(@class, 'field-name-field-note')]");
				IEnumerable<HtmlNode> additionalInfoFields = new List<HtmlNode>();
				if (null != additionalInfoNode && null != additionalInfoNode.ChildNodes && additionalInfoNode.HasChildNodes)
				{
					additionalInfoFields = additionalInfoNode.SelectNodes(".//div[contains(@class, 'field-item')]")
											.Where(n => n.HasAttributes
											&& null != n.Attributes["class"]
											&& n.Attributes["class"].Value.Contains("even"));
				}
				IEnumerable<HtmlNode> additionalInfoNodes = new List<HtmlNode>();
				var counter = 0;
				var skipNext = false;
				if (null != infoFields)
				{
					foreach (var node in infoFields)
					{
						if (node.ParentNode.HasAttributes &&
							null != node.ParentNode.Attributes &&
							null != node.ParentNode.ParentNode.Attributes["class"] &&
							node.ParentNode.ParentNode.Attributes["class"].Value.Contains("field field-name-field-note") &&
							node.HasAttributes &&
							null != node.Attributes["class"] &&
							node.Attributes["class"].Value.Contains("field-label"))
						{
							break;
						}
						if (notesList.Contains(node.InnerText) ||
							notesList.Contains(node.InnerHtml))
						{
							continue;
						}
						if (counter == 0 && node.InnerHtml.StartsWith("<a") || node.Name.StartsWith("a")) continue;
						skipNext = false;
						if (node.HasAttributes && !string.IsNullOrWhiteSpace(node.InnerHtml) || !string.IsNullOrWhiteSpace(node.InnerText))
						{
							if (!string.IsNullOrWhiteSpace(node.InnerHtml.Trim()) &&
								!notesList.Contains(node.InnerHtml.Trim()) &&
								!notesList.Contains(node.InnerText.Trim()) &&
								null != meeting.MeetingInfo &&
								string.IsNullOrWhiteSpace(meeting.MeetingInfo.Trim()))
							{
								if (node.InnerText.Trim().ToLower().Contains("möte")) meeting.MeetingInfo = node.InnerText.Trim();
							}
							if (!string.IsNullOrWhiteSpace(node.InnerHtml) &&
								!notesList.Contains(node.InnerHtml.Trim()) &&
								!notesList.Contains(node.InnerText.Trim()) &&
								node.InnerHtml.Contains(node.InnerText.Trim()))
							{
								var outerHtml = textUtility.ConfirmHtml(node.InnerHtml);
								if (!string.IsNullOrWhiteSpace(outerHtml)) notesList.Add(outerHtml);
								skipNext = true;
								continue;
							}
							if (!string.IsNullOrWhiteSpace(node.InnerText) &&
								!notesList.Contains(node.InnerText.Trim()) &&
								!notesList.Contains(node.InnerHtml.Trim()) &&
								!skipNext)
							{
								notesList.Add(node.InnerText.Trim());
								skipNext = false;
							}
						}
						counter++;
					}
				}
				if (null != additionalInfoNodes)
				{
					foreach (var node in additionalInfoFields)
					{
						if (additionalInformation.Contains(node.InnerText) ||
							additionalInformation.Contains(node.InnerHtml))
						{
							continue;
						}
						if (!string.IsNullOrWhiteSpace(node.InnerHtml.Trim()) &&
						!additionalInformation.Contains(node.InnerHtml.Trim()) &&
						!additionalInformation.Contains(node.InnerText.Trim()))
						{
							var outerHtml = textUtility.ConfirmHtml(node.InnerHtml);
							if (!string.IsNullOrWhiteSpace(outerHtml)) additionalInformation.Add(outerHtml);
							skipNext = true;
							continue;
						}
						if (!string.IsNullOrWhiteSpace(node.InnerText.Trim()) &&
							!additionalInformation.Contains(node.InnerText.Trim()) &&
							!additionalInformation.Contains(node.InnerHtml.Trim()) &&
							!skipNext)
						{
							additionalInformation.Add(node.InnerText.Trim());
							skipNext = false;
						}
					}
				}

				meeting.NotesList = notesList;
				meeting.AdditionalInfo = additionalInformation;
				var stringBuilder = new StringBuilder();
				foreach (var node in additionalInformation)
				{
					stringBuilder.AppendFormat("<p>{0}</p>", node).AppendLine();
				}
				meeting.AdditionalInformationHtmlString = stringBuilder.ToString();
				var flagNode = meetingNode.SelectSingleNode(".//div[contains(@class, 'meeting-schedule')]//div[contains(@class, 'flag')]");
				var tableBody = meetingNode.SelectSingleNode(".//table[contains(@class, 'meetings-table')]/tbody");
				var languageNode = flagNode.ChildNodes.Where(n => null != n.Attributes && n.HasAttributes
					&& null != n.Attributes["class"]
					&& n.Attributes["class"].Value.Contains("flag-")).SingleOrDefault();
				if (null != languageNode)
				{
					var flagArray = languageNode.Attributes["class"].Value.Split(new char[] { ' ', '.' });
					var flag = flagArray[flagArray.Length - 1].Replace("flag-", "");
					meeting.Language = flag;
				}
				var tableBodyRows = tableBody.SelectNodes(".//tr");
				var dayAndTimes = new List<MeetingSpecific>();
				for (var r = 0; r < tableBodyRows.Count; r++)
				{
					var tableBodyCells = tableBodyRows[r].SelectNodes(".//td");
					if (null == tableBodyCells) continue;
					for (var dt = 0; dt < tableBodyCells.Count; dt++)
					{
						var meetingSpecific = new MeetingSpecific();
						DateTime.TryParseExact(tableBodyCells[dt].InnerText.Trim(), "HH:mm", CultureInfo.InvariantCulture,
																	  DateTimeStyles.None, out DateTime dateTime);
						TimeSpan time = dateTime.TimeOfDay;
						meetingSpecific.StartTime = time;
						CssStyleCollection style = new Panel().Style;
						var tdStyles = (null != tableBodyCells[dt] && tableBodyCells[dt].HasAttributes
							&& null != tableBodyCells[dt].Attributes["style"]
							&& !string.IsNullOrWhiteSpace(tableBodyCells[dt].Attributes["style"].Value.Trim()))
							? tableBodyCells[dt].Attributes["style"].Value.Trim()
							: string.Empty;
						style.Value = tdStyles;
						style.Remove("width");
						var styleColor = (null != style
							&& null != style.Value
							&& !string.IsNullOrWhiteSpace(style.Value))
							? style.Value.ToLower().Replace("background-color:", string.Empty)
							.Replace(";", string.Empty)
							.Trim()
							: string.Empty;
						var meetingType = scraperUtility.GetMeetingTypesAA(styleColor);
						var id = -1;
						int.TryParse((r.ToString() + dt.ToString()), out id);
						meetingSpecific.Id = id;
						meetingSpecific.Row = r;
						meetingSpecific.StartTime = time;
						meetingSpecific.MeetingDay = dt;
						meetingSpecific.MeetingType = meetingType;
						dayAndTimes.Add(meetingSpecific);
						meeting.DayAndTime = dayAndTimes;
					}

				}

				meetingList.Add(meeting);
			}
			return meetingList;
		}
	}
}