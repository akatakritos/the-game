using System;
using System.Collections.Generic;
using System.Linq;

using NFluent;

using Xunit;

namespace TheGame.Tests
{
    public class ExpiredItemRemoverTests
    {
        [Fact]
        public void DeletesOldItems()
        {
            var state = new GameState();
            state.Items.Add(new GameItem { Name = "foo", ItemAcquired = DateTime.UtcNow });
            state.Items.Add(new GameItem { Name = "bar", ItemAcquired = DateTime.UtcNow.Subtract(ExpiredItemRemover.ExpireTime).AddMinutes(-1) });

            ExpiredItemRemover.ProcessState(state);

            Check.That(state.Items.Select(i => i.Name))
                .ContainsExactly("foo");
        }

        [Fact]
        public void ClearsThoseItemsFromTheQueueToo()
        {
            var state = new GameState();
            state.Items.Add(new GameItem { Name = "foo", Id="foo", ItemAcquired = DateTime.UtcNow });
            state.Items.Add(new GameItem { Name = "bar", Id="bar", ItemAcquired = DateTime.UtcNow.Subtract(ExpiredItemRemover.ExpireTime).AddMinutes(-1) });
            state.MoveQueue.Add(new Move {Item = state.Items[0]});
            state.MoveQueue.Add(new Move() { Item = state.Items[1] });

            ExpiredItemRemover.ProcessState(state);

            Check.That(state.MoveQueue.Select(i => i.Item.Name))
                .ContainsExactly("foo");
        }

        [Fact]
        public void ClearsManualNext()
        {
            var state = new GameState();
            state.Items.Add(new GameItem { Name = "foo", Id="foo", ItemAcquired = DateTime.UtcNow });
            state.Items.Add(new GameItem { Name = "bar", Id="bar", ItemAcquired = DateTime.UtcNow.Subtract(ExpiredItemRemover.ExpireTime).AddMinutes(-1) });
            state.NextAutomaticMove = new Move { Item = state.Items[1] };

            ExpiredItemRemover.ProcessState(state);

            Check.That(state.NextAutomaticMove).IsNull();
        }
    }
}
