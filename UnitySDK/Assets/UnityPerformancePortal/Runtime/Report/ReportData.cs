using System.Collections.Generic;
using System.IO;

namespace UnityPerformancePortal.Report
{
	[System.Serializable]
	public class ReportData
	{
		public string ReporterId;
		public string SessionId;
		public TimeInfo StartAt;
		public TimeInfo EndAt;
		public Dictionary<string, string> Meta = new Dictionary<string, string>();
		public List<CounterReport> Counter = new List<CounterReport>(32);
		public List<GaugeReport> Gauge = new List<GaugeReport>(32);
		public List<AverageReport> Average = new List<AverageReport>(32);
		public List<ElapsedTimeReport> ElapsedTime = new List<ElapsedTimeReport>(32);

		internal void Reset(TimeInfo now)
		{
			StartAt = now;
			Meta.Clear();
			Counter.Clear();
			Gauge.Clear();
			Average.Clear();
			ElapsedTime.Clear();
		}

		public void CopyTo(ReportData report)
		{
			report.Reset(StartAt);
			report.ReporterId = ReporterId;
			report.EndAt = EndAt;
			foreach (var kvp in Meta)
			{
				report.Meta[kvp.Key] = kvp.Value;
			}
			report.Counter.AddRange(Counter);
			report.Gauge.AddRange(Gauge);
			report.Average.AddRange(Average);
			report.ElapsedTime.AddRange(ElapsedTime);
		}

		public void ToJson(Stream stream)
		{
			var writer = new StreamWriter(stream);
			var json = Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
			writer.Write(json);
			writer.Flush();
		}

	}
}