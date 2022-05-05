namespace UnityPerformancePortal
{
	public class FPSMonitor : MonitorModule
	{
		Average m_Average;
		double m_SampleTimer;

		public double SampleTime { get; private set; } = 60;

		public override bool TryInit()
		{
			m_Average = new Average("FPS", "FPS");
			return true;
		}

		public override void Update(double delta)
		{
			m_Average.Calc(1 / delta);
			m_SampleTimer += delta;
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
			m_Average.Sample();
		}

	}
}