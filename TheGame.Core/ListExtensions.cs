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

        public static void Swap<T>(this List<T> list, int index1, int index2)
        {
            if (index2 < 0 || index1 < 0 || index2 >= list.Count || index1 >= list.Count)
                return;

            T tmp = list[index2];
            list[index2] = list[index1];
            list[index1] = tmp;
        }
    }
}
