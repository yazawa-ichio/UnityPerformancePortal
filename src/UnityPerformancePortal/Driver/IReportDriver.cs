using System;
using System.Threading.Tasks;
using UnityPerformancePortal.Model;

namespace UnityPerformancePortal.Driver
{
	public interface IReportDriver
	{
		Task Upload(ReportData data);
		Task<ReportData[]> Download(string id, DateTime startAt, DateTime endAt);
		Task<Reporter[]> Reporters(DateTime startAt, DateTime endAt);
	}
}