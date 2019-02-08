﻿using Newtonsoft.Json;
using System.IO;

namespace SRSDesktop.Utils
{
	public static class Json
	{
		public static T ReadJson<T>(string path)
		{
			return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
		}

		public static void WriteJson<T>(string path, T obj)
		{
			File.WriteAllText(path, JsonConvert.SerializeObject(obj));
		}
	}
}