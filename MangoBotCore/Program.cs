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
    public class Program
    {
        static void Main(string[] args)
        {
            new Program().RunBot().GetAwaiter().GetResult();
        }

        public static string AppData { get; private set; }
        public static string Exiled { get; private set; }
        public static string Data { get; private set; }
        public static string GetPlayer(string userId) => Path.Combine(Data, userId);

        public static Config Config { get; set; }

        // Creating the necessary variables
        public static DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        // Runbot task
        public async Task RunBot()
        {

            _client = new DiscordSocketClient(); // Define _client
            _commands = new CommandService(); // Define _commands
            _services = new ServiceCollection() // Define _services
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            try {
                AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                Exiled = Path.Combine(AppData, "EXILED");
                Data = Path.Combine(Exiled, "Data");
            } catch(Exception) {
                Console.WriteLine("ERROR: Couldn't find path files.");
            }

            // Config creation/reading.
            if (!File.Exists("config.json"))
            {
                Config = new Config();
                File.WriteAllText("config.json", JsonConvert.SerializeObject(Config, Formatting.Indented));
            }
            else
            {
                Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
            }

            //ulong botowner = config.botowner;

            _client.Log += Log; // Logging

            await RegisterCommandsAsync(); // Call registercommands

            await _client.LoginAsync(TokenType.Bot, Config.Token); // Log into the bot user

            await _client.StartAsync(); // Start the bot user

            await _client.SetGameAsync(Config.Game); // Set the game the bot is playing

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
            // Create a variable with the message as SocketUserMessage and check if the message is empty or sent by a bot
            if(!(arg is SocketUserMessage message) || message.Author.IsBot || message.Content.Contains($"{Config.Prefix} "))
                return;
            
            // Sets the argpos to 0 (the start of the message)
            int argumentPos = 0;

            // If the message has the prefix at the start or starts with someone mentioning the bot
            if (message.HasStringPrefix(Config.Prefix, ref argumentPos) || message.HasMentionPrefix(_client.CurrentUser, ref argumentPos)) 
            {
                // Create a variable called result
                var result = await _commands.ExecuteAsync(new SocketCommandContext(_client, message), argumentPos, _services); 

                if (!result.IsSuccess) // If the result is unsuccessful
                {
                    Console.WriteLine(result.ErrorReason); // Print the error to console
                    //await message.Channel.SendMessageAsync(result.ErrorReason);
                }
            }
        }
    }
}
