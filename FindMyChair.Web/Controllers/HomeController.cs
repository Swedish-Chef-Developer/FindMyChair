using System.Web.Mvc;
using System.Collections.Generic;
using FindMyChair.Models.Meetings;
using FindMyChair.Web.ViewModels.Meetings;
using FindMyChair.Client;
using System.Threading.Tasks;
using System.Configuration;
using BingMapsRESTToolkit;
using FindMyChair.Models.Mapping;
using static FindMyChair.Types.FilterSortTypes;
using FindMyChair.Utilities;
using FindMyChair.Web.Models.Meetings;
using System.Linq;
using System.Data.SqlClient;
using FindMyChair.Types;
using System;

namespace FindMyChair.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly AAClient _aaClient;
		private readonly BingClient _bingClient;
		private readonly string _bingApiKey;
		private readonly FilterAndSortingUtility _filterAndSortingUtility;

		public HomeController()
		{
			_bingApiKey = ConfigurationManager.AppSettings["BingApiKey"];
			_bingClient = new BingClient(_bingApiKey);
			_aaClient = new AAClient();
			_filterAndSortingUtility = new FilterAndSortingUtility();
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
			/*UNCOMMENT TO ADD MAP
			 * if (null == Session["AALocationLists"])
			{
				Session["AALocationLists"] = await _bingClient.GetLocations(meetingListViewModel.UpcomingMeetingsList);
			}
			meetingListViewModel.LocationLists = Session["AALocationLists"] as LocationLists;
			*/
			model.BingApiKey = _bingApiKey;
			model.NearMeViewModel = meetingListViewModel;
			var d = await _aaClient.GetTimes(aaMeetingList);
			var aaFilter = new FilterSorting
			{
				CityList = await _aaClient.GetCities(aaMeetingList),
				MeetingTypeList = await _aaClient.GetMeetingTypes(aaMeetingList),
				StartTimes = Castings.ToList<TimeSpan>(d),
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
			var filtered = false;
			var sorted = false;
			var onlyToday = false;
			if (null == Session["AAMeetingList"])
			{
				Session["AAMeetingList"] = await _aaClient.GetMeetingsList();
			}
			var sortedList = Session["AAMeetingList"] as List<Meeting>;
			if (null != form["onlyToday"])
			{
				onlyToday = form["onlyToday"] == "on";
			}
			if (null != form["cities"] && form["cities"].Length > 0 && form["cities"].ToLower() != "inget val")
			{
				var cities = Castings.ToList(form["cities"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
				sortedList = _filterAndSortingUtility.GetListFiltered(sortedList, cities, FilterTypes.Cities, onlyToday);
				filtered = true;
			}
			if (null != form["meetingtypes"] && form["meetingtypes"].Length > 0 && form["meetingtypes"].ToLower() != "inget val")
			{
				var meetingTypes = Castings.ToList(form["meetingtypes"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
				sortedList = _filterAndSortingUtility.GetListFiltered(sortedList, meetingTypes, FilterTypes.Meetings, onlyToday);
				filtered = true;
			}
			if (null != form["starttime"] && form["starttime"].Length > 0 && form["starttime"].ToLower() != "inget val")
			{
				var earlyTimes = Castings.ToList(form["starttime"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
				sortedList = _filterAndSortingUtility.GetListFiltered(sortedList, earlyTimes, FilterTypes.EarliestTime, onlyToday);
				filtered = true;
			}
			if (null != form["latesttime"] && form["latesttime"].Length > 0 && form["latesttime"].ToLower() != "inget val")
			{
				var meetingTimes = Castings.ToList(form["latesttime"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
				sortedList = _filterAndSortingUtility.GetListFiltered(sortedList, meetingTimes, FilterTypes.LatestTime, onlyToday);
				filtered = true;
			}
			if (null != form["sorting"] && form["sorting"].Length > 0 && form["sorting"].ToLower() != "inget val")
			{
				switch (form["sorting"])
				{
					case "acsending-name":
						sortedList = _filterAndSortingUtility.GetListSorted(sortedList, SortingTypes.NameAZ);
						sorted = true;
						break;
					case "decsending-name":
						sortedList = _filterAndSortingUtility.GetListSorted(sortedList, SortingTypes.NameZA);
						sorted = true;
						break;
					case "time-ascending":
						sortedList = _filterAndSortingUtility.GetListSorted(sortedList, SortingTypes.TimeEarlyToLate);
						sorted = true;
						break;
					case "time-descending":
						sortedList = _filterAndSortingUtility.GetListSorted(sortedList, SortingTypes.TimeLateToEarly);
						sorted = true;
						break;
				}
			}
			if (onlyToday)
			{
				sortedList = _filterAndSortingUtility.GetTodaysMeetings(sortedList);
			}
			if (null != sortedList && sortedList.Any())
				return PartialView("~/Views/Home/Partials/_MeetingList.cshtml", sortedList);
			else
				return Content("<div class=\"no-result\">Inga resultat...</div>");
		}
	}
}