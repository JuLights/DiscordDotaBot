using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordDotaBotConsole.models
{
    public class DiscordBotCreds
    {
        public string token { get; set; }
    }
    public class Root
    {
        public DiscordBotCreds DiscordBotCreds { get; set; }
    }
}
