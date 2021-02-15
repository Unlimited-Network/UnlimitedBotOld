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

namespace UnlimitedBotCore {
    public class Commands : ModuleBase<SocketCommandContext> {
        private BotConfig config = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText("config.json"));

        public bool IsDev() => Context.User.Id == 287778194977980416 || Context.User.Id == 267761639393067008;

        [Command("load")]
        [Alias("data", "stats")]
        private async Task Load(params string[] args) {
            if(IsDev()) {
                EmbedBuilder embed = new EmbedBuilder();

                if(!Player.TryGetPlayer(args[0], out var player)) {
                    embed.Color = Color.Red;
                    embed.Title = "Couldn't find data file.";
                    embed.Description = $"{args[0]} could **not** be found!";
                    await ReplyAsync(null, false, embed.Build());
                    return;
                }

                TimeSpan pt = TimeSpan.FromMinutes(player.Data.Minutes);
                var playedTime = string.Format("{0:D2} days, {1:D2} hours, {2:D2} minutes",
                    pt.Days,
                    pt.Hours,
                    pt.Minutes);

                embed.Color = Color.Green;
                embed.Title = $"Stats for: {player.UserId}";
                embed.AddField("Time played:", playedTime);
                embed.AddField("MVP Awards:", $"{player.Data.Mvp} times as MVP.");

                if(Context.Channel is IPrivateChannel) {
                    await ReplyAsync(null, false, embed.Build());
                    return;
                } else {
                    await Context.Message.DeleteAsync();
                    await ReplyAsync("**This command should only be used in DMs.**");
                    await Context.User.SendMessageAsync(null, false, embed.Build());
                }
            }
        }

        [Command("reloadconfig")]
        private async Task ReloadConfig(params string[] args) {
            if(IsDev()) {
                config = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText("config.json"));
                await ReplyAsync("**Config reloaded!**");
            }
        }

        [Command("ping")]
        private async Task Ping(params string[] args) {
            await ReplyAsync("Pong! 🏓 **" + Program._client.Latency + "ms**");
        }

        [Command("status")]
        [RequireOwner]
        private async Task status([Remainder] string args) {
            await Program._client.SetGameAsync($"{args}");
        }

        [Command("help")]
        private async Task help() {
            string prefix = config.prefix, 
                CommandsList = 
                $"**Commands:**\n" +
                $"***Current Prefix is \"{prefix}\"***\n" +
                $"----------------------------\n" +
                $"**help:** *Displays this command.*\n" +
                $"**about:** *Displays some information about the bot!*\n";

            if(!Context.IsPrivate && Context.Guild.Id == 687875961995132973) {
                CommandsList += "**\nUnlimited SCP Commands**:\n" +
                    "----------------------------\n" +
                    "**minecraft:** *Sends the current IP of the minecraft server*\n" +
                    "**appeal:** *Sends a invite link to the appeal discord*";
            }

            await ReplyAsync(CommandsList);
        }

        [Command("say")]
        [RequireOwner]
        private async Task say([Remainder] string args) {
            if(!Context.IsPrivate && Context.Guild.CurrentUser.GuildPermissions.ManageMessages == true) {
                await Context.Message.DeleteAsync();
            }
            await ReplyAsync(args);
        }

        [Command("about")]
        private async Task about() {
            await ReplyAsync("Made by `lXxMangoxXl#8878` and `Beryl#6677`\n" +
                "Fork of https://github.com/lXxMangoxXl/MangoBot/ \n" +
                "Made with Discord.Net, C#, and lots of love!");
        }
        [Command("minecraft")]
        private async Task minecraft() {
            //If you're in Unlimited, send the game address, if you aren't, spit out a generic error message.
            if(!Context.IsPrivate && Context.Guild.Id == 687875961995132973) {
                await ReplyAsync("The server IP is `mc.unlimitedscp.com`");
            }
        }
        [Command("appeal")]
        [Alias("appeals")]
        private async Task appeal() {
            //If you're in Unlimited, send the invite.
            if(!Context.IsPrivate && Context.Guild.Id == 687875961995132973) {
                await ReplyAsync($"The appeal URL is http://appeal.unlimitedscp.com");
            }

        }
    }
}