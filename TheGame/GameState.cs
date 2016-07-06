using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace TheGame
{
    public class GameState
    {
        public static readonly string[] DefenseiveItems = { "Star", "Gold Ring", "Varia Suit", "Fat Guys", "Skinny Guys" };
        public static readonly string[] SafeItemNames = { "Biggs", "Tanooki Suit", "Wedge", "Bo Jackson", "UUDDLRLRBA", "Treasure Chest", "Carbuncle", "Roger Wilco", "Da Da Da Da Daaa Da DAA da da", "Banana Peel", "Warthog", "Buffalo", "Red Crystal", "Moogle", "Pokeball", "Moogle" };
        public static readonly string[] AttackItemNames = { "Fire Flower", "Red Shell", "Cardboard Box", "Charizard", "Mushroom", "Hard Knuckle", "Crowbar", "Green Shell", "Hadouken", "Holy Water", "Pizza", "Fus Ro Dah", "Buster Sword", "Box of Bees", "Get Over Here", "Rail Gun", "Leisure Suit" };
        public static readonly string[] UserWhitelist = { "mburke" };

        public int Points { get; set; }
        public DateTime LastItemUse = DateTime.MinValue;
        public DateTime LastPoints = DateTime.MinValue;

        public List<GameItem> Items { get; set; } = new List<GameItem>();

        [JsonIgnore]
        public List<string> LastMessages { get; set; } = new List<string>();

        [JsonIgnore]
        public IEnumerable<GameItem> PowerUpItems => Items.Where(i => SafeItemNames.Contains(i.Name));

        [JsonIgnore]
        public IEnumerable<GameItem> AttackItems => Items.Where(i => AttackItemNames.Contains(i.Name));

        [JsonIgnore]
        public IEnumerable<GameItem> DefensiveItems => Items.Where(i => DefenseiveItems.Contains(i.Name));

        [JsonIgnore]
        public NextItem UseNext { get; set; }
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

    }
}