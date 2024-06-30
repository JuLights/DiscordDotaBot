using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordDotaBot.dota.interfaces
{
    internal interface IFriends
    {
        void SendMessage();
        void AddFriend(ulong steamId);
        void RemoveFriend();
    }
}
