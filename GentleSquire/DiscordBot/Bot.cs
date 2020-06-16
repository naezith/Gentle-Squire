using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using GentleSquire.Leaderboard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace GentleSquire.DiscordBot
{
	public class Bot
	{
		private const string BOT_CONFIG_PATH = "resources/config.json";



		private BotConfig _config;
		private DiscordClient _client;
		private LeaderboardUpdateFetcher _leaderboardUpdateFetcher;

		private Timer _messageQueueTimer;
		private Queue<QueuedMessage> _messageQueue;

		private DiscordChannel _worldRecordUpdatesChannel;
		private DiscordChannel _personalBestUpdatesChannel;

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

			_leaderboardUpdateFetcher = new LeaderboardUpdateFetcher();
			_leaderboardUpdateFetcher.NewWorldRecordEvent += PostNewWorldRecordAsync;
			_leaderboardUpdateFetcher.NewOldestRecordEvent += UpdateOldestRecordTopicAsync;
			_leaderboardUpdateFetcher.NewPersonalBestEvent += PostNewPersonalBestAsync;



			_messageQueueTimer = new Timer(_config.MessageQueueTimeInMilliseconds);
			_messageQueueTimer.Elapsed += SendMessageFromQueue;

			_messageQueue = new Queue<QueuedMessage>();



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
			_worldRecordUpdatesChannel = await _client.GetChannelAsync(_config.WorldRecordUpdatesChannelId);
			_personalBestUpdatesChannel = await _client.GetChannelAsync(_config.PersonalBestUpdatesChannelId);

			_leaderboardUpdateFetcher.Start();

			LogMessage(LogLevel.Info, "Ready.");
		}

		private async void SendMessageFromQueue(object sender, ElapsedEventArgs e)
		{
			if (_messageQueue.Count == 0)
			{
				return;
			}

			var queuedMessage = _messageQueue.Dequeue();
			await _client.SendMessageAsync(queuedMessage.Channel, queuedMessage.Content);
		}

		private void PostNewWorldRecordAsync(object sender, LeaderboardNewWorldRecordEventArgs e)
		{
			if (e.PreviousRecord == null) return;

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

			EnqueueMessage(_worldRecordUpdatesChannel, content.ToString());
		}

		private async void UpdateOldestRecordTopicAsync(object sender, LeaderboardNewWorldRecordEventArgs e)
		{
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
			var content = $"**{e.NewPersonalBest.Category.Name} - [{TimeToString(e.NewPersonalBest.TimeInMilliseconds)}] :tada: {e.NewPersonalBest.PlayerUsername}** improved their time by **{TimeToString(e.PreviousPersonalBestTimeInMilliseconds - e.NewPersonalBest.TimeInMilliseconds)}**";

			EnqueueMessage(_personalBestUpdatesChannel, content.ToString());
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

		private void EnqueueMessage(DiscordChannel channel, string content)
		{
			_messageQueue.Enqueue(new QueuedMessage(channel, content));
		}

		private void LogMessage(LogLevel level, string message)
		{
			_client.DebugLogger.LogMessage(level, "Bot", message, DateTime.Now);
		}
	}
}
