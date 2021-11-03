using System;

namespace UnityPerformancePortal
{
	public abstract class MonitorModule : IDisposable
	{
		public bool Disposed { get; private set; }

		public abstract bool TryInit();

		public virtual void Update() { }

		public virtual void OnPostReport() { }

		protected virtual void Dispose(bool disposing) { }

		public void Dispose()
		{
			Disposed = true;
			if (Client.IsValid)
			{
				Client.Monitor.Remove(this);
			}
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

	}
}