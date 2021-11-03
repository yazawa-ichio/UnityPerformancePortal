using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityPerformancePortal.Report;

namespace UnityPerformancePortal
{
	public abstract class Repoter : IDisposable
	{

		ConcurrentQueue<CounterReport> m_CounterReports = new ConcurrentQueue<CounterReport>();
		ConcurrentQueue<GaugeReport> m_GaugeReports = new ConcurrentQueue<GaugeReport>();
		ConcurrentQueue<AverageReport> m_AverageReports = new ConcurrentQueue<AverageReport>();
		ConcurrentQueue<ElapsedTimeReport> m_ElapsedTimeReports = new ConcurrentQueue<ElapsedTimeReport>();

		public readonly Dictionary<string, string> DefaultMeta = new Dictionary<string, string>();

		ClientImpl m_Impl;
		ReportData m_Report = new ReportData();

		protected Monitor Monitor => m_Impl.Monitor;

		public Repoter()
		{
			m_Report.StartAt = TimeInfo.Now;
		}

		public void Post()
		{
			Monitor.OnPostReport();
			DoUpdate();
			var now = TimeInfo.Now;
			m_Report.EndAt = now;
			Send(m_Report);
			m_Report.Reset(now);
			foreach (var kvp in DefaultMeta)
			{
				m_Report.Meta[kvp.Key] = kvp.Value;
			}
		}

		public abstract void Init(Action success, Action<Exception> error);

		protected abstract void Send(ReportData report);

		protected virtual void Update() { }

		public virtual void Dispose() { }

		internal void Setup(ClientImpl impl)
		{
			m_Impl = impl;
		}

		internal void DoUpdate()
		{
			while (m_CounterReports.TryDequeue(out var counter))
			{
				m_Report.Counter.Add(counter);
			}
			while (m_GaugeReports.TryDequeue(out var gauge))
			{
				m_Report.Gauge.Add(gauge);
			}
			while (m_AverageReports.TryDequeue(out var average))
			{
				m_Report.Average.Add(average);
			}
			var count = m_ElapsedTimeReports.Count;
			for (int i = 0; i < count; i++)
			{
				if (m_ElapsedTimeReports.TryDequeue(out var elapsedTime))
				{
					m_Report.ElapsedTime.Add(elapsedTime);
				}
			}
			Update();
		}

		internal void Add(in CounterReport report)
		{
			m_CounterReports.Enqueue(report);
		}

		internal void Add(in GaugeReport report)
		{
			m_GaugeReports.Enqueue(report);
		}

		internal void Add(in AverageReport report)
		{
			m_AverageReports.Enqueue(report);
		}

		internal void Add(in ElapsedTimeReport report)
		{
			m_ElapsedTimeReports.Enqueue(report);
		}

	}
}