using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using GentleSquire.Leaderboard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace GentleSquire.DiscordBot
{
	public class Bot
	{
		private const string BOT_CONFIG_PATH = "resources/config/config.json";



		private BotConfig _config;
		private DiscordClient _client;
		private LeaderboardUpdateFetcher _leaderboardUpdateFetcher;

		private Timer _messageQueueTimer;
		private Queue<Task<DiscordMessage>> _messageQueue;

		private DiscordChannel _worldRecordUpdatesChannel;
		private DiscordChannel _personalBestUpdatesChannel;
		private DiscordChannel _exceptionOutputChannel;

		public Bot()
		{
			_config = BotConfig.FromFile(BOT_CONFIG_PATH);

			_client = new DiscordClient(new DiscordConfiguration
			{
				Token = File.ReadLines(_config.TokenPath).First(),
				TokenType = TokenType.Bot,
				UseInternalLogHandler = true,
				LogLevel =
#if DEBUG
					LogLevel.Debug,
#else
					LogLevel.Info,
#endif
			});
			_client.ClientErrored += args => HandleException(args.Exception);
			_client.SocketErrored += args => HandleException(args.Exception);

			_leaderboardUpdateFetcher = new LeaderboardUpdateFetcher();
			_leaderboardUpdateFetcher.NewWorldRecordEvent += PostNewWorldRecordAsync;
			_leaderboardUpdateFetcher.NewOldestRecordEvent += UpdateOldestRecordTopicAsync;
			_leaderboardUpdateFetcher.NewPersonalBestEvent += PostNewPersonalBestAsync;



			_messageQueueTimer = new Timer(_config.MessageQueueTimeInMilliseconds);
			_messageQueueTimer.Elapsed += SendMessageFromQueue;

			_messageQueue = new Queue<Task<DiscordMessage>>();



			Category.SetCategoryList("resources/categories.json");
		}

		public async Task RunAsync()
		{
			await _client.ConnectAsync();
			_client.Ready += OnReady;
			_messageQueueTimer.Start();
			await Task.Delay(-1);
		}

		private async Task OnReady(ReadyEventArgs e)
		{
			if (_config.WorldRecordUpdatesChannelId != 0)
			{
				_worldRecordUpdatesChannel = await _client.GetChannelAsync(_config.WorldRecordUpdatesChannelId);
			}

			if (_config.PersonalBestUpdatesChannelId != 0)
			{
				_personalBestUpdatesChannel = await _client.GetChannelAsync(_config.PersonalBestUpdatesChannelId);
			}

			if (_config.ExceptionOutputChannelId != 0)
			{
				_exceptionOutputChannel = await _client.GetChannelAsync(_config.ExceptionOutputChannelId);
			}

			_leaderboardUpdateFetcher.Start();

			LogMessage(LogLevel.Info, "Ready.");
		}

		private async void SendMessageFromQueue(object sender, ElapsedEventArgs e)
		{
			if (_messageQueue.Count == 0)
			{
				return;
			}

			await _messageQueue.Dequeue();
		}

		private void PostNewWorldRecordAsync(object sender, LeaderboardNewWorldRecordEventArgs e)
		{
			if (e.PreviousRecord is null) return;
			if (_worldRecordUpdatesChannel is null) return;

			var dateDifferenceInDays = (e.NewRecord.Date - e.PreviousRecord.Date).Days;

			var content = new StringBuilder();
			content.Append($"**{e.NewRecord.Category.Name} - [{TimeToString(e.NewRecord.TimeInMilliseconds)}] :trophy: {e.NewRecord.PlayerUsername}**");
			if (e.NewRecord.PlayerId == e.PreviousRecord.PlayerId)
			{
				content.Append($" improved their record by");
			}
			else
			{
				content.Append($" beat **~~{e.PreviousRecord.PlayerUsername}~~**'s record by");
			}
			content.Append($" **{TimeToString(e.PreviousRecord.TimeInMilliseconds - e.NewRecord.TimeInMilliseconds)}** (Record stood for **{dateDifferenceInDays} days!**)");

			_messageQueue.Enqueue(_client.SendMessageAsync(_worldRecordUpdatesChannel, content.ToString()));
		}

		private async void UpdateOldestRecordTopicAsync(object sender, LeaderboardNewWorldRecordEventArgs e)
		{
			if (_worldRecordUpdatesChannel is null) return;

			var ageInDays = (DateTime.UtcNow - e.NewRecord.Date).Days;

			var topic = new StringBuilder();
			topic.AppendLine("Longest standing record:");
			topic.AppendLine($"{e.NewRecord.Category.Name} - [{TimeToString(e.NewRecord.TimeInMilliseconds)}] by {e.NewRecord.PlayerUsername} [{ageInDays} days]");
			try
			{
				await _worldRecordUpdatesChannel.ModifyAsync(m => m.Topic = topic.ToString());
			}
			catch (UnauthorizedException)
			{
				LogMessage(LogLevel.Error, $"Could not update channel topic: Insufficient permissions.");
			}
		}

		private void PostNewPersonalBestAsync(object sender, LeaderboardNewPersonalBestEventArgs e)
		{
			if (_personalBestUpdatesChannel is null) return;

			var content = $"**{e.NewPersonalBest.Category.Name} - [{TimeToString(e.NewPersonalBest.TimeInMilliseconds)}] :tada: {e.NewPersonalBest.PlayerUsername}** improved their time by **{TimeToString(e.PreviousPersonalBestTimeInMilliseconds - e.NewPersonalBest.TimeInMilliseconds)}**";

			_messageQueue.Enqueue(_client.SendMessageAsync(_personalBestUpdatesChannel, content.ToString()));
		}

		private Task HandleException(Exception e)
		{
			if (e is WebSocketException)
			{
				return Task.CompletedTask;
			}

			if (!(_exceptionOutputChannel is null))
			{
				_messageQueue.Enqueue(_client.SendMessageAsync(_exceptionOutputChannel, e.ToString()));
			}

			return Task.CompletedTask;
		}

		private string TimeToString(int totalMilliseconds)
		{
			var minutes = totalMilliseconds / 1000 / 60;
			var secondsAndMilliseconds = (float)(totalMilliseconds - (minutes * 1000 * 60)) / 1000;

			if (minutes == 0)
			{
				return $"{secondsAndMilliseconds}s";
			}
			else
			{
				return $"{minutes}m {secondsAndMilliseconds}s";
			}
		}

		private void LogMessage(LogLevel level, string message)
		{
			_client.DebugLogger.LogMessage(level, "Bot", message, DateTime.Now);
		}
	}
}
