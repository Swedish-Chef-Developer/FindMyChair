using FindMyChair.Models.Google;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using GoogleMaps.LocationServices;
using System.Web;
using System.Net;
using HtmlAgilityPack;
using System.Configuration;

namespace FindMyChair.Client
{
	public class GoogleClient : IGoogleClient
	{
		private string _googleApiKey;
		public GoogleClient()
		{

		}

		public GoogleClient(string apiKey)
		{
			_googleApiKey = apiKey;
		}

		public List<LongitudeAndLatitude> GetLongitudeAndLatitudeList(List<string> addresses)
		{
			return SetLongitudeAndLatitudesList(addresses);
		}

		public string GetLongitudeAndLatitudeListToJasonString(List<LongitudeAndLatitude> addresses)
		{
			return SetLongitudeAndLatitudeListToJasonString(addresses);
		}

		public string GetLongitudeAndLatitudeListToJasonString(List<string> addresses)
		{
			return SetLongitudeAndLatitudeListToJasonString(SetLongitudeAndLatitudesList(addresses));
		}

		public string GetLongitudeAndLatitudeListAsString(List<string> addresses)
		{
			return SetLongitudeAndLatitudeListAsString(addresses);
		}

		private List<LongitudeAndLatitude> SetLongitudeAndLatitudesList(List<string> addresses)
		{
			var returnList = new List<LongitudeAndLatitude>();

			
			return returnList;
		}

		private string SetLongitudeAndLatitudeListToJasonString(List<LongitudeAndLatitude> addresses)
		{
			// ['Maroubra Beach', -33.950198, 151.259302, 1]
			// Data for the markers consisting of a name, a LatLng and a zIndex for the
			// order in which these markers should display on top of each other.
			var stringBuilder = new StringBuilder();
			foreach (var link in addresses)
			{
				stringBuilder.AppendFormat("['Maroubra Beach', -33.950198, 151.259302, 1]").AppendLine();
			}
			return stringBuilder.ToString();
		}

		private LongitudeAndLatitude ParseStringToLongitudeAndLatitude(string address)
		{
			// ['Maroubra Beach', -33.950198, 151.259302, 1]
			// Data for the markers consisting of a name, a LatLng and a zIndex for the
			// order in which these markers should display on top of each other.
			var longitudeAndLatitude = new LongitudeAndLatitude();
			var json = JsonConvert.SerializeObject(longitudeAndLatitude);
			return longitudeAndLatitude;
		}

		private string SetLongitudeAndLatitudeListAsString(List<string> addresses)
		{
			// ['Maroubra Beach', -33.950198, 151.259302, 1]
			// Data for the markers consisting of a name, a LatLng and a zIndex for the
			// order in which these markers should display on top of each other.
			var stringBuilder = new StringBuilder();
			for (var i = 0; i < addresses.Count; i++)
			{
				var link = addresses[i];
				if (string.IsNullOrWhiteSpace(link)) continue;		
				var uri = new Uri(link);
				var query = uri.Query;
				var queryAddress = HttpUtility.UrlDecode(HttpUtility.ParseQueryString(query).Get("q"));
				// http://maps.google.com/maps?q=Vinkelv%C3%A4gen+1%2C+Matfors
				var address = queryAddress;
				/*
				var locationService = new GoogleLocationService(_googleApiKey);
				var point = locationService.GetLatLongFromAddress(address);
				var latitude = point.Latitude;
				var longitude = point.Longitude;
				*/
				var latitude = 59.3444559;
				var longitude = 18.0896937;
				stringBuilder.AppendFormat("['{0}', {1}, {2}, {3}]", queryAddress, latitude, longitude, i + 1).AppendLine();
			}
			return stringBuilder.ToString();
		}

		private string ParseStringToGoogleLongitudeAndLatitudeString(string address)
		{
			// ['Maroubra Beach', -33.950198, 151.259302, 1]
			// Data for the markers consisting of a name, a LatLng and a zIndex for the
			// order in which these markers should display on top of each other.
			return "";
		}
	}
}