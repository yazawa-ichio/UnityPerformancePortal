using System;

namespace UnityPerformancePortal
{
	[System.Serializable]
	public readonly struct TimeInfo
	{
		static int s_FrameCount;
		static float s_Time;
		public static TimeInfo Now => new TimeInfo(s_FrameCount, s_Time, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

		internal static void UpdateFrame()
		{
			s_FrameCount = UnityEngine.Time.frameCount;
			s_Time = UnityEngine.Time.realtimeSinceStartup;
		}

		public readonly int FrameCount;
		public readonly float Time;
		public readonly long UnixMillseconds;

		public TimeInfo(int frameCount, float time, long unixMillseconds)
		{
			FrameCount = frameCount;
			Time = time;
			UnixMillseconds = unixMillseconds;
		}

	}

}