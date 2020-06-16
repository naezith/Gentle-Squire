using System;

namespace GentleSquire.Leaderboard
{
	public class LeaderboardNewPersonalBestEventArgs : EventArgs
	{
		public LeaderboardEntry NewPersonalBest { get; set; }
		public int PreviousPersonalBestTimeInMilliseconds { get; set; }
	}
}
