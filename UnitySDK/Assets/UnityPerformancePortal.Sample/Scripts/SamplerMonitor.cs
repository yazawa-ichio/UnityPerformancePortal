using UnityPerformancePortal;

namespace UPP.Sample
{
	public class SamplerMonitor : MonitorModule
	{
		static ElapsedTimeDebugSampler s_Sampler = new ElapsedTimeDebugSampler("SamplerMonitor");
		Counter m_FrameCounter = new Counter("FrameCounter", DataUnit.Count);

		public override bool TryInit()
		{
			return true;
		}
		public override void Update(double delta)
		{
			m_FrameCounter.Inc();
		}

		public override void OnCollectReport()
		{
			using (s_Sampler.Scope("FrameCounter.Sample"))
			{
				m_FrameCounter.Sample();
			}
		}
	}
}