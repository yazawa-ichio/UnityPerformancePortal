using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityPerformancePortal.Report;

namespace UnityPerformancePortal
{
	public class DefaultRepoter : Repoter
	{
		ReporterSettings m_Setting;
		HttpClient m_Client;
		CancellationTokenSource m_CancellationTokenSource = new CancellationTokenSource();

		public const string TokenHeader = "x-upp-token";

		ReportConfig m_Config;
		MemoryStream m_Stream = new MemoryStream();
		float m_Timer;

		public DefaultRepoter(ReporterSettings setting)
		{
			m_Setting = setting;
		}

		public override void Dispose()
		{
			m_Client?.Dispose();
			m_CancellationTokenSource.Cancel();
		}

		protected override void Update()
		{
			if (m_Config == null || m_Config.Interval <= 0)
			{
				return;
			}
			m_Timer -= Time.unscaledDeltaTime;
			if (m_Timer < 0)
			{
				m_Timer = m_Config.Interval;
				Post();
			}
		}

		public override void Init(Action success, Action<Exception> error)
		{
			if (m_Setting.Config == null)
			{
				error?.Invoke(new Exception("config is null"));
				return;
			}
			if (m_Setting.Config.UseDefaultMonitor)
			{
				Monitor.AddDefaultMonitor();
			}
			m_CancellationTokenSource.Token.ThrowIfCancellationRequested();
			if (m_Setting.HttpClientHandler == null)
			{
				m_Client = new HttpClient();
			}
			else
			{
				m_Client = new HttpClient(m_Setting.HttpClientHandler);
			}
			m_Config = m_Setting.Config;
			m_Timer = m_Config.Interval;
			success?.Invoke();
		}

		protected override void Send(ReportData report)
		{
			m_CancellationTokenSource.Cancel();
			m_CancellationTokenSource = new CancellationTokenSource();
			m_Stream.SetLength(0);
			report.ToJson(m_Stream);
			_ = PostAsync<object>(m_Config.ReportUrl, m_Stream, m_CancellationTokenSource.Token);
		}


		async Task<T> PostAsync<T>(string url, Stream stream, CancellationToken token)
		{
			while (true)
			{
				token.ThrowIfCancellationRequested();

				stream.Position = 0;
				var content = new StreamContent(stream);
				if (!string.IsNullOrEmpty(m_Config.Token))
				{
					content.Headers.Add(TokenHeader, m_Config.Token);
				}
				content.Headers.Add("ContentType", "application/json");
				using (var response = await m_Client.PostAsync(url, content, token))
				{
					var str = await response.Content.ReadAsStringAsync();
					if (response.IsSuccessStatusCode)
					{
						if (typeof(T) == typeof(object))
						{
							return default;
						}
						return JsonUtility.FromJson<T>(str);
					}
					else
					{
						throw new Exception(str);
					}
				}
			}
		}

	}
}