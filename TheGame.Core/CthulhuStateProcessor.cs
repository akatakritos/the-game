using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TheGame
{
    public static class CthulhuStateProcessor
    {
        private static readonly Regex NonAsciiCharacters = new Regex(@"[^\u0000-\u007F]");
        public static void ProcessState(GameState state)
        {
            state.LastMessages = ProcessMessages(state.LastMessages).ToList();
        }

        public static IEnumerable<string> ProcessMessages(IEnumerable<string> messages)
        {
            return messages.Select(StripNonAscii);
        }

        private static string StripNonAscii(string lastMessage)
        {
            return NonAsciiCharacters.Replace(lastMessage, "");
        }
    }
}
