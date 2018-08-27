using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Bot.Extensions.Debug;

namespace Bot.Core
{
    public class FileIO
    {
        public static ConfigParameters ReadConfigParameters(string filename)
        {
            try
            {
            using (StreamReader readtext = new StreamReader(filename))
            {
                string json = "";
                string temp;
                while ((temp = readtext.ReadLine()) != null)
                {
                    json += temp;
                }
                return JsonConvert.DeserializeObject<ConfigParameters>(json);
            }
                        }
            catch (IOException e)
            {
                Log.Exception(e);
                System.Environment.Exit(1);
            }
            return new ConfigParameters();
        }

        public static T ReadConfigJson<T>(T moduleParams)
        {
            try
            {
                using (StreamReader readtext = new StreamReader(typeof(T).Name + ".json"))
                {
                    string json = "";
                    string temp;
                    while ((temp = readtext.ReadLine()) != null)
                    {
                        json += temp;
                    }
                    return JsonConvert.DeserializeObject<T>(json);
                }
            }
            catch (IOException e)
            {
                Log.Exception(e);
                System.Environment.Exit(1);
            }
            return moduleParams;
        }

        public static void WriteConfigJson<T>(T moduleParams)
        {
            try
            {
                using (StreamWriter writetext = new StreamWriter(typeof(T).Name + ".json", false))
                {
                    string json = JsonConvert.SerializeObject(moduleParams);
                    writetext.Write(json);
                    writetext.Flush();
                }
            }
            catch (IOException e)
            {
                Log.Exception(e);
                System.Environment.Exit(1);
            }
        }

        public static void WriteString(string filename, string text)
        {
            try
            {
                using (StreamWriter writetext = new StreamWriter(filename))
                {
                    writetext.Write(text);
                    writetext.Flush();
                }
            }
            catch (IOException e)
            {
                Log.Exception(e);
                System.Environment.Exit(1);
            }
        }

        public class ConfigParameters
        {
            public List<String> channels { get; set; }
            public string botName { get; set; }
            public string oauth { get; set; }
            public string clientID {get; set;}
            public int port { get; set; }
            public string ip { get; set; }
        }
    }
}
