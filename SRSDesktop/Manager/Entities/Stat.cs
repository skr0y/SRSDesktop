namespace SRSDesktop.Manager.Entities
{
	public class Stat
	{
		public bool IsLesson { get; }

		public int Lessons { get; }
		public int Correct { get; }
		public int Wrong { get; }
		public int Time { get; }


		private Stat(int time)
		{
			Time = time;
		}

		public Stat(int time, int lessons) : this(time)
		{
			IsLesson = true;
			Lessons = lessons;
		}

		public Stat(int time, int correct, int wrong) : this(time)
		{
			IsLesson = false;
			Correct = correct;
			Wrong = wrong;
		}
	}
}
