using SRSDesktop.Entities;
using SRSDesktop.Util;
using System.Linq;
using System.Windows;

namespace SRSDesktop.Windows
{
	public partial class DetailsWindow : Window
	{
		private Item Item;

		public DetailsWindow()
		{
			InitializeComponent();
		}

		public DetailsWindow(Item item) : this()
		{
			Item = item;
			FillDetails();

			// TODO:
			//  sound
		}

		private void FillDetails()
		{
			tbCharacter.Text = Item.Character;

			if (Item is Radical radical)
			{
				tbMeaning.Text = radical.Meaning;
				tbMeaningExplanation.Text = radical.Mnemonic;
			}
			else if (Item is Kanji kanji)
			{
				tbMeaning.Text = kanji.Meaning;
				tbReading.Text = "TBD";
				tbMeaningExplanation.Text = kanji.MeaningMnemonic;
				tbReadingExplanation.Text = kanji.ReadingMnemonic;
				//kanji.Examples
			}
			else if (Item is Vocab vocab)
			{
				tbMeaning.Text = vocab.Meaning;
				tbReading.Text = vocab.Kana;
				tbMeaningExplanation.Text = vocab.MeaningExplanation;
				tbReadingExplanation.Text = vocab.ReadingExplanation;
				//string.Join(vocab.ContextSentences.Select(cs => cs.Japanese + Environment.NewLine + cs.English)));

				/*var soundPath = Utils.Utils.GetResourcesPath() + "Sound/" + item.Character + ".ogg";
				if (File.Exists(soundPath))
				{
					VorbisWaveReader = new VorbisWaveReader(soundPath);
					WaveOutEvent = new WaveOutEvent();
					WaveOutEvent.Init(VorbisWaveReader);
					PlaySound();
				}*/
			}

			cbUserLevel.SelectedValuePath = "Key";
			cbUserLevel.DisplayMemberPath = "Value";
			foreach (var kvPair in Enumerable.Range(1, 8).ToDictionary(k => k, v => UserSpecific.GetLevelInfo(v)))
			{
				cbUserLevel.Items.Add(kvPair);
				if (kvPair.Key == Item.UserSpecific.SrsNumeric) cbUserLevel.SelectedItem = kvPair;
			}
		}

		private void BtnSaveClick(object sender, RoutedEventArgs e)
		{
			Item.Character = tbCharacter.Text;

			if (Item is Radical radical)
			{
				radical.Meaning = tbMeaning.Text;
				radical.Mnemonic = tbMeaningExplanation.Text;
			}
			else if (Item is Kanji kanji)
			{
				kanji.Meaning = tbMeaning.Text;
				kanji.MeaningMnemonic = tbMeaningExplanation.Text;
				//kanji.Reading
				kanji.ReadingMnemonic = tbReadingExplanation.Text;
				//kanji.Examples
			}
			else if (Item is Vocab vocab)
			{
				vocab.Meaning = tbMeaning.Text;
				vocab.Kana = tbReading.Text;
				vocab.MeaningExplanation = tbMeaningExplanation.Text;
				vocab.ReadingExplanation = tbReadingExplanation.Text;
				//vocab.ContextSentences
			}

			Item.UserSpecific.AddProgress((int)cbUserLevel.SelectedValue - Item.UserSpecific.SrsNumeric, chkResetTime.IsChecked.Value);

			DialogResult = true;
		}
	}
}
