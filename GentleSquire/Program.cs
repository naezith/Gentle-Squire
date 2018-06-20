using System;
using DSharpPlus.Net.WebSocket;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using System.Timers;
using DSharpPlus.Entities;

namespace GentleSquire
{
    class Program
    {
        public static DiscordClient _client;
        static CommandsNextModule _commands;

        static void Main(string[] args)
        {
            RefreshHandler.StartTimer();
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            _client = new DiscordClient(new DiscordConfiguration
            {
                Token = "NDEzMDU5NTg5MTk4ODM5ODE4.DWsP-Q.PAVmA6Tf7n5jc1ggPozXQVkJ6To",
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug

            });


            _client.SetWebSocketClient<WebSocket4NetCoreClient>();


            _commands = _client.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "+"
            });

            _commands.RegisterCommands<Commands>();

            await _client.ConnectAsync();
            await Task.Delay(-1);

        }

    }
}
