using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace UnlimitedBotCore {
    class Program
    {
        static void Main(string[] args)
        {
            new Program().RunBot().GetAwaiter().GetResult();
        }

        public static string AppData { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string GetPath(string userId) => Path.Combine(AppData, userId);

        // Creating the necessary variables
        public static DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;
        private BotConfig config;

        // Runbot task
        public async Task RunBot()
        {

            _client = new DiscordSocketClient(); // Define _client
            _commands = new CommandService(); // Define _commands
            _services = new ServiceCollection() // Define _services
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            // Config creation/reading.
            if (!File.Exists("config.json"))
            {
                config = new BotConfig()
                {
                    prefix = "^",
                    token = "",
                    game = "",
                    botowner = "",
                    disabledpenis = "1",
                    appealurl = ""
                };
                File.WriteAllText("config.json", JsonConvert.SerializeObject(config, Formatting.Indented));
            }
            else
            {
                config = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText("config.json"));
            }
            if (!File.Exists("ppsize.json"))
            {
                File.WriteAllText("ppsize.json", JsonConvert.SerializeObject(new PPSize()));
            }

            string botToken = config.token; // Make a string for the token

            string prefix = config.prefix;

            string disabledpenis = config.disabledpenis;

            string appealurl = config.appealurl;

            //ulong botowner = config.botowner;

            _client.Log += Log; // Logging

            await RegisterCommandsAsync(); // Call registercommands

            await _client.LoginAsync(TokenType.Bot, botToken); // Log into the bot user

            await _client.StartAsync(); // Start the bot user

            await _client.SetGameAsync(config.game); // Set the game the bot is playing

            await Task.Delay(-1); // Delay for -1 to keep the console window open

        }

        private async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync; // Messagerecieved

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), null); // Add module to _commands
        }

        private Task Log(LogMessage arg) // Logging
        {
            Console.WriteLine(arg); // Print the log to Console
            return Task.CompletedTask; // Return with completedtask
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            string messageLower = arg.Content.ToLower(); // Convert the message to a Lower
            var message = arg as SocketUserMessage; // Create a variable with the message as SocketUserMessage
            if (message is null || message.Author.IsBot || message.Content.Contains($"{config.prefix} ")) return; // Checks if the message is empty or sent by a bot
            int argumentPos = 0; // Sets the argpos to 0 (the start of the message)
            if (message.HasStringPrefix(config.prefix, ref argumentPos) || message.HasMentionPrefix(_client.CurrentUser, ref argumentPos)) // If the message has the prefix at the start or starts with someone mentioning the bot
            {
                var context = new SocketCommandContext(_client, message); // Create a variable called context
                var result = await _commands.ExecuteAsync(context, argumentPos, _services); // Create a veriable called result
                if (!result.IsSuccess) // If the result is unsuccessful
                {
                    Console.WriteLine(result.ErrorReason); // Print the error to console
                    //await message.Channel.SendMessageAsync(result.ErrorReason);
                }
            }
        }
    }



    public class BotConfig
    {
        public string token { get; set; }
        public string prefix { get; set; }
        public string game { get; set; }
        public string botowner { get; set; }
        public string disabledpenis { get; set; }
        public string appealurl { get; set; }
    }

}
