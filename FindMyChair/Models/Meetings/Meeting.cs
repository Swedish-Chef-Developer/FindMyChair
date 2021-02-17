using System.Collections.Generic;

namespace FindMyChair.Models.Meetings
{
    public class Meeting
    {
        public Meeting()
        {
            DayAndTime = new Dictionary<int, Dictionary<int, MeetingSpecific>>();
            Address = new Address();
        }
        public int Id { get; set; }
        public string GroupName { get; set; }
        public string Place { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Note { get; set; }
        public string DaysOpenMeeting { get; set; }
        public string MeetingInfo { get; set; }
        public bool HandicapFriendly { get; set; }
        public string AdditionalInformationHtmlString { get; set; }
        public List<string> NotesList { get; set; }
        public List<string> AdditionalInfo { get; set; }
        public Dictionary<int, Dictionary<int, MeetingSpecific>> DayAndTime { get; set; }
        public Address Address { get; set; }
    }
}