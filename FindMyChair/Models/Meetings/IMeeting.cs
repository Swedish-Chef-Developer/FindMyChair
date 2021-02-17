using System;
using System.Collections.Generic;

namespace FindMyChair.Models.Meetings
{
    public interface IMeeting
    {
        int Id { get; set; }
        string GroupName { get; set; }
        string Place { get; set; }
        string Email { get; set; }
        string Phone { get; set; }
        string Note { get; set; }
        string AdditionalInfo { get; set; }
        Dictionary<int, TimeSpan> DayAndTime { get; set; }
        Address Address { get; set; }
    }
}
