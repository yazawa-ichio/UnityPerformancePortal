using Blazored.LocalStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using UnityPerformancePortal.Model;

namespace UnityPerformancePortal.Front
{
	public class PortalClient
	{
		HttpClient m_Client;
		ILocalStorageService m_Storage;

		public PortalClient(HttpClient client, ILocalStorageService storage)
		{
			m_Client = client;
			m_Storage = storage;
		}

		public Task<Reporter[]> Reporters()
		{
			return Reporters(DateTimeOffset.UtcNow.AddDays(-1).ToUnixTimeMilliseconds(), DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
		}

		public async Task<Reporter[]> Reporters(long startAt, long? endAt)
		{
			var param = $"startAt={startAt}";
			if (endAt.HasValue)
			{
				param += $"&endAt={endAt.Value}";
			}
			return await m_Client.GetFromJsonAsync<Reporter[]>($"reporters?{param}");
		}

		public async Task<ReportData[]> Download(string id, long startAt, long endAt, bool cache = true)
		{
			List<ReportData> ret = new List<ReportData>();
			if (cache)
			{
				var keys = (await m_Storage.KeysAsync()).Where(x => x.StartsWith($"portal-client-cache-{id}"));
				foreach (var key in keys)
				{
					var data = key.Replace($"portal-client-cache-{id}-", "").Split(":");
					var cacheStartAt = long.Parse(data[0]);
					var cacheEndAt = long.Parse(data[1]);
					if (startAt <= cacheStartAt && cacheStartAt <= endAt)
					{
						ret.Add(await m_Storage.GetItemAsync<ReportData>(key));
					}
					else if (startAt <= cacheEndAt && cacheEndAt <= endAt)
					{
						ret.Add(await m_Storage.GetItemAsync<ReportData>(key));
					}
				}
				if (ret.Count > 0)
				{
					var cacheStartAt = ret.Select(x => x.StartAt.UnixMillseconds).Min();
					var cacheEndAt = ret.Select(x => x.EndAt.UnixMillseconds).Max();
					if (cacheStartAt < startAt && endAt < cacheEndAt)
					{
						return ret.OrderBy(x => x.StartAt.UnixMillseconds).ToArray();
					}
					if (cacheEndAt > startAt)
					{
						startAt = cacheEndAt;
					}
					if (cacheStartAt > endAt)
					{
						endAt = cacheStartAt;
					}
				}
			}
			var res = await m_Client.GetFromJsonAsync<ReportData[]>($"get?reporterId={id}&startAt={startAt}&endAt={endAt}");
			if (cache)
			{
				foreach (var item in res)
				{
					await m_Storage.SetItemAsync($"portal-client-cache-{id}-{item.StartAt.UnixMillseconds}:{item.EndAt.UnixMillseconds}", item);
				}
			}
			ret.AddRange(res);
			return ret.OrderBy(x => x.StartAt.UnixMillseconds).ToArray();
		}

	}
}