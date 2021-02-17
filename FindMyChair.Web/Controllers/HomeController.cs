using System.Web.Mvc;
using System.Collections.Generic;
using FindMyChair.Models.Meetings;
using FindMyChair.Web.ViewModels;
using FindMyChair.Client;
using System.Threading.Tasks;
using System.Linq;
using System.Configuration;
using System;

namespace FindMyChair.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly AAClient _aaClient;
        private readonly GoogleClient _googleClient;
        public HomeController()
		{
            var googleApiKey = ConfigurationManager.AppSettings["GoogleApiKey"];
            _aaClient = new AAClient();
            _googleClient = new GoogleClient(googleApiKey);
		}

        public async Task<ViewResult> Index()
        {
            var model = new MeetingsListViewModel();
            if (null == Session["AAMeetingList"])
			{
                Session["AAMeetingList"] = await _aaClient.GetMeetingsList();
            }
            var aaMeetingList = Session["AAMeetingList"] as List<Meeting>;
            model.AAMeetingsList = aaMeetingList;
			var meetingListViewModel = new NearMeViewModel
			{
				UpcomingMeetingsList = await _aaClient.GetUpcomingMeetingsList(aaMeetingList),
				LongitudesAndLattitudesListString = "['Sandelsgatan 29', 59.3444559, 18.0896937, 1]"
			};
			model.NearMeViewModel = meetingListViewModel;
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}