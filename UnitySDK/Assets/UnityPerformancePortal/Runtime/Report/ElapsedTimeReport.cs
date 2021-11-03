namespace UnityPerformancePortal.Report
{
	[System.Serializable]
	public readonly struct ElapsedTimeReport
	{
		public readonly string Category;
		public readonly string Name;
		public readonly string Lavel;
		public readonly TimeInfo StartAt;
		public readonly TimeInfo EndAt;

		public ElapsedTimeReport(string category, string name, string lavel, TimeInfo startAt, TimeInfo endAt)
		{
			Category = category;
			Name = name;
			Lavel = lavel;
			StartAt = startAt;
			EndAt = endAt;
		}
	}
}