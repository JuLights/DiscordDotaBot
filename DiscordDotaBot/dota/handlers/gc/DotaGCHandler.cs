using SteamKit2;
using SteamKit2.GC;
using SteamKit2.WebUI.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2.GC.Dota.Internal;
using DiscordDotaBot.dota.handlers.interaction;
using DiscordDotaBot.dota.enums;
using System.Collections.Immutable;
using System.Net;

namespace DiscordDotaBot.dota.handlers.gc
{
    internal class DotaGCHandler
    {
        private static SteamGameCoordinator? _handler { get; set; }
        internal static event EventHandler? OnAllInviteSent;
        
        internal static void InitDotaGameCoordinatorHandler()
        {
            _handler = DiscordDotaBot.steamClient.GetHandler<SteamGameCoordinator>();
        }

        internal static void PlayGame()
        {
            var playGame = new ClientMsgProtobuf<CMsgClientGamesPlayed>(EMsg.ClientGamesPlayed);

            playGame.Body.games_played.Add(new CMsgClientGamesPlayed_GamePlayed
            {
                game_id = new GameID(App.DOTAPPID), // or game_id = APPID,
            });

            DiscordDotaBot.steamClient.Send(playGame);
            // delay a little to give steam some time to establish a GC connection to us
            Thread.Sleep(5000);

            //handshake kinda
            var clientHello = new ClientGCMsgProtobuf<SteamKit2.GC.Dota.Internal.CMsgClientHello>((uint)EGCBaseClientMsg.k_EMsgGCClientHello);
            clientHello.Body.engine = ESourceEngine.k_ESE_Source2;
            _handler?.Send(clientHello, App.DOTAPPID);



            //// Set the persona state to online so we can receive GC messages
            //steamFriends.SetPersonaState(EPersonaState.Online);
        }


        internal static void CreateLobby()
        {
            var createLobbyMsg = new ClientGCMsgProtobuf<CMsgPracticeLobbyCreate>((uint)EDOTAGCMsg.k_EMsgGCPracticeLobbyCreate);

            // You can set various lobby parameters here
            createLobbyMsg.Body.lobby_details = new CMsgPracticeLobbySetDetails
            {
                game_name = "Test Lobby",
                server_region = (uint)DotaLobbyRegion.EUROPE,
                game_mode = (uint)DOTA_GameMode.DOTA_GAMEMODE_1V1MID,
                //requested_hero_ids :)
                allow_cheats = false,
                fill_with_bots = false,
                intro_mode = false,
                allow_spectating = true,
                pass_key = "test", // Set a password if needed
            };

            Console.WriteLine("Sending Lobby Settings to GC");

            _handler.Send(createLobbyMsg, App.DOTAPPID);

            Thread.Sleep(2000);
        }

        internal static void LeaveLobby()
        {
            var lobby = new ClientGCMsgProtobuf<CMsgPracticeLobbyLeave>(
                (uint)EDOTAGCMsg.k_EMsgGCPracticeLobbyLeave);
            _handler?.Send(lobby, App.DOTAPPID);
        }

        internal static void InvitePlayers(List<ulong> invitedPlayersSteamIds)
        {
            foreach (var steamId in invitedPlayersSteamIds)
            {
                var cmMsg = new ClientGCMsgProtobuf<CMsgInviteToParty>((uint)EGCBaseMsg.k_EMsgGCInviteToParty);
                var msg = new ClientGCMsgProtobuf<CMsgPartyInviteResponse>((uint)EGCBaseMsg.k_EMsgGCPartyInviteResponse);
                cmMsg.Body.steam_id = steamId;
                _handler?.Send(cmMsg, 570);
                Thread.Sleep(1000);
                _handler?.Send(msg, App.DOTAPPID);
                Thread.Sleep(500);
            }
            OnAllInviteSent.Invoke(null,EventArgs.Empty);

        }

        internal static void LaunchLobby()
        {
            var msg = new ClientGCMsgProtobuf<CMsgPracticeLobbyLaunch>((uint)EDOTAGCMsg.k_EMsgGCPracticeLobbyLaunch);

            _handler?.Send(msg, App.DOTAPPID);
            Thread.Sleep(5000);
        }

        internal static void StopGame() 
        {

        }
    }

    internal class AuthTicket
    {
        internal static ImmutableArray<byte> CreateAuthTicket(ImmutableArray<byte> token, IPAddress ip)
        {
            uint sessionSize = 4 + // unknown 1
                               4 + // unknown 2
                               4 + // external IP
                               4 + // filler
                               4 + // timestamp
                               4; // connection count

            MemoryStream stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(token.Length);
                writer.Write(token.ToArray());

                writer.Write(sessionSize);
                writer.Write(1);
                writer.Write(2);

                byte[] externalBytes = ip.GetAddressBytes();
                writer.Write(externalBytes.Reverse().ToArray());

                writer.Write((int)0);
                writer.Write(2038 /* ms since connected to steam? */);
                writer.Write(1 /* connection count to steam? */);
            }

            return stream.ToArray().ToImmutableArray();
        }

        internal static ImmutableArray<byte> CreateServerTicket(
                SteamID id, ImmutableArray<byte> auth, byte[] ownershipTicket)
        {
            long size = 8 + // steam ID
                        auth.Length +
                        4 + // length of ticket
                        ownershipTicket.Length;

            MemoryStream stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write((ushort)size);
                writer.Write(id.ConvertToUInt64());

                writer.Write(auth.ToArray());

                writer.Write(ownershipTicket.Length);
                writer.Write(ownershipTicket);

                writer.Write(0);
            }

            return stream.ToArray().ToImmutableArray();
        }
    }

    
}
