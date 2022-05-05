using System;

namespace UnityPerformancePortal
{
	public abstract class MonitorModule : IDisposable
	{
		public bool Disposed { get; private set; }

		public virtual bool TryInit() => true;

		public virtual void Update(double delta) { }

		public virtual void OnCollectReport() { }

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