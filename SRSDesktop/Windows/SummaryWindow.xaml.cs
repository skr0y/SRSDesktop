using SRSDesktop.Entities;
using SRSDesktop.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SRSDesktop.Windows
{
	public partial class SummaryWindow : Window
	{
		private const double margin = 20;


		public SummaryWindow()
		{
			InitializeComponent();
		}

		public SummaryWindow(IEnumerable<Item> badItems, IEnumerable<Item> goodItems) : this()
		{
			var widthProportion = badItems.Any() && goodItems.Any() ? 8 : 5;
			double marginTop = 5;

			double goodWidth = 0, badWidth = 0;
			List<Button> goodButtons = null, badButtons = null;

			if (goodItems.Count() > 0)
			{
				goodButtons = Utils.GenerateItemButtons(goodItems);
				goodWidth = CalculateWidth(goodButtons, widthProportion);
			}

			if (badItems.Count() > 0)
			{
				badButtons = Utils.GenerateItemButtons(badItems);
				badWidth = CalculateWidth(badButtons, widthProportion);
			}

			var width = Math.Max(goodWidth, badWidth);
			Width = Math.Max(width + margin * 2, Width);

			if (goodButtons != null)
			{
				lblCorrect.Visibility = Visibility.Visible;
				lblCorrect.Margin = new Thickness(lblCorrect.Margin.Left, marginTop, lblCorrect.Margin.Right, lblCorrect.Margin.Bottom);
				marginTop += 35;
				SetButtonsMargins(goodButtons, marginTop, width);
				//Height = Math.Max(resultHeight + margin * 2, Height);
				var resultHeight = goodButtons.Last().Margin.Top + goodButtons.Last().Height;
				Height = Math.Max(resultHeight + margin * 2 + 30, Height);
				goodButtons.ForEach(b => grid.Children.Add(b));
				marginTop += resultHeight;
			}

			if (badButtons != null)
			{
				lblWrong.Visibility = Visibility.Visible;
				lblWrong.Margin = new Thickness(lblWrong.Margin.Left, marginTop, lblWrong.Margin.Right, lblWrong.Margin.Bottom);
				marginTop += 35;
				SetButtonsMargins(badButtons, marginTop, width);
				Height = Math.Max(badButtons.Last().Margin.Top + badButtons.Last().Height + margin * 2 + 30, Height);
				badButtons.ForEach(b => grid.Children.Add(b));
			}
		}


		private void SetButtonsMargins(IEnumerable<Button> buttons, double marginTop, double width)
		{
			var currentWidth = margin;
			var offsetTop = marginTop;
			var offsetLeft = currentWidth;
			var maxWidth = width - margin * 2;
			var rowButtons = new List<Button>();
			foreach (var button in buttons)
			{
				if (currentWidth + button.Width > maxWidth)
				{
					var offset = (Width - margin * 2 - currentWidth) / 2;
					OffsetButtons(rowButtons, left: offset);
					rowButtons.Clear();
					offsetLeft -= currentWidth - margin;
					offsetTop += (int)button.Height + 5;
					currentWidth = margin;
				}
				rowButtons.Add(button);
				button.Margin = new Thickness(button.Margin.Left + offsetLeft, button.Margin.Top + offsetTop, button.Margin.Right, button.Margin.Bottom);
				currentWidth += (int)button.Width + 5;
			}
			var offset2 = (Width - margin * 2 - currentWidth) / 2;
			OffsetButtons(rowButtons, left: offset2);
		}

		private double CalculateWidth(IEnumerable<Button> buttons, int widthProportion)
		{
			var totalWidth = buttons.Last().Margin.Left + buttons.Last().Width;
			var height = buttons.First().Height + margin;
			var heightProportion = 3;
			var widthToHeightRatio = totalWidth / height;
			var x = Math.Sqrt(heightProportion) * Math.Sqrt(widthToHeightRatio) / Math.Sqrt(widthProportion);

			return totalWidth / x;
		}

		private void OffsetButtons(List<Button> buttons, double left = 0, double top = 0, double right = 0, double bottom = 0)
		{
			foreach (var button in buttons)
			{
				button.Margin = new Thickness(button.Margin.Left + left, button.Margin.Top + top, button.Margin.Right + right, button.Margin.Bottom + bottom);
			}
		}
	}
}
