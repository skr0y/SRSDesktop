using SRSDesktop.Manager.Entities;
using System.Collections.Generic;

namespace SRSDesktop.Manager
{
	public class ConfigManager
	{
		//protected const string ConfigFile = "config.txt";

		private string ResourcesPath;

		//private Dictionary<string, Config> Configs;

		public Config Config { get; }


		public ConfigManager(string resourcesPath)
		{
			ResourcesPath = resourcesPath;

			Config = LoadConfig();
		}


		public Config LoadConfig()
		{
			return new Config("");
		}

		//public void AddConfig(string configName, string fileName)
		//{

		//}

		//public Config GetConfig(string configName)
		//{
		//	return Configs[configName];
		//}
	}
}
