using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityPerformancePortal.Net;
using UnityPerformancePortal.Report;

namespace UnityPerformancePortal
{
	public class DefaultRepoter : Repoter
	{
		ReporterSettings m_Setting;
		HttpClient m_Client;
		CancellationTokenSource m_CancellationTokenSource = new CancellationTokenSource();

		public const string TokenHeader = "x-upp-token";

		string m_Token;
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
			if (m_Config == null)
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
			if (m_Setting.UseDefaultMonitor)
			{
				Monitor.AddDefaultMonitor();
			}
			Auth().Observe(x =>
			{
				if (x.IsSuccess)
				{
					success?.Invoke();
				}
				else
				{
					error?.Invoke(x.Error);
				}
			});
		}

		AsyncTask Auth()
		{
			return AsyncTask.Run(async () =>
			{
				m_CancellationTokenSource.Token.ThrowIfCancellationRequested();
				if (m_Setting.HttpClientHandler == null)
				{
					m_Client = new HttpClient();
				}
				else
				{
					m_Client = new HttpClient(m_Setting.HttpClientHandler);
				}
				var req = new AuthRequest
				{
					AuthToken = m_Setting.AuthTicket,
				};
				var stream = new MemoryStream();
				var writer = new StreamWriter(stream);
				writer.Write(JsonUtility.ToJson(req));
				writer.Flush();
				var res = await PostAsync<AuthResponse>(m_Setting.AuthUrl, stream, m_CancellationTokenSource.Token);
				Setup(res);
			});
		}

		void Setup(AuthResponse response)
		{
			m_Token = response.Token;
			m_Config = response.ReportConfig;
			m_Timer = m_Config.Interval;
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
				if (!string.IsNullOrEmpty(m_Token))
				{
					content.Headers.Add(TokenHeader, m_Token);
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