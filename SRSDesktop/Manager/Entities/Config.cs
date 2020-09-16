namespace SRSDesktop.Manager.Entities
{
	public class Config
	{
		public int DefaultLessonSlider { get; private set; }
		public OrderByAvailability DefaultLessonOrderByAvailability { get; private set; }
		public OrderByType DefaultLessonOrderByType { get; private set; }

		public int DefaultReviewSlider { get; private set; }
		public OrderByAvailability DefaultReviewOrderByAvailability { get; private set; }
		public OrderByType DefaultReviewOrderByType { get; private set; }

		public int StatDayStart { get; private set; }

		public int AgainInterval { get; private set; }
		public int RandomizationPercent { get; internal set; }


		public Config(string configPath)
		{
			DefaultLessonSlider = 20;
			DefaultLessonOrderByAvailability = OrderByAvailability.None;
			DefaultLessonOrderByType = OrderByType.AscLevel;

			DefaultReviewSlider = 70;
			DefaultReviewOrderByAvailability = OrderByAvailability.Shuffle;
			DefaultReviewOrderByType = OrderByType.None;

			StatDayStart = 8;
			AgainInterval = 18 * 60;
			RandomizationPercent = 15;
		}
	}
}
