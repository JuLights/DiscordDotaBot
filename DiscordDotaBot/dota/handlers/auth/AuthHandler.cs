using DiscordDotaBot.dota.states;
using SteamKit2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace DiscordDotaBot.dota.handlers.auth
{
    internal class AuthHandler
    {
        private static SteamUser _handler;
        internal static void InitAuthHandler()
        {
            _handler = DiscordDotaBot.steamClient.GetHandler<SteamUser>();
        }

        internal static Task Login(SteamUser.LogOnDetails logOnDetails)
        {
            DiscordDotaBot.steamClientState = SteamClientState.LoggedIn;
            _handler.LogOn(logOnDetails);

            return Task.CompletedTask;
        }

        internal static Task Disconnect()
        {
            _handler.LogOff();

            return Task.CompletedTask;
        }

    }
}
