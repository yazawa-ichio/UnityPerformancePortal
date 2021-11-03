using System.Net.Http;

namespace UnityPerformancePortal
{
	public interface IReporterSettings
	{
		Repoter CreateRepoter();
	}

	public class ReporterSettings : IReporterSettings
	{
		public string AuthUrl;
		public string AuthTicket;
		public HttpClientHandler HttpClientHandler;
		public bool UseDefaultMonitor = true;

		public Repoter CreateRepoter()
		{
			return new DefaultRepoter(this);
		}
	}
}