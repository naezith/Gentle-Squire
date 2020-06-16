using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace GentleSquire.Leaderboard
{
	public class LeaderboardEntry : IEquatable<LeaderboardEntry>
	{
		[JsonProperty(PropertyName = "rank")]
		public int Rank { get; set; }

		[JsonProperty(PropertyName = "username")]
		public string PlayerUsername { get; set; }

		[DefaultValue(-1)]
		[JsonProperty(PropertyName = "player_id", DefaultValueHandling = DefaultValueHandling.Populate)]
		public int PlayerId { get; set; }
		public int SpeedrunPlayerId { set { PlayerId = value; } }

		[JsonProperty(PropertyName = "steam_id")]
		public string PlayerSteamId { get; set; }

		[JsonProperty(PropertyName = "badge")]
		public int PlayerBadge { get; set; }

		[DefaultValue(null)]
		[JsonProperty(PropertyName = "global_score", DefaultValueHandling = DefaultValueHandling.Populate)]
		public double? PlayerTotalDominance { get; set; }

		[JsonProperty(PropertyName = "time")]
		public int TimeInMilliseconds { get; set; }
		[JsonProperty(PropertyName = "total_time")]
		public int SpeedrunTimeInMilliseconds { set { TimeInMilliseconds = value; } }
		[JsonProperty(PropertyName = "new_time")]
		public int NewPersonalBestTimeInMilliseconds { set { TimeInMilliseconds = value; } }

		[JsonProperty(PropertyName = "replay")]
		public string Replay { get; set; }

		[JsonProperty(PropertyName = "update_date")]
		public DateTime Date { get; set; }

		[JsonProperty(PropertyName = "verified")]
		public bool Verified { get; set; }

		[JsonProperty(PropertyName = "category")]
		public Category Category { get; set; }



		public bool Equals(LeaderboardEntry other)
		{
			return
				PlayerId == other.PlayerId
				&& TimeInMilliseconds == other.TimeInMilliseconds
				&& Date == other.Date
				&& Category == other.Category
			;
		}

		public static bool operator ==(LeaderboardEntry first, LeaderboardEntry second)
		{
			return first.Equals(second);
		}

		public static bool operator !=(LeaderboardEntry first, LeaderboardEntry second)
		{
			return !(first == second);
		}
	}
}
