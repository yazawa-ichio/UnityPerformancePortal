namespace UnityPerformancePortal.Report
{
	[System.Serializable]
	public readonly struct AverageReport
	{
		public readonly ReportEntryInfo Info;
		public readonly TimeInfo TimeAt;
		public readonly double Value;
		public readonly TimeInfo MinTimeAt;
		public readonly double MinValue;
		public readonly TimeInfo MaxTimeAt;
		public readonly double MaxValue;

		public AverageReport(ReportEntryInfo info, TimeInfo timeAt, double value, TimeInfo minTimeAt, double minValue, TimeInfo maxTimeAt, double maxValue)
		{
			Info = info;
			TimeAt = timeAt;
			Value = value;
			MinTimeAt = minTimeAt;
			MinValue = minValue;
			MaxTimeAt = maxTimeAt;
			MaxValue = maxValue;
		}
	}

}