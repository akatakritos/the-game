using System;
using System.Collections.Generic;
using System.Linq;

using NFluent;

using Xunit;

namespace TheGame.Tests
{
    public class PowerupSelectorTests : StateTest
    {
        [Fact]
        public void GetsFirstPowerUpItem()
        {
            State.AddItems("Carbuncle", "Wedge");

            var subject = new PowerupSelector();

            var move = subject.GetNextMove(State);


            Check.That(move.Item.Name == "Wedge");
            Check.That(move.Target).IsNull();
        }

        [Fact]
        public void DoesntStack()
        {
            State.AddEffects("Mushroom");
            State.AddItems("Mushroom");
            var subject = new PowerupSelector();

            var move = subject.GetNextMove(State);

            Check.That(move).IsNull();
        }

    }
}
