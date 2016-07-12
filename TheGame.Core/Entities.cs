using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TheGame
{
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

    public class PollResponse
    {
        public List<string> Messages { get; set; }
        public Item Item { get; set; }
        public decimal Points { get; set; }
        public List<object> Effects { get; set; }
        public List<object> Badges { get; set; }

        public void LogMessages()
        {
            Log.Write("\r\n > " + string.Join("\r\n > ", Messages));
        }

        public IEnumerable<GameItem> ExtractItems()
        {
            if (Item == null)
                return Enumerable.Empty<GameItem>();

            return Item.Fields.Select(fields => new GameItem()
            {
                Name = fields.Name,
                Description = fields.Description,
                Id = fields.Id,
                Rarity = fields.Rarity,
                ItemAcquired = DateTime.UtcNow
            });
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
        public decimal Points { get; set; }
        public string Title { get; set; }
        public List<string> Effects { get; set; }
        public List<Badge> Badges { get; set; }

        public override string ToString()
        {
            return PlayerName;
        }
    }


    public class UseItemResult
    {
        public static readonly UseItemResult NullObject = new UseItemResult()
        {
            Messages = new List<string>() { "Empty Response" },
            Points = 0,
            TargetName = ""
        };

        public List<string> Messages { get; set; }
        public string TargetName { get; set; }
        public decimal Points { get; set; }

        private static readonly Regex BonusItemRegex = new Regex(@"<([a-f0-9\-]+)> \| <(.+)>");


        private readonly Lazy<GameItem[]> _bonusItems;
        public GameItem[] BonusItems => _bonusItems.Value;

        public UseItemResult()
        {
            _bonusItems = new Lazy<GameItem[]>(GetBonusItems);
        }

        private GameItem[] GetBonusItems()
        {
            return Messages.Where(m => BonusItemRegex.IsMatch(m))
                .Select(m => BonusItemRegex.Match(m))
                .Select(match =>
                    new GameItem()
                    {
                        Name = match.Groups[2].Value,
                        Id = match.Groups[1].Value,
                        Description = "Bonus Item",
                        ItemAcquired = DateTime.UtcNow
                    })
                .ToArray();
        }

        public void LogMessages()
        {
            Log.Write("\r\n > " + string.Join("\r\n > ", Messages));

        }


    }

    public class PointsResponse
    {
        public string PlayerName { get; set; }
        public string AvatarUrl { get; set; }
        public List<Badge> Badges { get; set; }
        public List<string> Effects { get; set; }
        public string Title { get; set; }
        public decimal Points { get; set; }
        public int ItemsGained { get; set; }
        public int ItemsUsed { get; set; }
    }
}
