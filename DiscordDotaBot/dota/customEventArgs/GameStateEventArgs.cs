using DiscordDotaBot.dota.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordDotaBot.dota.customEventArgs
{
    internal class GameStateEventArgs : EventArgs
    {
        public DotaBotGameState _dotaBotGameState;
        public GameStateEventArgs(DotaBotGameState dotaBotGameState)
        {
            _dotaBotGameState = dotaBotGameState;
        }
    }
}
