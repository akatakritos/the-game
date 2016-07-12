using System;
using System.Collections.Generic;
using System.Linq;

namespace TheGame
{
    public interface IMoveSelector
    {
        Move GetNextMove(GameState state);
    }

    public interface IStrategy
    {
        Move GetMove(GameState state);
        bool CanPollPoints(GameState state);
    }

    public abstract class BaseStrategy : IStrategy
    {

        protected IList<IMoveSelector> MoveSelectors { get; } = new List<IMoveSelector>();

        public virtual Move GetMove(GameState state)
        {
            foreach (var selector in MoveSelectors)
            {
                var move = selector.GetNextMove(state);
                if (move != null)
                    return move;
            }

            return null;
        }

        public virtual bool CanPollPoints(GameState state)
        {
            return true;
        }
    }

    public class PowerupOnlyStrategy : BaseStrategy
    {
        public PowerupOnlyStrategy()
        {
            MoveSelectors.Add(new PowerupSelector());
        }
    }

    public class DefensiveStrategy : BaseStrategy
    {
        public DefensiveStrategy()
        {
            MoveSelectors.Add(new DefenseSelector());
            MoveSelectors.Add(new PowerupSelector());
            MoveSelectors.Add(new RobustAttackSelector());
        }
    }

    /// <summary>
    /// Growth focused. Right now its only powerups
    /// </summary>
    public class GrowthHackingStrategy : BaseStrategy
    {
        public GrowthHackingStrategy()
        {
            MoveSelectors.Add(new PowerupSelector());
            //MoveSelectors.Add(new AttackSelector());
        }
    }

    /// <summary>
    /// Strategy that plays no items automatically
    /// </summary>
    public class ManualStrategy : BaseStrategy
    {
        // no selectors means no moves played
    }

    /// <summary>
    /// Switches between two strategies depending if on leaderboard or not
    /// </summary>
    public class RootStrategy : IStrategy
    {
        private readonly IStrategy _leaderboardStrategy;
        private readonly IStrategy _losingStrategy;
        private readonly IStrategy _singleStrategy;// = new PowerupOnlyStrategy();

        public RootStrategy()
        {
            //_leaderboardStrategy = new DefensiveStrategy();
            //_losingStrategy = new GrowthHackingStrategy();
            _leaderboardStrategy = new ManualStrategy();
            _singleStrategy = new ManualStrategy();
        }

        private IStrategy InternalStrategy(GameState state)
        {
            if (!state.PollingEnabled)
                return new ManualStrategy();

            if (_singleStrategy != null)
            {
                return _singleStrategy;
            }

            if (state.OnLeaderboard)
                return _leaderboardStrategy;
            else
                return _losingStrategy;
        }

        public Move GetMove(GameState state)
        {
            return InternalStrategy(state).GetMove(state);
        }

        public bool CanPollPoints(GameState state)
        {
            return InternalStrategy(state).CanPollPoints(state);
        }
    }


    public class PowerupSelector : IMoveSelector
    {
        public Move GetNextMove(GameState state)
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
        public Move GetNextMove(GameState state)
        {
            if (state.DefensiveItems.Any())
            {
                foreach (var item in state.DefensiveItems)
                {
                    // if we already have a defensive item, lets wait
                    if (state.Effects.Any(e => Constants.DefenseiveItems.Contains(e)))
                        continue;

                    return new Move()
                    {
                        Item = item,
                        Target = null,
                        Mode = ItemMode.Automatic
                    };

                }
            }

            return null;
        }
    }

    public class AttackSelector : IMoveSelector
    {
        public Move GetNextMove(GameState state)
        {
            if (state.AttackItems.Any())
            {
                var item = state.AttackItems.First();
                return new Move()
                {
                    Item = item,
                    Target = FindOptimumOpponent(state),
                    Mode = ItemMode.Automatic
                };
            }

            return null;
        }

        private static string FindOptimumOpponent(GameState state)
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
