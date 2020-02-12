using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SRSDesktop.Util
{
	public static class Extensions
	{
		public static readonly IDictionary<string, string> RomajiToHiragana = new Dictionary<string, string>
		{
			{"a", "あ"},
			{"i", "い"},
			{"u", "う"},
			{"e", "え"},
			{"o", "お"},
			{"wi", "うぃ"},
			{"we", "うぇ"},
			{"va", "ゔぁ"},
			{"vi", "ゔぃ"},
			{"vu", "ゔ"},
			{"ve", "ゔぇ"},
			{"vo", "ゔぉ"},
			{"vya", "ゔゃ"},
			{"vyi", "ゔぃ"},
			{"vyu", "ゔゅ"},
			{"vye", "ゔぇ"},
			{"vyo", "ゔょ"},
			{"ka", "か"},
			{"ki", "き"},
			{"ku", "く"},
			{"ke", "け"},
			{"ko", "こ"},
			{"kya", "きゃ"},
			{"kyi", "きぃ"},
			{"kyu", "きゅ"},
			{"kye", "きぇ"},
			{"kyo", "きょ"},
			{"ga", "が"},
			{"gi", "ぎ"},
			{"gu", "ぐ"},
			{"ge", "げ"},
			{"go", "ご"},
			{"gya", "ぎゃ"},
			{"gyi", "ぎぃ"},
			{"gyu", "ぎゅ"},
			{"gye", "ぎぇ"},
			{"gyo", "ぎょ"},
			{"sa", "さ"},
			{"shi", "し"},
			{"su", "す"},
			{"se", "せ"},
			{"so", "そ"},
			{"za", "ざ"},
			{"ji", "じ"},
			{"zu", "ず"},
			{"ze", "ぜ"},
			{"zo", "ぞ"},
			{"sha", "しゃ"},
			{"shu", "しゅ"},
			{"she", "しぇ"},
			{"sho", "しょ"},
			{"ja", "じゃ"},
			{"ju", "じゅ"},
			{"je", "じぇ"},
			{"jo", "じょ"},
			{"ta", "た"},
			{"chi", "ち"},
			{"tsu", "つ"},
			{"te", "て"},
			{"to", "と"},
			{"cha", "ちゃ"},
			{"chu", "ちゅ"},
			{"che", "ちぇ"},
			{"cho", "ちょ"},
			{"tsa", "つぁ"},
			{"tsi", "つぃ"},
			{"tse", "つぇ"},
			{"tso", "つぉ"},
			{"tha", "てゃ"},
			{"thi", "てぃ"},
			{"thu", "てゅ"},
			{"tho", "てょ"},
			{"da", "だ"},
			{"di", "ぢ"},
			{"du", "づ"},
			{"de", "で"},
			{"do", "ど"},
			{"na", "な"},
			{"ni", "に"},
			{"nu", "ぬ"},
			{"ne", "ね"},
			{"no", "の"},
			{"nya", "にゃ"},
			{"nyi", "にぃ"},
			{"nyu", "にゅ"},
			{"nye", "にぇ"},
			{"nyo", "にょ"},
			{"ha", "は"},
			{"hi", "ひ"},
			{"hu", "ふ"},
			{"he", "へ"},
			{"ho", "ほ"},
			{"fu", "ふ"},
			{"hya", "ひゃ"},
			{"hyi", "ひぃ"},
			{"hyu", "ひゅ"},
			{"hye", "ひぇ"},
			{"hyo", "ひょ"},
			{"fya", "ふゃ"},
			{"fyu", "ふゅ"},
			{"fyo", "ふょ"},
			{"fa", "ふぁ"},
			{"fi", "ふぃ"},
			{"fe", "ふぇ"},
			{"fo", "ふぉ"},
			{"fyi", "ふぃ"},
			{"fye", "ふぇ"},
			{"ba", "ば"},
			{"bi", "び"},
			{"bu", "ぶ"},
			{"be", "べ"},
			{"bo", "ぼ"},
			{"bya", "びゃ"},
			{"byi", "びぃ"},
			{"byu", "びゅ"},
			{"bye", "びぇ"},
			{"byo", "びょ"},
			{"pa", "ぱ"},
			{"pi", "ぴ"},
			{"pu", "ぷ"},
			{"pe", "ぺ"},
			{"po", "ぽ"},
			{"pya", "ぴゃ"},
			{"pyi", "ぴぃ"},
			{"pyu", "ぴゅ"},
			{"pye", "ぴぇ"},
			{"pyo", "ぴょ"},
			{"ma", "ま"},
			{"mi", "み"},
			{"mu", "む"},
			{"me", "め"},
			{"mo", "も"},
			{"mya", "みゃ"},
			{"myi", "みぃ"},
			{"myu", "みゅ"},
			{"mye", "みぇ"},
			{"myo", "みょ"},
			{"ya", "や"},
			{"yu", "ゆ"},
			{"yo", "よ"},
			{"ra", "ら"},
			{"ri", "り"},
			{"ru", "る"},
			{"re", "れ"},
			{"ro", "ろ"},
			{"rya", "りゃ"},
			{"ryi", "りぃ"},
			{"ryu", "りゅ"},
			{"rye", "りぇ"},
			{"ryo", "りょ"},
			{"wa", "わ"},
			{"wo", "を"},
			{"nn", "ん"},
		};
		private static readonly Regex latinRegex = new Regex("[A-Za-z]", RegexOptions.Compiled);

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
		{
			var random = new Random();

			return source.OrderBy(item => random.Next());
		}

		public static bool IsKanji(this char c)
		{
			return c >= 0x4E00 && c <= 0x9FBF;
		}

		public static string ToHiragana(this string romaji)
		{
			var result = "";
			var vowels = "aiueo";

			for (var i = 0; i < romaji.Length; i++)
			{
				if (vowels.Contains(romaji[i])) result += RomajiToHiragana[romaji[i].ToString()];
				else if (romaji[i] == 'n' && (i + 1 == romaji.Length || !(vowels.Contains(romaji[i + 1]) || romaji[i + 1] == 'y'))) result += "ん";
				else if (i + 1 < romaji.Length && romaji[i] == romaji[i + 1] && latinRegex.IsMatch(romaji[i].ToString())) result += "っ";
				else
				{
					var nextVowel = romaji.IndexOfAny(vowels.ToCharArray(), i);
					if (nextVowel > 0)
					{
						var syllable = romaji.Substring(i, nextVowel - i + 1);
						if (RomajiToHiragana.ContainsKey(syllable))
						{
							result += RomajiToHiragana[syllable];
							i = nextVowel;
							continue;
						}
					}

					result += romaji[i];
				}
			}

			return result;
		}
	}
}
