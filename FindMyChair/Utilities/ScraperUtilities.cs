using FindMyChair.Types;
using Newtonsoft.Json;
using System.IO;

namespace FindMyChair.Utilities
{
	public class ScraperUtilities
	{
		/// <summary>
		/// Get type based on a string from style color
		/// in td of AA schedule table 
		/// </summary>
		/// <returns>MeetingType</returns>
		public MeetingTypes GetMeetingTypesAA(string cssColor)
		{
			return cssColor switch
			{
				("#000000") => MeetingTypes.Closed,
				("#97b6a7") => MeetingTypes.Mens,
				("#c987b6") => MeetingTypes.Womens,
				("#db9930") => MeetingTypes.YPAA,
				("#90b6c8") => MeetingTypes.Open,
				_ => MeetingTypes.NotSet,
			};
		}

		public MeetingTypes GetMeetingTypesCA(string type)
		{
			return type switch
			{
				("Slutet möte") => MeetingTypes.Closed,
				("Mansmöte") => MeetingTypes.Mens,
				("Kvinnomöte") => MeetingTypes.Womens,
				("#db9930") => MeetingTypes.YPAA,
				("Öppet möte") => MeetingTypes.Open,
				("Onlinemöte") => MeetingTypes.Online,
				("Stegmöte") => MeetingTypes.Step,
				("Traditionsmöte") => MeetingTypes.Tradition,
				_ => MeetingTypes.NotSet,
			};
		}

		public string GetMeetingTypesColorAA(MeetingTypes meetingType)
		{
			return meetingType switch
			{
				(MeetingTypes.Closed) => "#c1c1c1",
				(MeetingTypes.Mens) => "#90b6c8",
				(MeetingTypes.Womens) => "#c987b6",
				(MeetingTypes.YPAA) => "#db9930",
				(MeetingTypes.Open) => "#97b6a7",
				_ => "none",
			};
		}

		public object DeserializeFromStream(Stream stream)
		{
			var serializer = new JsonSerializer();
			using var sr = new StreamReader(stream);
			using var jsonTextReader = new JsonTextReader(sr);
			return serializer.Deserialize(jsonTextReader);
		}
	}
}