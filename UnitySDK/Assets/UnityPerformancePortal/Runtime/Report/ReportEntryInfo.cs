namespace UnityPerformancePortal.Report
{
	[System.Serializable]
	public struct ReportEntryInfo
	{
		public string Name;
		public string Unit;

		public ReportEntryInfo(string name, DataUnit unit) : this()
		{
			Name = name;
			Unit = unit.GetUnitId();
		}

		public ReportEntryInfo(string name, string unit) : this()
		{
			Name = name;
			Unit = unit;
		}
	}
}