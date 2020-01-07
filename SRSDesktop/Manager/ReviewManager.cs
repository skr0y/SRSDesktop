using SRSDesktop.Entities;
using SRSDesktop.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRSDesktop.Manager
{
	public class ReviewManager : Manager
	{
		public ReviewManager(string resourcesPath) : base(resourcesPath)
		{
		}

		protected override Predicate<Item> Selector => item => item.UserSpecific != null && item.UserSpecific.AvailableDate <= DateTime.Now;

		public override List<Item> Get(int count = 0, OrderByAvailability orderByAvailability = OrderByAvailability.None, OrderByType orderByType = OrderByType.None)
		{
			IEnumerable<Item> result = Cache.FindAll(Selector);

			if (orderByAvailability != OrderByAvailability.None)
			{
				switch (orderByAvailability)
				{
					case OrderByAvailability.Older:
						result = result.OrderBy(item => item.UserSpecific.AvailableDate);
						break;
					case OrderByAvailability.Recent:
						result = result.OrderByDescending(item => item.UserSpecific.AvailableDate);
						break;
					case OrderByAvailability.Shuffle:
						result = result.Shuffle();
						break;
				}
			}

			if (orderByType != OrderByType.None)
			{
				switch (orderByType)
				{
					case OrderByType.RadicalToVocab:
						result = result.OrderBy(item => item is Radical ? 0 : item is Kanji ? 1 : 2);
						break;
					case OrderByType.AscLevel:
						result = result.OrderBy(item => item.Level);
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