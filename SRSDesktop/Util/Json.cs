using Newtonsoft.Json;
using System.IO;

namespace SRSDesktop.Util
{
	public static class Json
	{
		public static T ReadJson<T>(string path)
		{
			return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
		}

		public static void WriteJson<T>(string path, T obj)
		{
			File.WriteAllText(path, JsonConvert.SerializeObject(obj, Formatting.Indented));
		}
	}
}