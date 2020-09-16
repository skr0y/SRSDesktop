using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRSDesktop.Entities
{
	public class UserSpecific
	{
		[JsonIgnore]
		private static Dictionary<int, SrsLevelInfo> SrsLevelInfos = new Dictionary<int, SrsLevelInfo>()
		{
			{ 1, new SrsLevelInfo(1, SrsLevel.Apprentice, new TimeSpan(4, 0, 0), "4h") },
			{ 2, new SrsLevelInfo(2, SrsLevel.Apprentice, new TimeSpan(18, 0, 0), "18h") },
			{ 3, new SrsLevelInfo(3, SrsLevel.Apprentice, new TimeSpan(2, 0, 0, 0), "2d") },
			{ 4, new SrsLevelInfo(4, SrsLevel.Guru, new TimeSpan(7, 0, 0, 0), "7d") },
			{ 5, new SrsLevelInfo(5, SrsLevel.Guru, new TimeSpan(14, 0, 0, 0), "14d") },
			{ 6, new SrsLevelInfo(6, SrsLevel.Master, new TimeSpan(30, 0, 0, 0), "1m") },
			{ 7, new SrsLevelInfo(7, SrsLevel.Enlighten, new TimeSpan(90, 0, 0, 0), "3m") },
			{ 8, new SrsLevelInfo(8, SrsLevel.Burned, new TimeSpan(180, 0, 0, 0), "6m") },
		};

		[JsonIgnore]
		private static Random Random = new Random();

		[JsonIgnore]
		private static int RandomizationPercent => SRS.ConfigManager.Config.RandomizationPercent;

		public SrsLevel Srs { get; set; }
		public int SrsNumeric { get; set; }

		[JsonConverter(typeof(UnixDateTimeConverter))]
		public DateTime UnlockedDate { get; set; }
		[JsonConverter(typeof(UnixDateTimeConverter))]
		public DateTime AvailableDate { get; set; }

		[JsonIgnore]
		public Item Item { get; set; }


		public UserSpecific() { }

		public UserSpecific(Item item)
		{
			Item = item;
			Srs = SrsLevelInfos[1].SrsLevel;
			SrsNumeric = SrsLevelInfos[1].Level;
			UnlockedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
			AvailableDate = DateTime.SpecifyKind(DateTime.Now + SrsLevelInfos[1].Time, DateTimeKind.Utc);
		}

		public static SrsLevelInfo GetLevelInfo(int srsLevel)
		{
			return SrsLevelInfos[Math.Min(Math.Max(1, srsLevel), SrsLevelInfos.Last().Value.Level)];/*^1*/
		}


		public void AddProgress(int levelChange, bool updateTime = true)
		{
			var now = DateTime.Now;

			SrsNumeric = Math.Max(1, Math.Min(SrsNumeric + levelChange, SrsLevelInfos.Count));
			Srs = SrsLevelInfos[SrsNumeric - 1].SrsLevel;

			if (updateTime)
			{
				var time = new TimeSpan(SrsLevelInfos[SrsNumeric - 1].Time.Ticks / 100 * (100 + Random.Next(-RandomizationPercent, RandomizationPercent)));
				AvailableDate = DateTime.SpecifyKind(now + time, DateTimeKind.Utc);
			}
		}

		public void Again()
		{
			AvailableDate = DateTime.SpecifyKind(DateTime.Now.AddMinutes(SRS.ConfigManager.Config.AgainInterval), DateTimeKind.Utc);
		}

		public void MakeAvailable()
		{
			AvailableDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
		}


		public class SrsLevelInfo
		{
			public int Level { get; }
			public SrsLevel SrsLevel { get; }
			public TimeSpan Time { get; }
			public string TimeString { get; }

			public SrsLevelInfo(int level, SrsLevel srsLevel, TimeSpan time, string timeString)
			{
				Level = level;
				SrsLevel = srsLevel;
				Time = time;
				TimeString = timeString;
			}
		}
	}
}