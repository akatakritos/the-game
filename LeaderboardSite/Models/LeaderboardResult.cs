using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeaderboardSite.Models
{
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

        public string EffectsList()
        {
            if (Effects == null)
                return "";

            return string.Join(", ", Effects);
        }


        public override string ToString()
        {
            return PlayerName;
        }
    }
}