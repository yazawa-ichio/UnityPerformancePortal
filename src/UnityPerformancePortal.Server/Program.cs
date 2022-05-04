using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace UnityPerformancePortal.Server
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
					string port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
					if (!string.IsNullOrEmpty(port))
					{
						webBuilder.UseUrls($"http://0.0.0.0:{port}");
					}
				});
	}
}