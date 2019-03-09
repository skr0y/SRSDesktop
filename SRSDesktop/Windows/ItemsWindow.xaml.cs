using SRSDesktop.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SRSDesktop.Windows
{
	public partial class ItemsWindow : Window
	{
		private List<Item> Items { get; set; }
		private Item CurrentItem { get; set; }
		private ItemsWindowMode Mode { get; set; }

		public ItemsWindow()
		{
			InitializeComponent();
		}

		public ItemsWindow(HashSet<Item> items, ItemsWindowMode mode) : this()
		{
			Items = new List<Item>(items);
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
				return;
			}

			CurrentItem = Items.First();

			EnableInputControls();
			ClearInputControls();
			DisableAnswerControls();
			ClearAnswerTextBlocks();

			labelCharacter.Content = CurrentItem.Character;
		}

		private void DisplayAnswer()
		{
			EnableAnswerControls();
			DisableInputControls();
			FillAnswerTextBlocks(CurrentItem);
		}

		private void AcceptAnswer(int levelChange)
		{
			var newLevel = CurrentItem.UserSpecific.SrsNumeric + levelChange;

			CurrentItem.UserSpecific.SetLevel(newLevel);

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
			textBlockExamples.Text = "";
			textBlockMeaning.Text = "";
			textBlockMeaningHints.Text = "";
			textBlockReading.Text = "";
			textBlockReadingHints.Text = "";
		}

		private void FillAnswerTextBlocks(Item item)
		{
			var examples = "";
			var meaningHints = "";
			var reading = "";
			var readingHints = "";

			if (item is Radical radical)
			{
				meaningHints = radical.Mnemonic;
			}

			if (item is Kanji kanji)
			{
				meaningHints = kanji.MeaningMnemonic + Environment.NewLine + kanji.MeaningHint;
				reading = kanji.Reading;
				readingHints = kanji.ReadingMnemonic + Environment.NewLine + kanji.ReadingHint;
			}

			if (item is Vocab vocab)
			{
				examples = vocab.ContextSentences[0].Japanese + Environment.NewLine + vocab.ContextSentences[0].English;
				meaningHints = vocab.MeaningExplanation;
				reading = vocab.Kana;
				readingHints = vocab.ReadingExplanation;
			}

			textBlockExamples.Text = examples;
			textBlockMeaning.Text = item.Meaning;
			textBlockMeaningHints.Text = meaningHints;
			textBlockReading.Text = reading;
			textBlockReadingHints.Text = readingHints;
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

		#endregion
	}
}
