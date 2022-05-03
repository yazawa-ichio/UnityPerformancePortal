namespace UnityPerformancePortal.Driver.LiteDB
{
	internal class ReportDataRecord
	{
		public int Id { get; set; }
		public string ReporterId { get; set; }
		public string SessionId { get; set; }
		public long StartUnixMillseconds { get; set; }
		public long EndUnixMillseconds { get; set; }
		public string Payload { get; set; }
	}
}