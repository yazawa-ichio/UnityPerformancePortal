using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityPerformancePortal.Report;

namespace UnityPerformancePortal
{
	public class DefaultRepoter : Repoter
	{
		ReporterSettings m_Setting;
		CancellationTokenSource m_CancellationTokenSource = new CancellationTokenSource();

		public const string TokenHeader = "x-upp-token";

		ReportConfig m_Config;
		MemoryStream m_Stream = new MemoryStream();
		float m_Timer;

		public DefaultRepoter(ReporterSettings setting) : base(GetReporterId(setting.Config.ReporterId))
		{
			m_Setting = setting;
		}

		static string GetReporterId(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				const string Key = "UPP-ReporterId";
				if (PlayerPrefs.HasKey(Key))
				{
					return PlayerPrefs.GetString(Key);
				}
				else
				{
					id = Guid.NewGuid().ToString();
					PlayerPrefs.SetString(Key, id);
					PlayerPrefs.Save();
					return id;
				}
			}
			return id;
		}

		public override void Dispose()
		{
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


		async Task<T> PostAsync<T>(string url, MemoryStream stream, CancellationToken token)
		{
			//while (true)
			try
			{
				token.ThrowIfCancellationRequested();
				var req = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
				req.SetRequestHeader("Content-Type", "application/json");
				if (!string.IsNullOrEmpty(m_Config.Token))
				{
					req.SetRequestHeader(TokenHeader, m_Config.Token);
				}
				req.downloadHandler = new DownloadHandlerBuffer();
				req.uploadHandler = new UploadHandlerRaw(stream.ToArray());
				if (m_Setting.CertificateHandler != null)
				{
					req.certificateHandler = m_Setting.CertificateHandler;
				}
				var op = req.SendWebRequest();
				var future = new TaskCompletionSource<string>();
				token.Register(() =>
				{
					future.TrySetCanceled(token);
				});
				op.completed += (_) =>
				{
					if (req.result == UnityWebRequest.Result.Success)
					{
						future.TrySetResult(req.downloadHandler.text);
					}
					else
					{
						future.TrySetException(new Exception(req.error));
					}
				};
				var res = await future.Task;
				if (typeof(T) == typeof(object))
				{
					return default;
				}
				return JsonUtility.FromJson<T>(res);
			}
			catch (Exception err)
			{
				if (token.IsCancellationRequested)
				{
					return default;
				}
				Debug.LogException(err);
				throw;
			}
		}
	}

}