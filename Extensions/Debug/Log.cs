using System.IO;
using System;

namespace Bot.Extensions.Debug
{
    public class Log
    {
        public static void WriteLine(string line)
        {
            FileStream fs = new FileStream("debug.log", FileMode.Append, FileAccess.Write);
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(System.DateTime.Now.ToUniversalTime() +": "+ line);
                }
        
            Console.WriteLine(System.DateTime.Now.ToUniversalTime() +": " +line);
        }

        public static void Exception (Exception e) {
            WriteLine(e.Message);
            WriteLine(e.StackTrace);
            Environment.Exit(1);
        }
    }
}