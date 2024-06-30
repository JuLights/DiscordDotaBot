#nullable disable
using DiscordDotaBot;
using DiscordDotaBot.dota.customEventArgs;
using DiscordDotaBot.dota.handlers.callbacks;
using DiscordDotaBot.dota.handlers.gc;
using DiscordDotaBot.dota.interfaces;
using SteamKit2;

namespace DiscordDotaBot.dota.handlers.interaction
{
    internal class FriendsHandler
    {
        private static SteamFriends _handler;
        public static void InitFriendsHandler()
        {
            _handler = DiscordDotaBot.steamClient.GetHandler<SteamFriends>();
            CManagerHandler.SteamStateChanged += CManagerHandler_StatusChanged;
        }

        private static void CManagerHandler_StatusChanged(object sender, SteamStateEventArgs e)
        {
            _handler.SetPersonaState(e._personState);
        }

        internal static void AddFriend(ulong steamId)
        {
            _handler.AddFriend(new SteamID(steamId));

            //throw new NotImplementedException();
        }

        public void RemoveFriend()
        {
            throw new NotImplementedException();
        }

        public void SendMessage()
        {
            throw new NotImplementedException();
        }
    }
}
