using System;
using System.Collections.Generic;
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
        public static List<LeaderboardResult> Get()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            List<LeaderboardResult> results = new List<LeaderboardResult>(50);
            for (int i = 0; i < 5; i++)
            {
                var result = client.GetStringAsync("http://thegame.nerderylabs.com:1337/?page=" + i).Result;
                var leaders = JsonConvert.DeserializeObject<LeaderboardResult[]>(result);
                results.AddRange(leaders);
                Thread.Sleep(50);
            }

            return results.OrderByDescending(l => l.Points).ToList();
        }
    }
}