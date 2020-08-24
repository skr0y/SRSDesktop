using SRSDesktop.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SRSDesktop.Windows
{
	public partial class StatsWindow : Window
	{
		private DateTime DayStart = DateTime.Now.Date;


		public StatsWindow()
		{
			InitializeComponent();
			SetTime(SRS.ConfigManager.Config.StatDayStart);
		}


		private void SetTime(int hour)
		{
			lblTime.Content = $"{hour:D2}:00";
			sldTime.Value = hour;

			DayStart = DateTime.Now.Date + new TimeSpan(hour, 00, 0);
			if (DayStart > DateTime.Now) DayStart = DayStart.AddDays(-1);

			UpdateStats();
		}

		private void UpdateStats()
		{
			var stats = SRS.StatsManager.LoadStats();

			var statLabels = new List<Tuple<DateInterval, string>>()
			{
				Tuple.Create(new DateInterval(DayStart), "lblToday"),
				Tuple.Create(new DateInterval(DayStart.AddDays(-1), DayStart), "lblYesterday"),
				Tuple.Create(new DateInterval(DayStart.AddDays(-6), DayStart), "lblWeek"),
				Tuple.Create(new DateInterval(DayStart.AddMonths(-1).AddDays(1), DayStart), "lblMonth"),
				Tuple.Create(new DateInterval(stats.Keys.First()), "lblAll")
			};

			foreach (var label in statLabels)
			{
				int correctTotal = 0, wrongTotal = 0, lessonTotal = 0;
				int reviewTime = 0, lessonTime = 0;

				foreach (var stat in stats.Where(p => label.Item1.IsInInterval(p.Key)))
				{
					if (stat.Value.IsLesson)
					{
						lessonTotal += stat.Value.Lessons;
						lessonTime += stat.Value.Time;
					}
					else
					{
						correctTotal += stat.Value.Correct;
						wrongTotal += stat.Value.Wrong;
						reviewTime += stat.Value.Time;
					}
				}

				var correctPercentage = correctTotal + wrongTotal == 0 ? 0 : (float)correctTotal / (correctTotal + wrongTotal);
				var reviewTimePerItem = correctTotal + wrongTotal == 0 ? 0 : reviewTime / (correctTotal + wrongTotal);
				((Label)FindName(label.Item2 + "ReviewsStat")).Content = $"{correctTotal} / {correctTotal + wrongTotal} ({correctPercentage * 100:N0}%)";
				((Label)FindName(label.Item2 + "ReviewsTime")).Content = $"{Utils.FormatTime(reviewTime)} ({reviewTimePerItem}s/item)";

				var lessonTimePerItem = lessonTotal == 0 ? 0 : lessonTime / lessonTotal;
				((Label)FindName(label.Item2 + "LessonsStat")).Content = $"{lessonTotal}";
				((Label)FindName(label.Item2 + "LessonsTime")).Content = $"{Utils.FormatTime(lessonTime)} ({lessonTimePerItem}s/item)";
			}
		}

		private void SliderTimeValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			SetTime((int)e.NewValue);
		}


		private class DateInterval
		{
			private DateTime StartDate;
			private DateTime EndDate;

			public DateInterval(DateTime startDate)
			{
				StartDate = startDate;
				EndDate = DateTime.Now;
			}

			public DateInterval(DateTime startDate, DateTime endDate)
			{
				StartDate = startDate;
				EndDate = endDate;
			}

			public bool IsInInterval(DateTime date)
			{
				return date >= StartDate && date <= EndDate;
			}
		}
	}
}
