namespace SRSDesktop.Entities
{
	public class Vocab : Item
	{
		public string Kana { get; set; }

		public string PartOfSpeech { get; set; }

		public string ReadingExplanation { get; set; }
		public string MeaningExplanation { get; set; }

		public ContextSentence[] ContextSentences { get; set; }
	}
}