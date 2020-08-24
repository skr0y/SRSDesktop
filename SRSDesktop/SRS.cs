using SRSDesktop.Manager;
using SRSDesktop.Manager.ItemManager;
using SRSDesktop.Util;

namespace SRSDesktop
{
	public static class SRS
	{
		public static LessonManager LessonManager { get; private set; }
		public static ReviewManager ReviewManager { get; private set; }
		public static StatsManager StatsManager { get; private set; }

		static SRS()
		{
			var fullPath = Utils.GetResourcesPath();

			LessonManager = new LessonManager(fullPath);
			ReviewManager = new ReviewManager(fullPath);

			StatsManager = new StatsManager(fullPath);
		}
	}
}
