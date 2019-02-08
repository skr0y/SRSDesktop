using System.Collections.Generic;
using System.Linq;
using SRSDesktop.Entities;
using SRSDesktop.Utils;

namespace SRSDesktop.Manager
{
	public abstract class Manager
	{
		protected const string radicalFile = "radicals.json";
		protected const string kanjiFile = "kanji.json";
		protected const string vocabFile = "vocabulary.json";

		public string Path { get; private set; }

		protected Manager(string path)
		{
			Path = path;
		}

		public HashSet<Item> GetAll()
		{
			var result = new HashSet<Item>();

			result.UnionWith(Json.ReadJson<Radical[]>(Path + radicalFile));
			result.UnionWith(Json.ReadJson<Kanji[]>(Path + kanjiFile));
			result.UnionWith(Json.ReadJson<Vocab[]>(Path + vocabFile));

			return result;
		}

		public abstract HashSet<Item> GetForLevel(int count = 0, int? level = null, ManagerOptions options = ManagerOptions.Default);

		public bool Save(IEnumerable<Item> items)
		{
			Json.WriteJson(Path + radicalFile, items.Where(item => item is Radical).Cast<Radical>().ToArray());
			Json.WriteJson(Path + kanjiFile, items.Where(item => item is Kanji).Cast<Kanji>().ToArray());
			Json.WriteJson(Path + vocabFile, items.Where(item => item is Vocab).Cast<Vocab>().ToArray());

			return true;
		}
	}
}