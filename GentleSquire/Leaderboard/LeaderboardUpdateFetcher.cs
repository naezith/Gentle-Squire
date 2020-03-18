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



		public delegate void LeaderboardNewRecordEventHandler(object sender, LeaderboardNewRecordEventArgs e);
		public event LeaderboardNewRecordEventHandler NewRecordEvent;
		public event LeaderboardNewRecordEventHandler NewOldestRecordEvent;



		private Timer _singleCategoryUpdateTimer;
		private int _categoryIndex;
		private List<Category> _categories;
		private List<LeaderboardEntry> _worldRecords;
		private LeaderboardEntry _oldestRun;

		public LeaderboardUpdateFetcher()
		{
			_categories = JArray.Parse(File.ReadAllText("resources/categories.json")).ToObject<List<Category>>();

			_singleCategoryUpdateTimer = new Timer(SINGLE_CATEGORY_UPDATE_TIMER.TotalMilliseconds);
			_singleCategoryUpdateTimer.Elapsed += (s, e) => UpdateSingleCategoryAsync().Wait();

			_worldRecords = new List<LeaderboardEntry>();
		}

		public void Start()
		{
			_categoryIndex = 0;
			_singleCategoryUpdateTimer.Start();
		}

		public void Stop()
		{
			_singleCategoryUpdateTimer.Stop();
		}

		public async Task UpdateSingleCategoryAsync()
		{
			var category = _categories[_categoryIndex];

			LeaderboardEntry serverRecord;
			switch (category.Type)
			{
				case Category.CategoryType.IndividualLevel:
					serverRecord = (await LeaderboardServerHandler.GetIndividualLevelReplaysAsync(category, 0, 1)).First();
					break;
				case Category.CategoryType.Speedrun:
					serverRecord = (await LeaderboardServerHandler.GetTenSpeedrunReplaysAsync(category, 0)).First();
					break;
				default:
					throw new NotImplementedException();
			}

			var localRecord = GetLocallyStoredRecord(category);

			if (localRecord == null || serverRecord.TimeInMilliseconds < localRecord.TimeInMilliseconds)
			{
				AddLocallyStoredRecord(category, serverRecord);
				NewRecordEvent?.Invoke(this, new LeaderboardNewRecordEventArgs
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

			_categoryIndex = (_categoryIndex + 1) % _categories.Count;

			if (_categoryIndex == 0)
			{
				var newOldestRun = _worldRecords.Where(r => r.Date == _worldRecords.Min(r => r.Date)).First();
				if (newOldestRun != _oldestRun)
				{
					NewOldestRecordEvent?.Invoke(this, new LeaderboardNewRecordEventArgs
					{
						NewRecord = newOldestRun,
						PreviousRecord = _oldestRun,
					});
				}
				_oldestRun = newOldestRun;

				_worldRecords.Clear();
			}
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
