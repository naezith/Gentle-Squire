using System;

namespace GentleSquire.Leaderboard
{
	public class LeaderboardNewRecordEventArgs : EventArgs
	{
		public LeaderboardEntry NewRecord { get; set; }
		public LeaderboardEntry PreviousRecord { get; set; }
	}
}
