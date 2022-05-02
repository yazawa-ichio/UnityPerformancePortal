using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using UnityPerformancePortal.Driver;
using UnityPerformancePortal.Model;

namespace UnityPerformancePortal.Local
{

	public class LocalHttpServer : IDisposable
	{

		string m_RootDirectory;
		HttpListener m_Listener;
		CancellationTokenSource m_Cancellation = new();
		IReportDriver m_ReportDriver;

		public int Port { get; private set; }

		public LocalHttpServer(string path, int port, IReportDriver reportDriver)
		{
			m_RootDirectory = path;
			Port = port;
			m_Listener = new();
			m_Listener.Prefixes.Add("http://127.0.0.1:" + Port.ToString() + "/");
			m_ReportDriver = reportDriver;
		}

		public void Dispose()
		{
			m_Cancellation.Cancel();
			m_Listener.Stop();
			m_Listener.Abort();
		}

		void Initialize(string path, int port)
		{
			m_RootDirectory = path;
			Port = port;
			m_Listener = new();
			m_Listener.Prefixes.Add("http://127.0.0.1:" + Port.ToString() + "/");
		}

		public Task Run()
		{

			m_Listener.Start();
			return Task.Run(Loop, m_Cancellation.Token);
		}

		void Loop()
		{
			while (!m_Cancellation.IsCancellationRequested)
			{
				try
				{
					var context = m_Listener.GetContext();
					using (context.Response)
					{
						if (context.Request.Url.PathAndQuery.StartsWith("/api", StringComparison.OrdinalIgnoreCase))
						{
							Api(context);
						}
						else
						{
							Static(context);
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error {0}", ex);
				}
			}
		}

		void Static(HttpListenerContext context)
		{
			string filename = context.Request.Url.AbsolutePath;

			filename = filename.Substring(1);
			if (string.IsNullOrEmpty(filename))
			{
				if (File.Exists(Path.Combine(m_RootDirectory, "index.html")))
				{
					filename = "index.html";
				}
			}

			filename = Path.Combine(m_RootDirectory, filename);

			// ページルーティングはindexに渡す
			if (!File.Exists(filename) && Path.GetExtension(filename) == "")
			{
				filename = Path.Combine(m_RootDirectory, "index.html");
			}

			if (File.Exists(filename))
			{
				try
				{
					using Stream input = new FileStream(filename, FileMode.Open);

					string mime;
					context.Response.ContentType = Util.MimeTypeMappings.TryGetValue(Path.GetExtension(filename), out mime)
						? mime
						: "application/octet-stream";
					context.Response.ContentLength64 = input.Length;
					context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
					context.Response.AddHeader("Last-Modified", File.GetLastWriteTime(filename).ToString("r"));

					byte[] buffer = new byte[1024 * 32];
					int nbytes;
					while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
						context.Response.OutputStream.Write(buffer, 0, nbytes);
					input.Close();
					context.Response.OutputStream.Flush();

					context.Response.StatusCode = (int)HttpStatusCode.OK;
				}
				catch (Exception ex)
				{
					context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
					Console.WriteLine("Error {0}", ex);
				}

			}
			else
			{
				context.Response.StatusCode = (int)HttpStatusCode.NotFound;
			}

			context.Response.OutputStream.Close();
		}

		void Api(HttpListenerContext context)
		{
			Console.WriteLine(context.Request.Url.PathAndQuery);
			try
			{
				switch (context.Request.Url.LocalPath.ToLower())
				{
					case "/api/post":
						Post(context);
						break;
					case "/api/get":
						Get(context);
						break;
					case "/api/reporters":
						Reporters(context);
						break;
					default:
						context.Response.StatusCode = (int)HttpStatusCode.NotFound;
						break;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
			}

			context.Response.OutputStream.Close();
		}

		void Post(HttpListenerContext context)
		{
			var data = JsonSerializer.DeserializeAsync<ReportData>(context.Request.InputStream).Result;
			m_ReportDriver.Upload(data).Wait();
			context.Response.StatusCode = (int)HttpStatusCode.OK;
		}

		void Get(HttpListenerContext context)
		{
			context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
			var reporterId = context.Request.QueryString.Get("reporterId");
			DateTime.TryParse(context.Request.QueryString.Get("startAt"), out var startAt);
			if (!DateTime.TryParse(context.Request.QueryString.Get("endAt"), out var endAt))
			{
				endAt = DateTime.MaxValue;
			}
			startAt = DateTime.SpecifyKind(startAt, DateTimeKind.Utc);
			endAt = DateTime.SpecifyKind(endAt, DateTimeKind.Utc);
			var data = m_ReportDriver.Download(reporterId, startAt, endAt).Result;
			JsonSerializer.SerializeAsync(context.Response.OutputStream, data).Wait();
			context.Response.OutputStream.Flush();
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)HttpStatusCode.OK;
		}

		void Reporters(HttpListenerContext context)
		{
			context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
			DateTime.TryParse(context.Request.QueryString.Get("startAt"), out var startAt);
			if (!DateTime.TryParse(context.Request.QueryString.Get("endAt"), out var endAt))
			{
				endAt = DateTime.MaxValue;
			}
			startAt = DateTime.SpecifyKind(startAt, DateTimeKind.Utc);
			endAt = DateTime.SpecifyKind(endAt, DateTimeKind.Utc);
			var data = m_ReportDriver.Reporters(startAt, endAt).Result;
			JsonSerializer.SerializeAsync(context.Response.OutputStream, data).Wait();
			context.Response.OutputStream.Flush();
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)HttpStatusCode.OK;
		}

	}

}