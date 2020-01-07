using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SRSDesktop.Entities
{
	public abstract class Item
	{
		public int Level { get; set; }
		public string Character { get; set; }
		public string Meaning { get; set; }
		public UserSpecific UserSpecific { get; set; }

		[JsonIgnore]
		public string[] Meanings => Meaning.Split(new string[] { ", " }, StringSplitOptions.None);

		[JsonIgnore]
		public bool Learnable;

		[JsonIgnore]
		public List<Item> Related;
	}
}