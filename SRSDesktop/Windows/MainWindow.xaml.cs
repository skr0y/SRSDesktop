using SRSDesktop.Manager;
using System;
using System.Windows;

namespace SRSDesktop.Windows
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			LoadData();
		}

		private void LoadData()
		{
			SRS.LessonManager.Update();
			SRS.ReviewManager.Update();

			if (sliderLessons != null)
			{
				sliderLessons.Maximum = SRS.LessonManager.Count;
				sliderLessons.Value = 20;
			}

			if (sliderReviews != null)
			{
				sliderReviews.Maximum = SRS.ReviewManager.Count;
				sliderReviews.Value = Math.Min(50, sliderReviews.Maximum);
			}

			if (progressBar != null)
			{
				var total = SRS.LessonManager.TotalCount;
				var learned = total - SRS.LessonManager.Count;

				progressBar.Value = (double)learned / total * 100;
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
				LoadData();
			}
		}

		private void LessonsButtonClick(object sender, RoutedEventArgs e)
		{
			var items = SRS.LessonManager.Get((int)sliderLessons.Value, (OrderByAvailability)comboLessonsAvailability.SelectedValue, (OrderByType)comboLessonsType.SelectedValue);
			var lessonWindow = new ItemsWindow(items, ItemsWindowMode.Lesson);

			if (lessonWindow.ShowDialog() == true)
			{
				SRS.LessonManager.Save();
				LoadData();
			}
		}

		private void SliderReviewsValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (buttonReviews != null)
			{
				buttonReviews.Content = $"Reviews ({e.NewValue})";
			}
		}

		private void SliderLessonsValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (buttonLessons != null)
			{
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

		private void ButtonUpdateClick(object sender, RoutedEventArgs e)
		{
			LoadData();
		}

		private void ButtonDatabaseClick(object sender, RoutedEventArgs e)
		{
			var databaseWindow = new DatabaseWindow();

			if (databaseWindow.ShowDialog() == true)
			{
				SRS.ReviewManager.Save();
			}
		}

		#endregion
	}
}
