using System;
using System.Collections.Generic;
using System.Linq;

namespace TheGame
{
    public static class ExpiredItemRemover
    {
        public static readonly TimeSpan ExpireTime = TimeSpan.FromHours(48);

        public static void ProcessState(GameState state)
        {
            var itemsToRemove = state.Items.Where(IsExpired).ToList();
            foreach (var item in itemsToRemove)
            {
                Log.Write($"Removed expired {item.Name} {item.Id}");
                state.Items.Remove(item);

                var queue = state.MoveQueue.FirstOrDefault(m => m.Item.Id == item.Id);
                if (queue != null)
                    state.MoveQueue.Remove(queue);

                if (state.NextAutomaticMove != null && state.NextAutomaticMove.Item.Id == item.Id)
                    state.NextAutomaticMove = null;
            }
        }

        private static bool IsExpired(GameItem i) => DateTime.UtcNow - i.ItemAcquired > ExpireTime;
    }
}
