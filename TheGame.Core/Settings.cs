using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace TheGame
{
    public static class Settings
    {
        public static bool ReadonlyMode => bool.Parse(ConfigurationManager.AppSettings["ReadonlyMode"] ?? "False");
        public static int ArchiveInterval => int.Parse(ConfigurationManager.AppSettings["ArchiveInterval"] ?? "10");
    }
}
