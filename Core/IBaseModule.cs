using System.Collections.Generic;

namespace WBot.Core
{
    public interface IBaseModule
    {
        List<string> getIds();
        List<string> getActiveChanels();
        bool isActiveOnChannel(string channel);
        void HandleMessage(string channel, string msg);

    }

}