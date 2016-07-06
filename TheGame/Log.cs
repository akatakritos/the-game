using System;
using System.Collections.Generic;
using System.Linq;

using log4net;

namespace TheGame
{
    public static class Log
    {
        private static ILog _log = LogManager.GetLogger("Main");

        public static void Write(string msg)
        {
            _log.Info(msg);
        }
    }
}