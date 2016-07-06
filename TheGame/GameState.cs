using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace TheGame
{
    public class GameState
    {
        public static readonly string[] UserWhitelist = { "mburke" };

        public int Points { get; set; }
        public DateTime LastItemUse = DateTime.MinValue;
        public DateTime LastPoints = DateTime.MinValue;

        public List<GameItem> Items { get; set; } = new List<GameItem>();

        [JsonIgnore]
        public List<string> LastMessages { get; set; } = new List<string>();

        [JsonIgnore]
        public IEnumerable<GameItem> PowerUpItems => Items.Where(i => Constants.SafeItemNames.Contains(i.Name) && !Effects.Contains(i.Name));

        [JsonIgnore]
        public IEnumerable<GameItem> AttackItems => Items.Where(i => Constants.AttackItemNames.Contains(i.Name) && !Effects.Contains(i.Name));

        [JsonIgnore]
        public IEnumerable<GameItem> DefensiveItems => Items.Where(i => Constants.DefenseiveItems.Contains(i.Name) && !Effects.Contains(i.Name));

        [JsonIgnore]
        public NextItem NextItem { get; set; }

        [JsonIgnore]
        public string[] Effects { get; set; } = new string[0];
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

    public class NextItem
    {
        public GameItem Item { get; set; }
        public string Target { get; set; }
        public ItemMode Mode { get; set; }
    }

    public enum ItemMode
    {
        Manual,
        Automatic
    }
}