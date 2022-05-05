using System.Collections.Generic;
using System.Linq;

namespace UnityPerformancePortal.Model
{
	public static class ReportDataExtension
	{
		public static Dictionary<string, string> GetMeta(this ReportData[] reports)
		{
			var ret = new Dictionary<string, string>();
			foreach (var report in reports)
			{
				foreach (var meta in report.Meta)
				{
					ret[meta.Key] = meta.Value;
				}
			}
			return ret;
		}


		public static IEnumerable<(string category, ReportEntryInfo info)> GetEntryInfos(this ReportData[] self)
		{
			foreach (var info in self.GetCounterEntryInfos())
			{
				yield return ("Counter", info);
			}
			foreach (var info in self.GetGaugeEntryInfos())
			{
				yield return ("Gauge", info);
			}
			foreach (var info in self.GetAverageEntryInfos())
			{
				yield return ("Average", info);
			}
		}


		public static IEnumerable<CounterReport> GetCounters(this ReportData[] self)
		{
			return self.SelectMany(x => x.Counter);
		}

		public static IEnumerable<ReportEntryInfo> GetCounterEntryInfos(this ReportData[] self)
		{
			return self.GetCounters().Select(x => x.Info).Distinct();
		}

		public static IEnumerable<GaugeReport> GetGauges(this ReportData[] self)
		{
			return self.SelectMany(x => x.Gauge);
		}

		public static IEnumerable<ReportEntryInfo> GetGaugeEntryInfos(this ReportData[] self)
		{
			return self.GetGauges().Select(x => x.Info).Distinct();
		}

		public static IEnumerable<AverageReport> GetAverages(this ReportData[] self)
		{
			return self.SelectMany(x => x.Average);
		}

		public static IEnumerable<ReportEntryInfo> GetAverageEntryInfos(this ReportData[] self)
		{
			return self.GetAverages().Select(x => x.Info).Distinct();
		}

		public static IEnumerable<ElapsedTimeReport> GetElapsedTimes(this ReportData[] self)
		{
			return self.SelectMany(x => x.ElapsedTime);
		}

	}
}