using DiscordDotaBot.commands;
using DiscordDotaBot.dota.enums;
using DiscordDotaBot.dota.handlers.callbacks;
using DiscordDotaBot.dota.handlers.gc;
using DiscordDotaBot.dota.handlers.interaction;
using SteamKit2.GC.Dota.Internal;
using SteamKit2.GC;
using DiscordDotaBot.dota.customEventArgs;

namespace DiscordDotaBot
{
    public static class App
    {
        public const int DOTAPPID = 570;
        public const int CSGOAPPID = 720;
        internal static ChallengeMode _challengeMode { get; set; }

        private static List<ulong>? _invitedPlayersSteamIds { get; set; }

        internal static Task StartGameAsync(ChallengeMode challengeMode, List<ulong> invitedPlayersSteamIds)
        {
            CManagerHandler.GameStateChanged += CManagerHandler_GameStateChanged;
            DotaGCHandler.OnAllInviteSent += DotaGCHandler_OnAllInviteSent;
            CManagerHandler.LobbyStateChanged += CManagerHandler_LobbyStateChanged;

            _invitedPlayersSteamIds = invitedPlayersSteamIds;
            DotaGCHandler.PlayGame();

            _challengeMode = challengeMode;
            switch (_challengeMode)
            {
                case ChallengeMode.OvOSoloMid:
                    break;
                case ChallengeMode.FvFiveRankedAllPick:
                    break;
                case ChallengeMode.FvFiveCaptainsMode:
                    break;
                default:
                    break;
            }

            return Task.CompletedTask;
        }

        

        private static void DotaGCHandler_OnAllInviteSent(object? sender, EventArgs e)
        {
            DotaGCHandler.LeaveLobby();
            Thread.Sleep(5000);
            ///CREATE LOBBY
            DotaGCHandler.CreateLobby();
            Thread.Sleep(5000);

            DotaGCHandler.LaunchLobby();

            ///response
        }

        #region Event Subscription
        private static void CManagerHandler_LobbyStateChanged(object? sender, LobbyStateEventArgs e)
        {
            Console.WriteLine($"Lobby Launched, Lobby State: {e.LobbyState.ToString()}");
            switch (e.LobbyState)
            {
                case LobbyState.Launched:
                    Console.WriteLine("Game is running!");
                    break;
                case LobbyState.Idle:
                    Console.WriteLine("Nothing happened");
                    break;
                case LobbyState.Destroyed:
                    Console.WriteLine("Lobby CRASH HELP!");
                    break;
                default:
                    break;
            }

            //throw new NotImplementedException();
        }

        private static void CManagerHandler_GameStateChanged(object? sender, GameStateEventArgs e)
        {
            if(e._dotaBotGameState == DotaBotGameState.Playing)
            {
                //invite to party -> //invite state
                if (_invitedPlayersSteamIds != null)
                {
                    DotaGCHandler.InvitePlayers(_invitedPlayersSteamIds);
                }
                
                //create lobby ->//lobby state

            }

            //throw new NotImplementedException();
        }
        #endregion

        internal static Task AddFriend(ulong steamId)
        {
            FriendsHandler.AddFriend(steamId);

            return Task.CompletedTask;
        }

    }
}
