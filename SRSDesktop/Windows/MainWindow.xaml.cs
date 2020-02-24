using SRSDesktop.Entities;
using SRSDesktop.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SRSDesktop.Windows
{
	public partial class MainWindow : Window
	{
		private List<Item> nextHourItems;


		public MainWindow()
		{
			InitializeComponent();
			UpdateControls();
		}


		private void Update()
		{
			SRS.LessonManager.Update();
			UpdateControls();
		}

		private void UpdateControls()
		{
			nextHourItems = SRS.LessonManager.GetAll().FindAll(i => i.UserSpecific?.AvailableDate <= DateTime.Now.AddHours(1) && i.UserSpecific?.AvailableDate > DateTime.Now);

			if (sliderReviews != null)
			{
				sliderReviews.Maximum = SRS.ReviewManager.Count;
				sliderReviews.Value = sliderReviews.Maximum;
			}

			if (sliderLessons != null)
			{
				sliderLessons.Maximum = SRS.LessonManager.Count;
				sliderLessons.Value = sliderLessons.Maximum;
			}

			if (labelNextHour != null)
			{
				labelNextHour.Content = nextHourItems.Count;
				btnNextHourNow.IsEnabled = nextHourItems.Count > 0;
			}

			if (labelLevel != null)
			{
				labelLevel.Content = $"Level {SRS.LessonManager.UserLevel}";
			}

			if (progressBarRadicals != null)
			{
				var radicals = SRS.LessonManager.GetAll().OfType<Radical>().Where(r => r.Level == SRS.LessonManager.UserLevel);
				progressBarRadicals.Maximum = radicals.Count();
				progressBarRadicals.Value = radicals.Count(r => r.UserSpecific?.SrsNumeric >= SRS.LessonManager.UnlockLevel);
			}

			if (progressBarKanji != null)
			{
				var kanji = SRS.LessonManager.GetAll().OfType<Kanji>().Where(k => k.Level == SRS.LessonManager.UserLevel);
				progressBarKanji.Maximum = kanji.Count();
				progressBarKanji.Value = kanji.Count(r => r.UserSpecific?.SrsNumeric >= SRS.LessonManager.UnlockLevel);
			}

			if (progressBarVocabs != null)
			{
				var vocabs = SRS.LessonManager.GetAll().OfType<Vocab>().Where(v => v.Level == SRS.LessonManager.UserLevel);
				progressBarVocabs.Maximum = vocabs.Count();
				progressBarVocabs.Value = vocabs.Count(r => r.UserSpecific?.SrsNumeric >= SRS.LessonManager.UnlockLevel);
			}

			if (progressBar != null)
			{
				progressBar.Maximum = SRS.LessonManager.TotalCount;
				progressBar.Value = SRS.LessonManager.GetAll().Count(i => i.UserSpecific != null);
			}
		}

		#region Controls

		private void ReviewsButtonClick(object sender, RoutedEventArgs e)
		{
			var items = SRS.ReviewManager.Get((int)sliderReviews.Value, (OrderByAvailability)comboReviewsAvailability.SelectedValue, (OrderByType)comboReviewsType.SelectedValue);
			var reviewWindow = new ItemsWindow(items, ItemsWindowMode.Review);

			if (reviewWindow.ShowDialog() == true)
			{
				SRS.ReviewManager.Save();
				Update();
			}
		}

		private void LessonsButtonClick(object sender, RoutedEventArgs e)
		{
			var items = SRS.LessonManager.Get((int)sliderLessons.Value, (OrderByAvailability)comboLessonsAvailability.SelectedValue, (OrderByType)comboLessonsType.SelectedValue);
			var lessonWindow = new ItemsWindow(items, ItemsWindowMode.Lesson);

			if (lessonWindow.ShowDialog() == true)
			{
				SRS.LessonManager.Save();
				Update();
			}
		}

		private void ButtonUpdateClick(object sender, RoutedEventArgs e)
		{
			Update();
		}

		private void ButtonDatabaseClick(object sender, RoutedEventArgs e)
		{
			var databaseWindow = new DatabaseWindow();

			if (databaseWindow.ShowDialog() == true)
			{
				SRS.ReviewManager.Save();
				UpdateControls();
			}
		}

		private void SliderReviewsValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (buttonReviews != null)
			{
				buttonReviews.IsEnabled = e.NewValue > 0;
				buttonReviews.Content = $"Reviews ({e.NewValue})";
			}
		}

		private void SliderLessonsValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (buttonLessons != null)
			{
				buttonLessons.IsEnabled = e.NewValue > 0;
				buttonLessons.Content = $"Lessons ({e.NewValue})";
			}
		}

		private void ButtonDecReviewsSliderClick(object sender, RoutedEventArgs e)
		{
			if (sliderReviews != null)
			{
				sliderReviews.Value--;
			}
		}

		private void ButtonIncReviewsSliderClick(object sender, RoutedEventArgs e)
		{
			if (sliderReviews != null)
			{
				sliderReviews.Value++;
			}
		}

		private void ButtonDecLessonsSliderClick(object sender, RoutedEventArgs e)
		{
			if (sliderLessons != null)
			{
				sliderLessons.Value--;
			}
		}

		private void ButtonIncLessonsSliderClick(object sender, RoutedEventArgs e)
		{
			if (sliderLessons != null)
			{
				sliderLessons.Value++;
			}
		}

		private void BtnNextHourNowClick(object sender, RoutedEventArgs e)
		{
			nextHourItems.ForEach(i => i.UserSpecific.MakeAvailable());
			SRS.ReviewManager.Save();
			UpdateControls();
		}

		#endregion

		private void Window_Closed(object sender, EventArgs e)
		{
			Application.Current.Shutdown();
		}
	}
}
