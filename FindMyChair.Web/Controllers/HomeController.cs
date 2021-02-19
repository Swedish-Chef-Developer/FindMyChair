using System.Web.Mvc;
using System.Collections.Generic;
using FindMyChair.Models.Meetings;
using FindMyChair.Web.ViewModels.Meetings;
using FindMyChair.Client;
using System.Threading.Tasks;
using System.Configuration;
using BingMapsRESTToolkit;
using FindMyChair.Models.Mapping;
using FindMyChair.Web.Models;
using FindMyChair.Web.Models.Meetings;
using System.Linq;

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
			var aaFilter = new FilterSorting
			{
				CityList = await _aaClient.GetCities(aaMeetingList),
				MeetingsList = aaMeetingList,
				BingApiKey = _bingApiKey
			};
			model.FilterSorting = aaFilter;
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

		[HttpPost]
		public async Task<ActionResult> AASortFilter(FormCollection form)
		{
			var sortedList = new List<Meeting>();
			var sorted = false;
			if (null == Session["AAMeetingList"])
			{
				Session["AAMeetingList"] = await _aaClient.GetMeetingsList();
			}
			var aaMeetingList = Session["AAMeetingList"] as List<Meeting>;
			if (null != form["cities"] && form["cities"].Length > 0)
			{
				if (form["cities"].Contains(','))
				{
					var cities = form["cities"].Split(',').ToList();
					var filteredMeetings = aaMeetingList.AsQueryable();
					foreach (var city in cities)
						filteredMeetings = filteredMeetings
							.Where(r => r.Address.City.Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries)
							.Where(s => s != string.Empty)
							.ToList()
							.Select(att => att.Trim())
							.Contains(city));
					sortedList = filteredMeetings.ToList();
					sorted = true;
				}
				else
				{
					var filteredMeetings = aaMeetingList.AsQueryable();
						filteredMeetings = filteredMeetings
							.Where(r => r.Address.City.Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries)
							.Where(s => s != string.Empty)
							.ToList()
							.Select(att => att.Trim())
							.Contains(form["cities"].Trim()));
					sortedList = filteredMeetings.ToList();
					sorted = true;
				}
			}
			if (null != form["sorting"] && form["sorting"].Length > 0)
			{
				switch (form["sorting"])
				{
					case "acsending-name":
						sortedList = aaMeetingList.OrderBy(m => m.GroupName).ToList();
						sorted = true;
						break;
					case "decsending-name":
						sortedList = aaMeetingList.OrderByDescending(m => m.GroupName).ToList();
						sorted = true;
						break;
				}

			}
			if (sorted)
				return PartialView("~/Views/Home/Partials/_MeetingList.cshtml", sortedList);
			else 
				return Content("<div>Inga resultat...</div>") ;
		}
	}
}