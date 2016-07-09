using System;
using System.Collections.Generic;
using System.Linq;

namespace TheGame
{
    public class RobustAttackSelector : IMoveSelector
    {
        readonly List<IAttackPipeline> _pipeline = new List<IAttackPipeline>();
        public RobustAttackSelector()
        {
            _pipeline.Add(new PlayerFilter(Constants.Me));
            _pipeline.Add(new InvinciblePlayerFilter());
            _pipeline.Add(new DontCrowbarIfPersonInFrontIsInvincible());
            _pipeline.Add(new OrderOpponentsInFrontOfMeFilter());
            _pipeline.Add(new ChooseWeaponAndTarget());
        }

        public Move GetNextMove(GameState state)
        {
            var args = new AttackItemPipelineArgs()
            {
                EligibleTargets = state.Leaderboard.ToList(),
                EligibleItems = state.Items.ToList(),
            };

            foreach (var processor in _pipeline)
            {
                processor.Process(state, args);

                if (args.Aborted)
                    return null;

                if (args.Selected)
                {
                    return new Move()
                    {
                        Item = args.SelectedItem,
                        Target = args.SelectedTarget,
                        Mode = ItemMode.Automatic
                    };
                }
            }

            return null;
        }
    }

    public class AttackItemPipelineArgs
    {
        public bool Aborted { get; private set; }
        public void Abort() => Aborted = true;
        public IList<GameItem> EligibleItems { get; set; }
        public IList<LeaderboardResult> EligibleTargets { get; set; }

        public bool Selected { get; private set; }
        public GameItem SelectedItem { get; private set; }
        public string SelectedTarget { get; private set; }

        public void Select(GameItem item, string target)
        {
            Selected = true;
            SelectedItem = item;
            SelectedTarget = target;
        }
    }


    public interface IAttackPipeline
    {
        void Process(GameState state, AttackItemPipelineArgs args);
    }

    public class AttackItemFilter : IAttackPipeline
    {
        public void Process(GameState state, AttackItemPipelineArgs args)
        {
            args.EligibleItems = args.EligibleItems.Where(i => i.IsOffensive).ToList();
        }
    }

    public class PlayerFilter : IAttackPipeline
    {
        private readonly string[] _whiteList;

        public PlayerFilter(params string[] whitelist)
        {
            _whiteList = whitelist.ToArray();
        }

        public void Process(GameState state, AttackItemPipelineArgs args)
        {
            args.EligibleTargets = args.EligibleTargets
                .Where(t => !_whiteList.Contains(t.PlayerName))
                .ToList();
        }
    }

    public class InvinciblePlayerFilter : IAttackPipeline
    {
        public void Process(GameState state, AttackItemPipelineArgs args)
        {
            args.EligibleTargets = args.EligibleTargets
                .Where(CanBeDamaged)
                .ToList();

            if (args.EligibleTargets.Count == 0)
                args.Abort();
        }

        private static bool CanBeDamaged(LeaderboardResult player)
        {
            return player.Effects.Any(e => !Constants.DefenseiveItems.Contains(e));
        }
    }

    public class DontCrowbarIfPersonInFrontIsInvincible : IAttackPipeline
    {
        public void Process(GameState state, AttackItemPipelineArgs args)
        {
            if (state.OnLeaderboard)
            {
                var botAhead = state.OpponentAtDelta(1);
                if (botAhead != null)
                {
                    if (botAhead.Effects.Any(e => Constants.DefenseiveItems.Contains(e)))
                    {
                        args.EligibleItems = args.EligibleItems
                            .Where(i => i.Name != "Crowbar")
                            .ToList();
                    }
                }
            }
        }
    }

    public class OrderOpponentsInFrontOfMeFilter : IAttackPipeline
    {
        public void Process(GameState state, AttackItemPipelineArgs args)
        {
            if (state.OnLeaderboard)
            {
                List<LeaderboardResult> output = new List<LeaderboardResult>();
                var leaderboard = state.Leaderboard.ToArray();

                var myPosition = state.Position;
                for (int i = myPosition; i >= 0; i--)
                {
                    var target = args.EligibleTargets.FirstOrDefault(t => t.PlayerName == leaderboard[i].PlayerName);
                    if (target != null)
                        output.Add(target);
                }

                for (int i = leaderboard.Length - 1; i >= myPosition; i--)
                {
                    var target = args.EligibleTargets.FirstOrDefault(t => t.PlayerName == leaderboard[i].PlayerName);
                    if (target != null)
                        output.Add(target);
                }

                args.EligibleTargets = output;
            }
        }
    }

    public class ChooseWeaponAndTarget : IAttackPipeline
    {
        public void Process(GameState state, AttackItemPipelineArgs args)
        {
            // assuming targets and weapons are already prioritized
            // just find the first compatible combination
            foreach (var target in args.EligibleTargets)
            {
                foreach (var item in args.EligibleItems)
                {
                    if (!target.Effects.Contains(item.Name))
                    {
                        args.Select(item, target.PlayerName);
                        return;
                    }
                }
            }

        }
    }


}
