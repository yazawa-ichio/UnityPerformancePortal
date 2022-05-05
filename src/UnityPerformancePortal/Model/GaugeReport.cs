namespace UnityPerformancePortal.Model
{
	[System.Serializable]
	public class GaugeReport
	{
		public ReportEntryInfo Info { get; set; }
		public double Value { get; set; }
		public TimeInfo TimeAt { get; set; }
	}
}