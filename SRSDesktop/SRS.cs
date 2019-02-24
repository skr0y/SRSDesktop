using SRSDesktop.Manager;
using System;
using System.IO;

namespace SRSDesktop
{
	public static class SRS
	{
		private const string resourcesPath = @"/Resources/";

		public static LessonManager LessonManager { get; private set; }
		public static ReviewManager ReviewManager { get; private set; }
		public static int TotalCount { get; }

		static SRS()
		{
			var projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
			var fullPath = projectDirectory + resourcesPath;

			LessonManager = new LessonManager(fullPath);
			ReviewManager = new ReviewManager(fullPath);
		}
	}
}
