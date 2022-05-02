using UnityPerformancePortal.Driver.LiteDB;

namespace UnityPerformancePortal.Local
{
	class Program
	{
		static void Main(string[] args)
		{
			string path = "wwwroot";
			int port = 5325;
			if (args.Length >= 1)
			{
				path = args[0];
			}
			if (args.Length >= 2)
			{
				port = int.Parse(args[1]);
			}
			using var server = new LocalHttpServer(path, port, new LiteDBReportDriver());
			server.Run().Wait();
		}
	}
}