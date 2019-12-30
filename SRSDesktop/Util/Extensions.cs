using System;
using System.Collections.Generic;
using System.Linq;

namespace SRSDesktop.Util
{
	public static class Extensions
	{
		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
		{
			var random = new Random();

			return source.OrderBy(item => random.Next());
		}

		public static bool IsKanji(this char c)
		{
			return c >= 0x4E00 && c <= 0x9FBF;
		}
	}
}
