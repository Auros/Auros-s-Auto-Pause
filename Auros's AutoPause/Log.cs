using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AutoPause
{
    static class Log 
    {
        private static string modname = "AutoPause";

        public static void AutoPause(object message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[" + modname + "] " + message);
            Console.ResetColor();
        }
    }
}