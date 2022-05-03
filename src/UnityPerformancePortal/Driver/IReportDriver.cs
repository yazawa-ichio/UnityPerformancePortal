using System.Threading.Tasks;
using UnityPerformancePortal.Model;

namespace UnityPerformancePortal.Driver
{
	public interface IReportDriver
	{
		Task Upload(ReportData data);
		Task<ReportData[]> Download(string id, long startUnixMillseconds, long endUnixMillseconds);
		Task<Reporter[]> Reporters(long startUnixMillseconds, long endUnixMillseconds);
	}
}