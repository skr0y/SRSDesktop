namespace SRSDesktop.Entities
{
	public class Radical : Item
	{
		public string Mnemonic { get; set; }

		public int ImageFileSize { get; set; }
		public string ImageFileName { get; set; }
		public string ImageContentType { get; set; }
		public string Image { get; set; }
	}
}