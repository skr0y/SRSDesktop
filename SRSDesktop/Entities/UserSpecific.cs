using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace SRSDesktop.Entities
{
	public class UserSpecific
	{
		[JsonIgnore]
		private static List<Tuple<SrsLevel, TimeSpan, string>> SrsLevelToTimeSpan = new List<Tuple<SrsLevel, TimeSpan, string>>()
		{
			new Tuple<SrsLevel, TimeSpan, string>(SrsLevel.Apprentice, new TimeSpan(4, 0, 0), "4h"),
			new Tuple<SrsLevel, TimeSpan, string>(SrsLevel.Apprentice, new TimeSpan(8, 0, 0), "8h"),
			new Tuple<SrsLevel, TimeSpan, string>(SrsLevel.Apprentice, new TimeSpan(23, 0, 0), "1d"),
			new Tuple<SrsLevel, TimeSpan, string>(SrsLevel.Apprentice, new TimeSpan(1, 23, 0, 0), "2d"),
			new Tuple<SrsLevel, TimeSpan, string>(SrsLevel.Guru, new TimeSpan(6, 23, 0, 0), "7d"),
			new Tuple<SrsLevel, TimeSpan, string>(SrsLevel.Guru, new TimeSpan(13, 23, 0, 0), "14d"),
			new Tuple<SrsLevel, TimeSpan, string>(SrsLevel.Master, new TimeSpan(29, 23, 0, 0), "1m"),
			new Tuple<SrsLevel, TimeSpan, string>(SrsLevel.Enlighten, new TimeSpan(119, 23, 0, 0), "4m"),
			new Tuple<SrsLevel, TimeSpan, string>(SrsLevel.Burned, new TimeSpan(), "BURN")
		};

		public SrsLevel Srs { get; set; }
		public int SrsNumeric { get; set; }

		[JsonConverter(typeof(UnixDateTimeConverter))]
		public DateTime UnlockedDate { get; set; }
		[JsonConverter(typeof(UnixDateTimeConverter))]
		public DateTime AvailableDate { get; set; }
		[JsonConverter(typeof(UnixDateTimeConverter))]
		public DateTime BurnedDate { get; set; }

		public bool Burned { get; set; }


		public static Tuple<SrsLevel, TimeSpan, string> GetLevelInfo(int srsLevel)
		{
			return SrsLevelToTimeSpan[srsLevel - 1];
		}


		public void AddProgress(int levelChange, bool updateTime = true)
		{
			var now = DateTime.Now;

			SrsNumeric = Math.Max(1, Math.Min(SrsNumeric + levelChange, SrsLevelToTimeSpan.Count));
			Srs = SrsLevelToTimeSpan[SrsNumeric - 1].Item1;

			if (updateTime)
			{
				AvailableDate = now + SrsLevelToTimeSpan[SrsNumeric - 1].Item2;
			}

			if (Srs == SrsLevel.Burned)
			{
				Burned = true;
				BurnedDate = now;
			}
		}
	}
}