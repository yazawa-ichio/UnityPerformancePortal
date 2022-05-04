using Google.Apis.Bigquery.v2.Data;
using Google.Cloud.BigQuery.V2;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using UnityPerformancePortal.Model;

namespace UnityPerformancePortal.Driver.BigQuery
{
	public class BigQueryReportDriver : IReportDriver, IDisposable
	{
		ILogger<BigQueryReportDriver> m_Logger;
		BigQueryClient m_Client;
		BigQueryTable m_ReportDataTable;
		BigQueryTable m_GaugeDataTable;
		BigQueryTable m_AverageDataTable;
		BigQueryTable m_CountDataTable;

		public BigQueryReportDriver(ILogger<BigQueryReportDriver> logger, BigQueryClient client)
		{
			m_Logger = logger;
			m_Client = client;
			Init();
		}

		void Init()
		{
			m_Client.GetOrCreateDataset("UnityPerformancePortal");
			{
				var schema = new TableSchemaBuilder
				{
					{ "reporter_id", BigQueryDbType.String },
					{ "session_id", BigQueryDbType.String },
					{ "start_at", BigQueryDbType.Timestamp },
					{ "end_at", BigQueryDbType.Timestamp },
					{ "payload", BigQueryDbType.String },
				}.Build();
				m_ReportDataTable = m_Client.GetOrCreateTable("UnityPerformancePortal", "ReportData", schema);
			}
			{
				var schema = new TableSchemaBuilder
				{
					{ "reporter_id", BigQueryDbType.String },
					{ "session_id", BigQueryDbType.String },
					{ "name", BigQueryDbType.String },
					{ "unit", BigQueryDbType.String },
					{ "time_at", BigQueryDbType.Timestamp },
					{ "value", BigQueryDbType.Float64 },
				}.Build();
				m_CountDataTable = m_Client.GetOrCreateTable("UnityPerformancePortal", "CountData", schema);
			}
			{
				var schema = new TableSchemaBuilder
				{
					{ "reporter_id", BigQueryDbType.String },
					{ "session_id", BigQueryDbType.String },
					{ "name", BigQueryDbType.String },
					{ "unit", BigQueryDbType.String },
					{ "time_at", BigQueryDbType.Timestamp },
					{ "value", BigQueryDbType.Float64 },
				}.Build();
				m_GaugeDataTable = m_Client.GetOrCreateTable("UnityPerformancePortal", "GaugeData", schema);
			}
			{
				var schema = new TableSchemaBuilder
				{
					{ "reporter_id", BigQueryDbType.String },
					{ "session_id", BigQueryDbType.String },
					{ "name", BigQueryDbType.String },
					{ "unit", BigQueryDbType.String },
					{ "time_at", BigQueryDbType.Timestamp },
					{ "value", BigQueryDbType.Float64 },
				}.Build();
				m_AverageDataTable = m_Client.GetOrCreateTable("UnityPerformancePortal", "AverageData", schema);
			}
		}


		public void Dispose()
		{
			m_Client?.Dispose();
			GC.SuppressFinalize(this);
		}

		public Task Upload(ReportData data)
		{
			UploadImpl(data);
			return Task.CompletedTask;
		}

		async void UploadImpl(ReportData data)
		{
			try
			{
				m_Logger.LogInformation("Start UploadTable");
				await Task.WhenAll(
					UploadTable(m_ReportDataTable, ReportDataRecord.Create(data)),
					UploadTable(m_CountDataTable, CounterDataRecord.Create(data)),
					UploadTable(m_GaugeDataTable, GaugeDataRecord.Create(data)),
					UploadTable(m_AverageDataTable, AverageDataRecord.Create(data))
				);
				m_Logger.LogInformation("Complete UploadTable");
			}
			catch (Exception ex)
			{
				m_Logger.LogError("Upload Record Error {0}", ex);
			}
		}

		async Task UploadTable(BigQueryTable table, params object[] datas)
		{
			if (datas.Length == 0)
			{
				return;
			}
			var ret = await table.UploadJsonAsync(datas.Select(x => JsonSerializer.Serialize(x)));
			ret = await ret.PollUntilCompletedAsync();
			if (ret.Status.Errors?.Count > 0)
			{
				foreach (ErrorProto error in ret.Status.Errors)
				{
					m_Logger.LogWarning(error.Message);
				}
			}
			ret.ThrowOnAnyError();
		}

		public async Task<ReportData[]> Download(string id, long startUnixMillseconds, long endUnixMillseconds)
		{
			var startAt = DateTimeOffset.FromUnixTimeMilliseconds(startUnixMillseconds).ToString("yyyy-MM-dd HH:mm:ss.ffffff");
			var endAt = DateTimeOffset.FromUnixTimeMilliseconds(endUnixMillseconds).ToString("yyyy-MM-dd HH:mm:ss.ffffff");
			var sql = @$"SELECT reporter_id,payload
FROM `{m_Client.ProjectId}.UnityPerformancePortal.ReportData` 
WHERE reporter_id = ""{id}""
AND　start_at BETWEEN ""{startAt}"" AND ""{endAt}""
LIMIT 100";

			m_Logger.LogInformation(sql);
			var query = await m_Client.ExecuteQueryAsync(sql, parameters: null);

			var ret = new List<ReportData>();
			await foreach (var item in query.GetRowsAsync())
			{
				ret.Add(JsonSerializer.Deserialize<ReportData>(item["payload"].ToString()));
			}

			return ret.ToArray();
		}

		public async Task<Reporter[]> Reporters(long startUnixMillseconds, long endUnixMillseconds)
		{
			var startAt = DateTimeOffset.FromUnixTimeMilliseconds(startUnixMillseconds).ToString("yyyy-MM-dd HH:mm:ss.ffffff");
			var endAt = DateTimeOffset.FromUnixTimeMilliseconds(endUnixMillseconds).ToString("yyyy-MM-dd HH:mm:ss.ffffff");
			var sql = @$"SELECT reporter_id,session_id,MAX(end_at) as end_at
FROM `{m_Client.ProjectId}.UnityPerformancePortal.ReportData` 
WHERE start_at BETWEEN ""{startAt}"" AND ""{endAt}""
GROUP BY reporter_id, session_id
LIMIT 100";

			m_Logger.LogInformation(sql);
			var query = await m_Client.ExecuteQueryAsync(sql, parameters: null);

			var ret = new List<Reporter>();
			await foreach (var item in query.GetRowsAsync())
			{
				ret.Add(new Reporter
				{
					ReporterId = item["reporter_id"].ToString(),
					SessionId = item["session_id"].ToString(),
					LastAt = new DateTimeOffset((DateTime)item["end_at"]).ToUnixTimeMilliseconds(),
				});
			}

			return ret.ToArray();
		}
	}
}