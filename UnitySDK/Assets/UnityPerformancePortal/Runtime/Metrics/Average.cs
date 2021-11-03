using UnityPerformancePortal.Report;

namespace UnityPerformancePortal
{

	public class Average : System.IDisposable
	{
		public string Name { get; private set; }

		public bool Disposed { get; private set; }

		public double Step
		{
			get => m_Step;
			set => m_Step = value;
		}

		bool m_Init;
		double m_Value;
		object m_Lock = new object();
		double m_Step = 0.1;
		double? m_MinValue;
		TimeInfo m_MinTimeAt;
		double? m_MaxValue;
		TimeInfo m_MaxTimeAt;
		ReportEntryInfo m_Info;

		public Average(string name, DataUnit unit)
		{
			Name = name;
			m_Info = new ReportEntryInfo(name, unit);
		}

		public Average(string name, string unit)
		{
			Name = name;
			m_Info = new ReportEntryInfo(name, unit);
		}

		public void Calc(double value)
		{
			lock (m_Lock)
			{
				if (!m_Init)
				{
					m_Init = true;
					m_MinValue = m_MaxValue = m_Value = value;
					m_MaxTimeAt = m_MinTimeAt = TimeInfo.Now;
					return;
				}
				m_Value = m_Value * (1 - m_Step) + value * m_Step;
				if (!m_MinValue.HasValue || m_MinValue.Value > m_Value)
				{
					m_MinValue = m_Value;
					m_MinTimeAt = TimeInfo.Now;
				}
				if (!m_MaxValue.HasValue || m_MaxValue.Value < m_Value)
				{
					m_MaxValue = m_Value;
					m_MaxTimeAt = TimeInfo.Now;
				}
			}
		}

		public void Dispose()
		{
			Disposed = true;
		}


		public void Sample()
		{
			lock (m_Lock)
			{
				if (m_MaxValue == null)
				{
					return;
				}
				var report = new AverageReport(m_Info, TimeInfo.Now, m_Value, m_MinTimeAt, m_MinValue.Value, m_MaxTimeAt, m_MaxValue.Value);
				m_MinValue = null;
				m_MaxValue = null;
				if (Client.IsValid)
				{
					Client.Impl.Repoter.Add(in report);
				}
			}
		}

	}

}