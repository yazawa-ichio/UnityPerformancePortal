using Unity.Profiling;

namespace UnityPerformancePortal
{
	public class RenderMonitor : MonitorModule
	{
		double m_SampleTimer;
		Gauge m_DrawCallGauge;
		ProfilerRecorder m_DrawCall;
		Gauge m_SetPassGauge;
		ProfilerRecorder m_SetPass;
		bool m_Dirty;

		public double SampleTime { get; private set; } = 60;

		public override bool TryInit()
		{
			m_DrawCall = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
			m_DrawCallGauge = new Gauge("DrawCall", DataUnit.Count);
			m_SetPass = ProfilerRecorder.StartNew(ProfilerCategory.Render, "SetPass Calls Count");
			m_SetPassGauge = new Gauge("SetPass", DataUnit.Count);
			return true;
		}

		protected override void Dispose(bool disposing)
		{
			m_DrawCall.Dispose();
			m_SetPass.Dispose();
		}

		public override void Update(double delta)
		{
			m_SampleTimer += delta;
			if (m_DrawCall.IsRunning)
			{
				m_Dirty = true;
				m_DrawCallGauge.Set(m_DrawCall.CurrentValueAsDouble);
			}
			if (m_SetPass.IsRunning)
			{
				m_Dirty = true;
				m_SetPassGauge.Set(m_SetPass.CurrentValueAsDouble);
			}
			if (m_SampleTimer > SampleTime)
			{
				Sample();
			}
		}

		public override void OnCollectReport()
		{
			Sample();
		}

		void Sample()
		{
			m_SampleTimer = 0;
			if (!m_Dirty)
			{
				return;
			}
			m_DrawCallGauge.Sample();
			m_SetPassGauge.Sample();
		}

	}
}