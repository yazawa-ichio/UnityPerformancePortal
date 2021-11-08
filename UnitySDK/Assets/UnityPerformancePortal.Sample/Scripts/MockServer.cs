using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

namespace UPP.Sample
{

	public class MockServer : MonoBehaviour
	{
		[SerializeField]
		bool m_Run;

		HttpListener m_Listener;
		bool m_End;

		void Awake()
		{
			if (!m_Run)
			{
				return;
			}
			m_Listener = new HttpListener();
			m_Listener.Prefixes.Add(@"http://+:8181/");
			m_Listener.Start();
			Loop();
		}

		void OnDestroy()
		{
			if (!m_Run)
			{
				return;
			}
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

		async Task Report(HttpListenerContext context)
		{
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