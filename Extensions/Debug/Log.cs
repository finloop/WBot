using System.IO;
using System;

namespace Bot.Extensions.Debug
{
    public class Log
    {
        public static void WriteLine(string line)
        {
            FileStream fs = new FileStream("debug.log", FileMode.OpenOrCreate, FileAccess.Write);
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(line);
                }
        
            Console.WriteLine(line);
        }
    }
}