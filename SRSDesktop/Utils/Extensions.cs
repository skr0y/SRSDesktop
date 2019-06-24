using System;
using System.Collections.Generic;
using System.Linq;

namespace SRSDesktop.Utils
{
	public static class Extensions
	{
		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
		{
			var random = new Random();

			return source.OrderBy(item => random.Next());
		}
	}
}
