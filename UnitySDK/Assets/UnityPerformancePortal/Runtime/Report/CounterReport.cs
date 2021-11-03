namespace UnityPerformancePortal.Report
{
	[System.Serializable]
	public readonly struct CounterReport
	{
		public readonly ReportEntryInfo Info;
		public readonly TimeInfo TimeAt;
		public readonly long Value;
		public readonly TimeInfo PrevAt;
		public readonly long Prev;

		public CounterReport(ReportEntryInfo info, TimeInfo timeAt, long value, TimeInfo prevAt, long prev)
		{
			Info = info;
			TimeAt = timeAt;
			Value = value;
			PrevAt = prevAt;
			Prev = prev;
		}
	}
}