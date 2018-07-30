using Bot.Core;
using System.Collections.Generic;

namespace Bot.Modules.Subscribe
{
    public class Subscriptions : PassiveModule
    {

        public Subscriptions(ModuleManager _moduleManager, IRC _irc) : base(_moduleManager, _irc) {
            addId("!sub");
            addId("!unsub");
        }
        
        override public void HandleMessage(string sender, string channel, string msg)
        {
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
                    for (int i = 1; i < temp.Length; i++)
                    {
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