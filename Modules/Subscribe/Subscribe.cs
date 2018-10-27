using Bot.Core;
using System.Collections.Generic;
using System;
namespace Bot.Modules.Subscribe
{
    public class Subscribe : PassiveModule
    {

        public Subscribe(ModuleManager _moduleManager, IRC _irc) : base(_moduleManager, _irc) {
            addId("!sub");
            addId("!unsub");
            addId("!debug");
        }
        
        override public void HandleMessage(string channel, string msg, string sender) 
        {
            if(msg.StartsWith("!debug") && sender == "lordozopl" && msg.Split(' ').Length >= 3) {
                string[] temp = msg.Split(' ');
                msg = temp[1];
                sender = temp[2];
            }
            Console.WriteLine(msg);
            if (msg.StartsWith("!sub"))
            {
                
                string[] temp = msg.Split(' ');
                if (temp.Length >= 2)
                {
                    for (int i = 1; i < temp.Length; i++)
                    {
                        base.moduleManager.AddModuleToChannel(channel, temp[i]);
                    }
                }

                else
                {
                    irc.ConnectTo(sender);
                    moduleManager.JoinChannel(sender);
                    
                }

            }
            else if (msg.StartsWith("!unsub") /* & sender.Equals(channel) */)
            {
                string[] temp = msg.Split(' ');
                if (temp.Length >= 2)
                {
                    Console.WriteLine(2);
                    for (int i = 1; i < temp.Length; i++)
                    {
                        Console.WriteLine(3);
                        moduleManager.RemoveModuleFromChannel(channel, temp[i]);
                    }
                }
                else
                {
                    irc.DisconnectFrom(channel);
                    moduleManager.LeaveChannel(channel);
                }
            }
        }
    }
}