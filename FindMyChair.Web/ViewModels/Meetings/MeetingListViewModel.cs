using FindMyChair.Models.Meetings;
using System.Collections.Generic;

namespace FindMyChair.Web.ViewModels.Meetings
{
	public class MeetingListViewModel
	{
		public string Title { get; set; }
		public string ListId { get; set; }
		public IEnumerable<Meeting> MeetingsList { get; set; }
	}
}