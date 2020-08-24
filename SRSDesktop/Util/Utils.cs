using SRSDesktop.Entities;
using SRSDesktop.Windows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

			var radicalBrush = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FFE9F4FF");
			var kanjiBrush = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FFFF9BFF");
			var vocabBrush = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FFC79BFF");

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

		public static void InvertColors(Bitmap bitmap)
		{
			var bitmapRead = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
			var bitmapLength = bitmapRead.Stride * bitmapRead.Height;
			var bitmapBGRA = new byte[bitmapLength];
			Marshal.Copy(bitmapRead.Scan0, bitmapBGRA, 0, bitmapLength);
			bitmap.UnlockBits(bitmapRead);

			for (int i = 0; i < bitmapLength; i += 4)
			{
				bitmapBGRA[i] = (byte)(255 - bitmapBGRA[i]);
				bitmapBGRA[i + 1] = (byte)(255 - bitmapBGRA[i + 1]);
				bitmapBGRA[i + 2] = (byte)(255 - bitmapBGRA[i + 2]);
			}

			var bitmapWrite = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
			Marshal.Copy(bitmapBGRA, 0, bitmapWrite.Scan0, bitmapLength);
			bitmap.UnlockBits(bitmapWrite);
		}

		public static BitmapImage ToBitmapImage(Bitmap bitmap)
		{
			using (var memory = new MemoryStream())
			{
				bitmap.Save(memory, ImageFormat.Png);
				memory.Position = 0;

				var bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.StreamSource = memory;
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.EndInit();
				bitmapImage.Freeze();

				return bitmapImage;
			}
		}

		public static string FormatTime(int seconds)
		{
			var minutes = seconds / 60;
			var result = minutes % 60 + "m";

			var hours = minutes / 60;
			if (hours > 0)
			{
				result = hours % 24 + "h, " + result;
			}

			var days = hours / 24;
			if (days > 0)
			{
				result = days + "d, " + result;
			}

			return result;
		}
	}
}
