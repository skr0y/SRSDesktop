using System;
using System.Collections.Generic;
using System.Linq;
using SRSDesktop.Entities;
using SRSDesktop.Utils;

namespace SRSDesktop.Manager
{
	public abstract class Manager
	{
		protected const string RadicalFile = "radicals.json";
		protected const string KanjiFile = "kanji.json";
		protected const string VocabFile = "vocabulary.json";

		public int Count { get; private set; }
		public int TotalCount { get; private set; }

		protected List<Item> Cache { get; set; }
		protected abstract Func<Item, bool> Selector { get; }

		private string resourcesPath;

		protected Manager(string resourcesPath)
		{
			this.resourcesPath = resourcesPath;
		}

		public abstract List<Item> Get(int count = 0, ManagerOptions options = ManagerOptions.Default);

		public void Update()
		{
			Load(true);
		}

		public bool Save()
		{
			Json.WriteJson(resourcesPath + RadicalFile, Cache.OfType<Radical>().ToArray());
			Json.WriteJson(resourcesPath + KanjiFile, Cache.OfType<Kanji>().ToArray());
			Json.WriteJson(resourcesPath + VocabFile, Cache.OfType<Vocab>().ToArray());

			return true;
		}

		protected List<Item> Load(bool forceUpdate = false)
		{
			if (!forceUpdate && Cache != null && Cache.Count > 0)
			{
				return Cache.Where(Selector).ToList();
			}

			Cache = null;

			var result = new List<Item>();

			var radicals = Json.ReadJson<Radical[]>(resourcesPath + RadicalFile);
			var kanjis = Json.ReadJson<Kanji[]>(resourcesPath + KanjiFile);
			var vocabs = Json.ReadJson<Vocab[]>(resourcesPath + VocabFile);

			foreach (var kanji in kanjis)
			{
				var examples = vocabs.Where(v => v.Character.Contains(kanji.Character)).Select(v => v.Character).Take(3);
				kanji.Examples = string.Join(", ", examples);
			}

			result.AddRange(radicals);
			result.AddRange(kanjis);
			result.AddRange(vocabs);

			Cache = result;
			TotalCount = result.Count;

			result = result.Where(Selector).ToList();
			Count = result.Count;

			return result;
		}
	}
}