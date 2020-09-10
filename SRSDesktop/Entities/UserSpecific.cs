using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace SRSDesktop.Entities
{
	public class UserSpecific
	{
		[JsonIgnore]
		private const int randomizationPercent = 15;

		[JsonIgnore]
		private static List<Tuple<SrsLevel, TimeSpan, string>> SrsLevelInfo = new List<Tuple<SrsLevel, TimeSpan, string>>()
		{
			new Tuple<SrsLevel, TimeSpan, string>(SrsLevel.Apprentice, new TimeSpan(4, 0, 0), "4h"),
			new Tuple<SrsLevel, TimeSpan, string>(SrsLevel.Apprentice, new TimeSpan(8, 0, 0), "8h"),
			new Tuple<SrsLevel, TimeSpan, string>(SrsLevel.Apprentice, new TimeSpan(23, 0, 0), "1d"),
			new Tuple<SrsLevel, TimeSpan, string>(SrsLevel.Apprentice, new TimeSpan(1, 23, 0, 0), "2d"),
			new Tuple<SrsLevel, TimeSpan, string>(SrsLevel.Guru, new TimeSpan(6, 23, 0, 0), "7d"),
			new Tuple<SrsLevel, TimeSpan, string>(SrsLevel.Guru, new TimeSpan(13, 23, 0, 0), "14d"),
			new Tuple<SrsLevel, TimeSpan, string>(SrsLevel.Master, new TimeSpan(29, 23, 0, 0), "1m"),
			new Tuple<SrsLevel, TimeSpan, string>(SrsLevel.Enlighten, new TimeSpan(89, 23, 0, 0), "3m"),
			new Tuple<SrsLevel, TimeSpan, string>(SrsLevel.Burned, new TimeSpan(179, 23, 0, 0), "6m")
		};
		[JsonIgnore]
		private static Random Random = new Random();

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
			Srs = SrsLevelInfo[0].Item1;
			SrsNumeric = 1;
			UnlockedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
			AvailableDate = DateTime.SpecifyKind(DateTime.Now + SrsLevelInfo[0].Item2, DateTimeKind.Utc);
		}

		public static Tuple<SrsLevel, TimeSpan, string> GetLevelInfo(int srsLevel)
		{
			return SrsLevelInfo[Math.Min(Math.Max(0, srsLevel - 1), SrsLevelInfo.Count - 1)];
		}


		public void AddProgress(int levelChange, bool updateTime = true)
		{
			var now = DateTime.Now;

			SrsNumeric = Math.Max(1, Math.Min(SrsNumeric + levelChange, SrsLevelInfo.Count));
			Srs = SrsLevelInfo[SrsNumeric - 1].Item1;

			if (updateTime)
			{
				var time = new TimeSpan(SrsLevelInfo[SrsNumeric - 1].Item2.Ticks / 100 * (100 + Random.Next(-randomizationPercent, randomizationPercent)));
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
	}
}