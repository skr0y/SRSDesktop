using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SRSDesktop.Entities;
using SRSDesktop.Util;

namespace SRSDesktop.Manager
{
	public abstract class Manager
	{
		protected const string RadicalFile = "radicals.json";
		protected const string KanjiFile = "kanji.json";
		protected const string VocabFile = "vocabulary.json";

		public int Count { get; private set; }
		public int TotalCount { get; private set; }

		protected abstract Predicate<Item> Selector { get; }

		private string ResourcesPath;

		protected Manager(string resourcesPath)
		{
			ResourcesPath = resourcesPath;
		}

		public abstract List<Item> Get(int count = 0, OrderByAvailability options = OrderByAvailability.None, OrderByType orderByType = OrderByType.None);

		public List<Item> GetAll()
		{
			return Cache;
		}

		public void Update()
		{
			Load(true);
		}

		public bool Save()
		{
			Json.WriteJson(ResourcesPath + RadicalFile, Cache.OfType<Radical>().ToArray());
			Json.WriteJson(ResourcesPath + KanjiFile, Cache.OfType<Kanji>().ToArray());
			Json.WriteJson(ResourcesPath + VocabFile, Cache.OfType<Vocab>().ToArray());

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

			var radicals = Json.ReadJson<Radical[]>(ResourcesPath + RadicalFile);
			var kanjis = Json.ReadJson<Kanji[]>(ResourcesPath + KanjiFile);
			var vocabs = Json.ReadJson<Vocab[]>(ResourcesPath + VocabFile);

			foreach (var vocab in vocabs)
			{
				vocab.Related = vocab.Character.Where(chr => chr.IsKanji()).Select(chr => kanjis.First(k => k.Character == chr.ToString())).Cast<Item>().ToList();
			}

			foreach (var kanji in kanjis)
			{
				var matches = Regex.Matches(kanji.MeaningMnemonic, @"\[.*?\]").Cast<Match>().Select(m => m.Value.Trim('[', ']').ToLower()).Distinct();
				var related = matches.Select(m => radicals.FirstOrDefault(r => r.Meaning == m && r.Level <= kanji.Level)).Where(m => m != null).Cast<Item>().ToList();
				related.AddRange(vocabs.Where(v => v.Related.Contains(kanji)));

				kanji.Related = related;
			}

			foreach (var radical in radicals)
			{
				radical.Related = kanjis.Where(k => k.Related.Contains(radical)).Cast<Item>().ToList();
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