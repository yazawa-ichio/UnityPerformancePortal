using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace UnityPerformancePortal
{
	public class Monitor : IDisposable
	{
		List<MonitorModule> m_Modules = new List<MonitorModule>();
		ConcurrentQueue<MonitorModule> m_RemoveQueue = new ConcurrentQueue<MonitorModule>();
		DateTime m_Prev;

		public FPSMonitor FPS { get; private set; }
		public MemoryMonitor Memory { get; private set; }
		public RenderMonitor Render { get; private set; }

		internal Monitor() { }

		public void AddDefaultMonitor()
		{
			TryAdd(FPS = new FPSMonitor());
			TryAdd(Memory = new MemoryMonitor());
			TryAdd(Render = new RenderMonitor());
		}

		public void Apply(ModuleConfig[] config)
		{

		}

		public bool TryAdd<T>() where T : MonitorModule, new()
		{
			return TryAdd(new T());
		}

		public bool TryAdd(MonitorModule module)
		{
			if (!module.TryInit())
			{
				return false;
			}
			m_Modules.Add(module);
			return true;
		}

		internal void Remove(MonitorModule module)
		{
			m_RemoveQueue.Enqueue(module);
		}

		void IDisposable.Dispose()
		{
			Dispose();
		}

		internal void Dispose()
		{
			foreach (var module in m_Modules)
			{
				if (module.Disposed)
				{
					continue;
				}
				module.Dispose();
			}
			m_Modules.Clear();
		}

		internal void Update()
		{
			var now = DateTime.UtcNow;
			var delta = (now - m_Prev).TotalSeconds;
			m_Prev = now;
			if (delta < 0 || delta > 1)
			{
				return;
			}
			foreach (var module in m_Modules)
			{
				if (module.Disposed)
				{
					continue;
				}
				module.Update(delta);
			}
			while (m_RemoveQueue.TryDequeue(out var mod))
			{
				m_Modules.Remove(mod);
			}
		}

		internal void OnCollectReport()
		{
			foreach (var module in m_Modules)
			{
				if (module.Disposed)
				{
					continue;
				}
				module.OnCollectReport();
			}
		}

	}
}