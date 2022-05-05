using UnityEngine.Networking;

namespace UnityPerformancePortal
{
	public interface IReporterSettings
	{
		Repoter CreateRepoter();
	}

	public class ReporterSettings : IReporterSettings
	{
		public ReportConfig Config;
		public CertificateHandler CertificateHandler;

		public Repoter CreateRepoter()
		{
			return new DefaultRepoter(this);
		}
	}
}