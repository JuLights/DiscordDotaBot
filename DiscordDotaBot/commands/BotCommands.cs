using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordDotaBot.commands
{
    internal class BotCommands : BaseCommandModule
    {

        [Command("challenge")]
        public async Task ChallengeUser(CommandContext ctx, string user)
        {
            await ctx.Channel.SendMessageAsync($"{ctx.User.Username} Challenged : {user}");
        }

        //embed
        [Command("embed")]
        public async Task EmbedMessage(CommandContext ctx)
        {
            var message = new DiscordEmbedBuilder()
            {
                Title = "this is my first Discord Embed",
                Description = $"this command was executed by {ctx.User.Username}",
                Color = DiscordColor.Azure
            };

            await ctx.Channel.SendMessageAsync(embed: message);
        }

        [Command("inter")]
        public async Task TestInteractivity(CommandContext ctx)
        {
            var interactivity = DiscordDotaBot._discordClient.GetInteractivity();

            var messageToRetrive = await interactivity.WaitForMessageAsync(message => message.Content == "Hello");
            if(messageToRetrive.Result.Content == "Hello")
            {
                await ctx.Channel.SendMessageAsync($"{ctx.User.Username} said Hello");
            }

            
        }


    }
}
