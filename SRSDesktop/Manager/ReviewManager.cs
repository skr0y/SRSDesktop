using SRSDesktop.Entities;
using SRSDesktop.Utils;
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

		protected override Func<Item, bool> Selector => item => item.UserSpecific != null && item.UserSpecific.Burned == false && item.UserSpecific.AvailableDate <= DateTime.Now;

		public override List<Item> Get(int count = 0, OrderByAvailability options = OrderByAvailability.Default, OrderByType orderByType = OrderByType.Default)
		{
			IEnumerable<Item> result = Load();

			if (options != OrderByAvailability.Default)
			{
				switch (options)
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