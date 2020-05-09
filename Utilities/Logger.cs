using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BSCTracker.Utilities
{
    public static class Logger
    {
        private static string _filepath = $"{AppDomain.CurrentDomain.BaseDirectory}/log.txt";
        public static void Log(string s)
        {
            using (StreamWriter writer = new StreamWriter(_filepath, true))
            {
                writer.WriteLine($"Message: {s} Date :{DateTime.Now.ToString()}");
            }

        }

        public static void Log(string s, Exception ex)
        {
            using (StreamWriter writer = new StreamWriter(_filepath, true))
            {
                writer.WriteLine( $"Log message:{s}\r\nError: {ex.Message}\r\n{ex.StackTrace}\r\n Date :{DateTime.Now.ToString()}");
            }
        }

        public static void Log(Exception ex)
        {
            using (StreamWriter writer = new StreamWriter(_filepath, true))
            {
                writer.WriteLine($"Error: {ex.Message}\r\n{ex.StackTrace}\r\n Date :{DateTime.Now.ToString()}");
            }

        }
    }
}
