using System;
using System.Collections.Generic;
using System.Linq;

using NFluent;

using Xunit;

namespace TheGame.Tests
{
    public class DefensiveItemSelectorTests : StateTest
    {
        private readonly DefenseSelector _subject;
        public DefensiveItemSelectorTests()
        {
            _subject = new DefenseSelector();
        }

        [Fact]
        public void ItSelectsDefensiveItems()
        {
            State.AddItems("Blue Shell", "Star");

            var move = _subject.GetNextMove(State);

            Check.That(move.Item.Name).IsEqualTo("Star");
            Check.That(move.Target).IsNull();
        }

        [Fact]
        public void ItOnlyPlaysOneDefensiveItemAtATime()
        {
            State.AddItems("Red Shell", "Carbuncle");
            State.AddEffects("Star");

            var move = _subject.GetNextMove(State);

            Check.That(move).IsNull();
        }
    }
}
