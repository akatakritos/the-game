using System;
using System.Collections.Generic;
using System.Linq;

using NFluent;
using NFluent.Extensions;

using Xunit;

namespace TheGame.Tests
{
    public class AttackItemPipelineTests : StateTest
    {
        public AttackItemPipelineArgs BuildArgsFromState()
        {
            return new AttackItemPipelineArgs()
            {
                EligibleItems = State.Items.ToList(),
                EligibleTargets = State.Leaderboard.ToList()
            };
        }
    }

    public class LeaderboardBuilder
    {
        private readonly GameState _state;
        private readonly List<LeaderboardResult> _leaders;

        public LeaderboardBuilder(GameState state)
        {
            _state = state;
            _leaders = new List<LeaderboardResult>();
        }

        public LeaderboardBuilder AddLeader(string name)
        {
            _leaders.Add(new LeaderboardResult()
            {
                PlayerName = name,
                Effects = new List<string>()
            });

            return this;
        }

        public LeaderboardBuilder WithEffects(params string[] effects)
        {
            _leaders.Last().Effects = effects.ToList();
            return this;
        }

        public void Done()
        {
            _state.Leaderboard = _leaders.ToArray();
        }
    }

    public class AttackItemFilterTests : AttackItemPipelineTests
    {
        [Fact]
        public void ItFiltersDownToOnlyAttackItems()
        {
            State.AddItems("Star", "Gold Ring", "Red Shell", "Green Shell");
            var subject = new AttackItemFilter();
            var args = BuildArgsFromState();

            subject.Process(State, args);

            Check.That(args.Selected).IsFalse();
            Check.That(args.EligibleItems.Select(i => i.Name)).Contains("Red Shell", "Green Shell");
        }
    }

    public class PlayerFilterTests : AttackItemPipelineTests
    {
        [Fact]
        public void ItFiltersOutMyFriends()
        {
            State.CreateLeaderboard()
                .AddLeader("eweiss")
                .AddLeader("revans")
                .AddLeader("mburke")
                .AddLeader("jkalhoff")
                .Done();
            var subject = new PlayerFilter("mburke", "revans");
            var args = BuildArgsFromState();

            subject.Process(State, args);

            Check.That(args.Selected).IsFalse();
            Check.That(args.EligibleTargets.Select(i => i.PlayerName))
                .Not.Contains("mburke", "revans");
        }
    }

    public class InvinciblePlayerFilterTests : AttackItemPipelineTests
    {
        [Fact]
        public void ItFiltersOutPlayersWithProtectiveItems()
        {
            State.CreateLeaderboard()
                .AddLeader("eweiss").WithEffects("Star", "Carbuncle")
                .AddLeader("revans").WithEffects("Moogle")
                .Done();
            var subject = new InvinciblePlayerFilter();
            var args = BuildArgsFromState();

            subject.Process(State, args);

            Check.That(args.Aborted).IsFalse();
            Check.That(args.EligibleTargets.Select(t => t.PlayerName))
                .Not.Contains("eweiss");
        }

        [Fact]
        public void ItAbortsIfThereAreNoneLeft()
        {
            State.CreateLeaderboard()
               .AddLeader("eweiss").WithEffects("Star", "Carbuncle")
               .AddLeader("revans").WithEffects("Gold Ring")
               .Done();
            var subject = new InvinciblePlayerFilter();
            var args = BuildArgsFromState();

            subject.Process(State, args);

            Check.That(args.Aborted).IsTrue();
            Check.That(args.EligibleTargets).IsEmpty();
        }
    }

    public class DontAutomaticallyTargetNextGuyIfIncinvibleTests : AttackItemPipelineTests
    {

        [Theory]
        [InlineData("Crowbar")]
        [InlineData("Red Shell")]
        public void ItRemovesTheItemFromTheList(string autoTargetingItem)
        {
            State.CreateLeaderboard()
                .AddLeader("eweiss")
                .AddLeader("revans").WithEffects("Star")
                .AddLeader(Constants.Me)
                .Done();
            State.AddItems(autoTargetingItem, "Foo Item");
            var args = BuildArgsFromState();
            var subject = new AutomaticTargetNextGuyHandler();

            subject.Process(State, args);

            Check.That(args.EligibleItems.Select(i => i.Name))
                .Not.Contains(autoTargetingItem);
        }
    }

    public class OrderOpponentsInFrontOfMeFilterTests : AttackItemPipelineTests
    {
        [Fact]
        public void OpponentsInFrontOfMeGoFirstTHenTheRest()
        {
            State.CreateLeaderboard()
                .AddLeader("eweiss")
                .AddLeader("nhotalli")
                .AddLeader("revans")
                .AddLeader(Constants.Me)
                .AddLeader("espies")
                .AddLeader("jsmith")
                .Done();
            var args = BuildArgsFromState();
            var subject = new OrderOpponentsInFrontOfMeFilter();

            new PlayerFilter(Constants.Me, "nhotalli").Process(State, args);
            subject.Process(State, args);

            Check.That(args.EligibleTargets.Select(t => t.PlayerName))
                .ContainsExactly("revans", "eweiss", "jsmith", "espies");
        }
    }

    public class ChooseWeaponAndTargetTests : AttackItemPipelineTests
    {
        [Fact]
        public void GrabsFirstWeaponAgainstFirstTarget()
        {
            State.CreateLeaderboard()
                .AddLeader("eweiss")
                .AddLeader("revans")
                .Done();
            State.AddItems("Red Shell", "Holy Water");

            var args = BuildArgsFromState();
            var subject = new ChooseWeaponAndTarget();

            subject.Process(State, args);

            Check.That(args.Selected).IsTrue();
            Check.That(args.SelectedItem.Name).IsEqualTo("Red Shell");
            Check.That(args.SelectedTarget).IsEqualTo("eweiss");
        }

        [Fact]
        public void DoesntPairItemAgainstOpponentWithThatEffectAlready()
        {
            State.CreateLeaderboard()
                .AddLeader("eweiss").WithEffects("Holy Water")
                .AddLeader("revans")
                .Done();
            State.AddItems("Holy Water", "Red Shell");
            var args = BuildArgsFromState();
            var subject = new ChooseWeaponAndTarget();

            subject.Process(State, args);

            Check.That(args.Selected).IsTrue();
            Check.That(args.SelectedItem.Name).IsEqualTo("Red Shell");
            Check.That(args.SelectedTarget).IsEqualTo("eweiss");
        }
    }

    public class BananaHandlerTests : AttackItemPipelineTests
    {
        [Fact]
        public void UsesBananaAgainstGuyBehindMe()
        {
            State.CreateLeaderboard()
                .AddLeader("eweiss")
                .AddLeader(Constants.Me)
                .AddLeader("revans")
                .Done();
            State.AddItems("Banana Peel");
            var args = BuildArgsFromState();
            var subject = new BananaHandler();

            subject.Process(State, args);

            Check.That(args.Selected).IsTrue();
            Check.That(args.SelectedItem.Name).IsEqualTo("Banana Peel");
            Check.That(args.SelectedTarget).IsEqualTo("revans");
        }

        [Fact]
        public void ItDoesntFreakOutIfThePlayerBehindMeIsNotEligible()
        {
            // for example if removed for being invincible
            State.CreateLeaderboard()
                .AddLeader("eweiss")
                .AddLeader(Constants.Me)
                .AddLeader("revans")
                .Done();
            var args = BuildArgsFromState();
            args.EligibleTargets.RemoveAt(2); //remove revans
            var subject = new BananaHandler();

            subject.Process(State, args);

            Check.That(args.Selected).IsFalse();
        }
    }
}
