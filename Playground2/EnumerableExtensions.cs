using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGame
{
    public static class EnumerableExtensions
    {
        public static string StringJoin(this IEnumerable<string> list)
        {
            if (list == null)
                return "";


            return string.Join(", ", list);
        }

        public static int IndexOf<T>(this IEnumerable<T> items, Predicate<T> predicate)
        {
            int index = 0;
            foreach (var item in items)
            {
                if (predicate(item))
                    return index;

                index++;
            }

            return -1;
        }
    }
}
