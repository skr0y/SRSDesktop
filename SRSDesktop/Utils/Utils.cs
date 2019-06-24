using System;
using System.IO;

namespace SRSDesktop.Utils
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
	}
}
