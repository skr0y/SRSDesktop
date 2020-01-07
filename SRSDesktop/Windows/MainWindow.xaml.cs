using SRSDesktop.Manager;
using System;
using System.Linq;
using System.Windows;

namespace SRSDesktop.Windows
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			UpdateControls();
		}


		private void UpdateControls()
		{
			if (sliderReviews != null)
			{
				sliderReviews.Maximum = SRS.ReviewManager.Count;
				sliderReviews.Value = Math.Min(50, sliderReviews.Maximum);
			}

			if (sliderLessons != null)
			{
				sliderLessons.Maximum = SRS.LessonManager.Count;
				sliderLessons.Value = 20;
			}

			if (progressBar != null)
			{
				var total = SRS.LessonManager.TotalCount;
				var learned = SRS.LessonManager.GetAll().Count(i => i.UserSpecific != null);

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
				SRS.ReviewManager.Update();
				UpdateControls();
			}
		}

		private void LessonsButtonClick(object sender, RoutedEventArgs e)
		{
			var items = SRS.LessonManager.Get((int)sliderLessons.Value, (OrderByAvailability)comboLessonsAvailability.SelectedValue, (OrderByType)comboLessonsType.SelectedValue);
			var lessonWindow = new ItemsWindow(items, ItemsWindowMode.Lesson);

			if (lessonWindow.ShowDialog() == true)
			{
				SRS.LessonManager.Save();
				SRS.LessonManager.Update();
				UpdateControls();
			}
		}

		private void ButtonUpdateClick(object sender, RoutedEventArgs e)
		{
			UpdateControls();
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

		#endregion

		private void Window_Closed(object sender, EventArgs e)
		{
			Application.Current.Shutdown();
		}
	}
}
