using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeaderboardSite
{
    public class Settings
    {
        public static TimeSpan CacheDuration { get; } = TimeSpan.FromMinutes(1);
    }
}