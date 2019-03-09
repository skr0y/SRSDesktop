using System;
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

		public int Count => Cache.Count;
		public int TotalCount { get; private set; }

		protected HashSet<Item> Cache { get; set; }
		protected abstract Func<Item, bool> Selector { get; }

		private string resourcesPath;

		protected Manager(string resourcesPath)
		{
			this.resourcesPath = resourcesPath;
		}

		public abstract HashSet<Item> Get(int count = 0, ManagerOptions options = ManagerOptions.Default);

		public void Update()
		{
			Load(true);
		}

		public bool Save(HashSet<Item> items)
		{
			Json.WriteJson(resourcesPath + radicalFile, items.Where(item => item is Radical).Cast<Radical>().ToArray());
			Json.WriteJson(resourcesPath + kanjiFile, items.Where(item => item is Kanji).Cast<Kanji>().ToArray());
			Json.WriteJson(resourcesPath + vocabFile, items.Where(item => item is Vocab).Cast<Vocab>().ToArray());

			Cache = items;

			return true;
		}

		protected HashSet<Item> Load(bool forceUpdate = false)
		{
			if (!forceUpdate && Cache != null && Cache.Count > 0)
			{
				return Cache;
			}

			var result = new HashSet<Item>();

			result.UnionWith(Json.ReadJson<Radical[]>(resourcesPath + radicalFile));
			result.UnionWith(Json.ReadJson<Kanji[]>(resourcesPath + kanjiFile));
			result.UnionWith(Json.ReadJson<Vocab[]>(resourcesPath + vocabFile));

			TotalCount = result.Count;

			result.IntersectWith(result.Where(Selector));

			Cache = result;

			return result;
		}
	}
}