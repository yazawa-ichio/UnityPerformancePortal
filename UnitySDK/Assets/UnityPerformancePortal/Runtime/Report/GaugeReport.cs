namespace UnityPerformancePortal.Report
{
	[System.Serializable]
	public readonly struct GaugeReport
	{
		public readonly ReportEntryInfo Info;
		public readonly double Value;
		public readonly TimeInfo TimeAt;

		public GaugeReport(ReportEntryInfo info, double value, TimeInfo timeAt)
		{
			Info = info;
			Value = value;
			TimeAt = timeAt;
		}
	}

}