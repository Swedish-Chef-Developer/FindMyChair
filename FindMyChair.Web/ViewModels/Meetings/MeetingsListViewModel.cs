﻿using FindMyChair.Models.Meetings;
using FindMyChair.Web.Models.Meetings;
using System.Collections.Generic;

namespace FindMyChair.Web.ViewModels.Meetings
{
    public class MeetingsListViewModel
    {
        public MeetingListViewModel  AAMeetingsList { get; set; }
        public MeetingListViewModel CAMeetingsList { get; set; }
        public  MeetingListViewModel NAMeetingsList { get; set; }
        public NearMeViewModel NearMeViewModel { get; set; }
        public string LongitudesAndLattitudesListString { get; set; }
        public string BingApiKey { get; set; }
        public string Title { get; set; }
        public FilterSorting FilterSorting { get; set; }
    }
}