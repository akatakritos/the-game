using System;
using System.Collections.Generic;
using System.Linq;

namespace TheGame.Tests
{
    public class StateTest
    {
        private readonly GameState _state;

        public StateTest()
        {
            _state = new GameState();
        }

        protected GameState State => _state;
    }
}