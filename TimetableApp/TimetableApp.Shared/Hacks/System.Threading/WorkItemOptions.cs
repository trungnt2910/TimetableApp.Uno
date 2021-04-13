using System;

namespace TimetableApp.Hacks.System.Threading
{
	[Flags]
	public enum WorkItemOptions
	{
		None = 0,
		TimeSliced = 1
	}
}
