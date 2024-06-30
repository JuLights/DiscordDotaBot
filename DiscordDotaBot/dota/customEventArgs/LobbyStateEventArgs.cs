using DiscordDotaBot.dota.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordDotaBot.dota.customEventArgs
{
    internal class LobbyStateEventArgs : EventArgs
    {
        internal LobbyState LobbyState;
        public LobbyStateEventArgs(LobbyState lobbyState) 
        {
            LobbyState = lobbyState;
        }
    }
}
