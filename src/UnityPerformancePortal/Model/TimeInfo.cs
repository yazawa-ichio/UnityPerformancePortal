using System;

namespace UnityPerformancePortal.Model
{
	public class TimeInfo
	{
		public int FrameCount { get; set; }
		public float Time { get; set; }
		public long UnixMillseconds { get; set; }

		public DateTime GetUtcTime() => DateTimeOffset.FromUnixTimeMilliseconds(UnixMillseconds).UtcDateTime;
	}

}