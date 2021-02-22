using FindMyChair.Models.Meetings;
using FindMyChair.Web.Models.Meetings;
using System.Collections.Generic;

namespace FindMyChair.Web.ViewModels.Meetings
{
    public class MeetingsListViewModel
    {
        public IEnumerable<Meeting>  AAMeetingsList { get; set; }
        public NearMeViewModel NearMeViewModel { get; set; }
        public string LongitudesAndLattitudesListString { get; set; }
        public string BingApiKey { get; set; }
        public FilterSorting FilterSorting { get; set; }
    }
}