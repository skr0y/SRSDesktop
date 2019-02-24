using System.Windows;

namespace SRSDesktop
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
			if (sliderLessons != null)
			{
				sliderLessons.Maximum = SRS.LessonManager.Count;
				sliderLessons.Value = 20;
			}

			if (sliderReviews != null)
			{
				sliderReviews.Maximum = SRS.ReviewManager.Count;
				sliderReviews.Value = sliderReviews.Maximum;
			}

			if (progressBar != null)
			{
				var total = SRS.LessonManager.TotalCount;
				var learned = total - SRS.LessonManager.Count;
				progressBar.Value = (double)learned / total * 100;
			}
		}

		private void ReviewsButtonClick(object sender, RoutedEventArgs e)
		{

		}

		private void SliderReviewsValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (buttonReviews != null)
			{
				buttonReviews.Content = $"Reviews ({e.NewValue})";
			}
		}

		private void LessonsButtonClick(object sender, RoutedEventArgs e)
		{

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
	}
}
