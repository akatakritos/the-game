using System;
using System.Collections.Generic;
using System.Linq;

namespace TheGame
{
    public interface IMoveSelector
    {
        Move GetNextMove(GameState state, RulesEngine rules);
    }

    public interface IStrategy
    {
        Move GetMove(GameState state, RulesEngine rules);
    }

    public abstract class BaseStrategy : IStrategy
    {

        protected IList<IMoveSelector> MoveSelectors { get; } = new List<IMoveSelector>();

        public virtual Move GetMove(GameState state, RulesEngine rules)
        {
            foreach (var selector in MoveSelectors)
            {
                var move = selector.GetNextMove(state, rules);
                if (move != null)
                    return move;
            }

            return null;
        }
    }

    public class DefensiveStrategy : BaseStrategy
    {
        public DefensiveStrategy()
        {
            MoveSelectors.Add(new DefenseSelector());
            MoveSelectors.Add(new PowerupSelector());
            MoveSelectors.Add(new AttackSelector());
        }
    }

    public class GrowthStrategy : BaseStrategy
    {
        public GrowthStrategy()
        {
            MoveSelectors.Add(new PowerupSelector());
            MoveSelectors.Add(new AttackSelector());
        }
    }

    public class RootStrategy : IStrategy
    {
        private readonly IStrategy _leaderboardStrategy;
        private readonly IStrategy _losingStrategy;

        public RootStrategy()
        {
            _leaderboardStrategy = new DefensiveStrategy();
            _losingStrategy = new GrowthStrategy();
        }

        public Move GetMove(GameState state, RulesEngine rules)
        {
            if (state.OnLeaderboard)
                return _leaderboardStrategy.GetMove(state, rules);
            else
                return _losingStrategy.GetMove(state, rules);
        }
    }


    public class PowerupSelector : IMoveSelector
    {
        public Move GetNextMove(GameState state, RulesEngine rules)
        {
            if (state.PowerUpItems.Any())
            {
                var item = state.PowerUpItems.First();
                return new Move()
                {
                    Item = item,
                    Target = null,
                    Mode = ItemMode.Automatic
                };
            }

            return null;
        }
    }

    public class DefenseSelector : IMoveSelector
    {
        public Move GetNextMove(GameState state, RulesEngine rules)
        {
            if (state.DefensiveItems.Any())
            {
                var item = state.DefensiveItems.First();
                return new Move()
                {
                    Item = item,
                    Target = null,
                    Mode = ItemMode.Automatic
                };
            }

            return null;
        }
    }

    public class AttackSelector : IMoveSelector
    {
        public Move GetNextMove(GameState state, RulesEngine rules)
        {
            if (state.AttackItems.Any())
            {
                var item = state.AttackItems.First();
                return new Move()
                {
                    Item = item,
                    Target = FindOptimumOpponent(state, item),
                    Mode = ItemMode.Automatic
                };
            }

            return null;
        }

        private static string FindOptimumOpponent(GameState state, GameItem item)
        {

            var nextUp = GetNextUp(state);
            if (nextUp != null)
            {
                return nextUp;
            }

            return state.Leaderboard.FirstOrDefault(l => l.PlayerName != Constants.Me)?.PlayerName ?? "eweiss";
        }

        private static string GetNextUp(GameState state)
        {
            int i = state.Position;

            if (i > 0)
            {
                return state.Leaderboard.ElementAtOrDefault(i-1)?.PlayerName;
            }

            return null;

        }
    }
}
