using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace TheGame
{
    public class GameState
    {
        public static readonly string[] UserWhitelist = { "mburke" };

        public decimal Points { get; set; }

        public DateTime LastItemUse = DateTime.MinValue;


        [JsonProperty(PropertyName = "LastPoints")]
        public DateTime LastTick = DateTime.MinValue;

        public bool PollingEnabled { get; set; } = true;

        public DateTime ReEnabledPollsAt { get; set; } = DateTime.MaxValue;

        public List<GameItem> Items { get; set; } = new List<GameItem>();

        [JsonIgnore]
        public List<string> LastMessages { get; set; } = new List<string>();

        [JsonIgnore]
        public IEnumerable<GameItem> PowerUpItems => Items.Where(i => Constants.PowerupItemNames.Contains(i.Name) && !Effects.Contains(i.Name));

        [JsonIgnore]
        public IEnumerable<GameItem> AttackItems => Items.Where(i => Constants.AttackItemNames.Contains(i.Name) && !Effects.Contains(i.Name));

        [JsonIgnore]
        public IEnumerable<GameItem> DefensiveItems => Items.Where(i => Constants.DefenseiveItems.Contains(i.Name) && !Effects.Contains(i.Name));

        [JsonIgnore]
        public List<Move> MoveQueue { get; } = new List<Move>();

        [JsonIgnore]
        public Move NextAutomaticMove { get; set; }

        [JsonIgnore]
        public Move NextMove => MoveQueue.Peek() ?? NextAutomaticMove;

        [JsonIgnore]
        public string[] Effects { get; set; } = new string[0];

        [JsonIgnore]
        public IEnumerable<LeaderboardResult> Leaderboard { get; set; } = new LeaderboardResult[0];

        [JsonIgnore]
        public int Position => PositionOf(Constants.Me);

        [JsonIgnore]
        public bool OnLeaderboard => Leaderboard.Any(l => l.PlayerName == Constants.Me);

        /// <summary>
        /// Negative is in front of you
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public LeaderboardResult OpponentAtDelta(int i)
        {
            if (!OnLeaderboard)
                return null;

            return Leaderboard.ElementAtOrDefault(Position + i);
        }

        public int PositionOf(string player)
        {
            return Leaderboard.IndexOf(l => l.PlayerName == player);
        }
    }

    public class GameItem
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        public int Rarity { get; set; }
        public DateTime ItemAcquired { get; set; }

        public override string ToString()
        {
            return $"{Name} - {Description}";
        }

        public bool IsOffensive => Constants.AttackItemNames.Contains(Name);
        public bool IsDefensive => Constants.DefenseiveItems.Contains(Name);
        public bool IsPowerup => Constants.PowerupItemNames.Contains(Name);
    }

    public class Move
    {
        public GameItem Item { get; set; }
        public string Target { get; set; }
        public ItemMode Mode { get; set; }

        public override string ToString()
        {
            return $"{Item.Name} on {Target ?? Constants.Me}";
        }
    }

    public enum ItemMode
    {
        Manual,
        Automatic
    }
}