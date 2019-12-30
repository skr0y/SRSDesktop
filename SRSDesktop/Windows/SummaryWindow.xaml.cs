using SRSDesktop.Entities;
using SRSDesktop.Util;
using System.Collections.Generic;
using System.Windows;

namespace SRSDesktop.Windows
{
	public partial class SummaryWindow : Window
	{
		public SummaryWindow()
		{
			InitializeComponent();

			// TODO:
			//  labels
			//  center
		}

		public SummaryWindow(IEnumerable<Item> badItems, IEnumerable<Item> goodItems) : this()
		{
			var buttons = Utils.GenerateItemButtons(badItems);
			buttons.ForEach(b => grid.Children.Add(b));

			buttons = Utils.GenerateItemButtons(goodItems);
			buttons.ForEach(b => grid.Children.Add(b));
		}
	}
}
