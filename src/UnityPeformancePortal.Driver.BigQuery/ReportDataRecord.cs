using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityPerformancePortal.Model;

namespace UnityPerformancePortal.Driver.BigQuery
{
	internal class ReportDataRecord
	{
		public static ReportDataRecord Create(ReportData data)
		{
			return new ReportDataRecord
			{
				ReporterId = data.ReporterId,
				SessionId = data.SessionId,
				StartAt = DateTimeOffset.FromUnixTimeMilliseconds(data.StartAt.UnixMillseconds).ToString("yyyy-MM-dd HH:mm:ss.ffffff"),
				EndAt = DateTimeOffset.FromUnixTimeMilliseconds(data.EndAt.UnixMillseconds).ToString("yyyy-MM-dd HH:mm:ss.ffffff"),
				Payload = JsonSerializer.Serialize(data),
			};
		}

		[JsonPropertyName("reporter_id")]
		public string ReporterId { get; set; }
		[JsonPropertyName("session_id")]
		public string SessionId { get; set; }
		[JsonPropertyName("start_at")]
		public string StartAt { get; set; }
		[JsonPropertyName("end_at")]
		public string EndAt { get; set; }
		[JsonPropertyName("payload")]
		public string Payload { get; set; }
	}

	internal class CounterDataRecord
	{
		public static CounterDataRecord[] Create(ReportData data)
		{
			return data.Counter.Select(x => new CounterDataRecord
			{
				ReporterId = data.ReporterId,
				SessionId = data.SessionId,
				Name = x.Info.Name,
				Unit = x.Info.Unit,
				TimeAt = DateTimeOffset.FromUnixTimeMilliseconds(x.TimeAt.UnixMillseconds).ToString("yyyy-MM-dd HH:mm:ss.ffffff"),
				Value = x.Value,
			}).ToArray();
		}

		[JsonPropertyName("reporter_id")]
		public string ReporterId { get; set; }
		[JsonPropertyName("session_id")]
		public string SessionId { get; set; }
		[JsonPropertyName("name")]
		public string Name { get; set; }
		[JsonPropertyName("unit")]
		public string Unit { get; set; }
		[JsonPropertyName("time_at")]
		public string TimeAt { get; set; }
		[JsonPropertyName("value")]
		public double Value { get; set; }
	}


	internal class GaugeDataRecord
	{
		public static GaugeDataRecord[] Create(ReportData data)
		{
			return data.Gauge.Select(x => new GaugeDataRecord
			{
				ReporterId = data.ReporterId,
				SessionId = data.SessionId,
				Name = x.Info.Name,
				Unit = x.Info.Unit,
				TimeAt = DateTimeOffset.FromUnixTimeMilliseconds(x.TimeAt.UnixMillseconds).ToString("yyyy-MM-dd HH:mm:ss.ffffff"),
				Value = x.Value,
			}).ToArray();
		}

		[JsonPropertyName("reporter_id")]
		public string ReporterId { get; set; }
		[JsonPropertyName("session_id")]
		public string SessionId { get; set; }
		[JsonPropertyName("name")]
		public string Name { get; set; }
		[JsonPropertyName("unit")]
		public string Unit { get; set; }
		[JsonPropertyName("time_at")]
		public string TimeAt { get; set; }
		[JsonPropertyName("value")]
		public double Value { get; set; }
	}


	internal class AverageDataRecord
	{
		public static AverageDataRecord[] Create(ReportData data)
		{
			return data.Average.Select(x => new AverageDataRecord
			{
				ReporterId = data.ReporterId,
				SessionId = data.SessionId,
				Name = x.Info.Name,
				Unit = x.Info.Unit,
				TimeAt = DateTimeOffset.FromUnixTimeMilliseconds(x.TimeAt.UnixMillseconds).ToString("yyyy-MM-dd HH:mm:ss.ffffff"),
				Value = x.Value,
			}).ToArray();
		}

		[JsonPropertyName("reporter_id")]
		public string ReporterId { get; set; }
		[JsonPropertyName("session_id")]
		public string SessionId { get; set; }
		[JsonPropertyName("name")]
		public string Name { get; set; }
		[JsonPropertyName("unit")]
		public string Unit { get; set; }
		[JsonPropertyName("time_at")]
		public string TimeAt { get; set; }
		[JsonPropertyName("value")]
		public double Value { get; set; }
	}

}