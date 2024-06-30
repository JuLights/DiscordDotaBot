using DiscordDotaBot.dota;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordDotaBot.commands
{
    internal class SlashCommands : ApplicationCommandModule
    {
        private const string steamIdFinderUrl = "https://steamid.io/";

        [SlashCommand("help", "challenge any opponent 1v1 lobby with desired hero", true)]
        [SlashCommandPermissions(Permissions.Administrator)]
        public async Task HelpSlashCommand(InteractionContext ctx)
        {
            //this 
            //await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Success!"));

            //or
            try
            {
                await ctx.DeferAsync();
                var author = new DiscordEmbedBuilder.EmbedAuthor();
                author.Name = DiscordDotaBot._discordClient.CurrentUser.ToString();
                var embedMessage = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Green,
                    Title = "Requirement for a challenge",
                    Author = author,
                    Description = $"1.Make sure you have added DotaBot on steam account use command /AddDotaBot and pass the SteamID\n" +
                    $"2. Enjoy\n" + "simple as that :)"
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed: embedMessage));

                //await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

                ////Some time consuming task like a database call or a complex operation

                //await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Thanks for waiting!"));

                ////await ctx.Channel.SendMessageAsync("this is a slash Command Congrats!");

            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }

        }

        [SlashCommand("addme","add Dota Bot in your steam account",true)]
        [SlashCommandPermissions(Permissions.Administrator)]
        public async Task AddSlashCommand(InteractionContext ctx, [Option("steamId64", $"you can do that easily on {steamIdFinderUrl}")] string steamId)
        {
            await ctx.DeferAsync();

            if(string.IsNullOrWhiteSpace(steamId))
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("try better you can do it"));
            }
            else
            {
                //Call SteamFriends Add friend
                Console.WriteLine($"DiscordBot Message: Add bot requested");
                ulong result = 0;
                ulong.TryParse(steamId, out result);
                if (result == 0)
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("can't parse steamId"));
                }
                else
                {
                    //await App.AddFriend(result);
                    Console.WriteLine("Discord Message sorry adding freind failed");
                    //
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("sorry adding freind failed"));
                }
                
            }

        }

        //[SlashCommand("challenge","this slash commands allows parameters")]
        //[SlashCommandPermissions(Permissions.Administrator)]
        //public async Task SlashCommandParameters(InteractionContext ctx, [Option("user","user to challenge")] DiscordUser user,
        //    [Choice("1v1 Solo Mid", "1v1SoloMid")]
        //    [Choice("5v5 Ranked All Pick", "RankedAllPick")]
        //    [Choice("5v5 Captains Mode", "CaptainsMode")]
        //    [Option("gameMode","game mode set by user")] string gameMode)
        //{
        //    await ctx.DeferAsync();

        //    var embedMessage = new DiscordEmbedBuilder()
        //    {
        //        Color = DiscordColor.Red,
        //        Title = "User Dota Challenge Request",
        //        Description = $"{user.Mention} was challenged by {ctx.User.Mention} in Dota. Game Mode: {gameMode}"
        //    };

        //    await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed: embedMessage));
        //}

        [SlashCommand("challenge","This slash commands allows parameters")]
        [SlashCommandPermissions(Permissions.Administrator)]
        public async Task SlashCommandParameters(InteractionContext ctx, [Option("user","ser to challenge")] DiscordUser user,

            [Option("gameMode","game mode set by user")] ChallengeMode challengeMode = ChallengeMode.OvOSoloMid)
        {
            await ctx.DeferAsync();

            var gameMode = "";

            switch (challengeMode)
            {
                case ChallengeMode.OvOSoloMid:
                    gameMode = "1v1 Solo Mid";
                    break;
                case ChallengeMode.FvFiveRankedAllPick:
                    gameMode = "5v5 Ranked All Pick";
                    break;
                case ChallengeMode.FvFiveCaptainsMode:
                    gameMode = "5v5 Captains Mode";
                    break;
                default:
                    break;
            }

            await App.StartGameAsync(challengeMode,new List<ulong>
            {
                76561198874647920,
                //76561198277627251, //real fish
                //76561198362427303, //smurf fish
                //76561198095586106, // anri
            });

            var embedMessage = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Red,
                Title = "User Dota Challenge Request",
                Description = $"{user.Mention} was challenged by {ctx.User.Mention} in Dota. Game Mode: {gameMode}"
            };

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed: embedMessage));
        }

        
    }
    public enum ChallengeMode
    {
        [ChoiceName("1v1 Solo Mid")]
        OvOSoloMid,
        [ChoiceName("5v5 Ranked All Pick")]
        FvFiveRankedAllPick,
        [ChoiceName("5v5 Captains Mode")]
        FvFiveCaptainsMode,
    }

}
