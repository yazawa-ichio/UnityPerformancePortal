using System;

namespace UnityPerformancePortal.Net
{
	[Serializable]
	public class ReportConfig
	{
		public string ReportUrl;
		public int Interval;
		public ConnectionType ConnectionType;
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