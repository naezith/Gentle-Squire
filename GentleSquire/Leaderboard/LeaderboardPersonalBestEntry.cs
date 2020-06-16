using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace GentleSquire.Leaderboard
{
	public class LeaderboardPersonalBestEntry : IEquatable<LeaderboardPersonalBestEntry>
	{
		public LeaderboardEntry Entry { get; set; }
		public int OldTime { get; set; }



		public bool Equals(LeaderboardPersonalBestEntry other)
		{
			return
				this.Entry == other.Entry
				&& this.OldTime == other.OldTime
			;
		}
	}
}
