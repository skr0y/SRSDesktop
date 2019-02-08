using System;

namespace SRSDesktop.Entities
{
	public class UserSpecific
	{
		public SrsLevel Srs { get; set; }
		public int SrsNumeric { get; set; }

		public int MeaningCorrect { get; set; }
		public int MeaningIncorrect { get; set; }
		public int MeaningMaxStreak { get; set; }
		public int MeaningCurrentStreak { get; set; }

		public int ReadingCorrect { get; set; }
		public int ReadingIncorrect { get; set; }
		public int ReadingMaxStreak { get; set; }
		public int ReadingCurrentStreak { get; set; }

		public DateTime UnlockedDate { get; set; }
		public DateTime AvailableDate { get; set; }
		public DateTime BurnedDate { get; set; }

		public bool Burned { get; set; }
	}
}