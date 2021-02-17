﻿using System.Web.Mvc;
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
            var meetingListViewModel = new NearMeViewModel
			{
				UpcomingMeetingsList = await _aaClient.GetUpcomingMeetingsList(aaMeetingList),
				LongitudesAndLattitudesListString = "['Sandelsgatan 29', 59.3444559, 18.0896937, 1]"
			};
            meetingListViewModel.BingApiKey = _bingApiKey;
            meetingListViewModel.LocationLists = await _bingClient.GetLocations(meetingListViewModel.UpcomingMeetingsList);
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