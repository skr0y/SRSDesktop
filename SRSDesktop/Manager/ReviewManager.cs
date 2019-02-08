using SRSDesktop.Entities;
using SRSDesktop.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRSDesktop.Manager
{
	public class ReviewManager : Manager
	{
		public ReviewManager(string path) : base(path)
		{
		}

		public override HashSet<Item> GetForLevel(int count = 0, int? level = null, ManagerOptions options = ManagerOptions.Default)
		{
			var result = GetAll().Where(item => item.UserSpecific.UnlockedDate == null && (!level.HasValue || item.Level == level.Value));

			if ((options & ManagerOptions.Older) != 0)
			{
				result = result.OrderBy(item => item.UserSpecific.AvailableDate);
			}

			if ((options & ManagerOptions.Recent) != 0)
			{
				result = result.OrderByDescending(item => item.UserSpecific.AvailableDate);
			}

			if ((options & ManagerOptions.Shuffle) != 0)
			{
				throw new NotImplementedException("Shuffle is not implemented");
			}

			return new HashSet<Item>(result);
		}
	}
}