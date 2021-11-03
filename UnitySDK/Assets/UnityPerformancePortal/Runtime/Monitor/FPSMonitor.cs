using System;
namespace UnityPerformancePortal
{
	public class FPSMonitor : MonitorModule
	{
		Average m_Average;
		DateTime m_Prev;
		double m_SampleTimer;

		public double SampleTime { get; private set; } = 60;

		public override bool TryInit()
		{
			m_Average = new Average("FPS", "FPS");
			m_Prev = DateTime.UtcNow;
			return true;
		}

		public override void Update()
		{
			var now = DateTime.UtcNow;
			var delta = (now - m_Prev).TotalSeconds;
			m_Prev = now;
			if (delta < 0 || delta > 1)
			{
				return;
			}
			m_Average.Calc(1 / delta);
			m_SampleTimer += delta;
			if (m_SampleTimer > SampleTime)
			{
				Sample();
			}
		}

		public override void OnPostReport()
		{
			m_SampleTimer = 0;
			m_Average.Sample();
		}

		void Sample()
		{
			m_SampleTimer = 0;
			m_Average.Sample();
		}

	}
}