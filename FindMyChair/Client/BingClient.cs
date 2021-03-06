﻿using FindMyChair.Models.Meetings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BingMapsRESTToolkit;
using System.Linq;
using FindMyChair.Models.Mapping;
using System.Globalization;
using FindMyChair.Client;
using FindMyChair.Types;
using FindMyChair.Utilities;
using System.Text;

namespace FindMyChair.Client
{
	public class BingClient : IBingClient
	{

		private string _bingApiKey;
		public BingClient()
		{

		}

		public BingClient(string apiKey)
		{
			_bingApiKey = apiKey;
		}

		public async Task<LocationLists> GetLocations(List<Meeting> meetings)
		{
			return await SetLocations(meetings);
		}

		private async Task<LocationLists> SetLocations(List<Meeting> meetings)
		{
			var returnList = new LocationLists();
			var workList = new List<Locations>();
			var aaClient = new AAClient();
			var utilities = new TextUtility();
			var today = aaClient.GetCurrentDay();
			for (var i = 0; i <  meetings.Count; i++)
			{
				if (string.IsNullOrWhiteSpace(meetings[i].Address.Street) || string.IsNullOrWhiteSpace(meetings[i].Address.City)) continue;
				var address = string.Format("{0}, {1}", meetings[i].Address.Street, meetings[i].Address.City);
				var time = meetings[i].DayAndTime.Where(m => m.MeetingDay == today)
					.Select(m => m.StartTime)
					.FirstOrDefault();
				DateTime dateTime = new DateTime(Math.Abs(time.Ticks));
				var type = meetings[i].DayAndTime.Select(m => m.MeetingType).FirstOrDefault();
				var startTime = dateTime.ToString("HH:mm", CultureInfo.CurrentCulture);
				var meetingType = utilities.GetEnumDescription(type);
				var description = string.Format("<div>{0}</div><div>{1} {2}</div><div>{3}</div>", address, startTime, meetingType, meetings[i].Place);
				workList.Add(await GetLonAndLatFromAddress(address,
					meetings[i].GroupName,
					i,
					description));
			}
			returnList.LocationList = workList;
			return returnList;
		}

		private async Task<Locations> GetLonAndLatFromAddress(string address, string groupName, int id, string description = "")
		{
			var location = new Locations();
			var request = new GeocodeRequest
			{
				BingMapsKey = _bingApiKey,
				Query = address
			};
			var result = await request.Execute();
			if (result.StatusCode == 200)
			{
				var toolkitLocation = (result?.ResourceSets?.FirstOrDefault())
						?.Resources?.FirstOrDefault()
						as Location;
				if (null != toolkitLocation && null == toolkitLocation.Point || toolkitLocation.Point.Coordinates.Length <= 0) return location;
				var latitude = toolkitLocation.Point.Coordinates[0];
				var longitude = toolkitLocation.Point.Coordinates[1];
				location.Longitude = longitude;
				location.Latitude = latitude;
				location.LocationId = id;
				location.Description = description;
				location.Title = groupName;			
					// 1, "Bhubaneswar","Bhubaneswar, Odisha", 20.296059, 85.824539
				/*
				var mapLocation = new Microsoft.Maps.MapControl.WPF.Location(latitude, longitude);
				this.userControl11.myMap.SetView(mapLocation, 15);
				var p = new Pushpin() { Location = mapLocation, ToolTip = "KLCC Park" };
				this.userControl11.myMap.Children.Add(p);*/
				return location;
			}
			return location;
		}
	}
}