using SRSDesktop.Entities;
using SRSDesktop.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SRSDesktop.Util
{
	public static class Utils
	{
		public static string GetApplicationPath()
		{
			return Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
		}

		public static string GetResourcesPath()
		{
			return GetApplicationPath() + @"\Resources\";
		}

		public static List<Button> GenerateItemButtons(IEnumerable<Item> items, double maxWidth = -1)
		{
			var result = new List<Button>();

			var radicalBrush = (Brush)new BrushConverter().ConvertFrom("#FFE9F4FF");
			var kanjiBrush = (Brush)new BrushConverter().ConvertFrom("#FFFF9BFF");
			var vocabBrush = (Brush)new BrushConverter().ConvertFrom("#FFC79BFF");

			var marginX = 0;
			
			foreach (var item in items)
			{
				var button = new Button();

				button.Content = item.Character ?? item.Meaning;
				button.ToolTip = item.Meaning;
				button.Margin = new Thickness(marginX, 0, 0, 0);
				button.Width = 10 + (item.Character?.Length * 2 ?? item.Meaning.Length) * 10;
				button.Height = 30;
				button.VerticalAlignment = VerticalAlignment.Top;
				button.HorizontalAlignment = HorizontalAlignment.Left;
				button.FontSize = 18;
				button.Background = item is Radical ? radicalBrush : item is Kanji ? kanjiBrush : vocabBrush;
				button.Click += (s, e) => new DetailsWindow(item).ShowDialog();

				result.Add(button);

				marginX += (int)button.Width + 5;
				if (maxWidth > 0 && marginX > maxWidth) break;
			}

			return result;
		}
	}
}
