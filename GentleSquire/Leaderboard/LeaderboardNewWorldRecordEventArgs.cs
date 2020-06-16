using System;

namespace GentleSquire.Leaderboard
{
	public class LeaderboardNewWorldRecordEventArgs : EventArgs
	{
		public LeaderboardEntry NewRecord { get; set; }
		public LeaderboardEntry PreviousRecord { get; set; }
	}
}
