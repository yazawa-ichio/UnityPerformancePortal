using LiteDB;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityPerformancePortal.Model;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace UnityPerformancePortal.Driver.LiteDB
{
	public class LiteDBReportDriver : IReportDriver, IDisposable
	{
		LiteDatabase m_Database;
		ILiteCollection<ReportDataRecord> m_Collection;

		public LiteDBReportDriver() : this(new LiteDatabase("lite.db"))
		{
		}

		public LiteDBReportDriver(LiteDatabase db)
		{
			m_Database = db;
			m_Collection = m_Database.GetCollection<ReportDataRecord>();
			m_Collection.EnsureIndex(nameof(ReportDataRecord.StartAt));
		}

		public void Dispose()
		{
			m_Database.Dispose();
			GC.SuppressFinalize(this);
		}

		public Task<ReportData[]> Download(string id, DateTime startAt, DateTime endAt)
		{
			return Task.Run(() =>
			{
				var query = Query.And(Query.Between(nameof(ReportDataRecord.StartAt), startAt, endAt), Query.EQ(nameof(ReportDataRecord.ReporterId), id));
				return m_Collection.Find(query)
					.Select(x => JsonSerializer.Deserialize<ReportData>(x.Payload))
					.ToArray();
			});
		}

		public Task<Reporter[]> Reporters(DateTime startAt, DateTime endAt)
		{
			return Task.Run(() =>
			{
				var query = Query.Between(nameof(ReportDataRecord.StartAt), startAt, endAt);
				return m_Collection.Find(query)
					.Select(x => new Reporter
					{
						ReporterId = x.ReporterId,
						SessionId = x.SessionId,
						LastAt = x.EndAt,
					})
					.OrderByDescending(x => x.LastAt)
					.DistinctBy(x => x.ReporterId + x.SessionId)
					.ToArray();
			});
		}

		public Task Upload(ReportData data)
		{
			return Task.Run(() =>
			{
				m_Collection.Insert(new ReportDataRecord
				{
					ReporterId = data.ReporterId,
					SessionId = data.SessionId,
					StartAt = DateTimeOffset.FromUnixTimeMilliseconds(data.StartAt.UnixMillseconds).UtcDateTime,
					EndAt = DateTimeOffset.FromUnixTimeMilliseconds(data.EndAt.UnixMillseconds).UtcDateTime,
					Payload = JsonSerializer.Serialize(data),
				});
			});
		}
	}
}