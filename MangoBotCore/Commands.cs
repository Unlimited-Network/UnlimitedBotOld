using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Mono.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using unirest_net.http;

namespace UnlimitedBotCore {
    public class PPSize
    {
        public Dictionary<ulong, int> ppsize { get; set; } = new Dictionary<ulong, int>();
    }

    public class Commands : ModuleBase<SocketCommandContext>
    {
        private BotConfig config;

        [Command("load")]
        private async Task Load(params string[] args) {
            if (Context.User.Id == 287778194977980416 || Context.User.Id == 267761639393067008)
            {
                var player = new Player(args[0]);
                await ReplyAsync($"{args[0]}: {player.Data.Minutes}m \"{player.Data.Club}\"");
            }
        }

        [Command("ping")]
        private async Task Ping(params string[] args)
        {
            await ReplyAsync("Pong! 🏓 **" + Program._client.Latency + "ms**");
        }
        [Command("status")]
        [RequireOwner]
        private async Task status([Remainder] string args)
        {
             await Program._client.SetGameAsync($"{args}");
        }
        [Command("help")]
        private async Task help()
        {
            ulong authorid = Context.Message.Author.Id;
            config = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText("config.json"));
            string prefix = config.prefix;
            string CommandsList = $"**Commands:**\n" +
    $"***Current Prefix is {prefix}***\n" +
    $"----------------------------\n" +
    $"**help:** *Displays this command.*\n" +
    $"**about:** *Displays some information about the bot!*\n";
            if (!Context.IsPrivate && Context.Guild.GetUser(authorid).GuildPermissions.ManageMessages == true)
            {
                CommandsList = (CommandsList + $"\n**Moderator Commands:**\n" +
                    $"----------------------------\n" +
                    $"**purge:** *Purges amount of messages specified (Requires Manage Messages)*\n" +
                    $"**ban:** *Bans mentioned user with reason specified. Ex. `^ban @lXxMangoxXl Not working on MangoBot`. (Requires Ban Members)*\n");
            }
            if (!Context.IsPrivate && Context.Guild.Id == 687875961995132973)
            {
                CommandsList = (CommandsList + "**\nUnlimited SCP Commands**:\n" +
                    "----------------------------\n" +
                    "**minecraft:** *Sends the current IP of the minecraft server*\n" +
                    "**appeal:** *Sends a invite link to the appeal discord*");
            }
            await ReplyAsync(CommandsList);
        }
        [Command("say")]
        [RequireOwner]
        private async Task say([Remainder] string args)
        {
            if (!Context.IsPrivate && Context.Guild.CurrentUser.GuildPermissions.ManageMessages == true)
            {
                await Context.Message.DeleteAsync();
            }
            await ReplyAsync(args);
        }

        [Command("about")]
        private async Task about()
        {
            await ReplyAsync("Made by `lXxMangoxXl#8878` and `Beryl#6677`\n" +
                "Fork of https://github.com/lXxMangoxXl/MangoBot/ \n" +
                "Made with Discord.Net, C#, and lots of love!");
        }
        [Command("minecraft")]
        private async Task minecraft()
        {
            //If you're in Unlimited, send the game address, if you aren't, spit out a generic error message.
            if (!Context.IsPrivate && Context.Guild.Id == 687875961995132973)
            {
                await ReplyAsync("The server IP is `mc.unlimitedscp.com`");
            }
        }
        [Command("appeal")]
        [Alias("appeals")]
        private async Task appeal()
        {
            //If you're in Unlimited, send the invite.
            if (!Context.IsPrivate && Context.Guild.Id == 687875961995132973)
            {
                await ReplyAsync($"The appeal URL is http://appeal.unlimitedscp.com");
            }
        }
    }
}
