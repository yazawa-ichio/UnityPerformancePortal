using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityPerformancePortal;
using UnityPerformancePortal.Net;

namespace UPP.Sample
{

	public class MockServer : MonoBehaviour
	{
		HttpListener m_Listener;
		bool m_End;
		HashSet<string> m_Token = new HashSet<string>();

		void Awake()
		{
			m_Listener = new HttpListener();
			m_Listener.Prefixes.Add(@"http://+:8181/");
			m_Listener.Start();
			Loop();
		}

		void OnDestroy()
		{
			m_End = true;
			m_Listener.Stop();
			m_Listener = null;
		}

		async void Loop()
		{
			try
			{
				while (m_Listener != null)
				{
					var context = await m_Listener.GetContextAsync();
					try
					{
						switch (context.Request.Url.LocalPath)
						{
							case "/auth":
								await Auth(context);
								continue;
							case "/report":
								await Report(context);
								continue;
						}
					}
					catch (Exception err)
					{
						using (var response = context.Response)
						using (var writer = new StreamWriter(context.Response.OutputStream))
						{
							Debug.LogException(err);
							response.StatusCode = (int)HttpStatusCode.BadRequest;
							writer.Write(err.Message);
							writer.Flush();
							response.Close();
						}
						continue;
					}
					Debug.LogError(context.Request.Url.LocalPath);
					using (var response = context.Response)
					{
						response.StatusCode = (int)HttpStatusCode.BadRequest;
						response.Close();
					}
				}
			}
			catch (Exception err)
			{
				if (m_End)
				{
					return;
				}
				Debug.LogException(err);
			}
		}

		async Task Auth(HttpListenerContext context)
		{
			var reader = new StreamReader(context.Request.InputStream);
			var json = await reader.ReadToEndAsync();
			var req = JsonUtility.FromJson<AuthRequest>(json);
			var response = context.Response;
			using (var writer = new StreamWriter(context.Response.OutputStream))
			{
				response.StatusCode = (int)HttpStatusCode.OK;
				var token = Guid.NewGuid().ToString();
				m_Token.Add(token);
				var res = new AuthResponse
				{
					Token = token,
					ReportConfig = new ReportConfig()
					{
						ReportUrl = "http://localhost:8181/report",
						ConnectionType = ConnectionType.Http,
						Interval = 5,
					}
				};
				await writer.WriteAsync(JsonUtility.ToJson(res));
				writer.Flush();
				response.Close();
			}
		}

		async Task Report(HttpListenerContext context)
		{
			var token = context.Request.Headers.Get(DefaultRepoter.TokenHeader);
			if (!m_Token.Contains(token))
			{
				throw new Exception("invalid token");
			}
			var reader = new StreamReader(context.Request.InputStream);
			var json = await reader.ReadToEndAsync();
			Debug.Log(json);
			using (var response = context.Response)
			{
				response.StatusCode = (int)HttpStatusCode.OK;
				response.Close();
			}
		}

	}

}