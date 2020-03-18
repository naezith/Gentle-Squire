using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using GentleSquire.Leaderboard;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GentleSquire
{
	public class Bot
	{
		private const string TOKEN_PATH = "resources/token";
		private const ulong LEADERBOARD_UPDATE_CHANNEL_ID = 1; // TODO



		private DiscordClient _client;
		private LeaderboardUpdateFetcher _leaderboardUpdateFetcher;

		private DiscordChannel _leaderboardUpdateChannel;

		public Bot()
		{
			_client = new DiscordClient(new DiscordConfiguration
			{
				Token = File.ReadLines(TOKEN_PATH).First(),
				TokenType = TokenType.Bot,
				UseInternalLogHandler = true,
				LogLevel = LogLevel.Debug,
			});

			_leaderboardUpdateFetcher = new LeaderboardUpdateFetcher();
			_leaderboardUpdateFetcher.NewRecordEvent += PostNewRecordAsync;
			_leaderboardUpdateFetcher.NewOldestRecordEvent += UpdateOldestRecordTopicAsync;
		}

		public async Task RunAsync()
		{
			await _client.ConnectAsync();
			_leaderboardUpdateChannel = await _client.GetChannelAsync(LEADERBOARD_UPDATE_CHANNEL_ID);
			_leaderboardUpdateFetcher.Start();
			await Task.Delay(-1);
		}

		private async void PostNewRecordAsync(object sender, LeaderboardNewRecordEventArgs e)
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

			await _client.SendMessageAsync(_leaderboardUpdateChannel, content.ToString());
		}

		private async void UpdateOldestRecordTopicAsync(object sender, LeaderboardNewRecordEventArgs e)
		{
			var ageInDays = (DateTime.UtcNow - e.NewRecord.Date).Days;

			var topic = new StringBuilder();
			topic.AppendLine("Longest standing record:");
			topic.AppendLine($"{e.NewRecord.Category.Name} - [{TimeToString(e.NewRecord.TimeInMilliseconds)}] by {e.NewRecord.PlayerUsername} [{ageInDays} days]");
			try
			{
				await _leaderboardUpdateChannel.ModifyAsync(topic: topic.ToString());
			}
			catch (UnauthorizedException)
			{
				LogMessage(LogLevel.Error, $"Could not update channel topic: Insufficient permissions.");
			}
		}

		private string TimeToString(int milliseconds)
		{
			if (milliseconds < 60 * 1000)
			{
				return $"{(float)milliseconds / 1000}s";
			}
			else
			{
				return $"{milliseconds / 1000 / 60}m {((float)milliseconds / 1000) % 60}s";
			}
		}

		private void LogMessage(LogLevel level, string message)
		{
			_client.DebugLogger.LogMessage(level, "GentleSquire", message, DateTime.Now);
		}
	}
}
