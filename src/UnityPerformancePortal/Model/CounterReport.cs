namespace UnityPerformancePortal.Model
{
	public class CounterReport
	{
		public ReportEntryInfo Info { get; set; }
		public TimeInfo TimeAt { get; set; }
		public long Value { get; set; }
		public TimeInfo PrevAt { get; set; }
		public long Prev { get; set; }
	}
}