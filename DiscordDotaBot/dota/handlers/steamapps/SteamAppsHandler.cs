using SteamKit2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace DiscordDotaBot.dota.handlers.steamapps
{
    internal class SteamAppsHandler
    {
        private static SteamApps _handler;
        internal static void InitSteamAppsHandler()
        {
            _handler = DiscordDotaBot.steamClient.GetHandler<SteamApps>();
        }


    }
}
