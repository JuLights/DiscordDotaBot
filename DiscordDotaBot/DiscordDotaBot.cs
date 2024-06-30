using DiscordDotaBot.commands;
using DiscordDotaBot.dota;
using DiscordDotaBot.dota.handlers.auth;
using DiscordDotaBot.dota.handlers.callbacks;
using DiscordDotaBot.dota.handlers.gc;
using DiscordDotaBot.dota.handlers.interaction;
using DiscordDotaBot.dota.handlers.steamapps;
using DiscordDotaBot.dota.states;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using SteamKit2;
using SteamKit2.WebUI.Internal;

#nullable disable

namespace DiscordDotaBot
{
    public class DiscordDotaBot
    {
        //Discord things
        internal static DiscordClient _discordClient { get; set; }
        private static CommandsNextExtension _commands { get; set; }
        private static SlashCommandsExtension _slashCommands { get; set; }
        private static string _discordToken { get; set; }
        private static bool discordBotCreated = false;

        //Steam things
        internal static SteamClient steamClient;
        //private static SteamMatchmaking steamMatchmaking;

        //Steam enums
        internal static SteamClientState steamClientState = SteamClientState.LoggedIn;
        //Steam fields
        internal static bool isRunning;
        //this is bad i know
        private static string _steamUser { get; set; }
        private static string _password { get; set; }
        internal static bool _rememberMe { get; set; }

        /// <summary>
        /// DiscordDotaBot public constructor
        /// </summary>
        /// <param name="discordToken">discord bot token</param>
        /// <param name="steamUser">Steam username</param>
        /// <param name="password">Steam password</param>
        /// <param name="rememberMe">so next time you will not be asked for guard data</param>
        public DiscordDotaBot(string discordToken, string steamUser, string password,bool rememberMe)
        {
            _discordToken = discordToken;
            _steamUser = steamUser;
            _password = password;
            _rememberMe = rememberMe;
        }

        public async Task CreateDiscordDotaBotAsync()
        {

            //Discord init
            InitDiscordClient();

            SetupDiscordCommands();

            _discordClient.Ready += _discordClient_Ready;

            //Steam init
            DiscordDotaBot.isRunning = true;

            Task.Run(()=>InitSteamClient());

            Console.WriteLine("Initializing SteamClient..");
            //Task.Run(() => InitSteamClient());
            //InitSteamClient();


            await _discordClient.ConnectAsync();
            await Task.Delay(-1);
        }

        private void InitDiscordClient()
        {
            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = _discordToken,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
            };
            //init discord Client
            _discordClient = new DiscordClient(discordConfig);
            //default timeout for iteractivity
            _discordClient.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

        }

        private void SetupDiscordCommands()
        {
            //normal commands
            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { "!" },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false
            };

            //SlashCommands
            var slashCommandsConfig = _discordClient.UseSlashCommands();
            _commands = _discordClient.UseCommandsNext(commandsConfig);



            //register basic commands
            slashCommandsConfig.RegisterCommands<SlashCommands>();
            _commands.RegisterCommands<BotCommands>();
        }

        private Task _discordClient_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs args)
        {
            return Task.CompletedTask;
        }

        private async Task InitSteamClient()
        {
            // create our steamclient instance
            steamClient = new SteamClient();

            
            // create the callback manager which will route callbacks to function calls
            CManagerHandler.InitCManagerHandler(_steamUser,_password);
            // get the steamuser handler, which is used for logging on after successfully connecting
            AuthHandler.InitAuthHandler();

            FriendsHandler.InitFriendsHandler();

            //dota game coordinator
            DotaGCHandler.InitDotaGameCoordinatorHandler();

            SteamAppsHandler.InitSteamAppsHandler();
            //steamMatchmaking = steamClient.GetHandler<SteamMatchmaking>();

            steamClient.Connect();

        }

    }
}
