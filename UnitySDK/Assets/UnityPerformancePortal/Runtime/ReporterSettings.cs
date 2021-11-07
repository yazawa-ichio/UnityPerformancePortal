using System.Net.Http;

namespace UnityPerformancePortal
{
	public interface IReporterSettings
	{
		Repoter CreateRepoter();
	}

	public class ReporterSettings : IReporterSettings
	{
		public ReportConfig Config;
		public HttpClientHandler HttpClientHandler;

		public Repoter CreateRepoter()
		{
			return new DefaultRepoter(this);
		}
	}
}