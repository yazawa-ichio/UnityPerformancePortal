using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using UnityPerformancePortal.Driver;
using UnityPerformancePortal.Model;

namespace UnityPerformancePortal.Server.Controllers
{
	[ApiController]
	[Route("api/[action]")]
	public class ReportController : ControllerBase
	{
		readonly ILogger<ReportController> m_Logger;
		readonly IReportDriver m_Driver;

		public ReportController(ILogger<ReportController> logger, IReportDriver driver)
		{
			m_Logger = logger;
			m_Driver = driver;
		}

		[HttpPost]
		public Task Post(ReportData data)
		{
			m_Logger.LogInformation("Post {0}", data.ReporterId);
			return m_Driver.Upload(data);
		}

		[HttpGet]
		public Task<ReportData[]> Get(string reporterId, long startAt, long endAt)
		{
			m_Logger.LogInformation("Reporter {0} {1}, {2}", reporterId, startAt, endAt);
			return m_Driver.Download(reporterId, startAt, endAt);
		}

		[HttpGet]
		public Task<Reporter[]> Reporters(long startAt, long endAt)
		{
			if (endAt == default)
			{
				endAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
			}
			m_Logger.LogInformation("Reporters {0}, {1}", startAt, endAt);
			return m_Driver.Reporters(startAt, endAt);
		}
	}
}