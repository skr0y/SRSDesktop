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
		public const int UnlockLevel = 3;

		protected const string RadicalFile = "radicals.json";
		protected const string KanjiFile = "kanji.json";
		protected const string VocabFile = "vocabulary.json";

		public int Count => Cache.FindAll(Selector).Count;
		public int TotalCount => Cache.Count;

		protected static List<Item> Cache { get; set; }

		protected abstract Predicate<Item> Selector { get; }
		public int UserLevel { get; protected set; }

		private string ResourcesPath;

		protected Manager(string resourcesPath)
		{
			ResourcesPath = resourcesPath;
			if (Cache == null) Update();
		}

		public abstract List<Item> Get(int count = 0, OrderByAvailability options = OrderByAvailability.None, OrderByType orderByType = OrderByType.None);

		public List<Item> GetAll()
		{
			return Cache;
		}

		public void Update()
		{
			var result = new List<Item>();

			var radicals = Json.ReadJson<Radical[]>(ResourcesPath + RadicalFile);
			var kanjis = Json.ReadJson<Kanji[]>(ResourcesPath + KanjiFile);
			var vocabs = Json.ReadJson<Vocab[]>(ResourcesPath + VocabFile);

			result.AddRange(radicals);
			result.AddRange(kanjis);
			result.AddRange(vocabs);

			foreach (var vocab in vocabs)
			{
				vocab.Related = vocab.Character.Where(chr => chr.IsKanji()).Select(chr => kanjis.First(k => k.Character == chr.ToString())).Cast<Item>().ToList();
				vocab.Learnable = vocab.UserSpecific == null && vocab.Related.All(r => r.UserSpecific?.SrsNumeric >= UnlockLevel);
			}

			foreach (var kanji in kanjis)
			{
				var matches = Regex.Matches(kanji.MeaningMnemonic, @"\[.*?\]").Cast<Match>().Select(m => m.Value.Trim('[', ']').ToLower()).Distinct();
				var radical = radicals.FirstOrDefault(r => r.Character == kanji.Character);
				kanji.Related = matches.Select(m => radicals.FirstOrDefault(r => r.Meaning == m && r.Level <= kanji.Level)).Where(m => m != null).Cast<Item>().ToList();
				if (radical != null && !kanji.Related.Contains(radical)) kanji.Related.Insert(0, radical);
				kanji.Related.AddRange(vocabs.Where(v => v.Related.Contains(kanji)));
				kanji.Learnable = kanji.UserSpecific == null && kanji.Related.OfType<Radical>().All(r => r.UserSpecific?.SrsNumeric >= UnlockLevel);
			}

			foreach (var radical in radicals)
			{
				radical.Related = kanjis.Where(k => k.Related.Contains(radical)).Cast<Item>().ToList();
				radical.Learnable = radical.UserSpecific == null && (radical.Level == 1 || result.FindAll(i => i.Level == radical.Level - 1).All(i => i.UserSpecific?.SrsNumeric >= UnlockLevel));
			}

			for (var i = 1; i <= 60; i++)
			{
				if (radicals.Where(r => r.Level == i).All(r => r.UserSpecific != null || r.Learnable)) UserLevel = i;
				else break;
			}

			foreach (var item in result)
			{
				if (item.UserSpecific != null) item.UserSpecific.Item = item;
				item.Learnable &= item.Level <= UserLevel;
			}

			Cache = result;
		}

		public bool Save()
		{
			Json.WriteJson(ResourcesPath + RadicalFile, Cache.OfType<Radical>().ToArray());
			Json.WriteJson(ResourcesPath + KanjiFile, Cache.OfType<Kanji>().ToArray());
			Json.WriteJson(ResourcesPath + VocabFile, Cache.OfType<Vocab>().ToArray());

			return true;
		}
	}
}