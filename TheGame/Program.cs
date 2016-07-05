using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;

using log4net;

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

                if (Console.KeyAvailable)
                    return Console.ReadKey(true);

                try
                {
                    var result = Post("/points");
                    var response = JsonConvert.DeserializeObject<RootObject>(result);
                    _state.Points = response.Points;
                    _state.LastMessages = response.Messages;
                    _state.LastPoints = DateTime.Now;
                    response.LogMessages();

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


                if (_rules.CanUseItem())
                {
                    try
                    {
                        if (_state.UseNext != null)
                        {
                            Log.Write($"Manually using {_state.UseNext.Item.Name} on '{_state.UseNext.Target}'.");
                            var useResult = UseItem(_state.UseNext.Item, _state.UseNext.Target);

                            _state.LastMessages.Insert(0, $"Automatically used {_state.UseNext.Item.Name}");
                            _state.LastMessages.InsertRange(0, useResult.Messages);

                            _state.UseNext = null;
                        }
                        else if (GetLeaders().Select(l => l.PlayerName).Contains(Me))
                        {
                            //Favor defense first since im on leaderboard
                            if (!TryDefend())
                                TryAttack();
                        }
                        else
                        {
                            if (!TryAttack())
                                TryDefend();
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Write(e.Message);
                        _state.UseNext = null;
                    }
                }

                ShowMenu();
                Sleep();
            }
        }

        private static bool TryDefend()
        {
            if (_state.DefenseItems.Any())
            {
                var item = _state.DefenseItems.First();
                Log.Write($"Auto applying {item.Name} to myself.");

                var useResult = UseItem(item);
                _state.LastMessages.Insert(0, $"Automatically used {item.Name}");
                _state.LastMessages.InsertRange(0, useResult.Messages);
                return true;
            }

            return false;
        }

        private static bool TryAttack()
        {
            if (_state.AttackItems.Any())
            {
                var item = _state.AttackItems.First();
                Log.Write($"Auto attacking someone with {item.Name}");

                var useResult = Attack(item);
                _state.LastMessages.Insert(0, $"Automatically used {item.Name} against {useResult.TargetName}");
                _state.LastMessages.InsertRange(0, useResult.Messages);

                return true;
            }

            return false;
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

            _state.UseNext = new NextItem()
            {
                Item = _state.Items[choice],
                Target = target
            };
        }

        private static void ShowMenu()
        {
            Console.Clear();
            ShowLeaders();

            Console.WriteLine($"You: {_state.Points}");

            foreach (var msg  in _state.LastMessages)
            {
                Console.WriteLine("> " + msg);
            }

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
            if (LeadersCache == null || LeaderCacheExpires < DateTime.Now)
            {
                LeadersCache = GetLeadersImpl();
                LeaderCacheExpires = DateTime.Now.AddSeconds(30);
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

        private static UseItemResult Attack(GameItem item)
        {
            var target = GetLeaders().Where(l => _rules.CanAttack(l.PlayerName)).Select(l => l.PlayerName).FirstOrDefault() ?? "eweis";
            return UseItem(item, target);

        }

        private static UseItemResult UseItem(GameItem item, string target = null)
        {
            _state.Items.Remove(item);

            var result = UseItem(item.Id, target);
            _state.LastItemUse = DateTime.Now;

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
            if (result.BonusItem != null)
            {
                _state.Items.Add(result.BonusItem);
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
            var nextTick = _state.LastPoints.AddSeconds(1);
            var sleepTime = nextTick - DateTime.Now;
            if (sleepTime.TotalMilliseconds < 0)
                sleepTime = TimeSpan.FromMilliseconds(0);

            Thread.Sleep(sleepTime);
        }
    }

    public class GameState
    {
        public static readonly string[] SafeItemNames = { "Biggs", "Tanooki Suit", "Wedge", "Bo Jackson", "UUDDLRLRBA", "Mushroom", "Treasure Chest", "Carbuncle", "Gold Ring", "Roger Wilco", "Da Da Da Da Daaa Da DAA da da", "Banana Peel", "Warthog", "Buffalo", "Red Crystal", "Moogle", "Pokeball" };
        public static readonly string[] AttackItemNames = { "Fire Flower", "Red Shell", "Cardboard Box", "Charizard", "Hard Knuckle", "Crowbar", "Green Shell", "Hadouken", "Holy Water", "Pizza", "Blue Shell", "Fus Ro Dah", "Buster Sword", "Box of Bees", "Get Over Here" };
        public static readonly string[] UserWhitelist = { "espies", "nhotalli", "rvanbelk", "revans" };

        public int Points { get; set; }
        public DateTime LastItemUse = DateTime.MinValue;
        public DateTime LastPoints = DateTime.MinValue;
        public List<GameItem> Items { get; set; } = new List<GameItem>();
        public List<string> LastMessages { get; set; } = new List<string>();
        public IEnumerable<GameItem> DefenseItems => Items.Where(i => SafeItemNames.Contains(i.Name));
        public IEnumerable<GameItem> AttackItems => Items.Where(i => AttackItemNames.Contains(i.Name));
        public NextItem UseNext { get; set; }
    }

    public class NextItem
    {
        public GameItem Item { get; set; }
        public string Target { get; set; }
    }

    public class RulesEngine
    {
        private readonly GameState _state;

        public RulesEngine(GameState state)
        {
            _state = state;
        }

        public bool CanUseItem()
        {
            return DateTime.Now - _state.LastItemUse > TimeSpan.FromMinutes(1);
        }

        public bool CanAttack(string name)
        {
            return !GameState.UserWhitelist.Contains(name);
        }
    }

    public class GameItem
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        public int Rarity { get; set; }

        public override string ToString()
        {
            return $"{Name} - {Description}";
        }
    }

    public static class Log
    {
        private static ILog _log = LogManager.GetLogger("Main");

        public static void Write(string msg)
        {
            _log.Info(msg);
        }
    }


    public class Field
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public int Rarity { get; set; }
        public string Description { get; set; }
    }

    public class Item
    {
        public string Case { get; set; }
        public List<Field> Fields { get; set; }
    }

    public class RootObject
    {
        public List<string> Messages { get; set; }
        public Item Item { get; set; }
        public int Points { get; set; }
        public List<object> Effects { get; set; }
        public List<object> Badges { get; set; }

        public void LogMessages()
        {
            Log.Write("\r\n > " + string.Join("\r\n > ", Messages));
        }
    }

    public class Badge
    {
        public string BadgeName { get; set; }
    }

    public class LeaderboardResult
    {
        public string PlayerName { get; set; }
        public string AvatarUrl { get; set; }
        public int Points { get; set; }
        public string Title { get; set; }
        public List<object> Effects { get; set; }
        public List<Badge> Badges { get; set; }
    }


    public class UseItemResult
    {
        public List<string> Messages { get; set; }
        public string TargetName { get; set; }
        public int Points { get; set; }

        private static readonly Regex BonusItemRegex = new Regex(@"You found a bonus item! <([a-f0-9\-]+)> \| <(.+)>");

        public GameItem BonusItem
        {
            get
            {
                var bonusMessage = Messages.FirstOrDefault(m => BonusItemRegex.IsMatch(m));
                if (bonusMessage != null)
                {
                    var match = BonusItemRegex.Match(bonusMessage);
                    return new GameItem()
                    {
                        Name = match.Groups[2].Value,
                        Id = match.Groups[1].Value,
                        Description = "Bonus Item",
                    };
                }

                return null;
            }
        }

        public void LogMessages()
        {
            Log.Write("\r\n > " + string.Join("\r\n > ", Messages));

        }


    }
}
