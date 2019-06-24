using NAudio.Vorbis;
using NAudio.Wave;
using SRSDesktop.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SRSDesktop.Windows
{
	public partial class ItemsWindow : Window
	{
		private readonly Brush radicalBrush = (Brush)(new BrushConverter().ConvertFrom("#FFE9F4FF"));
		private readonly Brush kanjiBrush = (Brush)(new BrushConverter().ConvertFrom("#FFFF9BFF"));
		private readonly Brush vocabBrush = (Brush)(new BrushConverter().ConvertFrom("#FFC79BFF"));
		private readonly Brush correctBrush = (Brush)(new BrushConverter().ConvertFrom("#FFF4FFF4"));
		private readonly Brush wrongBrush = (Brush)(new BrushConverter().ConvertFrom("#FFFFF4F4"));
		private readonly Brush clearBrush = (Brush)(new BrushConverter().ConvertFrom("#FFFFFFFF"));

		private VorbisWaveReader VorbisWaveReader;
		private WaveOutEvent WaveOutEvent;

		private List<Item> Items { get; set; }
		private Item CurrentItem { get; set; }
		private ItemsWindowMode Mode { get; set; }

		public ItemsWindow()
		{
			InitializeComponent();
		}

		public ItemsWindow(List<Item> items, ItemsWindowMode mode) : this()
		{
			Items = items;
			Mode = mode;

			switch (Mode)
			{
				case ItemsWindowMode.Lesson:
					DisplayAnswer();
					break;
				case ItemsWindowMode.Review:
					WaitForInput();
					break;
				case ItemsWindowMode.View:
					throw new NotImplementedException("View mode is not implemented");
				default:
					throw new ArgumentException("Unsupported ItemsWindowMode");
			}
		}

		private void WaitForInput()
		{
			if (Items.Count == 0)
			{
				DialogResult = true;
				return;
			}

			CurrentItem = Items.First();

			EnableInputControls();
			ClearInputControls();
			DisableAnswerControls();
			ClearAnswerTextBlocks();
			SetItemBackgroundColor();
			ClearAnswerBackgroundColor();

			labelItemLvl.Content = CurrentItem.Level;
			labelUserLvl.Content = CurrentItem.UserSpecific.SrsNumeric;

			textBlockCharacter.Text = CurrentItem.Character;
			textBlockCharacter.FontSize = 150 / CurrentItem.Character.Length;

			if (CurrentItem.Character == null && CurrentItem is Radical radical)
			{
				var uri = new Uri(Utils.Utils.GetResourcesPath() + radical.Image, UriKind.Absolute);
				imageCharacter.Source = new BitmapImage(uri);
			}
		}

		private void DisplayAnswer()
		{
			EnableAnswerControls();
			DisableInputControls();
			FillAnswerTextBlock(CurrentItem);
			SetAnswerBackgroudColor(IsCorrectAnswer());
		}

		private void AcceptAnswer(int levelChange)
		{
			CurrentItem.UserSpecific.AddLevel(levelChange);

			Items.Remove(CurrentItem);

			WaitForInput();
		}

		private void SkipAnswer(bool readd = false)
		{
			Items.Remove(CurrentItem);

			if (readd)
			{
				Items.Add(CurrentItem);
			}

			WaitForInput();
		}

		private bool IsCorrectAnswer()
		{
			//if (CurrentItem is Kanji kanji && !textBoxReadingInput.Text.Equals(kanji.Reading, StringComparison.InvariantCultureIgnoreCase))
			//	return false;
			//else if (CurrentItem is Vocab vocab && !textBoxReadingInput.Text.Equals(vocab.Kana, StringComparison.InvariantCultureIgnoreCase))
			//	return false;

			if (!CurrentItem.Meanings.Contains(textBoxMeaningInput.Text, StringComparer.InvariantCultureIgnoreCase))
				return false;

			return true;
		}

		#region Controls

		private void EnableInputControls()
		{
			textBoxMeaningInput.IsEnabled = true;
			textBoxReadingInput.IsEnabled = true;
			buttonAnswer.IsEnabled = true;
			buttonSkip.IsEnabled = true;
		}

		private void DisableInputControls()
		{
			textBoxMeaningInput.IsEnabled = false;
			textBoxReadingInput.IsEnabled = false;
			buttonAnswer.IsEnabled = false;
			buttonSkip.IsEnabled = false;
		}

		private void EnableAnswerControls()
		{
			buttonBad.IsEnabled = true;
			buttonAgain.IsEnabled = true;
			buttonOkay.IsEnabled = true;
			buttonGood.IsEnabled = true;
			buttonEasy.IsEnabled = true;
		}

		private void DisableAnswerControls()
		{
			buttonBad.IsEnabled = false;
			buttonAgain.IsEnabled = false;
			buttonOkay.IsEnabled = false;
			buttonGood.IsEnabled = false;
			buttonEasy.IsEnabled = false;
		}

		private void ClearInputControls()
		{
			textBoxMeaningInput.Text = "";
			textBoxReadingInput.Text = "";
		}

		private void ClearAnswerTextBlocks()
		{
			textBlockInfo.Text = "";
			scrollViewer.ScrollToTop();

			VorbisWaveReader?.Dispose();
			WaveOutEvent?.Dispose();
			VorbisWaveReader = null;
			WaveOutEvent = null;
		}

		private void FillAnswerTextBlock(Item item)
		{
			List<Run> runs;

			if (item is Radical radical)
			{
				runs = GenerateRuns("Mnemonic", radical.Mnemonic);
				textBlockInfo.Inlines.AddRange(runs);
			}
			else if (item is Kanji kanji)
			{
				runs = GenerateRuns("Examples", kanji.Examples);
				textBlockInfo.Inlines.AddRange(runs);
				runs = GenerateRuns("Meaning hints", kanji.MeaningMnemonic + Environment.NewLine + kanji.MeaningHint);
				textBlockInfo.Inlines.AddRange(runs);
				runs = GenerateRuns("Reading", kanji.Reading);
				textBlockInfo.Inlines.AddRange(runs);
				runs = GenerateRuns("Reading hints", kanji.ReadingMnemonic + Environment.NewLine + kanji.ReadingHint);
				textBlockInfo.Inlines.AddRange(runs);
			}
			else if (item is Vocab vocab)
			{
				runs = GenerateRuns("Context sentences", string.Join(Environment.NewLine, vocab.ContextSentences.Select(cs => cs.Japanese + Environment.NewLine + cs.English)));
				textBlockInfo.Inlines.AddRange(runs);
				runs = GenerateRuns("Meaning explanation", vocab.MeaningExplanation);
				textBlockInfo.Inlines.AddRange(runs);
				runs = GenerateRuns("Reading", vocab.Kana);
				textBlockInfo.Inlines.AddRange(runs);
				runs = GenerateRuns("Reading explanation", vocab.ReadingExplanation);
				textBlockInfo.Inlines.AddRange(runs);
			}

			var soundPath = Utils.Utils.GetResourcesPath() + "Sound/" + item.Character + ".ogg";
			if (File.Exists(soundPath))
			{
				VorbisWaveReader = new VorbisWaveReader(soundPath);
				WaveOutEvent = new WaveOutEvent();
				WaveOutEvent.Init(VorbisWaveReader);
			}
		}

		private void SetItemBackgroundColor()
		{
			if (CurrentItem is Radical)
				rectItem.Fill = radicalBrush;
			else if (CurrentItem is Kanji)
				rectItem.Fill = kanjiBrush;
			else if (CurrentItem is Vocab)
				rectItem.Fill = vocabBrush;
		}

		private void SetAnswerBackgroudColor(bool correct)
		{
			rectAnswer.Fill = correct ? correctBrush : wrongBrush;
		}

		private void ClearAnswerBackgroundColor()
		{
			rectAnswer.Fill = clearBrush;
		}

		private void ButtonAnswerClick(object sender, RoutedEventArgs e)
		{
			DisplayAnswer();
		}

		private void ButtonSkipClick(object sender, RoutedEventArgs e)
		{
			SkipAnswer();
		}

		private void ButtonAgainClick(object sender, RoutedEventArgs e)
		{
			SkipAnswer(true);
		}

		private void ButtonBadClick(object sender, RoutedEventArgs e)
		{
			AcceptAnswer(-1);
		}

		private void ButtonOkayClick(object sender, RoutedEventArgs e)
		{
			AcceptAnswer(0);
		}

		private void ButtonGoodClick(object sender, RoutedEventArgs e)
		{
			AcceptAnswer(1);
		}

		private void ButtonEasyClick(object sender, RoutedEventArgs e)
		{
			AcceptAnswer(2);
		}

		private void CharacterClick(object sender, MouseButtonEventArgs e)
		{
			if (WaveOutEvent != null)
			{
				WaveOutEvent.Play();
				VorbisWaveReader.Position = 0;
			}
		}

		#endregion

		private List<Run> GenerateRuns(string label, string text)
		{
			var result = new List<Run>();

			label = label + Environment.NewLine;
			text = text + Environment.NewLine + Environment.NewLine;

			var run = new Run(label);
			run.Foreground = Brushes.Gray;
			result.Add(run);

			result.Add(new Run(text));

			return result;
		}
	}
}
