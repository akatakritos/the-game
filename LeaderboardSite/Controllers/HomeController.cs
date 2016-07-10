using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

using LeaderboardSite.Models;

namespace LeaderboardSite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var leaderboard = HttpRuntime.Cache.Get("leaderboard");
            return View(leaderboard as List<LeaderboardResult> ?? new List<LeaderboardResult>());
        }
    }
}