using System;

namespace SRSDesktop.Manager
{
	[Flags]
	public enum ManagerOptions
	{
		Default = 0,
		Older = 1,
		Recent = 2,
		Shuffle = 4
	}
}