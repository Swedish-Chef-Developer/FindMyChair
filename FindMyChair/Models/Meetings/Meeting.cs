using System.Collections.Generic;
using System.ComponentModel;

namespace FindMyChair.Models.Meetings
{
    public class Meeting
    {
        public Meeting()
        {
            DayAndTime = new List<MeetingSpecific>();
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
        [DisplayName("Day And Time")]
        public List<MeetingSpecific> DayAndTime { get; set; }
        public Address Address { get; set; }
    }
}