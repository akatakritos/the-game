using System;
using System.Collections.Generic;
using System.Linq;

using log4net;

namespace TheGame
{
    public static class Log
    {
        private static readonly ILog _log = LogManager.GetLogger("Main");

        public static void Write(string msg)
        {
            _log.Info(msg);
        }

        public static void Exception(Exception ex)
        {
            _log.Error(ex);
        }

        public static void Error(string s)
        {
            _log.Error(s);
        }
    }
}