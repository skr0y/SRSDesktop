using SRSDesktop.Manager.Entities;
using System;
using System.Collections.Generic;
using System.IO;

namespace SRSDesktop.Manager
{
	public class StatsManager
	{
		protected const string StatsFile = "stats.txt";

		private string ResourcesPath;


		public StatsManager(string resourcesPath)
		{
			ResourcesPath = resourcesPath;
		}


		public Dictionary<DateTime, Stat> LoadStats()
		{
			var result = new Dictionary<DateTime, Stat>();
			var lines = File.ReadAllLines(ResourcesPath + StatsFile);

			foreach (var line in lines)
			{
				var split = line.Split(' ');
				if (split.Length < 4)
				{
					result[DateTime.Parse(split[0])] = new Stat(int.Parse(split[1]), int.Parse(split[2]));
				}
				else
				{
					result[DateTime.Parse(split[0])] = new Stat(int.Parse(split[1]), int.Parse(split[2]), int.Parse(split[3]));
				}
			}

			return result;
		}

		public void AddStats(int time, int lessons)
		{
			File.AppendAllLines(ResourcesPath + StatsFile, new string[] { $"{DateTime.Now:s} {time} {lessons}" });
		}

		public void AddStats(int time, int correct, int wrong)
		{
			File.AppendAllLines(ResourcesPath + StatsFile, new string[] { $"{DateTime.Now:s} {time} {correct} {wrong}" });
		}
	}
}
