using System.Web.Mvc;
using System.Collections.Generic;
using FindMyChair.Models.Meetings;
using FindMyChair.Web.ViewModels;
using FindMyChair.Client;
using System.Threading.Tasks;
using System.Configuration;
using BingMapsRESTToolkit;
using FindMyChair.Models.Mapping;

namespace FindMyChair.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly AAClient _aaClient;
        private readonly BingClient _bingClient;
        private readonly string _bingApiKey;
        public HomeController()
		{
            _bingApiKey = ConfigurationManager.AppSettings["BingApiKey"];
            _bingClient = new BingClient(_bingApiKey);
            _aaClient = new AAClient();
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
            if (null == Session["AATodaysMeetingList"])
            {
                Session["AATodaysMeetingList"] = await _aaClient.GetUpcomingMeetingsList(aaMeetingList); 
            }
            var meetingListViewModel = new NearMeViewModel
            {
                UpcomingMeetingsList = Session["AATodaysMeetingList"] as List<Meeting>
            };
            meetingListViewModel.BingApiKey = _bingApiKey;
            if (null == Session["AALocationLists"])
            {
                Session["AALocationLists"] = await _bingClient.GetLocations(meetingListViewModel.UpcomingMeetingsList);
            }
            meetingListViewModel.LocationLists = Session["AALocationLists"] as LocationLists;
            model.BingApiKey = _bingApiKey;
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