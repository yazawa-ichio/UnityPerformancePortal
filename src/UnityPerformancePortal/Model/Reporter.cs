using System;

namespace UnityPerformancePortal.Model
{
	public class Reporter
	{
		public string ReporterId { get; set; }
		public string SessionId { get; set; }
		public DateTime LastAt { get; set; }
	}
}