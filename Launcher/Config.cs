using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;

namespace Launcher
{
    [ConfigPath(@".\Launcher.ini")]
    public static class Config
    {
        [ConfigSection("Patcher")]
        public static string Host { get; set; } = @"https://mirfiles.com/resources/mir3/zircon/patch/";
        public static bool UseLogin { get; set; }
        public static string Username { get; set; }
        public static string Password { get; set; }
    }
}
