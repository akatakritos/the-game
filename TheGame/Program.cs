using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;

using Newtonsoft.Json;

namespace TheGame
{
    public static class Program
    {
        private static HttpClient client;
        private static GameState _state;
        private static RulesEngine _rules;
        const string Me = "mburke";

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            _state = LoadState();
            _rules = new RulesEngine(_state);

            client = new HttpClient();
            client.DefaultRequestHeaders.Add("apiKey", "3f1d83f7-f778-425f-8a02-aa7001713183");
            client.BaseAddress = new Uri("http://thegame.nerderylabs.com");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            bool exit = false;
            while (!exit)
            {
                var key = LoopUntilKey();

                if (key.Key == ConsoleKey.Q)
                {
                    exit = true;
                }
                if (key.Key == ConsoleKey.U)
                {
                    PickItem();
                }
            }

            SaveState(_state);
        }

        private static ConsoleKeyInfo LoopUntilKey()
        {
            while (true)
            {
                SaveState(_state);
                //LogScore();

                if (Console.KeyAvailable)
                    return Console.ReadKey(true);

                try
                {
                    var result = Post("/points");
                    var response = JsonConvert.DeserializeObject<RootObject>(result);
                    _state.Points = response.Points;
                    _state.LastMessages = response.Messages;
                    _state.LastPoints = DateTime.UtcNow;
                    response.LogMessages();

                    _state.Effects = GetEffects();

                    if (response.Item != null)
                    {
                        foreach (var fields in response.Item.Fields)
                        {
                            _state.Items.Add(new GameItem()
                            {
                                Name = fields.Name,
                                Description = fields.Description,
                                Id = fields.Id,
                                Rarity = fields.Rarity
                            });
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Write(e.ToString());
                }

                if (_state.NextItem == null || _state.NextItem.Mode == ItemMode.Automatic)
                {
                    var optimalNextMove = GetOptimalNextItem();
                    _state.NextItem = optimalNextMove;
                }


                if (_rules.CanUseItem() && _state.NextItem != null)
                {
                    try
                    {
                        var useResult = UseItem(_state.NextItem.Item, _state.NextItem.Target);
                        _state.LastMessages.Insert(0, $"{_state.NextItem.Mode}ly used {_state.NextItem.Item.Name} on '{_state.NextItem.Target}'");
                        _state.LastMessages.InsertRange(0, useResult.Messages);
                        _state.NextItem = null;
                    }
                    catch (AggregateException e)
                    {
                        Log.Write(e.Message);
                        _state.NextItem = null;
                    }
                }

                ShowMenu();
                Sleep();
            }
        }

        private static string[] GetEffects()
        {
            try
            {
                var response = JsonConvert.DeserializeObject<PointsResponse>(Get("/points/mburke"));
                return response.Effects?.ToArray() ?? new string[0];
            }
            catch (AggregateException ex)
            {
                Log.Write("Error getting effects");
                Log.Write(ex.Message);
                return new string[0];
            }
        }

        public static NextItem GetOptimalNextItem()
        {
            if (GetLeaders().Select(l => l.PlayerName).Contains(Constants.Me))
            {
                //Favor defense first since im on leaderboard
                var defensive = TryDefensive();
                if (defensive != null)
                    return defensive;

                var powerup = TryPowerups();
                if (powerup != null)
                    return powerup;

                var attack = TryAttack();
                if (attack != null)
                    return attack;
            }
            else
            {
                //hoard defensive items

                var powerup = TryPowerups();
                if (powerup != null)
                    return powerup;

                var attack = TryAttack();
                if (attack != null)
                    return attack;
            }

            return null;
        }

        public static NextItem TryDefensive()
        {
            if (_state.DefensiveItems.Any())
            {
                var item = _state.DefensiveItems.First();
                return new NextItem()
                {
                    Item = item,
                    Target = null,
                    Mode = ItemMode.Automatic
                };
            }

            return null;
        }

        private static NextItem TryPowerups()
        {
            if (_state.PowerUpItems.Any())
            {
                var item = _state.PowerUpItems.First();
                return new NextItem()
                {
                    Item = item,
                    Target = null,
                    Mode = ItemMode.Automatic
                };
            }

            return null;
        }

        private static NextItem TryAttack()
        {
            if (_state.AttackItems.Any())
            {
                var item = _state.AttackItems.First();
                return new NextItem()
                {
                    Item = item,
                    Target = FindOptimumOpponent(item),
                    Mode = ItemMode.Automatic
                };
            }

            return null;
        }

        public static void PickItem()
        {
            Console.Clear();
            ShowLeaders();

            for (int i = 0; i < _state.Items.Count; i++)
            {
                Console.WriteLine($"{i}: {_state.Items[i]} ({_state.Items[i].Rarity})");
            }

            var s = Console.ReadLine();
            if (s == "q")
            {
                return;
            }

            var pieces = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int choice = int.Parse(pieces[0]);
            string target = pieces.Length > 1 ? pieces[1] : null;

            _state.NextItem = new NextItem()
            {
                Item = _state.Items[choice],
                Target = target,
                Mode = ItemMode.Manual
            };
        }

        private static void ShowMenu()
        {
            Console.Clear();
            ShowLeaders();

            foreach (var msg  in _state.LastMessages)
            {
                Console.WriteLine("> " + msg);
            }

            if (_state.NextItem != null)
            {
                Console.WriteLine($"{_state.NextItem.Mode} Item Scheduled: {_state.NextItem.Item.Name} on '{_state.NextItem.Target}'");
            }

            var remaining = _rules.NextItemTime - DateTime.UtcNow;
            Console.WriteLine($"{remaining.TotalSeconds} seconds until Item is ready");

            if (_rules.CanUseItem())
            {
                Console.WriteLine($"U: {_state.Items.Count} Items Available");
            }

            Console.WriteLine("Q: Quit");
        }

        private static void ShowLeaders()
        {
            var leaders = GetLeaders();
            foreach (var leader in leaders)
            {
                Console.WriteLine($"{leader.Points} - {leader.PlayerName}");
            }
        }

        private static LeaderboardResult[] LeadersCache = null;
        private static DateTime LeaderCacheExpires = DateTime.MinValue;

        private static IEnumerable<LeaderboardResult> GetLeaders()
        {
            if (LeadersCache == null || LeaderCacheExpires < DateTime.UtcNow)
            {
                LeadersCache = GetLeadersImpl();
                LeaderCacheExpires = DateTime.UtcNow.AddSeconds(10);
            }

            return LeadersCache;
        }

        private static LeaderboardResult[] GetLeadersImpl()
        {
            try
            {
                var result = Get("/");
                return JsonConvert.DeserializeObject<LeaderboardResult[]>(result);
            }
            catch (AggregateException e)
            {
                // oh well
                return LeadersCache;
            }
        }

        private static GameState LoadState()
        {
            if (File.Exists("state.json"))
            {
                return JsonConvert.DeserializeObject<GameState>(File.ReadAllText("state.json"));
            }

            return new GameState();
        }

        private static void SaveState(GameState state)
        {
            File.WriteAllText("state.json", JsonConvert.SerializeObject(state));
        }

        private static string FindOptimumOpponent(GameItem item)
        {
            var leaderboard = GetLeaders().ToArray();

            var nextUp = GetNextUp(leaderboard);
            if (nextUp != null)
            {
                return nextUp;
            }

            return leaderboard.FirstOrDefault(l => l.PlayerName != Me)?.PlayerName ?? "eweiss";
        }

        private static string GetNextUp(LeaderboardResult[] leaderboard)
        {
            var i = Array.IndexOf(leaderboard.Select(l => l.PlayerName).ToArray(), Me);
            if (i > 0)
            {
                return leaderboard[i - 1].PlayerName;
            }

            return null;
        }



        private static UseItemResult UseItem(GameItem item, string target = null)
        {
            _state.Items.Remove(item);

            var result = UseItem(item.Id, target);
            _state.LastItemUse = DateTime.UtcNow;

            return result;
        }

        private static UseItemResult UseItem(string itemId, string target = null)
        {
            var url = "/items/use/" + itemId;
            if (target != null)
            {
                url += "?target=" + target;
            }

            var response = Post(url);

            var result = JsonConvert.DeserializeObject<UseItemResult>(response);
            if (result.BonusItems.Any())
            {
                _state.Items.AddRange(result.BonusItems);
            }

            result.LogMessages();

            return result;
        }

        private static string Post(string url, string body = null)
        {
            var response = client.PostAsync(url, body == null ? null : new StringContent(body)).Result.Content.ReadAsStringAsync().Result;
            Log.Write($"POST {url} : {response}");

            return response;
        }

        private static string Get(string url)
        {
            var response = client.GetStringAsync(url).Result;
            //Log.Write($"GET {url} : {response}");

            return response;
        }

        private static void Sleep()
        {
            var nextTick = _state.LastPoints.AddMilliseconds(RulesEngine.PointsRateLimitMS);
            var sleepTime = nextTick - DateTime.UtcNow;
            if (sleepTime.TotalMilliseconds < 0)
                sleepTime = TimeSpan.FromMilliseconds(0);

            Thread.Sleep(sleepTime);
        }
    }
}
