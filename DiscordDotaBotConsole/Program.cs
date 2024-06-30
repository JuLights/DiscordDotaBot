using DiscordDotaBot;
using DiscordDotaBotConsole.models;
using System.Text.Json;

#nullable disable

namespace DiscordDotaBotConsole
{
    internal class Program
    {
        private const string tokenPath = "token.json";
        private static DiscordDotaBot.DiscordDotaBot discordBot;
        async static Task Main(string[] args)
        {
            var token = await GetToken();
            Console.WriteLine(token);
            discordBot = new DiscordDotaBot.DiscordDotaBot(token,"snoopdogi7","jugeljke1996", true);

            await discordBot.CreateDiscordDotaBotAsync();

        }

        public static async Task<string> GetToken()
        {
            if (File.Exists(tokenPath))
            {
                try
                {
                    string jsonString = await File.ReadAllTextAsync(tokenPath);
                    Root root = JsonSerializer.Deserialize<Root>(jsonString);
                    return root.DiscordBotCreds.token;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
