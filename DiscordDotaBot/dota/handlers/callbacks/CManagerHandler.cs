using DiscordDotaBot.dota.handlers.auth;
using DiscordDotaBot.dota.handlers.gc;
using DiscordDotaBot.dota.states;
using SteamKit2;
using SteamKit2.Authentication;
using SteamKit2.GC.Dota.Internal;
using SteamKit2.GC;
using System.Text.Json;
using DiscordDotaBot.dota.customEventArgs;
using DiscordDotaBot.dota.enums;

#nullable disable
namespace DiscordDotaBot.dota.handlers.callbacks
{
    internal class CManagerHandler
    {
        private static string _username { get; set; }
        private static string _password { get; set; }

        private static string previouslyStoredGuardData { get; set; }
        private static bool previouslyLoggedIn { get; set; }

        internal static event EventHandler<SteamStateEventArgs> SteamStateChanged;
        internal static event EventHandler<GameStateEventArgs> GameStateChanged;
        internal static event EventHandler<LobbyStateEventArgs> LobbyStateChanged;

        private static CallbackManager _handler;
        internal static void InitCManagerHandler(string username, string password)
        {
            _username = username;
            _password = password;

            _handler = new CallbackManager(DiscordDotaBot.steamClient);
            //register callbacks

            // these are registered upon creation to a callback manager, which will then route the callbacks
            // to the functions specified

            _handler.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
            _handler.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);
            _handler.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
            _handler.Subscribe<SteamUser.LoggedOffCallback>(OnLoggedOff);


            //_handler.Subscribe<SteamGameServer.>
            //_handler.Subscribe<SteamU>
            _handler.Subscribe<SteamGameCoordinator.MessageCallback>(OnGCMessage);
            _handler.Subscribe<SteamFriends.FriendsListCallback>(OnFriendsList);
            _handler.Subscribe<SteamFriends.FriendAddedCallback>(OnFriendsAdded);



            Task backgroundCManager = Task.Run((Action)CManagerTask);

            
        }


        /// <summary>
        /// CAN BE MODIFIED IDK YET
        /// </summary>
        private static void CManagerTask()
        {
            // create our callback handling loop
            while (DiscordDotaBot.isRunning)
            {
                // in order for the callbacks to get routed, they need to be handled by the manager
                _handler.RunWaitCallbacks(TimeSpan.FromSeconds(1));
            }
        }

        private static void OnGCMessage(SteamGameCoordinator.MessageCallback callback)
        {
            // setup our dispatch table for messages
            // this makes the code cleaner and easier to maintain
            //callback.
            
            Console.WriteLine($"EMsg:{ callback.EMsg.ToString()}" );
            Console.WriteLine($"JobID: {callback.JobID.Value}");
            Console.WriteLine($"MessageType to string: {callback.Message.MsgType.ToString()}");
            Console.WriteLine($"MessageType to string: {callback.Message.MsgType}");
            Console.WriteLine();

            var messageMap = new Dictionary<uint, Action<IPacketGCMsg>>()
            {
                { (uint)EGCBaseClientMsg.k_EMsgGCClientWelcome, OnClientWelcome },
                //{ (uint)EDOTAGCMsg.k_EMsgGCPracticeLobbyCreate, OnPracticeLobbyCreate },
                { (uint)EGCBaseMsg.k_EMsgGCInvitationCreated, OnInvitationCreated },
                { (uint)EGCBaseMsg.k_EMsgGCPartyInviteResponse, OnPartyInviteResponse },
                { (uint)EDOTAGCMsg.k_EMsgGCPracticeLobbyLaunch, OnPracticeLobbyLaunch },
                //{ (uint)EGCBaseMsg.k_EMsgGCInviteToLobby, OnInviteToLobby },
                //{ (uint)EGCBaseMsg.k_EMsgGCInvitationCreated, OnInvitationCreated },
                //{ (uint)EGCBaseMsg.k_EMsgGCLobbyInviteResponse, OnLobbyInviteResponse },
                //{ (uint)EGCBaseMsg.k_EMsgGCPartyInviteResponse, OnPartyInviteResponse }
            };

            Action<IPacketGCMsg> func;
            if (!messageMap.TryGetValue(callback.EMsg, out func))
            {
                // this will happen when we recieve some GC messages that we're not handling
                // this is okay because we're handling every essential message, and the rest can be ignored
                return;
            }

            func(callback.Message);

            //throw new NotImplementedException();
        }

        //STEAM
        private static void OnFriendsAdded(SteamFriends.FriendAddedCallback callback)
        {
            Console.WriteLine($"{nameof(CManagerHandler)}: callback Fired");
            
            if (callback != null)
            {
                if(callback.Result == EResult.AccessDenied)
                {
                    Console.WriteLine($"{nameof(CManagerHandler)}: Access Denied");
                }

                var name = callback.PersonaName;
                Console.WriteLine($"{nameof(CManagerHandler)}: {name} Added");
            }
        }
        private static void OnFriendsList(SteamFriends.FriendsListCallback callback)
        {
            Console.WriteLine("Friend List callback");
        }

        //DOTA
        #region DOTA
        private static void OnInvitationCreated(IPacketGCMsg packetMsg)
        {
            var message = new ClientGCMsgProtobuf<CMsgInvitationCreated>(packetMsg);
            Console.WriteLine($"Invited player steamid: {message.Body.steam_id}");
            //Console.WriteLine("Party Invite Callback");
        }

        private static void OnPracticeLobbyLaunch(IPacketGCMsg msg)
        {
            var response = new ClientGCMsgProtobuf<CMsgPracticeLobbyLaunch>(msg);
            Console.WriteLine($"Practice Lobby Launched, client_version : {response.Body.client_version}");
            LobbyStateChanged.Invoke(null, new LobbyStateEventArgs(LobbyState.Launched));

        }

        private static void OnPartyInviteResponse(IPacketGCMsg packetMsg)
        {
            var message = new ClientGCMsgProtobuf<CMsgPartyInviteResponse>(packetMsg);
            Console.WriteLine($"On Party Invite Response party_id : {message.Body.party_id}");
            Console.WriteLine($"On Party Invite Response accepted? : {message.Body.accept}");
        }
        #endregion DOTAEND

        //HANDSHAKE
        private static void OnClientWelcome(IPacketGCMsg msg)
        {
            GameStateChanged.Invoke(null, new GameStateEventArgs(DotaBotGameState.Playing));
        }

        



        //CONNECTIONS
        #region CONNECTION_THINGS
        private async static void OnConnected(SteamClient.ConnectedCallback callback)
        {
            Console.WriteLine("Connected to Steam! Logging in...");

            var authSessionDetails = new AuthSessionDetails();
            authSessionDetails.Username = _username;
            authSessionDetails.Password = _password;
            authSessionDetails.IsPersistentSession = DiscordDotaBot._rememberMe;

            CheckGuardData();

            authSessionDetails.GuardData = previouslyStoredGuardData;



            if (!previouslyLoggedIn)
            {
                authSessionDetails.Authenticator = new UserConsoleAuthenticator();
            }

            var authSession = await DiscordDotaBot.steamClient.Authentication.BeginAuthSessionViaCredentialsAsync(authSessionDetails);

            if (authSession != null)
            {
                AuthPollResult pollResponse = await authSession.PollingWaitForResultAsync();

                if (pollResponse.NewGuardData != null)
                {
                    // When using certain two factor methods (such as email 2fa), guard data may be provided by Steam
                    // for use in future authentication sessions to avoid triggering 2FA again (this works similarly to the old sentry file system).
                    // Do note that this guard data is also a JWT token and has an expiration date.
                    previouslyStoredGuardData = pollResponse.NewGuardData;
                    File.WriteAllText("guardData.txt", previouslyStoredGuardData, System.Text.Encoding.UTF8);
                    previouslyLoggedIn = true;
                }


                await AuthHandler.Login(new SteamUser.LogOnDetails
                {
                    Username = pollResponse.AccountName,
                    AccessToken = pollResponse.RefreshToken,
                    ShouldRememberPassword = DiscordDotaBot._rememberMe,
                });

                // This is not required, but it is possible to parse the JWT access token to see the scope and expiration date.
                ParseJsonWebToken(pollResponse.AccessToken, nameof(pollResponse.AccessToken));
                ParseJsonWebToken(pollResponse.RefreshToken, nameof(pollResponse.RefreshToken));
            }

            //throw new NotImplementedException();
        }

        private static void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            DiscordDotaBot.isRunning = false;

        }
        private static void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            if (callback.Result != EResult.OK)
            {
                Console.WriteLine("Unable to logon to Steam: {0} / {1}", callback.Result, callback.ExtendedResult);

                DiscordDotaBot.isRunning = false;
                RetryConnection();
                return;
            }

            Console.WriteLine($"Setting Steam status Online...");
            // notify FriendsHandler to change person state online
            SteamStateChanged?.Invoke(null, new SteamStateEventArgs(EPersonaState.Online));

        }
        private static void OnLoggedOff(SteamUser.LoggedOffCallback callback)
        {
            DiscordDotaBot.steamClientState = SteamClientState.LoggedOut;
            Console.WriteLine("User Logged out");
        }
        
        static void RetryConnection()
        {
            if (!DiscordDotaBot.isRunning)
            {
                DiscordDotaBot.steamClient.Servers.ResetOldScores();
                DiscordDotaBot.steamClient.Servers.ResetBadServers();
                DiscordDotaBot.steamClient.Connect();
                DiscordDotaBot.isRunning = true;
            }
        }
        #endregion CONNECTION_THINGS_END
        private static void CheckGuardData()
        {
            try
            {
                previouslyStoredGuardData = File.ReadAllText("guardData.txt");
                if (previouslyStoredGuardData == null)
                {
                    previouslyLoggedIn = false;
                }
                else
                {
                    previouslyLoggedIn = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"guardData not found, {ex.Message}");
            }
        }


        // This is simply showing how to parse JWT, this is not required to login to Steam
        static void ParseJsonWebToken(string token, string name)
        {
            // You can use a JWT library to do the parsing for you
            var tokenComponents = token.Split('.');

            // Fix up base64url to normal base64
            var base64 = tokenComponents[1].Replace('-', '+').Replace('_', '/');

            if (base64.Length % 4 != 0)
            {
                base64 += new string('=', 4 - base64.Length % 4);
            }

            var payloadBytes = Convert.FromBase64String(base64);

            // Payload can be parsed as JSON, and then fields such expiration date, scope, etc can be accessed
            var payload = JsonDocument.Parse(payloadBytes);

            // For brevity we will simply output formatted json to console
            var formatted = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                WriteIndented = true,
            });
            //formatted Parsed token output
            //Console.WriteLine($"{name}: {formatted}");
            Console.WriteLine();
        }

    }
}
