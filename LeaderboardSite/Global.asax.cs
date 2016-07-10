using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Routing;

using LeaderboardSite.Models;

using Newtonsoft.Json;

namespace LeaderboardSite
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            AddToCache();
        }

        private void AddToCache()
        {
            HttpRuntime.Cache.Add("leaderboard", GetLeaderboard(), null, DateTime.Now.AddMinutes(3), TimeSpan.Zero,
                CacheItemPriority.Default, OnRemoveCallback);
        }

        private void OnRemoveCallback(string key, object value, CacheItemRemovedReason reason)
        {
            AddToCache();
        }

        private IEnumerable<LeaderboardResult> GetLeaderboard()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            List<LeaderboardResult> results = new List<LeaderboardResult>(50);
            for (int i = 0; i < 5; i++)
            {
                var result = client.GetStringAsync("http://thegame.nerderylabs.com:1337/?page=" + i).Result;
                var leaders = JsonConvert.DeserializeObject<LeaderboardResult[]>(result);
                results.AddRange(leaders);
                Thread.Sleep(300);
            }

            return results.OrderByDescending(l => l.Points).ToList();
        }
    }
}
