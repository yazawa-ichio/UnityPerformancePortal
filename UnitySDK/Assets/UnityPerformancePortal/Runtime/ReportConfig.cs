using System;

namespace UnityPerformancePortal
{
	[Serializable]
	public class ReportConfig
	{
		public string Token;
		public string ReportUrl;
		public int Interval = 60;
		public bool UseDefaultMonitor = true;
		public ModuleConfig[] Modules = Array.Empty<ModuleConfig>();
	}

	[Serializable]
	public class ModuleConfig
	{
		public string Name;
		public int Interval;
		public bool Enabled;
	}

}