using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FindMyChair.Models.Meetings
{
	public class CAJsonToClass
	{
        public int id { get; set; }
        public List<Tag> tags { get; set; }
        public double distance { get; set; }
        public double city_distance { get; set; }
        public string timezone { get; set; }
        public Group group { get; set; }
        public bool suspended { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string day { get; set; }
        public string time { get; set; }
        public int duration { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Tag
    {
        public string word { get; set; }
        public string color { get; set; }
    }

    public class Area
    {
        public string area_name { get; set; }
        public int id { get; set; }
    }

    public class CaLocation
    {
        public string language { get; set; }
        public string country { get; set; }
        public string localized_country { get; set; }
        public string county { get; set; }
        public string localized_county { get; set; }
        public string city { get; set; }
        public string localized_city { get; set; }
        public string road { get; set; }
        public string localized_road { get; set; }
        public string lng { get; set; }
        public string lat { get; set; }
        public string formatted_address { get; set; }
        public string localized_formatted_address { get; set; }
        public string instructions { get; set; }
        public string virtual_location { get; set; }
    }

    public class Group
    {
        public int id { get; set; }
        public Area area { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public CaLocation location { get; set; }
        public bool is_virtual { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string web { get; set; }
    }
}