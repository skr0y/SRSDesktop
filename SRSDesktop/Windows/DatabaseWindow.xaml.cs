using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SRSDesktop.Entities;

namespace SRSDesktop.Windows
{
	public partial class DatabaseWindow : Window
	{
		private List<Item> items;

		public Item ReturnValue { get; set; }


		public DatabaseWindow()
		{
			// TODO:
			//  row color by type
			//  select by level & user level

			InitializeComponent();
			Update();
		}


		private void Update()
		{
			items = SRS.ReviewManager.GetAll();
			lsvDatabase.ItemsSource = items;
		}

		private void Filter()
		{
			if (lsvDatabase != null && items != null)
			{
				var result = items;

				if (tbSearch.Text != "") result = result.FindAll(i => i.Meaning.Contains(tbSearch.Text));

				if (chkRadicals.IsChecked == false) result = result.FindAll(i => !(i is Radical));
				if (chkKanji.IsChecked == false) result = result.FindAll(i => !(i is Kanji));
				if (chkVocab.IsChecked == false) result = result.FindAll(i => !(i is Vocab));

				if (radLearned.IsChecked == true) result = result.FindAll(i => i.UserSpecific != null);
				else if (radUnknown.IsChecked == true) result = result.FindAll(i => i.UserSpecific == null);

				lsvDatabase.ItemsSource = result;
			}
		}

		private void BtnConfirmClick(object sender, RoutedEventArgs e)
		{
			SRS.ReviewManager.Save();

			if (lsvDatabase.SelectedItem == null)
			{
				MessageBox.Show("Nothing's selected");
				return;
			}

			ReturnValue = (Item)lsvDatabase.SelectedItem;
			DialogResult = true;
		}

		private void LsvDatabaseMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var detailsWindow = new DetailsWindow((Item)lsvDatabase.SelectedItem);

			if (detailsWindow.ShowDialog() == true)
			{
				//Update();
			}
		}

		private void TextBoxTextChanged(object sender, TextChangedEventArgs e) => Filter();

		private void ChkRadicalsClick(object sender, RoutedEventArgs e) => Filter();

		private void ChkKanjiClick(object sender, RoutedEventArgs e) => Filter();

		private void ChkVocabClick(object sender, RoutedEventArgs e) => Filter();

		private void RadAllClick(object sender, RoutedEventArgs e) => Filter();

		private void RadLearnedClick(object sender, RoutedEventArgs e) => Filter();

		private void RadUnknownClick(object sender, RoutedEventArgs e) => Filter();
	}
}
