using System;
using System.Collections.Generic;
using System.Linq;

namespace TheGame.Tests
{
    internal static class GameStateExtensions
    {
        public static void AddItems(this GameState state, params string[] items)
        {
            state.Items.AddRange(items.Select(i => new GameItem() { Name = i }));
        }

        public static void AddEffects(this GameState state, params string[] states)
        {
            state.Effects = states.ToArray();
        }

        public static LeaderboardBuilder CreateLeaderboard(this GameState state)
        {
            return new LeaderboardBuilder(state);
        }
    }
}