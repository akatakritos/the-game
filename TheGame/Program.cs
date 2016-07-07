using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;

using Newtonsoft.Json;

namespace TheGame
{
    public static class Program
    {
        private static HttpClient client;
        private static GameState _state;
        private static RulesEngine _rules;
        private static IStrategy _strategy = new RootStrategy();
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
                    if (!_rules.HasNegativeEffect)
                    {
                        var result = Post("/points");
                        var response = JsonConvert.DeserializeObject<RootObject>(result);
                        _state.Points = response.Points;
                        _state.LastMessages = response.Messages;
                        response.LogMessages();

                        _state.Effects = GetEffects();
                        _state.Leaderboard = GetLeaderboard();

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
                    else
                    {
                        _state.LastMessages = new List<string>()
                        {
                            "Has a negative state, waiting"
                        };
                    }

                    // last loop
                    _state.LastPoints = DateTime.UtcNow;
                }
                catch (Exception e)
                {
                    Log.Write(e.ToString());
                }

                if (_state.NextMove == null || _state.NextMove.Mode == ItemMode.Automatic)
                {
                    _state.NextMove = _strategy.GetMove(_state, _rules);
                }


                if (_rules.CanUseItem() && _state.NextMove != null)
                {
                    try
                    {
                        var useResult = UseItem(_state.NextMove.Item, _state.NextMove.Target);
                        _state.LastMessages.Insert(0, $"{_state.NextMove.Mode}ly used {_state.NextMove.Item.Name} on '{_state.NextMove.Target}'");
                        _state.LastMessages.InsertRange(0, useResult.Messages);
                        _state.NextMove = null;
                    }
                    catch (AggregateException e)
                    {
                        Log.Write(e.Message);
                        _state.NextMove = null;
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
                return _state.Effects;
            }
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

            _state.NextMove = new Move()
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

            if (_state.NextMove != null)
            {
                Console.WriteLine($"{_state.NextMove.Mode} Item Scheduled: {_state.NextMove.Item.Name} on '{_state.NextMove.Target}'");
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
            var leaders = GetLeaderboard();
            foreach (var leader in leaders)
            {
                Console.WriteLine($"{leader.Points} - {leader.PlayerName}");
            }
        }

        private static LeaderboardResult[] LeadersCache = null;
        private static DateTime LeaderCacheExpires = DateTime.MinValue;

        private static IEnumerable<LeaderboardResult> GetLeaderboard()
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
                return LeadersCache ?? new LeaderboardResult[0];
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

        private static UseItemResult UseItem(GameItem item, string target = null)
        {
            var result = TryUseItem(item, target);

            _state.Items.Remove(item);
            _state.LastItemUse = DateTime.UtcNow;
            return result;
        }

        private static UseItemResult TryUseItem(GameItem item, string target = null)
        {
            for (int tries = 0; tries < 3; tries++)
            {
                Log.Write($"Using {item.Name} - Try {tries + 1}");
                var result = UseItem(item.Id, target);
                if (result != UseItemResult.NullObject)
                {
                    return result;
                }

                Thread.Sleep(250);
            }

            return UseItemResult.NullObject;
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
            if (result == null)
            {
                result = UseItemResult.NullObject;
            }
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
