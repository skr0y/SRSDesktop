using Newtonsoft.Json;

namespace SRSDesktop.Entities
{
	public abstract class Item
	{
		public int Level { get; set; }
		public string Character { get; set; }
		public string Meaning { get; set; }
		public UserSpecific UserSpecific { get; set; }
	}
}