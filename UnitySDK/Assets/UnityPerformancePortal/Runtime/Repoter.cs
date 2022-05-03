using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityPerformancePortal.Report;

namespace UnityPerformancePortal
{
	public abstract class Repoter : IDisposable
	{

		ConcurrentQueue<CounterReport> m_CounterReports = new ConcurrentQueue<CounterReport>();
		ConcurrentQueue<GaugeReport> m_GaugeReports = new ConcurrentQueue<GaugeReport>();
		ConcurrentQueue<AverageReport> m_AverageReports = new ConcurrentQueue<AverageReport>();
		ConcurrentQueue<ElapsedTimeReport> m_ElapsedTimeReports = new ConcurrentQueue<ElapsedTimeReport>();

		public readonly Dictionary<string, string> Meta = new Dictionary<string, string>();

		ClientImpl m_Impl;
		ReportData m_Report = new ReportData();

		public string Id { get; private set; }

		public string SessionId { get; private set; } = System.Guid.NewGuid().ToString();

		protected Monitor Monitor => m_Impl.Monitor;

		public Repoter(string id)
		{
			Id = id;
			m_Report.ReporterId = id;
			m_Report.SesstionId = SessionId;
			m_Report.StartAt = TimeInfo.Now;
			Meta["version"] = Application.version;
			Meta["platform"] = Application.platform.ToString();
		}

		public void Post()
		{
			Monitor.OnCollectReport();
			Collect();
			var now = TimeInfo.Now;
			m_Report.EndAt = now;
			Send(m_Report);
			m_Report.Reset(now);
			foreach (var kvp in Meta)
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
			Collect();
			Update();
		}

		void Collect()
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