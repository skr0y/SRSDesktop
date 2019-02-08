using System;
using System.IO;
using System.Windows;
using SRSDesktop.Manager;

namespace SRSDesktop
{
	public partial class MainWindow : Window
	{
		private const string path = @"/Resources/";

		public MainWindow()
		{
			InitializeComponent();
			LoadData();
		}

		private void LoadData()
		{
			var projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
			var fullPath = projectDirectory + path;

			var lessonManager = new LessonManager(fullPath);
			var reviewManager = new ReviewManager(fullPath);
		}
	}
}
