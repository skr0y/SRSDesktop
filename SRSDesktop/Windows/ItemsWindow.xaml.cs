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
		private readonly Brush correctBrush = (Brush)(new BrushConverter().ConvertFrom("#FFEEFFEE"));
		private readonly Brush wrongBrush = (Brush)(new BrushConverter().ConvertFrom("#FFFFEEEE"));
		private readonly Brush clearBrush = (Brush)(new BrushConverter().ConvertFrom("#FFFFFFFF"));

		private VorbisWaveReader VorbisWaveReader;
		private WaveOutEvent WaveOutEvent;

		private List<Item> Items { get; set; }
		private int TotalCount { get; set; }
		private Item CurrentItem { get; set; }
		private ItemsWindowMode Mode { get; set; }
		private State AppState { get; set; }

		public ItemsWindow()
		{
			InitializeComponent();
		}

		public ItemsWindow(List<Item> items, ItemsWindowMode mode) : this()
		{
			Items = items;
			TotalCount = Items.Count;
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
			if (Mode == ItemsWindowMode.Lesson)
			{
				DisplayAnswer();
				return;
			}

			AppState = State.AwaitInput;

			if (Items.Count == 0)
			{
				DialogResult = true;
				return;
			}

			CurrentItem = Items.First();

			SetWindowTitle();
			EnableInputControls();
			ClearInputControls();
			DisableAnswerControls();
			ClearAnswerTextBlocks();
			SetItemBackgroundColor();
			SetItemLvlInfo();

			if (CurrentItem.Character == null && CurrentItem is Radical radical)
			{
				var imageUrl = Utils.Utils.GetResourcesPath() + radical.Image;
				if (File.Exists(imageUrl))
				{
					var uri = new Uri(imageUrl, UriKind.Absolute);
					imageCharacter.Source = new BitmapImage(uri);
				}
				else
				{
					MessageBox.Show(radical.Image + " has no image");
				}
			}
			else
			{
				textBlockCharacter.Text = CurrentItem.Character;
				textBlockCharacter.FontSize = 150 / CurrentItem.Character.Length;
			}
		}

		private void DisplayAnswer()
		{
			AppState = State.DisplayAnswer;

			EnableAnswerControls();
			DisableInputControls();
			FillAnswerTextBlock(CurrentItem);
		}

		private void AcceptAnswer(int levelChange)
		{
			CurrentItem.UserSpecific.AddProgress(levelChange);

			if (CurrentItem.UserSpecific.Burned)
			{
				MessageBox.Show("BURNED!");
			}

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

		private void Answer()
		{
			DisplayAnswer();
		}

		private void PlaySound()
		{
			if (WaveOutEvent != null)
			{
				WaveOutEvent.Play();
				VorbisWaveReader.Position = 0;
			}
		}

		#region Controls

		private void SetWindowTitle()
		{
			Title = $"{Mode.ToString()} [{TotalCount - Items.Count + 1}/{TotalCount}]";
		}

		private void EnableInputControls()
		{
			buttonAnswer.IsEnabled = true;
			buttonSkip.IsEnabled = true;
		}

		private void DisableInputControls()
		{
			buttonAnswer.IsEnabled = false;
			buttonSkip.IsEnabled = false;
		}

		private void EnableAnswerControls()
		{
			if (Mode == ItemsWindowMode.Lesson)
			{
				buttonGood.IsEnabled = true;
				buttonGood.Content = $"Next";

				return;
			}

			var srsNumeric = CurrentItem.UserSpecific.SrsNumeric;

			if (CurrentItem.UserSpecific.SrsNumeric > 1)
			{
				buttonBad.IsEnabled = true;
				buttonBad.Content = $"Bad ({UserSpecific.GetLevelInfo(srsNumeric - 1).Item3})";
			}

			buttonAgain.IsEnabled = true;

			buttonOkay.IsEnabled = true;
			buttonOkay.Content = $"Okay ({UserSpecific.GetLevelInfo(srsNumeric).Item3})";

			buttonGood.IsEnabled = true;
			buttonGood.Content = $"Good ({UserSpecific.GetLevelInfo(srsNumeric + 1).Item3})";

			if (CurrentItem.UserSpecific.SrsNumeric < 7)
			{
				buttonEasy.IsEnabled = true;
				buttonEasy.Content = $"Easy ({UserSpecific.GetLevelInfo(srsNumeric + 2).Item3})";
			}
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
			textBlockCharacter.Text = "";
			imageCharacter.Source = null;
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
				runs = GenerateRuns("Meaning", radical.Meaning);
				textBlockInfo.Inlines.AddRange(runs);
				runs = GenerateRuns("Mnemonic", radical.Mnemonic);
				textBlockInfo.Inlines.AddRange(runs);
			}
			else if (item is Kanji kanji)
			{
				runs = GenerateRuns("Meaning", kanji.Meaning);
				textBlockInfo.Inlines.AddRange(runs);
				runs = GenerateRuns("Reading", kanji.Reading);
				textBlockInfo.Inlines.AddRange(runs);
				runs = GenerateRuns("Meaning hints", kanji.MeaningMnemonic);// + Environment.NewLine + kanji.MeaningHint);
				textBlockInfo.Inlines.AddRange(runs);
				runs = GenerateRuns("Reading hints", kanji.ReadingMnemonic);// + Environment.NewLine + kanji.ReadingHint);
				textBlockInfo.Inlines.AddRange(runs);
				runs = GenerateRuns("Examples", kanji.Examples);
				textBlockInfo.Inlines.AddRange(runs);
			}
			else if (item is Vocab vocab)
			{
				runs = GenerateRuns("Meaning", vocab.Meaning);
				textBlockInfo.Inlines.AddRange(runs);
				runs = GenerateRuns("Reading", vocab.Kana);
				textBlockInfo.Inlines.AddRange(runs);
				runs = GenerateRuns("Meaning explanation", vocab.MeaningExplanation);
				textBlockInfo.Inlines.AddRange(runs);
				runs = GenerateRuns("Reading explanation", vocab.ReadingExplanation);
				textBlockInfo.Inlines.AddRange(runs);
				runs = GenerateRuns("Context sentences", string.Join(Environment.NewLine + Environment.NewLine, vocab.ContextSentences.Select(cs => cs.Japanese + Environment.NewLine + cs.English)));
				textBlockInfo.Inlines.AddRange(runs);

				var soundPath = Utils.Utils.GetResourcesPath() + "Sound/" + item.Character + ".ogg";
				if (File.Exists(soundPath))
				{
					VorbisWaveReader = new VorbisWaveReader(soundPath);
					WaveOutEvent = new WaveOutEvent();
					WaveOutEvent.Init(VorbisWaveReader);
					PlaySound();
				}
			}
		}

		private void SetItemLvlInfo()
		{
			labelItemLvl.Content = CurrentItem.Level;

			var lvlInfo = UserSpecific.GetLevelInfo(CurrentItem.UserSpecific.SrsNumeric);
			labelUserLvl.Content = lvlInfo.Item1.ToString() + " " + CurrentItem.UserSpecific.SrsNumeric;
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

		private void ButtonAnswerClick(object sender, RoutedEventArgs e)
		{
			Answer();
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
			PlaySound();
		}

		private void ButtonDetailsClick(object sender, RoutedEventArgs e)
		{
			var detailsWindow = new DetailsWindow(CurrentItem);

			if (detailsWindow.ShowDialog() == true)
			{
				ClearAnswerTextBlocks();
				FillAnswerTextBlock(CurrentItem);
			}
		}

		private void WindowKeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				if (AppState == State.AwaitInput)
				{
					Answer();
				}
				else if (AppState == State.DisplayAnswer)
				{
					PlaySound();
				}
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


		private enum State
		{
			DisplayAnswer,
			AwaitInput
		}
	}

}
