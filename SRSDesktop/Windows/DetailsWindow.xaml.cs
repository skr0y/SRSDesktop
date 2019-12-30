using SRSDesktop.Entities;
using SRSDesktop.Util;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SRSDesktop.Windows
{
	public partial class DetailsWindow : Window
	{
		private readonly Brush radicalBrush = (Brush)new BrushConverter().ConvertFrom("#FFE9F4FF");
		private readonly Brush kanjiBrush = (Brush)new BrushConverter().ConvertFrom("#FFFF9BFF");
		private readonly Brush vocabBrush = (Brush)new BrushConverter().ConvertFrom("#FFC79BFF");

		private const int btnRelatedCount = 5;

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
			//  manually add related items
		}


		private void FillDetails()
		{
			var allItems = SRS.ReviewManager.GetAll();

			Title = Item.Character ?? Item.Meaning;
			tbCharacter.Text = Item.Character;
			tbMeaning.Text = Item.Meaning;

			if (Item is Radical radical)
			{
				tbMeaningExplanation.Text = radical.Mnemonic;
				tbReading.Visibility = Visibility.Hidden;
				tbReadingExplanation.Visibility = Visibility.Hidden;
				lblRelated.Margin = new Thickness(lblRelated.Margin.Left, lblRelated.Margin.Top - 145, lblRelated.Margin.Right, lblRelated.Margin.Bottom);
				grdRelated.Margin = new Thickness(grdRelated.Margin.Left, grdRelated.Margin.Top - 145, grdRelated.Margin.Right, grdRelated.Margin.Bottom);
				Height -= 145;

				var buttons = Utils.GenerateItemButtons(radical.Related, grdRelated.Width);
				buttons.ForEach(btn => grdRelated.Children.Add(btn));
			}
			else if (Item is Kanji kanji)
			{
				tbReading.Visibility = Visibility.Hidden;
				lblOnyomi.Visibility = Visibility.Visible;
				lblKunyomi.Visibility = Visibility.Visible;
				lblNanori.Visibility = Visibility.Visible;
				tbOnyomiReading.Visibility = Visibility.Visible;
				tbKunyomiReading.Visibility = Visibility.Visible;
				tbNanoriReading.Visibility = Visibility.Visible;
				tbOnyomiReading.Text = kanji.Onyomi;
				tbKunyomiReading.Text = kanji.Kunyomi;
				tbNanoriReading.Text = kanji.Nanori;
				tbMeaningExplanation.Text = kanji.MeaningMnemonic;
				tbReadingExplanation.Text = kanji.ReadingMnemonic;

				var buttons = Utils.GenerateItemButtons(kanji.Related, grdRelated.Width);
				buttons.ForEach(btn => grdRelated.Children.Add(btn));
			}
			else if (Item is Vocab vocab)
			{
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

				var buttons = Utils.GenerateItemButtons(vocab.Related, grdRelated.Width);
				buttons.ForEach(btn => grdRelated.Children.Add(btn));
			}

			if (Item.UserSpecific != null)
			{
				lblTime.Content = ToFormatString(Item.UserSpecific.AvailableDate - DateTime.Now);
				lblUnlocked.Content = Item.UserSpecific.UnlockedDate;

				cbUserLevel.SelectedValuePath = "Key";
				cbUserLevel.DisplayMemberPath = "Value";
				foreach (var kvPair in Enumerable.Range(1, 9).ToDictionary(k => k, v => $"{UserSpecific.GetLevelInfo(v).Item1} {v}"))
				{
					cbUserLevel.Items.Add(kvPair);
					if (kvPair.Key == Item.UserSpecific.SrsNumeric) cbUserLevel.SelectedItem = kvPair;
				}
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
				kanji.Onyomi = tbOnyomiReading.Text;
				kanji.Kunyomi = tbKunyomiReading.Text;
				kanji.Nanori = tbNanoriReading.Text;
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

			if (Item.UserSpecific != null && (int)cbUserLevel.SelectedValue != Item.UserSpecific.SrsNumeric)
			{
				Item.UserSpecific.AddProgress((int)cbUserLevel.SelectedValue - Item.UserSpecific.SrsNumeric, chkResetTime.IsChecked.GetValueOrDefault());
			}

			DialogResult = true;
		}

		private string ToFormatString(TimeSpan timeSpan)
		{
			if (timeSpan.TotalMilliseconds <= 0) return "Now";

			var days = timeSpan.Days;
			var hours = timeSpan.Hours;
			var minutes = timeSpan.Minutes;
			var sb = new StringBuilder();

			if (days > 0) sb.Append($"{days}d, ");
			if (hours > 0 || days > 0) sb.Append($"{hours}h, ");
			sb.Append($"{minutes}m");

			return sb.ToString();
		}
	}
}
