using UnityPerformancePortal.Report;

namespace UnityPerformancePortal
{

	public class Gauge : System.IDisposable
	{
		public string Name { get; private set; }

		public bool Disposed { get; private set; }


		double m_Value;
		object m_Lock = new object();
		ReportEntryInfo m_Info;

		public Gauge(string name, DataUnit unit)
		{
			Name = name;
			m_Info = new ReportEntryInfo(name, unit);
		}

		public Gauge(string name, string unit)
		{
			Name = name;
			m_Info = new ReportEntryInfo(name, unit);
		}

		public void Set(double value)
		{
			lock (m_Lock)
			{
				m_Value = value;
			}
		}

		public void Add(double value)
		{
			lock (m_Lock)
			{
				Set(m_Value + value);
			}
		}

		public void Sub(double value)
		{
			Add(-value);
		}

		public void Inc()
		{
			Add(1);
		}

		public void Dec()
		{
			Add(-1);
		}


		public void Dispose()
		{
			Disposed = true;
		}


		public void Sample()
		{
			lock (m_Lock)
			{
				var report = new GaugeReport(m_Info, m_Value, TimeInfo.Now);
				if (Client.IsValid)
				{
					Client.Impl.Repoter.Add(in report);
				}
			}
		}

	}

}