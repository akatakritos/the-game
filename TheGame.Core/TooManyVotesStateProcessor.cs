using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text.RegularExpressions;

namespace TheGame
{
    public static class TooManyVotesStateProcessor
    {
        public static void ProcessState(GameState state)
        {
            foreach (var msg in state.LastMessages)
            {
                if (CheckTooManyVotesRule(msg))
                {
                    Log.Write("Voted too many times. Turning off.");
                    state.PollingEnabled = false;
                    state.ReEnabledPollsAt = GetTomorrowInUtc();
                }
            }

            CheckForReenable(state);
        }

        private static void CheckForReenable(GameState state)
        {
            if (!state.PollingEnabled && DateTime.UtcNow > state.ReEnabledPollsAt)
            {
                Log.Write("Voting re-enabled");
                state.PollingEnabled = true;
                state.ReEnabledPollsAt = DateTime.MaxValue;
            }
        }

        private static readonly Regex TooManyVotesRegex =
            new Regex(@"Voted over [\d,]+ times per day");

        private static bool CheckTooManyVotesRule(string msg)
        {
            if (TooManyVotesRegex.IsMatch(msg))
            {
                return true;
            }

            return false;
        }

        private static DateTime GetTomorrowInUtc()
        {
            var utcnow = DateTime.UtcNow;
            var localNow = TimeZoneInfo.ConvertTimeFromUtc(utcnow, Constants.TimeZone);
            var tomorrowMidnight = localNow.AddDays(1).Date;
            return tomorrowMidnight.ToUniversalTime();
        }
    }
}
