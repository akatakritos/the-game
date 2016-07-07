using System;
using System.Collections.Generic;
using System.Linq;

namespace TheGame
{
    public static class ExceptionExtensions
    {
        public static void Log(this Exception ex, string context = null)
        {
            TheGame.Log.Error("Exception: " + context);
            TheGame.Log.Exception(ex);
        }
    }
}
