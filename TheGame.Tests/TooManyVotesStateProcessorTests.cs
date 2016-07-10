using System;
using System.Collections.Generic;
using System.Linq;

using NFluent;

using Xunit;

namespace TheGame.Tests
{
    public class TooManyVotesStateProcessorTests
    {
        [Theory]
        [InlineData("You broke rule <Voted over 50,000 times per day> and got Ice Storm")]
        [InlineData("You broke rule <Voted over 40000 times per day> and got Some Penalty")]
        public void RecognizesTooManyVotesRule(string msg)
        {
            var state = new GameState();
            state.LastMessages.Add(msg);

            TooManyVotesStateProcessor.ProcessState(state);

            Check.That(state.PollingEnabled).IsFalse();
        }

        [Fact]
        public void SetsToMidnightForMyTimezone()
        {
            var state = new GameState();
            state.LastMessages.Add("You broke rule <Voted over 50,000 times per day> and got Ice Storm");

            TooManyVotesStateProcessor.ProcessState(state);

            // assumes test is running in cdt
            Console.WriteLine(state.ReEnabledPollsAt);
            Check.That(state.ReEnabledPollsAt)
                .IsEqualTo(DateTime.Today.AddDays(1).Date.ToUniversalTime());
        }

        [Fact]
        public void AfterTheNewDayStartsItsRenabled()
        {
            var state = new GameState();
            state.PollingEnabled = false;
            state.ReEnabledPollsAt = DateTime.Today.ToUniversalTime();

            TooManyVotesStateProcessor.ProcessState(state);

            Check.That(state.PollingEnabled).IsTrue();
            Check.That(state.ReEnabledPollsAt).IsEqualTo(DateTime.MaxValue);
        }

        [Fact]
        public void NothingCHangesIfItsNotTimeYet()
        {
            var state = new GameState();
            state.PollingEnabled = false;
            state.ReEnabledPollsAt = DateTime.UtcNow.Date.AddDays(1);

            TooManyVotesStateProcessor.ProcessState(state);

            Check.That(state.PollingEnabled).IsFalse();
        }
    }
}
