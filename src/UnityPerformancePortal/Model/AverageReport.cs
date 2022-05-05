namespace UnityPerformancePortal.Model
{
	public class AverageReport
	{
		public ReportEntryInfo Info { get; set; }
		public TimeInfo TimeAt { get; set; }
		public double Value { get; set; }
		public TimeInfo MinTimeAt { get; set; }
		public double MinValue { get; set; }
		public TimeInfo MaxTimeAt { get; set; }
		public double MaxValue { get; set; }
	}
}