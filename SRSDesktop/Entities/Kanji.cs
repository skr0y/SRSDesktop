using Newtonsoft.Json;
using System;

namespace SRSDesktop.Entities
{
	public class Kanji : Item
	{
		public string Onyomi { get; set; }
		public string Kunyomi { get; set; }
		public string Nanori { get; set; }

		public ReadingType ImportantReading { get; set; }

		public string ReadingMnemonic { get; set; }
		public string ReadingHint { get; set; }

		public string MeaningMnemonic { get; set; }
		public string MeaningHint { get; set; }

		[JsonIgnore]
		public string Examples { get; set; }

		[JsonIgnore]
		public string[] Readings => new string[] { Onyomi, Kunyomi, Nanori };

		[Obsolete]
		[JsonIgnore]
		public string Reading
		{
			get
			{
				var result = $"Important: {ImportantReading}.";

				if (Onyomi != null)
				{
					result += $" Onyomi: {Onyomi}";
				}

				if (Kunyomi != null)
				{
					result += $" Kunyomi: {Kunyomi}";
				}

				if (Nanori != null)
				{
					result += $" Nanori: {Nanori}";
				}

				return result;
			}
		}
	}
}