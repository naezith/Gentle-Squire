using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace GentleSquire.Leaderboard
{
	public class LeaderboardUpdateFetcher
	{
		private static readonly TimeSpan SINGLE_CATEGORY_UPDATE_TIMER = TimeSpan.FromSeconds(2);
		private static readonly TimeSpan RECENT_PERSONAL_BESTS_UPDATE_TIMER = TimeSpan.FromSeconds(30);



		public delegate void LeaderboardNewWorldRecordEventHandler(object sender, LeaderboardNewWorldRecordEventArgs e);
		public delegate void LeaderboardNewPersonalBestEventHandler(object sender, LeaderboardNewPersonalBestEventArgs e);
		public event LeaderboardNewWorldRecordEventHandler NewWorldRecordEvent;
		public event LeaderboardNewWorldRecordEventHandler NewOldestRecordEvent;
		public event LeaderboardNewPersonalBestEventHandler NewPersonalBestEvent;



		private Timer _singleCategoryUpdateTimer;
		private Timer _recentPersonalBestsUpdateTimer;
		private int _categoryIndex;
		private List<LeaderboardEntry> _worldRecords;
		private LeaderboardEntry _oldestWorldRecord;
		private LeaderboardPersonalBestEntry _newestPersonalBest;

		public LeaderboardUpdateFetcher()
		{
			_singleCategoryUpdateTimer = new Timer(SINGLE_CATEGORY_UPDATE_TIMER.TotalMilliseconds);
			_singleCategoryUpdateTimer.Elapsed += (s, e) => UpdateSingleCategoryAsync().Wait();

			_recentPersonalBestsUpdateTimer = new Timer(RECENT_PERSONAL_BESTS_UPDATE_TIMER.TotalMilliseconds);
			_recentPersonalBestsUpdateTimer.Elapsed += (s, e) => UpdateRecentPersonalBestsAsync().Wait();

			_worldRecords = new List<LeaderboardEntry>();
			_oldestWorldRecord = null;
			_newestPersonalBest = null;
		}

		public void Start()
		{
			_categoryIndex = 0;
			_singleCategoryUpdateTimer.Start();
			_recentPersonalBestsUpdateTimer.Start();
			UpdateRecentPersonalBestsAsync().Wait();
		}

		public void Stop()
		{
			_singleCategoryUpdateTimer.Stop();
			_recentPersonalBestsUpdateTimer.Stop();
		}

		public async Task UpdateSingleCategoryAsync()
		{
			var category = Category.Categories[_categoryIndex];

			LeaderboardEntry serverRecord;
			switch (category.Type)
			{
				case Category.CategoryType.IndividualLevel:
					serverRecord = (await LeaderboardAPI.GetIndividualLevelReplaysAsync(category, 0, 1)).First();
					break;
				case Category.CategoryType.Speedrun:
					serverRecord = (await LeaderboardAPI.GetTenSpeedrunReplaysAsync(category, 0)).First();
					break;
				default:
					throw new NotImplementedException();
			}

			var localRecord = GetLocallyStoredRecord(category);

			if (localRecord is null || serverRecord.TimeInMilliseconds < localRecord.TimeInMilliseconds)
			{
				AddLocallyStoredRecord(category, serverRecord);
				NewWorldRecordEvent?.Invoke(this, new LeaderboardNewWorldRecordEventArgs
				{
					NewRecord = serverRecord,
					PreviousRecord = localRecord,
				});

				_worldRecords.Add(serverRecord);
			}
			else
			{
				_worldRecords.Add(localRecord);
			}

			_categoryIndex = (_categoryIndex + 1) % Category.Categories.Count;

			if (_categoryIndex == 0)
			{
				var newOldestRun = _worldRecords.Where(r => r.Date == _worldRecords.Min(r => r.Date)).First();
				if (!newOldestRun.Equals(_oldestWorldRecord))
				{
					NewOldestRecordEvent?.Invoke(this, new LeaderboardNewWorldRecordEventArgs
					{
						NewRecord = newOldestRun,
						PreviousRecord = _oldestWorldRecord,
					});
				}
				_oldestWorldRecord = newOldestRun;

				_worldRecords.Clear();
			}
		}

		public async Task UpdateRecentPersonalBestsAsync()
		{
			var recentRecords = await LeaderboardAPI.GetTenRecentPersonalBestsAsync(false);

			if (_newestPersonalBest != null)
			{
				var numberOfEntriesToAnnounce = recentRecords.IndexOf(_newestPersonalBest);
				if (numberOfEntriesToAnnounce == -1) // not found
				{
					numberOfEntriesToAnnounce = recentRecords.Count;
				}

				foreach (var announcedRecord in recentRecords.Take(numberOfEntriesToAnnounce).Reverse())
				{
					NewPersonalBestEvent.Invoke(this, new LeaderboardNewPersonalBestEventArgs()
					{
						NewPersonalBest = announcedRecord.Entry,
						PreviousPersonalBestTimeInMilliseconds = announcedRecord.OldTime,
					});
				}
			}

			_newestPersonalBest = recentRecords.First();
		}

		private string GetCategoryDirectory(Category category)
		{
			return $"database/records/{category.Name}.txt";
		}

		private LeaderboardEntry GetLocallyStoredRecord(Category category)
		{
			var file = GetCategoryDirectory(category);

			if (!File.Exists(file))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(file));
				File.WriteAllText(file, new JArray().ToString());
			}

			var records = JArray.Parse(File.ReadAllText(file));

			if (records.Count == 0)
			{
				return null;
			}
			else
			{
				return records.Last.ToObject<LeaderboardEntry>();
			}
		}

		private void AddLocallyStoredRecord(Category category, LeaderboardEntry run)
		{
			var file = GetCategoryDirectory(category);

			if (!File.Exists(file))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(file));
				File.WriteAllText(file, new JArray().ToString());
			}

			var records = JArray.Parse(File.ReadAllText(file));
			records.Add(JObject.FromObject(run));

			File.WriteAllText(file, records.ToString());
		}
	}
}
