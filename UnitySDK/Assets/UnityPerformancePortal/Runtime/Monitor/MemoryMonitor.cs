using UnityEngine.Profiling;
namespace UnityPerformancePortal
{
	public class MemoryMonitor : MonitorModule
	{
		Average m_Average;
		double m_SampleTimer;

		public double SampleTime { get; private set; } = 60 * 5;

		public override bool TryInit()
		{
			m_Average = new Average("Memory", DataUnit.Bytes);
			return true;
		}

		public override void Update(double delta)
		{
			m_SampleTimer += delta;
			if (m_SampleTimer > SampleTime)
			{
				Sample();
			}
		}

		public override void OnCollectReport()
		{
			m_SampleTimer = 0;
			m_Average.Calc(Profiler.GetTotalAllocatedMemoryLong());
			m_Average.Sample();
		}

		void Sample()
		{
			m_SampleTimer = 0;
			m_Average.Calc(Profiler.GetTotalAllocatedMemoryLong());
			m_Average.Sample();
		}

	}

}