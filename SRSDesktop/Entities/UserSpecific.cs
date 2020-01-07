using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace SRSDesktop.Entities
{
	public class UserSpecific
	{
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

		public SrsLevel Srs { get; set; }
		public int SrsNumeric { get; set; }

		[JsonConverter(typeof(UnixDateTimeConverter))]
		public DateTime UnlockedDate { get; set; }
		[JsonConverter(typeof(UnixDateTimeConverter))]
		public DateTime AvailableDate { get; set; }



		public static Tuple<SrsLevel, TimeSpan, string> GetLevelInfo(int srsLevel)
		{
			return SrsLevelInfo[srsLevel - 1];
		}


		public void AddProgress(int levelChange, bool updateTime = true)
		{
			var now = DateTime.Now;

			SrsNumeric = Math.Max(1, Math.Min(SrsNumeric + levelChange, SrsLevelInfo.Count));
			Srs = SrsLevelInfo[SrsNumeric - 1].Item1;

			if (updateTime)
			{
				AvailableDate = now + SrsLevelInfo[SrsNumeric - 1].Item2;
			}
		}
	}
}