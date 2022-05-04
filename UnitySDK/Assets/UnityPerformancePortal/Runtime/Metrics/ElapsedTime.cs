using System.Collections.Generic;

namespace UnityPerformancePortal
{
	public interface IElapsedTimeSampler
	{
		string Category { get; }
		bool Enabled { get; set; }
	}

	public static partial class ElapsedTime
	{
		static Dictionary<string, IElapsedTimeSampler> s_Sampler = new Dictionary<string, IElapsedTimeSampler>();

		public static readonly ElapsedTimeTraceSampler Trace;
		public static readonly ElapsedTimeDebugSampler Debug;
		public static readonly ElapsedTimeInfoSampler Info;

		static ElapsedTime()
		{
			Trace = new ElapsedTimeTraceSampler("");
			Debug = new ElapsedTimeDebugSampler("");
			Info = new ElapsedTimeInfoSampler("");
		}

		internal static void Add(IElapsedTimeSampler sampler)
		{
			if (sampler.Category == "")
			{
				return;
			}
			lock (s_Sampler)
			{
				s_Sampler[sampler.Category] = sampler;
			}
		}

		public static void SetEnabled(string category, bool enabled)
		{
			lock (s_Sampler)
			{
				if (s_Sampler.TryGetValue(category, out var sampler))
				{
					sampler.Enabled = enabled;
				}
			}
		}

	}

}