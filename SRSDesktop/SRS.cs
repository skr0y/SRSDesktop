using SRSDesktop.Manager;

namespace SRSDesktop
{
	public static class SRS
	{
		public static LessonManager LessonManager { get; private set; }
		public static ReviewManager ReviewManager { get; private set; }

		static SRS()
		{
			var fullPath = Utils.Utils.GetResourcesPath();

			LessonManager = new LessonManager(fullPath);
			ReviewManager = new ReviewManager(fullPath);
		}
	}
}
