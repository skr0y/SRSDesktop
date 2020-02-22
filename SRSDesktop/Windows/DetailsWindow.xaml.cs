using NAudio.Vorbis;
using NAudio.Wave;
using SRSDesktop.Entities;
using SRSDesktop.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace SRSDesktop.Windows
{
	public partial class DetailsWindow : Window
	{
		private Item Item;
		private bool Unlock = false;
		private bool AvailableNow = false;
		private VorbisWaveReader VorbisWaveReader;
		private WaveOutEvent WaveOutEvent;

		public List<Item> ToEnqueue;


		public DetailsWindow()
		{
			InitializeComponent();
		}

		public DetailsWindow(Item item) : this()
		{
			Item = item;
			ToEnqueue = new List<Item>();
			FillDetails();
		}


		private void FillDetails()
		{
			var allItems = SRS.ReviewManager.GetAll();

			Title = Item.Character ?? Item.Meaning;
			tbCharacter.Text = Item.Character;
			tbMeaning.Text = Item.Meaning;
			lblLevel.Content = Item.Level;
			chkLearnable.IsChecked = Item.Learnable;

			if (Item is Radical radical)
			{
				tbMeaningExplanation.Text = radical.Mnemonic;
				tbReading.Visibility = Visibility.Hidden;
				tbReadingExplanation.Visibility = Visibility.Hidden;
				lblRelated.Margin = new Thickness(lblRelated.Margin.Left, lblRelated.Margin.Top - 145, lblRelated.Margin.Right, lblRelated.Margin.Bottom);
				scrRelated.Margin = new Thickness(scrRelated.Margin.Left, scrRelated.Margin.Top - 145, scrRelated.Margin.Right, scrRelated.Margin.Bottom);
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

				var soundPath = Utils.GetResourcesPath() + "Sound/" + Item.Character + "_2.ogg";
				if (File.Exists(soundPath))
				{
					VorbisWaveReader = new VorbisWaveReader(soundPath);
					WaveOutEvent = new WaveOutEvent();
					WaveOutEvent.Init(VorbisWaveReader);
				}
				else
				{
					btnSound.IsEnabled = false;
				}

				btnSound.Visibility = Visibility.Visible;
				tbCharacter.Width = 149;

				var buttons = Utils.GenerateItemButtons(vocab.Related, grdRelated.Width);
				buttons.ForEach(btn => grdRelated.Children.Add(btn));
			}

			if (Item.UserSpecific != null)
			{
				btnUnlock.Visibility = Visibility.Hidden;
				lblTime.Content = ToFormatString(Item.UserSpecific.AvailableDate - DateTime.Now);
				lblTime.ToolTip = Item.UserSpecific.AvailableDate;
				lblUnlocked.Content = Item.UserSpecific.UnlockedDate;
				chkLearnable.Visibility = Visibility.Hidden;
				if ((string)lblTime.Content == "Now") btnNow.Visibility = Visibility.Hidden;

				cbUserLevel.SelectedValuePath = "Key";
				cbUserLevel.DisplayMemberPath = "Value";
				foreach (var kvPair in Enumerable.Range(1, 9).ToDictionary(k => k, v => $"{UserSpecific.GetLevelInfo(v).Item1} {v}"))
				{
					cbUserLevel.Items.Add(kvPair);
					if (kvPair.Key == Item.UserSpecific.SrsNumeric) cbUserLevel.SelectedItem = kvPair;
				}
			}
			else
			{
				lblTime.Visibility = Visibility.Hidden;
				lblTimeText.Visibility = Visibility.Hidden;
				lblUnlocked.Visibility = Visibility.Hidden;
				lblUnlockedText.Visibility = Visibility.Hidden;
				btnNow.Visibility = Visibility.Hidden;
				cbUserLevel.IsEnabled = false;
				chkResetTime.IsEnabled = false;
			}
		}

		private void BtnSaveClick(object sender, RoutedEventArgs e)
		{
			Item.Character = tbCharacter.Text;
			Item.Learnable = chkLearnable.IsChecked.GetValueOrDefault();

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
			}
			else if (Item is Vocab vocab)
			{
				vocab.Meaning = tbMeaning.Text;
				vocab.Kana = tbReading.Text;
				vocab.MeaningExplanation = tbMeaningExplanation.Text;
				vocab.ReadingExplanation = tbReadingExplanation.Text;
			}

			if (Item.UserSpecific != null && (int)cbUserLevel.SelectedValue != Item.UserSpecific.SrsNumeric)
			{
				Item.UserSpecific.AddProgress((int)cbUserLevel.SelectedValue - Item.UserSpecific.SrsNumeric, chkResetTime.IsChecked.GetValueOrDefault());
			}

			if (Unlock)
			{
				Item.UserSpecific = new UserSpecific(Item);
			}

			if (AvailableNow)
			{
				Item.UserSpecific.MakeAvailable();
			}

			if (chkQueue.IsChecked == true)
			{
				ToEnqueue.Add(Item);
			}

			DialogResult = true;
		}

		private void BtnUnlockClick(object sender, RoutedEventArgs e)
		{
			Unlock = true;

			btnUnlock.Visibility = Visibility.Hidden;
			lblTime.Visibility = Visibility.Visible;
			lblTime.Content = "Now";
			lblTimeText.Visibility = Visibility.Visible;
			lblUnlocked.Visibility = Visibility.Visible;
			lblUnlocked.Content = DateTime.Now;
			lblUnlockedText.Visibility = Visibility.Visible;
			cbUserLevel.IsEnabled = true;
			chkResetTime.IsEnabled = true;
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

		private void BtnSoundClick(object sender, RoutedEventArgs e)
		{
			if (WaveOutEvent != null)
			{
				WaveOutEvent.Play();
				VorbisWaveReader.Position = 0;
			}
		}

		private void ScrRelatedPreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
		{
			var scrollviewer = sender as ScrollViewer;
			if (e.Delta > 0) scrollviewer.LineLeft();
			else scrollviewer.LineRight();
			e.Handled = true;
		}

		private void BtnNowClick(object sender, RoutedEventArgs e)
		{
			AvailableNow = true;

			lblTime.Content = "Now";
			btnNow.IsEnabled = false;
		}
	}
}
