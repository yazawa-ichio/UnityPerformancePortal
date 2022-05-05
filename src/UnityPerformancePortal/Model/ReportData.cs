using System.Collections.Generic;

namespace UnityPerformancePortal.Model
{
	public class ReportData
	{
		public string ReporterId { get; set; }
		public string SessionId { get; set; }
		public TimeInfo StartAt { get; set; }
		public TimeInfo EndAt { get; set; }
		public Dictionary<string, string> Meta { get; set; }
		public CounterReport[] Counter { get; set; } = System.Array.Empty<CounterReport>();
		public GaugeReport[] Gauge { get; set; } = System.Array.Empty<GaugeReport>();
		public AverageReport[] Average { get; set; } = System.Array.Empty<AverageReport>();
		public ElapsedTimeReport[] ElapsedTime { get; set; } = System.Array.Empty<ElapsedTimeReport>();
	}
}