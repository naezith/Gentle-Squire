using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GentleSquire.Leaderboard
{
	public static class LeaderboardAPI
	{
		private const string URL_BASE = @"http://ron.naezith.com";
		private const string URL_IL_LEADERBOARD = URL_BASE + @"/fetchLeaderboard";
		private const string URL_SPEEDRUN_LEADERBOARD = URL_BASE + @"/fetchSpeedrunLB";
		private const string URL_PERSONAL_BEST_LIST = URL_BASE + @"/fetchRecentScores";



		private static HttpClient _httpClient = new HttpClient();

		public static async Task<IList<LeaderboardEntry>> GetIndividualLevelReplaysAsync(Category category, int startRank, int count)
		{
			var httpContent = new FormUrlEncodedContent(new Dictionary<string, string>
			{
				{ "level_id", category.Id.ToString() },
				{ "start_rank", startRank.ToString() },
				{ "line_count", count.ToString() },
			});

			var content = (await _httpClient.PostAsync(URL_IL_LEADERBOARD, httpContent)).Content;
			var result = await content.ReadAsStringAsync();
			var leaderboardEntries = JObject.Parse(result)["lb_data"].ToObject<List<LeaderboardEntry>>();
			leaderboardEntries.ForEach(e => e.Category = category);

			return leaderboardEntries;
		}

		public static async Task<IList<LeaderboardEntry>> GetTenSpeedrunReplaysAsync(Category category, int startRank)
		{
			var httpContent = new FormUrlEncodedContent(new Dictionary<string, string>
			{
				{ "type", category.Id.ToString() },
				{ "start_rank", startRank.ToString() },
			});

			var content = (await _httpClient.PostAsync(URL_SPEEDRUN_LEADERBOARD, httpContent)).Content;
			var result = await content.ReadAsStringAsync();
			var leaderboardEntries = JObject.Parse(result)["lb_data"].ToObject<List<LeaderboardEntry>>();
			leaderboardEntries.ForEach(e => e.Category = category);

			return leaderboardEntries;
		}

		public static async Task<IList<LeaderboardPersonalBestEntry>> GetTenRecentPersonalBestsAsync(bool customLevels = false)
		{
			var httpContent = new FormUrlEncodedContent(new Dictionary<string, string>
			{
				{ "customLevels", (customLevels ? 1 : 0).ToString() }
			});

			var content = (await _httpClient.PostAsync(URL_PERSONAL_BEST_LIST, httpContent)).Content;
			var result = await content.ReadAsStringAsync();

			var leaderboardEntries = new List<LeaderboardPersonalBestEntry>();
			foreach (var jsonEntry in JObject.Parse(result)["data"])
			{
				var entry = jsonEntry.ToObject<LeaderboardEntry>();
				if (jsonEntry["type"].ToObject<int>() == 0) // individual level
				{
					entry.Category = Category.GetCategory(Category.CategoryType.IndividualLevel, jsonEntry["level_id"].ToObject<int>());
				}
				else // speedrun
				{
					entry.Category = Category.GetCategory(Category.CategoryType.Speedrun, jsonEntry["type"].ToObject<int>());
				}

				var oldTime = jsonEntry["old_time"].ToObject<int>();

				leaderboardEntries.Add(new LeaderboardPersonalBestEntry()
				{
					Entry = entry,
					OldTime = oldTime,
				});
			}

			return leaderboardEntries;
		}
	}
}
