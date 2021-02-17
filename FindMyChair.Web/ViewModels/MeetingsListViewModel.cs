using FindMyChair.Models.Meetings;
using FindMyChair.Models.Google;
using System.Collections.Generic;

namespace FindMyChair.Web.ViewModels
{
    public class MeetingsListViewModel
    {
        public List<Meeting>  AAMeetingsList { get; set; }
        public NearMeViewModel NearMeViewModel { get; set; }
        public string LongitudesAndLattitudesListString { get; set; }
        public List<LongitudeAndLatitude> LongitudesAndLattitudesList { get; set; }
    }
}