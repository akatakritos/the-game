using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Web;

using LeaderboardSite.Models;

using Newtonsoft.Json;

namespace LeaderboardSite
{
    public class LeaderboardClient
    {
        public static int MaxPages { get; } = int.Parse(ConfigurationManager.AppSettings["MaxPages"]);

        public static List<LeaderboardResult> Get()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            List<LeaderboardResult> results = new List<LeaderboardResult>(150);
            LeaderboardResult[] leaders;
            int page = 0;

            do
            {
                var result = client.GetStringAsync("http://thegame.nerderylabs.com:1337/?page=" + page).Result;
                leaders = JsonConvert.DeserializeObject<LeaderboardResult[]>(result);
                results.AddRange(leaders);
                page++;
                Thread.Sleep(5);
            } while (leaders.Length > 0 && page < MaxPages);

            return results.OrderByDescending(l => l.Points).ToList();
        }
    }
}