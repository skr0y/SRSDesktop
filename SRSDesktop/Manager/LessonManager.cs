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

		public override HashSet<Item> Get(int count = 0, ManagerOptions options = ManagerOptions.Default)
		{
			IEnumerable<Item> result = Load();

			if (options != ManagerOptions.Default)
			{
				switch (options)
				{
					case ManagerOptions.Older:
						result = result.OrderBy(item => item.Level);
						break;
					case ManagerOptions.Recent:
						result = result.OrderByDescending(item => item.Level);
						break;
					case ManagerOptions.Shuffle:
						result = result.Shuffle();
						break;
				}
			}

			if (count > 0)
			{
				result = result.Take(count);
			}

			return new HashSet<Item>(result);
		}
	}
}