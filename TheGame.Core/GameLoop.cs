using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace TheGame
{
    public class GameLoop
    {
        private readonly HttpClient client;
        private readonly GameState _state;
        private readonly RulesEngine _rules;
        private readonly IStrategy _strategy = new RootStrategy();
        private bool _readonly = true;

        public GameLoop(GameState state, RulesEngine rules)
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("apiKey", "3f1d83f7-f778-425f-8a02-aa7001713183");
            //client.BaseAddress = new Uri("http://thegame.nerderylabs.com");
            client.BaseAddress = new Uri("http://thegame.nerderylabs.com:1337");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _state = state;
            _rules = rules;
        }

        public async Task Tick()
        {
            // DONT TOUCH ANY FORM CONTROLS IN HERE
            try
            {
                if (_strategy.CanPollPoints(_state))
                {
                    if (!_readonly)
                    {
                        var response = await PostForPoints();
                        _state.LastPoints = DateTime.UtcNow;

                        _state.Points = response.Points;
                        _state.LastMessages = response.Messages;
                    }

                    _state.Effects = await GetEffects();
                    _state.Leaderboard = await GetLeaderboard();
                }
                else
                {
                    _state.LastMessages = new List<string>()
                    {
                        "Don't want to poll right now."
                    };
                }

            }
            catch (Exception e)
            {
                e.Log("Global Catch");
            }

            if (_state.NextMove == null || _state.NextMove.Mode == ItemMode.Automatic)
            {
                _state.NextAutomaticMove = _strategy.GetMove(_state);
            }

            if (_rules.CanUseItem() && _state.NextMove != null && !_readonly)
            {
                try
                {
                    var nextMove = _state.NextMove;

                    var useResult = await UseItem(nextMove.Item, nextMove.Target);
                    _state.LastMessages.Insert(0, $"{nextMove.Mode}ly used {nextMove.Item.Name} on '{nextMove.Target}'");
                    _state.LastMessages.InsertRange(0, useResult.Messages);

                    if (nextMove.Mode == ItemMode.Manual)
                        _state.MoveQueue.Pop();
                    else
                    {
                        _state.NextAutomaticMove = null;
                    }
                }
                catch (AggregateException e)
                {
                    e.Log("Using Item");
                    _state.NextAutomaticMove = null;
                }
            }
        }


        public async Task<PollResponse> PostForPoints()
        {
            for (int i = 0; i < 3; i++)
            {
                var result = await Post("/points");
                var response = JsonConvert.DeserializeObject<PollResponse>(result);
                if (response == null)
                {
                    Log.Write("Got empty reponse from /points. Trying again (1/3)");
                    await Task.Delay(100);
                    continue;
                }

                response.LogMessages();

                if (response.Item != null)
                {
                    _state.Items.AddRange(response.ExtractItems());
                }

                return response;
            }

            Log.Write("Could not load points");
            throw new Exception("Could not load points");
        }

        public async Task<string[]> GetEffects()
        {
            try
            {
                var response = JsonConvert.DeserializeObject<PointsResponse>(await Get("/points/mburke"));
                return response.Effects?.ToArray() ?? new string[0];
            }
            catch (AggregateException ex)
            {
                ex.Log("Getting Effects");
                return _state.Effects;
            }

        }

        public async Task<UseItemResult> UseItem(GameItem item, string target = null)
        {
            var result = await TryUseItem(item, target);

            _state.Items.Remove(item);
            _state.LastItemUse = DateTime.UtcNow;
            return result;
        }

        private async Task<UseItemResult> TryUseItem(GameItem item, string target = null)
        {
            for (int tries = 0; tries < 3; tries++)
            {
                Log.Write($"Using {item.Name} - Try {tries + 1}");
                var result = await UseItem(item.Id, target);
                if (result != UseItemResult.NullObject)
                {
                    return result;
                }

                await Task.Delay(250);
            }

            return UseItemResult.NullObject;
        }

        private async Task<UseItemResult> UseItem(string itemId, string target = null)
        {
            var url = "/items/use/" + itemId;
            if (target != null)
            {
                url += "?target=" + target;
            }

            var response = await Post(url);

            var result = response.StartsWith("No", StringComparison.Ordinal) || response.Contains("Invalid item GUID") ? null : JsonConvert.DeserializeObject<UseItemResult>(response);
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

        private async Task<string> Post(string url, string body = null)
        {
            Log.Write("POST " + url);
            var response = await client.PostAsync(url, body == null ? null : new StringContent(body)).Result.Content.ReadAsStringAsync();
            Log.Write($"response: {response}");

            return response;
        }

        private async Task<string> Get(string url)
        {
            await Task.Delay(300);
            var response = await client.GetStringAsync(url);
            //Log.Write($"GET {url} : {response}");

            return response;
        }

        private LeaderboardResult[] _leadersCache = null;
        private DateTime _leaderCacheExpires = DateTime.MinValue;

        public async Task<IEnumerable<LeaderboardResult>> GetLeaderboard()
        {
            if (_leadersCache == null || _leaderCacheExpires < DateTime.UtcNow)
            {
                _leadersCache = await GetLeadersImpl();
                _leaderCacheExpires = DateTime.UtcNow.AddSeconds(5);
            }

            return _leadersCache;
        }

        private async Task<LeaderboardResult[]> GetLeadersImpl()
        {
            try
            {
                var result = await Get("/");
                return JsonConvert.DeserializeObject<LeaderboardResult[]>(result);
            }
            catch (AggregateException e)
            {
                // oh well
                e.Log("Getting leaderboard");
                return _leadersCache ?? new LeaderboardResult[0];
            }
        }
    }
}
