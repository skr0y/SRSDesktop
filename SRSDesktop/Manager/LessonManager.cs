using SRSDesktop.Entities;
using SRSDesktop.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRSDesktop.Manager
{
	public class LessonManager : Manager
	{
		public LessonManager(string resourcesPath) : base(resourcesPath)
		{
		}

		protected override Func<Item, bool> Selector => item => item.UserSpecific == null;

		public override List<Item> Get(int count = 0, OrderByAvailability orderByAvailability = OrderByAvailability.Default, OrderByType orderByType = OrderByType.Default)
		{
			IEnumerable<Item> result = Load();

			if (orderByAvailability != OrderByAvailability.Default)
			{
				switch (orderByAvailability)
				{
					case OrderByAvailability.Older:
						result = result.OrderBy(item => item.Level);
						break;
					case OrderByAvailability.Recent:
						result = result.OrderByDescending(item => item.Level);
						break;
					case OrderByAvailability.Shuffle:
						result = result.Shuffle();
						break;
				}
			}

			if (orderByType != OrderByType.Default)
			{
				switch (orderByType)
				{
					case OrderByType.RadicalToVocab:
						result = result.OrderBy(item => item is Radical ? 0 : item is Kanji ? 1 : 2);
						break;
				}
			}

			if (count > 0)
			{
				result = result.Take(count);
			}

			return result.ToList();
		}
	}
}