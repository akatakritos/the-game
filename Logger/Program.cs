using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

using Dapper;

using Newtonsoft.Json;

namespace Logger
{
    class Program
    {
        private static HttpClient _client;
        const string _connectionString = "Server=thegamemdb.database.windows.net;Database=thegame;User Id=thegame;Password=WinTh3Gam3!!";
        private static DateTime LastLoop = DateTime.MinValue;
        private static DateTime LastEffect = DateTime.MinValue;

        static void Main(string[] args)
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("apiKey", "3f1d83f7-f778-425f-8a02-aa7001713183");
            _client.BaseAddress = new Uri("http://thegame.nerderylabs.com");
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            LastEffect = GetLastLoggedEventTime();
            Poll();
        }

        private static DateTime GetLastLoggedEventTime()
        {
            const string query = "SELECT MAX(Timestamp) FROM EffectLog";
            using (var conn = OpenConnection())
            {
                Console.WriteLine("Getting last logged effect");
                return conn.ExecuteScalar<DateTime>(query);
            }
        }

        private static SqlConnection OpenConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public static void Poll()
        {
            while (true)
            {
                LastLoop = DateTime.UtcNow;

                try
                {
                    LogCurrentEffects();
                    LogPoints();
                }
                catch (AggregateException ex)
                {
                    // Probably means an issue with the apis
                    Console.WriteLine(ex);
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                }

                Sleep();
            }
        }

        private static void Sleep()
        {
            var nextLoop = LastLoop.AddSeconds(1);
            var duration = nextLoop - DateTime.UtcNow;
            if (duration.TotalMilliseconds < 0)
                return;

            Thread.Sleep(duration);
        }

        private static void LogCurrentEffects()
        {
            var effects = JsonConvert.DeserializeObject<EffectResult[]>(_client.GetStringAsync("/effects/mburke").Result);
            const string insert = "INSERT INTO EffectLog (Timestamp, Creator, Targets, EffectName, EffectType, Duration, VoteGain, Description, StatusEffect, StatusEffectDuration) VALUES (@Timestamp, @Creator, @Targets, @EffectName, @EffectType, @Duration, @VoteGain, @Description, @StatusEffect, @StatusEffectDuration)";
            using (var connection = OpenConnection())
            {
                foreach (var effect in effects.Where(e => e.Timestamp > LastEffect))
                {
                    var log = EffectLog.FromJson(effect);
                    Console.WriteLine($"Saving {log.Timestamp} - {log.EffectName}");
                    connection.Execute(insert, log);
                    LastEffect = log.Timestamp;
                }
            }

        }

        private static void LogPoints()
        {
            var response = JsonConvert.DeserializeObject<PointResponse>(_client.GetStringAsync("/points/mburke").Result);
            const string insert = "INSERT INTO PointLog (Timestamp, PlayerName, Badges, Effects, Title, Points, ItemsGained, ItemsUsed) VALUES (@Timestamp, @PlayerName, @Badges, @Effects, @Title, @Points, @ItemsGained, @ItemsUsed)";
            using (var connection = OpenConnection())
            {
                var log = PointLog.FromJson(response);
                Console.WriteLine($"Current Points: {log.Points}");
                connection.Execute(insert, log);
            }
        }
    }

    public class EffectLog
    {
        public DateTime Timestamp { get; set; }
        public string Creator { get; set; }
        public string Targets { get; set; }
        public string EffectName { get; set; }
        public string EffectType { get; set; }
        public string Duration { get; set; }
        public float VoteGain { get; set; }
        public string Description { get; set; }
        public string StatusEffect { get; set; }
        public string StatusEffectDuration { get; set; }

        public static EffectLog FromJson(EffectResult result)
        {
            return new EffectLog
            {
                Timestamp = result.Timestamp,
                Creator = result.Creator,
                Targets = result.Targets,
                Description = result.Effect.Description,
                Duration = result.Effect.Duration.Case,
                EffectName = result.Effect.EffectName,
                EffectType = result.Effect.EffectType,
                VoteGain = float.Parse(result.Effect.VoteGain),
                StatusEffect = result.Effect.Duration.Item?.Case,
                StatusEffectDuration = result.Effect.Duration.Item?.Item
            };
        }
    }

    public class PointLog
    {
        public DateTime Timestamp { get; set; }
        public string PlayerName { get; set; }
        public string Badges { get; set; }
        public string Effects { get; set; }
        public string Title { get; set; }
        public int Points { get; set; }
        public int ItemsGained { get; set; }
        public int ItemsUsed { get; set; }

        public static PointLog FromJson(PointResponse response)
        {
            return new PointLog
            {
                Timestamp = DateTime.UtcNow,
                PlayerName = response.PlayerName,
                Points = response.Points,
                Badges = response.Badges?.Select(b => b.BadgeName).StringJoin(),
                Effects = response.Effects.StringJoin(),
                ItemsGained = response.ItemsGained,
                ItemsUsed = response.ItemsUsed,
                Title = response.Title
            };
        }
    }

    public static class EnumerableExtensions
    {
        public static string StringJoin(this IEnumerable<string> list)
        {
            if (list == null)
                return "";


            return string.Join(", ", list);
        }
    }


    public class Badge
    {
        public string BadgeName { get; set; }
    }

    public class PointResponse
    {
        public string PlayerName { get; set; }
        public string AvatarUrl { get; set; }
        public List<Badge> Badges { get; set; }
        public List<string> Effects { get; set; }
        public string Title { get; set; }
        public int Points { get; set; }
        public int ItemsGained { get; set; }
        public int ItemsUsed { get; set; }
    }

    public class ItemObj
    {
        public string Case { get; set; }
        public string Item { get; set; }
    }

    public class Duration
    {
        public string Case { get; set; }
        public ItemObj Item { get; set; }
    }

    public class Effect
    {
        public string EffectName { get; set; }
        public string EffectType { get; set; }
        public Duration Duration { get; set; }
        public string VoteGain { get; set; }
        public string Description { get; set; }
    }

    public class EffectResult
    {
        public DateTime Timestamp { get; set; }
        public string Creator { get; set; }
        public string Targets { get; set; }
        public Effect Effect { get; set; }
    }
}
