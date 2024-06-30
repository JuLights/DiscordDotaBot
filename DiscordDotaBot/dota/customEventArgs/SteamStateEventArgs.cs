using DiscordDotaBot.dota.enums;
using SteamKit2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordDotaBot.dota.customEventArgs
{
    internal class SteamStateEventArgs : EventArgs
    {
        public EPersonaState _personState;
        public SteamStateEventArgs(EPersonaState personState)
        {
            _personState = personState;
        }
    }

}
