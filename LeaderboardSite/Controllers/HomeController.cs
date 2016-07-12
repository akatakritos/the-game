using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
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
            var leaderboard = GetLeaderboard();
            return View(leaderboard as List<LeaderboardResult>);
        }

        public IEnumerable<LeaderboardResult> GetLeaderboard()
        {
            var leaderboard = MemoryCache.Default.Get("leaderboard");
            if (leaderboard == null)
            {
                leaderboard = LeaderboardClient.Get();
                MemoryCache.Default.Add("leaderboard", leaderboard, DateTime.UtcNow.Add(Settings.CacheDuration));
            }

            return (List<LeaderboardResult>)leaderboard;
        }
    }
}