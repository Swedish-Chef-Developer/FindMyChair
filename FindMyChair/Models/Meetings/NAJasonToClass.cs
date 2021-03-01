using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FindMyChair.Models.Meetings
{
    public class NAMeeting
    {
        public string id_bigint { get; set; }
        public string worldid_mixed { get; set; }
        public string service_body_bigint { get; set; }
        public string weekday_tinyint { get; set; }
        public string start_time { get; set; }
        public string duration_time { get; set; }
        public string formats { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public string meeting_name { get; set; }
        public string location_text { get; set; }
        public string location_info { get; set; }
        public string location_street { get; set; }
        public string location_neighborhood { get; set; }
        public string location_municipality { get; set; }
        public string location_sub_province { get; set; }
        public string location_province { get; set; }
        public string location_postal_code_1 { get; set; }
        public string comments { get; set; }
        public string contact_phone_2 { get; set; }
        public string contact_email_2 { get; set; }
        public string contact_name_2 { get; set; }
        public string contact_phone_1 { get; set; }
        public string contact_email_1 { get; set; }
        public string contact_name_1 { get; set; }
        public string phone_meeting_number { get; set; }
        public string virtual_meeting_link { get; set; }
        public string root_server_uri { get; set; }
        public string format_shared_id_list { get; set; }
    }

    public class Format
    {
        public string key_string { get; set; }
        public string name_string { get; set; }
        public string description_string { get; set; }
        public string lang { get; set; }
        public string id { get; set; }
        public string world_id { get; set; }
        public string root_server_uri { get; set; }
        public string format_type_enum { get; set; }
    }

    public class NAJsonToClass
    {
        public List<NAMeeting> meetings { get; set; }
        public List<Format> formats { get; set; }
    }
}