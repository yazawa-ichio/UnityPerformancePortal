using System;

namespace UnityPerformancePortal.Driver.LiteDB
{
	internal class ReportDataRecord
	{
		public int Id { get; set; }
		public string ReporterId { get; set; }
		public string SessionId { get; set; }
		public DateTime StartAt { get; set; }
		public DateTime EndAt { get; set; }
		public string Payload { get; set; }
	}
}