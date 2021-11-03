using System.Threading;
using UnityPerformancePortal.Report;

namespace UnityPerformancePortal
{

	public class Counter : System.IDisposable
	{
		public string Name { get; private set; }
		public bool Disposed { get; private set; }

		long m_Value;
		long m_Prev;
		TimeInfo m_PrevAt;
		ReportEntryInfo m_Info;

		public Counter(string name, DataUnit unit)
		{
			Name = name;
			m_PrevAt = TimeInfo.Now;
			m_Info = new ReportEntryInfo(name, unit);
		}

		public Counter(string name, string unit)
		{
			Name = name;
			m_PrevAt = TimeInfo.Now;
			m_Info = new ReportEntryInfo(name, unit);
		}

		public void Inc()
		{
			Interlocked.Increment(ref m_Value);
			m_Value++;
		}

		public void Add(long value)
		{
			Interlocked.Add(ref m_Value, value);
		}

		public void Dispose()
		{
			Disposed = true;
		}

		public void Sample()
		{
			var value = m_Value;
			var now = TimeInfo.Now;
			var report = new CounterReport(m_Info, now, value, m_PrevAt, m_Prev);
			m_PrevAt = now;
			m_Prev = value;
			if (Client.IsValid)
			{
				Client.Impl.Repoter.Add(in report);
			}
		}

	}
}