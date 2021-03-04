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
using FindMyChair.Types;
using System;
using System.Web.Caching;

namespace FindMyChair.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly AAClient _aaClient;
		private readonly CAClient _caClient;
		private readonly NAClient _naClient;
		private readonly BingClient _bingClient;
		private readonly string _bingApiKey;
		private readonly int _daysToCache;
		private readonly DateTime _dateToCache;
		private readonly FilterAndSortingUtility _filterAndSortingUtility;

		public HomeController()
		{
			_bingApiKey = ConfigurationManager.AppSettings["BingApiKey"];
			_daysToCache = Convert.ToInt32(ConfigurationManager.AppSettings["MeetingJsonDayspan"]);
			_dateToCache = DateTime.Now.AddDays(_daysToCache);
			_bingClient = new BingClient(_bingApiKey);
			_aaClient = new AAClient();
			_caClient = new CAClient();
			_naClient = new NAClient();
			_filterAndSortingUtility = new FilterAndSortingUtility();
		}

		public async Task<ViewResult> Index()
		{
			var model = new MeetingsListViewModel();
			var meetingModel = new MeetingListViewModel();
			if (null == HttpContext.Cache.Get("AAMeetingList"))
			{
				var meetings = await _aaClient.GetMeetingsList();
				HttpContext.Cache.Insert("AAMeetingList", meetings, null, _dateToCache, Cache.NoSlidingExpiration);
			}
			var aaMeetingList = HttpContext.Cache.Get("AAMeetingList") as List<Meeting>;
			if (null == HttpContext.Cache.Get("CAMeetingList"))
			{
				var meetings = await _caClient.GetMeetingsList() as List<Meeting>;
				HttpContext.Cache.Insert("CAMeetingList", meetings, null, _dateToCache, Cache.NoSlidingExpiration);
			}
			var caMeetingList = HttpContext.Cache.Get("CAMeetingList") as List<Meeting>;
			var naList = _naClient.GetMeetingsList();
			meetingModel.Title = "AA möten";
			meetingModel.ListId = "aa";
			meetingModel.MeetingsList = aaMeetingList;
			model.AAMeetingsList = meetingModel;
			meetingModel = new MeetingListViewModel();
			meetingModel.Title = "CA möten";
			meetingModel.ListId = "ca";
			meetingModel.MeetingsList = caMeetingList;
			model.CAMeetingsList = meetingModel;
			meetingModel = new MeetingListViewModel();
			meetingModel.Title = "NA möten";
			meetingModel.ListId = "na";
			meetingModel.MeetingsList = await _naClient.GetMeetingsList();
			model.NAMeetingsList = meetingModel;
			if (null == Session["AATodaysMeetingList"])
			{
				Session["AATodaysMeetingList"] = await _aaClient.GetUpcomingMeetingsList(aaMeetingList);
			}
			meetingModel = new MeetingListViewModel();
			meetingModel.Title = "Dagens möten";
			meetingModel.ListId = "today";
			meetingModel.MeetingsList = Session["AATodaysMeetingList"] as List<Meeting>;
			var meetingListViewModel = new NearMeViewModel
			{
				UpcomingMeetingsList = meetingModel
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
				StartTimes = Castings.CustomToList<TimeSpan>(d),
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
			var earlyAndLate = false;
			var onlyToday = false;
			if (null == HttpContext.Cache.Get("AAMeetingList"))
			{
				var meetings = await _aaClient.GetMeetingsList();
				HttpContext.Cache.Insert("AAMeetingList", meetings, null, _dateToCache, Cache.NoSlidingExpiration);
			}
			var sortedList = HttpContext.Cache.Get("AAMeetingList") as List<Meeting>;
			if (null != form["onlyToday"])
			{
				onlyToday = form["onlyToday"] == "on";
			}
			if (null != form["cities"] && form["cities"].Length > 0 && form["cities"].ToLower() != "inget val")
			{
				var cities = Castings.CustomToList(form["cities"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
				sortedList = _filterAndSortingUtility.GetListFiltered(sortedList, cities, FilterTypes.Cities) as List<Meeting>;
			}
			if (null != form["meetingtypes"] && form["meetingtypes"].Length > 0 && form["meetingtypes"].ToLower() != "inget val")
			{
				var meetingTypes = Castings.CustomToList(form["meetingtypes"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
				sortedList = _filterAndSortingUtility.GetListFiltered(sortedList, meetingTypes, FilterTypes.Meetings) as List<Meeting>;
			}
			if ((null != form["starttime"] && form["starttime"].Length > 0 && form["starttime"].ToLower() != "inget val" && !earlyAndLate) &&
					(null != form["latesttime"] && form["latesttime"].Length > 0 && form["latesttime"].ToLower() != "inget val"))
			{
				var earlyAndLateTimes = new List<string> { form["starttime"].ToString(), form["latesttime"].ToString() };
				sortedList = _filterAndSortingUtility.GetListFiltered(sortedList, earlyAndLateTimes, FilterTypes.TimeBetweenEarlyAndLate) as List<Meeting>;
				earlyAndLate = true;
			}
			if (null != form["starttime"] && form["starttime"].Length > 0 && form["starttime"].ToLower() != "inget val" && !earlyAndLate)
			{
				var earlyTimes = Castings.CustomToList(form["starttime"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
				sortedList = _filterAndSortingUtility.GetListFiltered(sortedList, earlyTimes, FilterTypes.EarliestTime, onlyToday) as List<Meeting>;
			}
			if (null != form["latesttime"] && form["latesttime"].Length > 0 && form["latesttime"].ToLower() != "inget val" && !earlyAndLate)
			{
				var meetingTimes = Castings.CustomToList(form["latesttime"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
				sortedList = _filterAndSortingUtility.GetListFiltered(sortedList, meetingTimes, FilterTypes.LatestTime, onlyToday) as List<Meeting>;
			}
			if (onlyToday)
			{
				sortedList = Castings.CustomToList<Meeting>(_filterAndSortingUtility.GetTodaysMeetings(sortedList));
				var ee = sortedList;
			}
			if (null != form["sorting"] && form["sorting"].Length > 0 && form["sorting"].ToLower() != "inget val")
			{
				switch (form["sorting"])
				{
					case "acsending-name":
						sortedList = _filterAndSortingUtility.GetListSorted(sortedList, SortingTypes.NameAZ) as List<Meeting>;
						break;
					case "decsending-name":
						sortedList = _filterAndSortingUtility.GetListSorted(sortedList, SortingTypes.NameZA) as List<Meeting>;
						break;
					case "time-ascending":
						sortedList = _filterAndSortingUtility.GetListSorted(sortedList, SortingTypes.TimeEarlyToLate) as List<Meeting>;
						break;
					case "time-descending":
						sortedList = _filterAndSortingUtility.GetListSorted(sortedList, SortingTypes.TimeLateToEarly) as List<Meeting>;
						break;
				}
			}
			var model = new MeetingListViewModel
			{
				MeetingsList = sortedList,
				Title = "AA möten",
				ListId = "aa"
			};
			if (null != sortedList && sortedList.Any())
				return PartialView("~/Views/Home/Partials/_MeetingList.cshtml", model);
			else
				return Content("<div class=\"no-result\">Inga resultat...</div>");
		}
	}
}