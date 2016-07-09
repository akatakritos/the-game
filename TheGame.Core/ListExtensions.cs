using System;
using System.Collections.Generic;
using System.Linq;

namespace TheGame
{
    public static class ListExtensions
    {
        public static T Pop<T>(this List<T> queue)
        {
            if (queue.Count == 0)
                return default(T);

            var element = queue[0];
            queue.RemoveAt(0);
            return element;
        }

        public static T Peek<T>(this List<T> queue)
        {
            if (queue.Count == 0)
                return default(T);

            return queue[0];
        }
    }
}
