using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace SRSDesktop.Entities
{
	public class UserSpecific
	{
		[JsonIgnore]
		private static List<Tuple<SrsLevel, TimeSpan>> SrsLevelToTimeSpan = new List<Tuple<SrsLevel, TimeSpan>>()
		{
			new Tuple<SrsLevel, TimeSpan>(SrsLevel.Apprentice, new TimeSpan(4, 0, 0)),
			new Tuple<SrsLevel, TimeSpan>(SrsLevel.Apprentice, new TimeSpan(8, 0, 0)),
			new Tuple<SrsLevel, TimeSpan>(SrsLevel.Apprentice, new TimeSpan(23, 0, 0)),
			new Tuple<SrsLevel, TimeSpan>(SrsLevel.Apprentice, new TimeSpan(1, 23, 0, 0)),
			new Tuple<SrsLevel, TimeSpan>(SrsLevel.Guru, new TimeSpan(6, 23, 0, 0)),
			new Tuple<SrsLevel, TimeSpan>(SrsLevel.Guru, new TimeSpan(13, 23, 0, 0)),
			new Tuple<SrsLevel, TimeSpan>(SrsLevel.Master, new TimeSpan(29, 23, 0, 0)),
			new Tuple<SrsLevel, TimeSpan>(SrsLevel.Enlighten, new TimeSpan(119, 23, 0, 0)),
			new Tuple<SrsLevel, TimeSpan>(SrsLevel.Burned, new TimeSpan())
		};

		public SrsLevel Srs { get; set; }
		public int SrsNumeric { get; set; }

		public int? MeaningCorrect { get; set; }
		public int? MeaningIncorrect { get; set; }
		public int? MeaningMaxStreak { get; set; }
		public int? MeaningCurrentStreak { get; set; }

		public int? ReadingCorrect { get; set; }
		public int? ReadingIncorrect { get; set; }
		public int? ReadingMaxStreak { get; set; }
		public int? ReadingCurrentStreak { get; set; }

		public string MeaningNote { get; set; }
		public string ReadingNote { get; set; }
		public string[] UserSynonyms { get; set; }

		[JsonConverter(typeof(UnixDateTimeConverter))]
		public DateTime UnlockedDate { get; set; }
		[JsonConverter(typeof(UnixDateTimeConverter))]
		public DateTime AvailableDate { get; set; }
		[JsonConverter(typeof(UnixDateTimeConverter))]
		public DateTime BurnedDate { get; set; }

		public bool Burned { get; set; }

		public void AddLevel(int levelChange)
		{
			var now = DateTime.Now;

			SrsNumeric = Math.Max(1, Math.Min(SrsNumeric + levelChange, SrsLevelToTimeSpan.Count));
			Srs = SrsLevelToTimeSpan[SrsNumeric - 1].Item1;
			AvailableDate = now + SrsLevelToTimeSpan[SrsNumeric - 1].Item2;

			if (Srs == SrsLevel.Burned)
			{
				Burned = true;
				BurnedDate = now;
			}
		}
	}
}